using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace NXA.SC.Caas.Models
{
    public sealed class CompilerError
    {
        [Required]
        [DataMember]
        public string File { get; private set; } = String.Empty;

        [Required]
        [DataMember]
        public uint Line { get; private set; } = 0;

        [Required]
        [DataMember]
        public string Code { get; private set; } = String.Empty;

        [Required]
        [DataMember]
        public string Messsage { get; private set; } = String.Empty;

        [DataMember]
        public string? Trace { get; private set; } = null;

        public CompilerError(string file, uint line, string code, string messsage, string? trace) {
            File = file;
            Line = line;
            Code = code;
            Messsage = messsage;
            Trace = trace;
        }
    }
}
