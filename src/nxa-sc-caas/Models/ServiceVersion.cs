using System;
using System.Reflection;

namespace NXA.SC.Caas.Models {
    public sealed class ServiceVersion
    {
        public string Api { get; set; } = String.Empty;
        public string Compiler { get; set; } = String.Empty;
        public string Flamework { get; set; } = String.Empty;
        public string Abi { get; set; } = String.Empty;
        
        public ServiceVersion() 
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Api = assembly.GetCustomAttribute<ApiVersionAttribute>()!.Version;
            Compiler = assembly.GetCustomAttribute<CompilerVersionAttribute>()!.Version;
            Flamework = assembly.GetCustomAttribute<FlameworkVersionAttribute>()!.Version;
            Abi = assembly.GetCustomAttribute<AbiVersionAttribute>()!.Version;
        }
    }

    public sealed class ApiVersionAttribute : Attribute
    {
        public string Version = String.Empty;
    }

    public sealed class CompilerVersionAttribute : Attribute
    {
        public string Version = String.Empty;
    }

    public sealed class FlameworkVersionAttribute : Attribute
    {
        public string Version = String.Empty;
    }

    public sealed class AbiVersionAttribute : Attribute
    {
        public string Version = String.Empty;
    }
}
