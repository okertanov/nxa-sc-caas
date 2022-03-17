using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NXA.SC.Caas.Services.Persist;
using NXA.SC.Caas.Services.Persist.Impl;
using NXA.SC.Caas.Services.Compiler;
using NXA.SC.Caas.Services.Compiler.Impl;
using NXA.SC.Caas.Services.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NXA.SC.Caas.Models;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using System.IO;
using NXA.SC.Caas.Services.Db;
using Microsoft.AspNetCore.Diagnostics;
using MediatR;
using System.Reflection;
using NXA.SC.Caas.Services;
using NXA.SC.Caas.Shared.Utils;
using NXA.SC.Caas.Services.Mq;

namespace NXA.SC.Caas
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddNodeServices();
            services.AddScoped<ITaskPersistService, TaskPersistService>();
            services.AddScoped<ICompilerService, CSharpCompilerService>();
            services.AddScoped<ICompilerService, SolidityCompilerService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDbSettings, DbSettings>();
            services.AddScoped<ITemplatePreprocessService, TemplatePreprocessService>();
            services.AddScoped<INodeInteropService, NodeInteropService>();
            services.AddSingleton<IMqService, MqService>();
            services.AddDbContext<ApiTokenContext>();
            services.AddAuthentication(TokenAuthOptions.DefaultScemeName)
                    .AddScheme<TokenAuthOptions, ApiTokenHandler>(TokenAuthOptions.DefaultScemeName, null);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NXA SC Caas", Version = "v1" });
                c.AddSecurityDefinition("Token", new OpenApiSecurityScheme { Type = SecuritySchemeType.ApiKey, Name = "Token", In = ParameterLocation.Header });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Token"
                                }
                            },
                            Enumerable.Empty<string>().ToList()
                        }
                    });
            });
            services.AddHealthChecks();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddHostedService<CompilerBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.Use((context, next) =>
            {
                context.Request.PathBase = new PathString("/api");
                return next();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NXA SC Caas v1"));
            app.UsePathBase("/api");
            app.UseExceptionHandler(c => c.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
                var lf = app.ApplicationServices.GetService<ILoggerFactory>();
                var logger = lf?.CreateLogger("exceptionHandlerLogger");
                logger?.LogDebug(exception?.StackTrace);
                await context.Response.WriteAsJsonAsync(exception?.Message);
            }));
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/Status");
            });
            app.UseFileServer();
            ApplicationLogging.LoggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
        }
    }
}
