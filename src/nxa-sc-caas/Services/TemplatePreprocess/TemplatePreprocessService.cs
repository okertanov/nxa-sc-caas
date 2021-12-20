using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NXA.SC.Caas.Services
{
    public class TemplatePreprocessService : ITemplatePreprocessService
    {
        private readonly ILogger<TemplatePreprocessService> logger;

        public TemplatePreprocessService(ILogger<TemplatePreprocessService> logger)
        {
            this.logger = logger;
        }

        public async Task<IEnumerable<TemplateParam>> FindParams(string sourceStr, string fileName) 
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceStr);
            var root = syntaxTree.GetCompilationUnitRoot();

            var namespaceDecl= root.Members.FirstOrDefault() as NamespaceDeclarationSyntax;
            if(namespaceDecl == null) 
            {
                throw new TemplatePreprocessException("Invalid namespace declaration");
            }

            var namespaceMembers = namespaceDecl.Members;
            var namespaceName = namespaceDecl.Name.ToString();
            logger.LogInformation($"Namespace - {namespaceName}");
            if (!namespaceMembers.Any(m => m.Kind() == SyntaxKind.ClassDeclaration))
            {
                throw new TemplatePreprocessException("No class declaration found");
            }

            var classDeclMembers = namespaceMembers.Where(m => m.Kind() == SyntaxKind.ClassDeclaration);
            var tasksToRun = classDeclMembers.Select(mamberDecl => ProcessClass(mamberDecl, fileName, syntaxTree));
            await Task.WhenAll(tasksToRun);
            return tasksToRun.SelectMany(t=>t.Result);
        }

        private async Task<IEnumerable<TemplateParam>> ProcessClass(MemberDeclarationSyntax memberDecl, string fileName, SyntaxTree syntaxTree)
        {
            var classDecl = memberDecl as ClassDeclarationSyntax;
            if(classDecl == null)
            {
                throw new TemplatePreprocessException("Invalid class declaration");
            }
            var classAtr = classDecl.AttributeLists.Where(att => att.ToString().Contains("{{")).SelectMany(a => a.Attributes);
            var classMemb = classDecl.Members;
            var inputs = classMemb.Where(m => m.ToString().Contains("{{"));

            var attrResult = await ProcessAttributes(classAtr, fileName, syntaxTree);
            var inputResult = await ProcessInputs(inputs, fileName, syntaxTree);

            var result = attrResult.Concat(inputResult);
            return result;
        }

        private Task<IEnumerable<TemplateParam>> ProcessInputs(IEnumerable<MemberDeclarationSyntax> inputs, string fileName, SyntaxTree syntaxTree) 
        {
            return Task.FromResult(GetInputValues(inputs, fileName, syntaxTree));
        }
        private IEnumerable<TemplateParam> GetInputValues(IEnumerable<MemberDeclarationSyntax> inputs, string fileName, SyntaxTree syntaxTree)
        {
            foreach (var input in inputs)
            {
                object type = new();
                logger.LogInformation($"Processing input: {input}");

                switch (input.Kind())
                {
                    case SyntaxKind.FieldDeclaration:
                        var node = input.ChildNodesAndTokens().FirstOrDefault(n => n.Kind() == SyntaxKind.VariableDeclaration).AsNode();
                        var decl = node as VariableDeclarationSyntax;
                        if (decl == null)
                        {
                            logger.LogError("Variable declaration not found");
                            continue;
                        }

                        if (decl.Type.GetType() == typeof(IdentifierNameSyntax))
                        {
                            var declType = decl.Type as IdentifierNameSyntax;
                            if (declType == null || declType.Identifier.Value == null)
                            {
                                logger.LogError("Identifier type not found");
                                continue;
                            }

                            type = declType.Identifier.Value;
                        }
                        else if (decl.Type.GetType() == typeof(PredefinedTypeSyntax))
                        {
                            var declType = decl.Type as PredefinedTypeSyntax;
                            if (declType == null || declType.Keyword.Value == null)
                            {
                                logger.LogError("Predefined type not found");
                                continue;
                            }

                            type = declType.Keyword.Value;
                        }
                        break;
                    case SyntaxKind.MethodDeclaration:
                        var nodes = input.ChildNodesAndTokens();
                        var methodDecl = input as MethodDeclarationSyntax;
                        if(methodDecl == null)
                        {
                            logger.LogError("Method declaration not found");
                            continue;
                        }
                        var predefType = methodDecl.ReturnType as PredefinedTypeSyntax;
                        if (predefType == null || predefType.Keyword.Value == null)
                        {
                            logger.LogError("Method return type not found");
                            continue;
                        }

                        type = predefType.Keyword.Value;
                        break;
                    default:
                        logger.LogError($"Invalid input type: {input.Kind()}");
                        break;
                }

                var paramToAdd = CreateTemplateParam(input.ToString(), type, fileName, input.Span, syntaxTree);
                yield return paramToAdd;
            }
        }

        private Task<IEnumerable<TemplateParam>> ProcessAttributes(IEnumerable<AttributeSyntax> classAtr, string fileName, SyntaxTree syntaxTree)
        {
            return Task.FromResult(GetAttributeVals(classAtr, fileName, syntaxTree));
        }
        private IEnumerable<TemplateParam> GetAttributeVals(IEnumerable<AttributeSyntax> classAtr, string fileName, SyntaxTree syntaxTree)
        {
            foreach (var attr in classAtr)
            {
                logger.LogInformation($"Processing attribute: {attr}");
                if (attr.ArgumentList == null)
                {
                    logger.LogError("No arguments");
                    continue;
                }

                var attrVal = attr.ArgumentList.Arguments.First(a => a.ToString().Contains("{{"));
                var expType = attrVal.Expression.Kind().ToString().ToLower().Replace("literalexpression", "");
                var paramToAdd = CreateTemplateParam(attrVal.ToString(), expType, fileName, attrVal.Span, syntaxTree);
                yield return paramToAdd;
            }
        }

        private TemplateParam CreateTemplateParam(string name, object type, string fileName, TextSpan span, SyntaxTree syntaxTree) 
        {
            var typeStr = type.ToString() ?? "unknown";
            var lineSpanStart = syntaxTree.GetLineSpan(span).StartLinePosition;
            var pattern = new Regex(@"{{(?<nameString>\w+)}}");
            var match = pattern.Match(name);
            var nameRes = match.Groups["nameString"].Value;
            return new TemplateParam
            {
                Name = nameRes,
                Type = typeStr,
                Source = new ParamSourceInfo
                {
                    File = fileName,
                    Line = lineSpanStart.Line, 
                    Column = lineSpanStart.Character
                },
                Validation = new ParamValidation
                {
                    Type = typeStr,
                    DefaultValue = GetDefaultValue(typeStr, nameRes)
                }
            };
        }

        private object GetDefaultValue(string type, string nameSplitted)
        {
            switch (type)
            {
                case "int":
                    return default(int);
                case "UInt160":
                    return string.Empty;
                default:
                    return nameSplitted;
            }
        }
    }
    public struct PreprocessTemplateCommand : IRequest<IEnumerable<TemplateParam>>
    {
        public string SourceStr { get; set; }
        public string FileName { get; set; }
    }

    public class PreprocessTemplateHandler : IRequestHandler<PreprocessTemplateCommand, IEnumerable<TemplateParam>>
    {
        private readonly ITemplatePreprocessService templatePreprocessService;

        public PreprocessTemplateHandler(ITemplatePreprocessService templatePreprocessService)
        {
            this.templatePreprocessService = templatePreprocessService;
        }

        public async Task<IEnumerable<TemplateParam>> Handle(PreprocessTemplateCommand request, CancellationToken cancellationToken)
        {
            var templateParams = await templatePreprocessService.FindParams(request.SourceStr, request.FileName);
            return templateParams;
        }
    }

    public sealed class TemplatePreprocessException : SystemException
    {
        public TemplatePreprocessException(string? message) : base(message){ }
    }
}
