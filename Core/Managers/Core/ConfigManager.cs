using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Newtonsoft.Json;

namespace Base.Core.Managers
{
    public class ConfigManager : BaseManager
    {
        private readonly Dictionary<string, BaseConfig> cachedValues = new();
        
        public ConfigManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            SetDefault();
        }

        private void SetDefault()
        {
            Dictionary<string, object> defaults = new();

            defaults.Add("UpgradeablesConfig", "");
            
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
            {
                OnDefaultComplete();
            });
        }
        
        private void OnDefaultComplete()
        {
            FetchConfigs();
        }
        
        private void FetchConfigs()
        {
            FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(task => { SyncConfigs(); });
        }

        private void SyncConfigs()
        {
            FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task => OnSyncComplete());
        }

        private void OnSyncComplete()
        {
            OnInitComplete();
        }

        public T GetConfig<T>() where T : BaseConfig
        {
            var configName = typeof(T).Name;
            
            if (!cachedValues.TryGetValue(configName, out var jsonResult))
            {
                var configResult = FirebaseRemoteConfig.DefaultInstance.GetValue(configName);
                jsonResult = cachedValues[configName] = JsonConvert.DeserializeObject<T>(configResult.StringValue);
                MyDebug.Log($"GetConfig of {configName} is {configResult.StringValue}");
            }

            return (T) jsonResult;
        }
    }
    
    [Serializable]
    public class BaseConfig
    {
        public bool IsEnabled = true;
        public int ConfigVersion = 1;
    }
}