using System.Collections.Generic;
using Firebase.Analytics;

namespace Base.Core.Managers
{
    public class FirebaseAnalyticAdapter : IAnalyticAdapter
    {
        public void SendAnalytics(string eventName, Dictionary<string, object> data)
        {
            FirebaseAnalytics.LogEvent(eventName, ConvertData(data));
        }

        private Parameter[] ConvertData(Dictionary<string, object> data)
        {
            var convertedListData = new List<Parameter>();

            foreach (var keyVal in data)
            {
                if (keyVal.Value == null)
                {
                    GameManager.Instance.MonitorManager.ReportException($"ConvertData value is null, of key {keyVal.Key}");
                    continue;
                }

                var objType = keyVal.Value.GetType();

                var keyName = keyVal.Key.ToString();
                if (objType == typeof(string))
                {
                    convertedListData.Add(new Parameter(keyName, (string) keyVal.Value));
                }
                else if(objType == typeof(float))
                {   
                    convertedListData.Add(new Parameter(keyName, (float) keyVal.Value));
                }
                else if(objType == typeof(int))
                {
                    convertedListData.Add(new Parameter(keyName, (int) keyVal.Value));
                }
                else if(objType == typeof(bool))
                {
                    convertedListData.Add(new Parameter(keyName, (bool) keyVal.Value ? 1 : 0));
                }
            }

            var convertedData = convertedListData.ToArray();
            
            return convertedData;
        }
    }
}