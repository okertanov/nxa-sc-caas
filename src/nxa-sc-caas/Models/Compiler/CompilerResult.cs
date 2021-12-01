using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace NXA.SC.Caas.Models
{
    public sealed class CompilerResult
    {
        [Required]
        [DataMember]
        public byte[] NefScript { get; private set; } = {};

        [Required]
        [DataMember]
        public byte[] NefImage { get; private set; } = {};

        [Required]
        [DataMember]
        public string NefScriptBase64 => Convert.ToBase64String(NefScript);

        [Required]
        [DataMember]
        public string NefImageBase64 => Convert.ToBase64String(NefImage);

        [Required]
        [DataMember]
        public string Manifest { get; private set; } = String.Empty;

        public CompilerResult(byte[] nefScript, byte[] nefImage, string manifest)
        {
            NefScript = nefScript;
            Manifest = manifest;
            NefImage = nefImage;
        }
    }
}
