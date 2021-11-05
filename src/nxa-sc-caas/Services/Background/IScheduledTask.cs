namespace NXA.SC.Caas.Models
{
    public interface IScheduledTask
    {
        string Identifier { get; }
        CompilerTaskStatus Status { get; }
    }
}
