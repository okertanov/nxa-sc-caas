using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace NXA.SC.Caas.Models
{
    [DataContract]
    public sealed class CreateCompilerTask
    {
        [Required]
        [DataMember]
        public CompilerTaskTypeEnum CompilerTaskType { get; set; }

        [Required]
        [DataMember]
        public string SystemOwnerAddress { get; set; } = String.Empty;

        [Required]
        [DataMember]
        public string ContractAuthorAddress { get; set; } = String.Empty;

        [Required]
        [DataMember]
        public string ContractAuthorName { get; set; } = String.Empty;

        [Required]
        [DataMember]
        public string ContractAuthorEmail { get; set; } = String.Empty;

        [Required]
        [DataMember]
        public string ContractName { get; set; } = String.Empty;

        [Required]
        [DataMember]
        public string ContractDescription { get; set; } = String.Empty;

        [Required]
        [DataMember]
        public string ContractSymbol { get; set; } = String.Empty;

        [Required]
        [DataMember]
        public string ContractDecimals { get; set; } = String.Empty;

        [Required]
        [DataMember]
        public string ContractInitialCoins { get; set; } = String.Empty;

        [Required]
        [DataMember]
        public string ContractSource { get; set; } = String.Empty;

        [DataMember]
        public string[] ContractCompileOptions { get; set; } = {};
    }
}
