using System;
using Firebase.Crashlytics;

namespace Base.Core.Managers
{
    public class MonitorManager : BaseManager
    {
        public MonitorManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            OnInitComplete();
        }

        public void ReportException(string message)
        {
            var exc = new Exception(message);
            Crashlytics.LogException(exc);
            MyDebug.LogException(exc);
        }
    }
}