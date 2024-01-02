using System;
using System.Collections.Generic;

namespace Base.Core.Managers
{
    public class AnalyticsManager : BaseManager
    {
        private List<IAnalyticAdapter> analyticAdapters = new ();
        
        public AnalyticsManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            analyticAdapters.Add(new UnityAnalyticAdapter());
            analyticAdapters.Add(new FirebaseAnalyticAdapter());

            OnInitComplete();
        }

        public void SendAnalytics(AnalyticsEventName eventName, Dictionary<string, object> data)
        {
            foreach (var adapter in analyticAdapters)
            {
                adapter.SendAnalytics(eventName.ToString(), data);
            }
        }
    }
    
    public interface IAnalyticAdapter
    {
        public void SendAnalytics(string eventName, Dictionary<string, object> data);
    }
    
    public enum AnalyticsEventName
    {
        Research,
        Upgrade,
        AdDisplayed,
        Milestone
    }
}