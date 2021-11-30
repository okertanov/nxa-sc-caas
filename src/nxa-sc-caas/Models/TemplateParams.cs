using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace NXA.SC.Caas.Models
{
    public class TemplateParam
    {
        [DataMember]
        public string Name { get; set; } = String.Empty;
        [DataMember]
        public string Type { get; set; } = String.Empty;
        [DataMember]
        public ParamSourceInfo? Source { get; set; } = null;
        [DataMember]
        public ParamValidation? Validation { get; set; } = null;
    }
    public class ParamSourceInfo
    {
        [DataMember]
        public string File { get; set; } = String.Empty;
        [DataMember]
        public int Line { get; set; }
        [DataMember]
        public int Column { get; set; }
    }
    public class ParamValidation
    {
        [DataMember]
        public string Type { get; set; } = String.Empty;
        [DataMember]
        public object? DefaultValue { get; set; } = null;
    }
}
