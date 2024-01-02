using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Base.Core.Managers
{
    public class TimeManager : BaseManager
    {
        private bool isLooping;
        private List<AlarmData> activeAlarms = new();
        private OfflineTimeData offlineTimeData;
        public int OfflineSeconds;
        private int tickSeconds = 1;

        public TimeManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            InitializeOfflineTime();
            
            OnInitComplete();
        }
        private void InitializeOfflineTime()
        {
            offlineTimeData = GameManager.SaveManager.LoadDataAndCreateIfNull<OfflineTimeData>();
            CheckOfflineTime();
        }
        
        private void CheckOfflineTime()
        {
            var timePassed = DateTime.Now - offlineTimeData.LastTimeTest;
            
            OfflineSeconds = (int)timePassed.TotalSeconds;
            
            offlineTimeData.LastTimeTest = DateTime.Now;
            offlineTimeData.SaveData();
        }

        public async Task TimerLoop()
        {
            isLooping = true;
            while (isLooping)
            {
                MyDebug.Log($"{typeof(TimeManager)} is Delaying 1000 milliseconds");
                
                await Task.Delay(TimeSpan.FromSeconds(tickSeconds));
                GameManager.EventsManager.InvokeEvent(EventType.Tick,null);
            }
            
            isLooping = false;
        }
        
        private void InvokeTime()
        {
            for (var i = 0; i < activeAlarms.Count; i++)
            {
                var currentAlarm = activeAlarms[i];
                if (DateTime.Compare(currentAlarm.InvokeTime, DateTime.Now) < 0)
                {
                    currentAlarm.AlarmAction.Invoke();
                    activeAlarms.Remove(currentAlarm);
                }
            }
        }
        
        public void ScheduleAlarm(DateTime invokeTime, Action alarmAction)
        {
            activeAlarms.Add(new AlarmData { AlarmAction = alarmAction, InvokeTime = invokeTime });
        }
    }
    
    public class AlarmData
    {
        public Action AlarmAction;
        public DateTime InvokeTime;
    }
    
    [Serializable]
    public class OfflineTimeData : ISaveData
    {
        public DateTime LastTimeTest = DateTime.Now;
    }
}