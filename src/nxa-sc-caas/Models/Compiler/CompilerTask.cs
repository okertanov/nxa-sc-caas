using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace NXA.SC.Caas.Models
{
    public sealed class CompilerTask : IScheduledTask
    {
        [Required]
        [DataMember]
        public string Identifier { get; private set; } = String.Empty;

        [Required]
        [DataMember]
        public CompilerTaskStatus Status { get; private set; } = CompilerTaskStatus.CREATED;

        [DataMember]
        public CreateCompilerTask Create { get; private set; } = default!;

        [DataMember]
        public CompilerResult? Result { get; private set; } = null;

        [DataMember]
        public CompilerError? Error { get; private set; } = null;

        public CompilerTask(): this(String.Empty, CompilerTaskStatus.CREATED, new CreateCompilerTask(), null, null)
        { 
        }

        public CompilerTask(
            string identifier,
            CompilerTaskStatus status,
            CreateCompilerTask create,
            CompilerResult? result,
            CompilerError? error
        ) {
            Identifier = identifier;
            Status = status;
            Create = create;
            Result = result;
            Error = error;
        }
    }
}
