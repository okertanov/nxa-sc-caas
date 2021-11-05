using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Extensions
{
    public static class CompilerTaskExtensions
    {
        public static CompilerTask SetIdentifier(this CompilerTask task, string identifier)
        {
            var newTask = new CompilerTask(identifier, task.Status, task.Create, task.Result, task.Error);
            return newTask;
        }

        public static CompilerTask SetStatus(this CompilerTask task, CompilerTaskStatus status)
        {
            var newTask = new CompilerTask(task.Identifier, status, task.Create, task.Result, task.Error);
            return newTask;
        }

        public static CompilerTask SetCreate(this CompilerTask task, CreateCompilerTask create)
        {
            var newTask = new CompilerTask(task.Identifier, task.Status, create, task.Result, task.Error);
            return newTask;
        }

        public static CompilerTask SetResult(this CompilerTask task, CompilerResult result)
        {
            var newTask = new CompilerTask(task.Identifier, task.Status, task.Create, result, task.Error);
            return newTask;
        }

        public static CompilerTask SetError(this CompilerTask task, CompilerError error)
        {
            var newTask = new CompilerTask(task.Identifier, task.Status, task.Create, task.Result, error);
            return newTask;
        }
    }
}
