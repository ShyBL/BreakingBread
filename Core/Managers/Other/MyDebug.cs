using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


namespace Base.Core.Managers
{
    public class MyDebug
    {
        [Conditional("My_LOGS")]
        public static void Log(string msg)
        {
            Debug.Log(msg);
        }
        
        [Conditional("My_LOGS")]
        public static void LogError(string msg)
        {
            Debug.LogError(msg);
        }

        [Conditional("My_LOGS")]
        public static void LogException(Exception exc)
        {
            Debug.LogException(exc);
        }
    }
}