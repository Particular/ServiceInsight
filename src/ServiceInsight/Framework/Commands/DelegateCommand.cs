namespace ServiceInsight.Framework.Commands
{
    using System;

    public class DelegateCommand : BaseCommand
    {
        Action<object> execute;

        public DelegateCommand(Action<object> execute)
        {
            this.execute = execute;
        }

        public override void Execute(object parameter) => execute(parameter);
    }
}