using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;

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
        public string ContractSource { get; set; } = String.Empty;

        [DataMember]
        public string[] ContractCompileOptions { get; set; } = { };

        [Required]
        [ContractDictionary()]
        [DataMember]
        public Dictionary<string, object> ContractValues { get; set; } = new Dictionary<string, object>();

        public object GetNamedContractVal(ContractValueEnum contractValEnum)
        {
            return ContractValues[contractValEnum.ToString()];
        }
    }

    public enum ContractValueEnum
    {
        ContractName,
        SystemOwnerAddress,
        ContractAuthorAddress,
        ContractAuthorName,
        ContractAuthorEmail,
        ContractDescription,
        ContractSymbol,
        ContractDecimals,
        ContractInitialCoins
    }

    public class ContractDictionary : DefaultValueAttribute
    {
        public ContractDictionary()
            : base(new Dictionary<string, object>() {
                { ContractValueEnum.ContractName.ToString(), String.Empty },
                { ContractValueEnum.ContractSymbol.ToString(), String.Empty },
                { ContractValueEnum.SystemOwnerAddress.ToString(), String.Empty },
                { ContractValueEnum.ContractAuthorAddress.ToString(), String.Empty },
                { ContractValueEnum.ContractAuthorName.ToString(), String.Empty },
                { ContractValueEnum.ContractAuthorEmail.ToString(), String.Empty },
                { ContractValueEnum.ContractDescription.ToString(), String.Empty },
                { ContractValueEnum.ContractDecimals.ToString(), String.Empty },
                { ContractValueEnum.ContractInitialCoins.ToString(), String.Empty },
                {"any", "any"}
            })
        {
        }
    }
}
