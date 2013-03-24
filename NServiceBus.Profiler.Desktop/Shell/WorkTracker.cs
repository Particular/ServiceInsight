using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.Shell
{
//    public class WorkTracker : 
//        PropertyChangedBase,
//        IWorkTracker,
//        IHandle<WorkStarted>,
//        IHandle<WorkFinished>
//    {
//        private int _workCounter;
//
//        public bool WorkInProgress 
//        {
//            get { return _workCounter > 0; }
//        }
//
//        public void Start()
//        {
//            _workCounter++;
//            NotifyOfPropertyChange("WorkInProgress");
//        }
//
//        public void Stop()
//        {
//            if (_workCounter > 0)
//                _workCounter--;
//
//            NotifyOfPropertyChange("WorkInProgress");
//        }
//
//        public void Handle(WorkStarted message)
//        {
//            Start();
//        }
//
//        public void Handle(WorkFinished message)
//        {
//            Stop();
//        }
//    }
}