using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NXA.SC.Caas.Models
{
    public enum CompilerTaskStatus
    {
        CREATED,
        SCHEDULED,
        PROCESSED,
        FAILED,
        DELETED
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CompilerTaskTypeEnum
    {
        SOLIDITY,
        CSHARP
    }
}
