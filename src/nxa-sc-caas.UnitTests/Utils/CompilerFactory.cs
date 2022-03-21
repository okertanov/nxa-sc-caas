using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NXA.SC.Caas;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Services.Compiler;
using NXA.SC.Caas.Services.Compiler.Impl;
using NXA.SC.Caas.Services.Db;
using NXA.SC.Caas.Services.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace nxa_sc_caas.UnitTests
{
    public class CompilerFactory
    {
        private static ILogger<CSharpCompilerService> _logger { get { return new LoggerFactory().CreateLogger<CSharpCompilerService>(); } set { } }

        public static ICompilerService CreateCompilerService()
        {
            var hostBuilder = Program.CreateHostBuilder(new string[] { });
            var host = hostBuilder.Build();
            var compilerService = host.Services.GetRequiredService<ICompilerService>();
            return compilerService;
        }
        public static CSharpCompilerService CreateCompilerServiceCSharp()
        {
            var compilerService = new CSharpCompilerService(_logger);
            return compilerService;
        }
        public static CompilerTask GetValidSmartContractTask()
        {
            var taskContractVals = new Dictionary<string, object>
            {
                { ContractValueEnum.ContractName.ToString(), "name123" },
            };
            var compilerTaskCreate = new CreateCompilerTask { ContractSource = @"
                using Neo;
                using Neo.SmartContract;
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Native;
                using Neo.SmartContract.Framework.Services;
                namespace ProjectName
                {
                    public class Contract1 : SmartContract
                    {
                        private static int privateMethod()
                        {
                            return 1;
                        }
                    }
                }",
                CompilerTaskType = CompilerTaskTypeEnum.CSHARP,
                ContractValues = taskContractVals
            };
            var task = new CompilerTask("123", CompilerTaskStatus.SCHEDULED, compilerTaskCreate, null, null);
            return task;
        }
        public static CompilerTask GetSmartContractTaskWithClassNameAsInput()
        {
            var taskContractVals = new Dictionary<string, object>
            {
                { ContractValueEnum.ContractName.ToString(), "name123" },
                { "class_name", "MOZ Coin" }
            };
            var compilerTaskCreate = new CreateCompilerTask
            {
                ContractSource = @"
                using Neo;
                using Neo.SmartContract;
                using Neo.SmartContract.Framework;
                using Neo.SmartContract.Framework.Native;
                using Neo.SmartContract.Framework.Services;
                namespace ProjectName
                {
                    public class {{class_name}} : SmartContract
                    {
                        private static int privateMethod()
                        {
                            return 1;
                        }
                    }
                }",
                CompilerTaskType = CompilerTaskTypeEnum.CSHARP,
                ContractValues = taskContractVals
            };
            var task = new CompilerTask("123", CompilerTaskStatus.SCHEDULED, compilerTaskCreate, null, null);
            return task;
        }
        public static CompilerTask GetInvalidSmartContractTask()
        {
            var taskContractVals = new Dictionary<string, object>
            {
                { ContractValueEnum.ContractName.ToString(), "name12" },
            };
            var compilerTaskCreate = new CreateCompilerTask { ContractSource = "invalidcontract", CompilerTaskType = CompilerTaskTypeEnum.CSHARP, ContractValues = taskContractVals };
            var task = new CompilerTask("1234", CompilerTaskStatus.SCHEDULED, compilerTaskCreate, null, null);
            return task;
        }
        public static CompilerTask GetSolidityValidContractTask()
        {
            var compilerTaskCreate = new CreateCompilerTask { ContractSource = Encoding.UTF8.GetString(Convert.FromBase64String(@"
Ly8gU1BEWC1MaWNlbnNlLUlkZW50aWZpZXI6IE1JVApwcmFnbWEgc29saWRpdHkgXjAuOC40OwoKaW1wb3J0ICJAb3BlbnplcHBlbGluL2NvbnRyYWN0cy11cGdyYWRlYWJsZS90b2tlbi9FUkM3MjEvZXh0ZW5zaW9ucy9FUkM3MjFVUklTdG9yYWdlVXBncmFkZWFibGUuc29sIjsKaW1wb3J0ICJAb3BlbnplcHBlbGluL2NvbnRyYWN0cy11cGdyYWRlYWJsZS9hY2Nlc3MvQWNjZXNzQ29udHJvbFVwZ3JhZGVhYmxlLnNvbCI7CmltcG9ydCAiQG9wZW56ZXBwZWxpbi9jb250cmFjdHMtdXBncmFkZWFibGUvc2VjdXJpdHkvUmVlbnRyYW5jeUd1YXJkVXBncmFkZWFibGUuc29sIjsKaW1wb3J0ICJAb3BlbnplcHBlbGluL2NvbnRyYWN0cy11cGdyYWRlYWJsZS9zZWN1cml0eS9QYXVzYWJsZVVwZ3JhZGVhYmxlLnNvbCI7CmltcG9ydCAiQG9wZW56ZXBwZWxpbi9jb250cmFjdHMtdXBncmFkZWFibGUvcHJveHkvdXRpbHMvSW5pdGlhbGl6YWJsZS5zb2wiOwppbXBvcnQgIkBvcGVuemVwcGVsaW4vY29udHJhY3RzLXVwZ3JhZGVhYmxlL3Byb3h5L3V0aWxzL1VVUFNVcGdyYWRlYWJsZS5zb2wiOwppbXBvcnQgIkBvcGVuemVwcGVsaW4vY29udHJhY3RzLXVwZ3JhZGVhYmxlL3V0aWxzL0NvdW50ZXJzVXBncmFkZWFibGUuc29sIjsKCmNvbnRyYWN0IFRhbnRhbGlzTkZUIGlzIEluaXRpYWxpemFibGUsIEVSQzcyMVVSSVN0b3JhZ2VVcGdyYWRlYWJsZSwgUGF1c2FibGVVcGdyYWRlYWJsZSwgQWNjZXNzQ29udHJvbFVwZ3JhZGVhYmxlLCBSZWVudHJhbmN5R3VhcmRVcGdyYWRlYWJsZSwgVVVQU1VwZ3JhZGVhYmxlIHsKICAgIHVzaW5nIENvdW50ZXJzVXBncmFkZWFibGUgZm9yIENvdW50ZXJzVXBncmFkZWFibGUuQ291bnRlcjsKICAgIENvdW50ZXJzVXBncmFkZWFibGUuQ291bnRlciBwcml2YXRlIF90b2tlbklkQ291bnRlcjsKICAgIAogICAgYnl0ZXMzMiBwdWJsaWMgY29uc3RhbnQgQURNSU5fUk9MRSA9IGtlY2NhazI1NigiQURNSU5fUk9MRSIpOwogICAgYnl0ZXMzMiBwdWJsaWMgY29uc3RhbnQgUEFVU0VSX1JPTEUgPSBrZWNjYWsyNTYoIlBBVVNFUl9ST0xFIik7CiAgICBieXRlczMyIHB1YmxpYyBjb25zdGFudCBVUEdSQURFUl9ST0xFID0ga2VjY2FrMjU2KCJVUEdSQURFUl9ST0xFIik7CgogICAgbWFwcGluZyhhZGRyZXNzID0+IGJvb2wpIHB1YmxpYyBibG9ja0xpc3Q7CgogICAgLyoqCiAgICAqIEBub3RpY2UgRXZlbnQgZmlyaW5nIHdoZW5ldmVyIGJsb2NrbGlzdCB1cGRhdGVzCiAgICAqIEBwYXJhbSB1c2VyIHVzZXIgYWRkcmVzcwogICAgKi8KICAgIGV2ZW50IEJsb2NrTGlzdFVwZGF0ZWQoYWRkcmVzcyBpbmRleGVkIHVzZXIsIGJvb2wgdmFsdWUpOwogICAgCiAgICAvKioKICAgICogQGRldiBOZWVkIHRvIGJlIGluaXRpYWxpemVkLiAKICAgICogRm9yIG1vcmUgaW5mb3JtYXRpb246IGh0dHBzOi8vZG9jcy5vcGVuemVwcGVsaW4uY29tL3VwZ3JhZGVzLXBsdWdpbnMvMS54L3dyaXRpbmctdXBncmFkZWFibGUjaW5pdGlhbGl6aW5nX3RoZV9pbXBsZW1lbnRhdGlvbl9jb250cmFjdAogICAgKiBAY3VzdG9tOm96LXVwZ3JhZGVzLXVuc2FmZS1hbGxvdyBjb25zdHJ1Y3RvcgogICAgKi8KICAgIGNvbnN0cnVjdG9yKCkgaW5pdGlhbGl6ZXIge30KCiAgICAvKioKICAgICogQG5vdGljZSBDcmVhdGluZyBUYW50YWxpcyBORlQKICAgICogQGRldiBDYWxscyBvbmx5IG9uY2UKICAgICogQHBhcmFtIG5hbWUgbmFtZSBvZiB0aGUgdG9rZW4KICAgICogQHBhcmFtIHN5bWJvbCBzeW1ib2wgb2YgdGhlIHRva2VuCiAgICAqLwogICAgZnVuY3Rpb24gaW5pdGlhbGl6ZShzdHJpbmcgY2FsbGRhdGEgbmFtZSwgc3RyaW5nIGNhbGxkYXRhIHN5bWJvbCkgaW5pdGlhbGl6ZXIgcHVibGljIHsKICAgICAgICBfX0VSQzcyMV9pbml0KG5hbWUsIHN5bWJvbCk7CiAgICAgICAgX19FUkM3MjFVUklTdG9yYWdlX2luaXQoKTsKICAgICAgICBfX1BhdXNhYmxlX2luaXQoKTsKICAgICAgICBfX0FjY2Vzc0NvbnRyb2xfaW5pdCgpOwogICAgICAgIF9fUmVlbnRyYW5jeUd1YXJkX2luaXQoKTsKICAgICAgICBfX1VVUFNVcGdyYWRlYWJsZV9pbml0KCk7CiAgICAgICAgCiAgICAgICAgX3NldHVwUm9sZShERUZBVUxUX0FETUlOX1JPTEUsIG1zZy5zZW5kZXIpOwogICAgICAgIF9zZXR1cFJvbGUoQURNSU5fUk9MRSwgbXNnLnNlbmRlcik7CiAgICAgICAgX3NldHVwUm9sZShQQVVTRVJfUk9MRSwgbXNnLnNlbmRlcik7CiAgICAgICAgX3NldHVwUm9sZShVUEdSQURFUl9ST0xFLCBtc2cuc2VuZGVyKTsKICAgICAgICBfc2V0Um9sZUFkbWluKFBBVVNFUl9ST0xFLCBBRE1JTl9ST0xFKTsKICAgICAgICBfc2V0Um9sZUFkbWluKFVQR1JBREVSX1JPTEUsIEFETUlOX1JPTEUpOwogICAgfQoKICAgIC8qKgogICAgKiBAbm90aWNlIFBhdXNpbmcgZnVuY3Rpb24KICAgICogQGRldiBvbmx5IHVzZXJzIHdpdGggUGF1c2VyIHJvbGUgY2FuIGNhbGwgdGhpcyBmdW5jdGlvbgogICAgKi8KICAgIGZ1bmN0aW9uIHBhdXNlKCkgcHVibGljIHsKICAgICAgICByZXF1aXJlKGhhc1JvbGUoUEFVU0VSX1JPTEUsIG1zZy5zZW5kZXIpLCAiWW91IHNob3VsZCBoYXZlIGEgcGF1c2VyIHJvbGUiKTsKCiAgICAgICAgX3BhdXNlKCk7CiAgICB9CgogICAgLyoqCiAgICAqIEBub3RpY2UgVW5wYXVzaW5nIGZ1bmN0aW9uCiAgICAqIEBkZXYgb25seSB1c2VycyB3aXRoIFBhdXNlciByb2xlIGNhbiBjYWxsIHRoaXMgZnVuY3Rpb24KICAgICovCiAgICBmdW5jdGlvbiB1bnBhdXNlKCkgcHVibGljIHsKICAgICAgICByZXF1aXJlKGhhc1JvbGUoUEFVU0VSX1JPTEUsIG1zZy5zZW5kZXIpLCAiWW91IHNob3VsZCBoYXZlIGEgcGF1c2VyIHJvbGUiKTsKCiAgICAgICAgX3VucGF1c2UoKTsKICAgIH0KCiAgICAvKioKICAgICogQG5vdGljZSBVcGRhdGluZyB1c2VycyBibG9ja2xpc3QKICAgICogQGRldiBvbmx5IHVzZXJzIHdpdGggQWRtaW4gcm9sZSBjYW4gY2FsbCB0aGlzIGZ1bmN0aW9uCiAgICAqIEBwYXJhbSBfdXNlciB1c2VyIGFkZHJlc3MKICAgICogQHBhcmFtIF92YWx1ZSB1c2VyIGFkZHJlc3MKICAgICovCiAgICBmdW5jdGlvbiBibG9ja0xpc3RVcGRhdGUoYWRkcmVzcyBfdXNlciwgYm9vbCBfdmFsdWUpIHB1YmxpYyB7CiAgICAgICAgcmVxdWlyZShoYXNSb2xlKEFETUlOX1JPTEUsIG1zZy5zZW5kZXIpLCAiWW91IHNob3VsZCBoYXZlIGFuIGFkbWluIHJvbGUiKTsKCiAgICAgICAgYmxvY2tMaXN0W191c2VyXSA9IF92YWx1ZTsKICAgICAgICBlbWl0IEJsb2NrTGlzdFVwZGF0ZWQoX3VzZXIsIF92YWx1ZSk7CiAgICB9CgogICAgLyoqCiAgICAqIEBub3RpY2UgQ3JlYXRpbmcgbmV3IE5GVAogICAgKiBAcGFyYW0gX3RvIGFkZHJlc3Mgd2hlcmUgbmV3IE5GVCB3aWxsIGJlIG1pbnRlZAogICAgKiBAcGFyYW0gX3Rva2VuVVJJIHVyaSB0byB0aGUgbWV0YWRhdGUgb3IgbWV0YWRhdGUgb2YgdGhlIHRva2VuCiAgICAqIEByZXR1cm4gbmV3bHkgY3JlYXRlZCB0b2tlbiBpZAogICAgKi8KICAgIGZ1bmN0aW9uIGNyZWF0ZVRva2VuKGFkZHJlc3MgX3RvLCBzdHJpbmcgbWVtb3J5IF90b2tlblVSSSkgZXh0ZXJuYWwgbm9uUmVlbnRyYW50IHJldHVybnMgKHVpbnQyNTYpIHsKICAgICAgICByZXR1cm4gX2NyZWF0ZVRva2VuKF90bywgX3Rva2VuVVJJKTsKICAgIH0KCiAgICAvKioKICAgICogQG5vdGljZSBDcmVhdGluZyBiYXRjaCBvZiBuZXcgTkZUcwogICAgKiBAcGFyYW0gX3RvIGFkZHJlc3NlcyB3aGVyZSBuZXcgTkZUcyB3aWxsIGJlIG1pbnRlZAogICAgKiBAcGFyYW0gX3Rva2Vuc1VSSSB1cmlzIGFycmF5IGZvciBtZXRhZGF0YSBvciB0b2tlbiBtZXRhZGF0YSBhcnJheQogICAgKiBAcmV0dXJuIG5ld2x5IGNyZWF0ZWQgYXJyYXkgb2YgdG9rZW4gaWRzCiAgICAqLwogICAgZnVuY3Rpb24gY3JlYXRlQmF0Y2hUb2tlbnMoYWRkcmVzc1tdIG1lbW9yeSBfdG8sIHN0cmluZ1tdIG1lbW9yeSBfdG9rZW5zVVJJKSBleHRlcm5hbCBub25SZWVudHJhbnQgcmV0dXJucyAodWludDI1NltdIG1lbW9yeSkgewogICAgICAgIHJlcXVpcmUoX3Rva2Vuc1VSSS5sZW5ndGggPiAwLCAiWW91IG5lZWQgdG8gcGFzcyBhdCBsZWFzdCBvbmUgdG9rZW4gVVJJIik7CiAgICAgICAgcmVxdWlyZShfdG8ubGVuZ3RoID09IF90b2tlbnNVUkkubGVuZ3RoLCAiVXNlcnMgYXJyYXkgbGVuZ3RoIG11c3QgYmUgZXF1YWwgdG8gdG9rZW5zIFVSSSBhcnJheSBsZW5ndGgiKTsKCiAgICAgICAgdWludDI1NltdIG1lbW9yeSBuZXdseUNyZWF0ZWRUb2tlbnMgPSBuZXcgdWludDI1NltdKF90b2tlbnNVUkkubGVuZ3RoKTsKCiAgICAgICAgZm9yKHVpbnQyNTYgaSA9IDA7IGkgPCBfdG9rZW5zVVJJLmxlbmd0aDsgaSsrKSB7CiAgICAgICAgICAgIG5ld2x5Q3JlYXRlZFRva2Vuc1tpXSA9IF9jcmVhdGVUb2tlbihfdG9baV0sIF90b2tlbnNVUklbaV0pOwogICAgICAgIH0KCiAgICAgICAgcmV0dXJuIG5ld2x5Q3JlYXRlZFRva2VuczsKICAgIH0KCiAgICAvKioKICAgICogQG5vdGljZSBCYXRjaCBUcmFuc2ZlciBvZiBORlRzCiAgICAqIEBwYXJhbSBfZnJvbSBhZGRyZXNzIHNlbmRpbmcgZnJvbQogICAgKiBAcGFyYW0gX3RvIGFkZHJlc3Mgc2VuZGluZyB0bwogICAgKiBAcGFyYW0gX2lkcyB0b2tlbiBpZHMKICAgICovCiAgICBmdW5jdGlvbiBzYWZlQmF0Y2hUcmFuc2ZlckZyb20oYWRkcmVzcyBfZnJvbSwgYWRkcmVzcyBfdG8sIHVpbnQyNTZbXSBtZW1vcnkgX2lkcykgZXh0ZXJuYWwgbm9uUmVlbnRyYW50IHsKICAgICAgICByZXF1aXJlKF9mcm9tID09IG1zZy5zZW5kZXIgfHwgaXNBcHByb3ZlZEZvckFsbChfZnJvbSwgbXNnLnNlbmRlciksICJUcmFuc2ZlciBjYWxsZXIgaXMgbm90IG93bmVyIG5vciBhcHByb3ZlZCBmb3IgYWxsIHRva2VucyIpOwogICAgICAgIHJlcXVpcmUoX2lkcy5sZW5ndGggPiAwLCAiWW91IG5lZWQgdG8gcGFzcyBhdCBsZWFzdCBvbmUgdG9rZW4gdG8gdHJhbnNmZXIiKTsKICAgICAgICAKICAgICAgICBmb3IodWludDI1NiBpID0gMDsgaSA8IF9pZHMubGVuZ3RoOyBpKyspIHsKICAgICAgICAgICAgX3NhZmVUcmFuc2ZlcihfZnJvbSwgX3RvLCBfaWRzW2ldLCAiIik7CiAgICAgICAgfQogICAgfQoKICAgIGZ1bmN0aW9uIF9jcmVhdGVUb2tlbihhZGRyZXNzIF90bywgc3RyaW5nIG1lbW9yeSBfdG9rZW5VUkkpIGludGVybmFsIHJldHVybnMgKHVpbnQyNTYpIHsKICAgICAgICBfc2FmZU1pbnQoX3RvLCBfdG9rZW5JZENvdW50ZXIuY3VycmVudCgpKTsKICAgICAgICBfc2V0VG9rZW5VUkkoX3Rva2VuSWRDb3VudGVyLmN1cnJlbnQoKSwgX3Rva2VuVVJJKTsKICAgICAgICBfdG9rZW5JZENvdW50ZXIuaW5jcmVtZW50KCk7CgogICAgICAgIHJldHVybiBfdG9rZW5JZENvdW50ZXIuY3VycmVudCgpIC0gMTsKICAgIH0KCiAgICBmdW5jdGlvbiBfYXV0aG9yaXplVXBncmFkZShhZGRyZXNzIG5ld0ltcGxlbWVudGF0aW9uKQogICAgICAgIGludGVybmFsCiAgICAgICAgb25seVJvbGUoVVBHUkFERVJfUk9MRSkKICAgICAgICBvdmVycmlkZQogICAge30KCiAgICBmdW5jdGlvbiBfYmVmb3JlVG9rZW5UcmFuc2ZlcihhZGRyZXNzIF9mcm9tLCBhZGRyZXNzIF90bywgdWludDI1NiBfdG9rZW5JZCkKICAgICAgICBpbnRlcm5hbAogICAgICAgIHdoZW5Ob3RQYXVzZWQKICAgICAgICBvdmVycmlkZQogICAgewogICAgICAgIHJlcXVpcmUgKCFibG9ja0xpc3RbX2Zyb21dLCAiVG9rZW4gc2VuZGVyIGlzIGluIHRoZSBibG9jayBsaXN0Iik7CiAgICAgICAgcmVxdWlyZSAoIWJsb2NrTGlzdFtfdG9dLCAiVG9rZW4gcmVjZWl2ZXIgaXMgaW4gdGhlIGJsb2NrIGxpc3QiKTsKCiAgICAgICAgc3VwZXIuX2JlZm9yZVRva2VuVHJhbnNmZXIoX2Zyb20sIF90bywgX3Rva2VuSWQpOwogICAgfQoKICAgIC8qKgogICAgKiBAbm90aWNlIFRoZSBmb2xsb3dpbmcgZnVuY3Rpb24gb3ZlcnJpZGUgaXMgcmVxdWlyZWQgZm9yIFNvbGlkaXR5CiAgICAqIEBkZXYgU2VlIHtJRVJDMTY1LXN1cHBvcnRzSW50ZXJmYWNlfS4KICAgICogQHBhcmFtIF9pbnRlcmZhY2VJZCBpbnRlcmZhY2UgaWQKICAgICovCiAgICBmdW5jdGlvbiBzdXBwb3J0c0ludGVyZmFjZShieXRlczQgX2ludGVyZmFjZUlkKQogICAgICAgIHB1YmxpYwogICAgICAgIHZpZXcKICAgICAgICBvdmVycmlkZShFUkM3MjFVcGdyYWRlYWJsZSwgQWNjZXNzQ29udHJvbFVwZ3JhZGVhYmxlKQogICAgcmV0dXJucyAoYm9vbCkKICAgIHsKICAgICAgICByZXR1cm4gc3VwZXIuc3VwcG9ydHNJbnRlcmZhY2UoX2ludGVyZmFjZUlkKTsKICAgIH0KfQo=
                ")), CompilerTaskType = CompilerTaskTypeEnum.SOLIDITY };
            var task = new CompilerTask("1234", CompilerTaskStatus.SCHEDULED, compilerTaskCreate, null, null);
            return task;
        }
        public static CompilerTask GetSolidityInvalidContractTask()
        {
            var compilerTaskCreate = new CreateCompilerTask { ContractSource = Encoding.UTF8.GetString(Convert.FromBase64String(@"
CnByYWdtYSBzb2xpZGl0eSBeMC44LjQ7CgppbXBvcnQgIkBvcGVuemVwcGVsaW4vY29udHJhY3RzLXVwZ3JhZGVhYmxlL3Rva2VuL0VSQzcyMS9leHRlbnNpb25zL0VSQzcyMVVSSVN0b3JhZ2VVcGdyYWRlYWJsZS5zb2wiOwppbXBvcnQgIkBvcGVuemVwcGVsaW4vY29udHJhY3RzLXVwZ3JhZGVhYmxlL2FjY2Vzcy9BY2Nlc3NDb250cm9sVXBncmFkZWFibGUuc29sIjsKaW1wb3J0ICJAb3BlbnplcHBlbGluL2NvbnRyYWN0cy11cGdyYWRlYWJsZS9zZWN1cml0eS9SZWVudHJhbmN5R3VhcmRVcGdyYWRlYWJsZS5zb2wiOwppbXBvcnQgIkBvcGVuemVwcGVsaW4vY29udHJhY3RzLXVwZ3JhZGVhYmxlL3NlY3VyaXR5L1BhdXNhYmxlVXBncmFkZWFibGUuc29sIjsKaW1wb3J0ICJAb3BlbnplcHBlbGluL2NvbnRyYWN0cy11cGdyYWRlYWJsZS9wcm94eS91dGlscy9Jbml0aWFsaXphYmxlLnNvbCI7CmltcG9ydCAiQG9wZW56ZXBwZWxpbi9jb250cmFjdHMtdXBncmFkZWFibGUvcHJveHkvdXRpbHMvVVVQU1VwZ3JhZGVhYmxlLnNvbCI7CmltcG9ydCAiQG9wZW56ZXBwZWxpbi9jb250cmFjdHMtdXBncmFkZWFibGUvdXRpbHMvQ291bnRlcnNVcGdyYWRlYWJsZS5zb2wiOwoKY29udHJhY3QgSW52YWxpZFRlc3RDb250cmFjdCBpcyBJbml0aWFsaXphYmxlLCBFUkM3MjFVUklTdG9yYWdlVXBncmFkZWFibGUsIFBhdXNhYmxlVXBncmFkZWFibGUsIEFjY2Vzc0NvbnRyb2xVcGdyYWRlYWJsZSwgUmVlbnRyYW5jeUd1YXJkVXBncmFkZWFibGUsIFVVUFNVcGdyYWRlYWJsZSB7CiAgICB1c2luZyBDb3VudGVyc1VwZ3JhZGVhYmxlIGZvciBDb3VudGVyc1VwZ3JhZGVhYmxlLkNvdW50ZXI7ICAgIAoKICAgIGNvbnN0cnVjdG9yKCkgaW5pdGlhbGl6ZXIge30KCn0K
                ")), CompilerTaskType = CompilerTaskTypeEnum.SOLIDITY };
            var task = new CompilerTask("123", CompilerTaskStatus.SCHEDULED, compilerTaskCreate, null, null);
            return task;
        }
    }
}
