using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace NXA.SC.Caas.Models {
    public sealed class CompilerResult {
        [Required]
        [DataMember]
        public byte[] Nef { get; private set; } = {};

        [Required]
        [DataMember]
        public string NefBase64 => Convert.ToBase64String(Nef);

        [Required]
        [DataMember]
        public string Manifest { get; private set; } = String.Empty;

        public CompilerResult(byte[] nef, string manifest) {
            Nef = nef;
            Manifest = manifest;
        }
    }
}
