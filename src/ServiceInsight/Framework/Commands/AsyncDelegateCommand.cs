namespace ServiceInsight.Framework.Commands
{
    using System;
    using System.Threading.Tasks;

    public class AsyncDelegateCommand : BaseCommand
    {
        Func<object, Task> execute;

        public AsyncDelegateCommand(Func<object, Task> execute)
        {
            this.execute = execute;
        }

        public override async void Execute(object parameter) => await execute(parameter);
    }
}