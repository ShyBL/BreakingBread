using System.Collections.Generic;
using UnityEngine.Analytics;

namespace Base.Core.Managers
{
    public class UnityAnalyticAdapter : IAnalyticAdapter
    {
        public void SendAnalytics(string eventName, Dictionary<string, object> data)
        {
            Analytics.CustomEvent(eventName, data);
        }
    }
}