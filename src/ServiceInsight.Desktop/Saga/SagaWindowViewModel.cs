using Caliburn.PresentationFramework.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Saga
{
    public class SagaWindowViewModel : Screen, ISagaWindowViewModel
    {
        private ISagaWindowView _view;

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (ISagaWindowView)view;

            this.Name = "OrderProcessed";
        }

        public string Name { get; private set; }

        public Guid Guid { get; private set; }
    }

    public interface ISagaWindowViewModel : IScreen
    {
    }
}
