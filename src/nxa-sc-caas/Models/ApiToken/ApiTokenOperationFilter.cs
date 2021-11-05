using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NXA.SC.Caas.Models
{
    public sealed class ApiTokenOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var allowAnonymous = context.ApiDescription
                .CustomAttributes()
                .Any(attr => attr.GetType() == typeof(AllowAnonymousAttribute));

            if (!allowAnonymous)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Token",
                    In = ParameterLocation.Header,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                    }
                });
            }
        }
    }
}
