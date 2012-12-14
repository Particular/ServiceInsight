using System;
using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Tests.Helpers
{
    public class TestConductorScreen : Conductor<IScreen>
    {
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        public override void DeactivateItem(IScreen item, bool close)
        {
            item.Deactivate(close);
            base.DeactivateItem(item, close);
        }

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }

        public override void TryClose(bool? dialogResult)
        {
            base.TryClose(dialogResult);
        }
    }
}