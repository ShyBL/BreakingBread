using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Base.Core.Managers
{
    public class SaveManager : BaseManager
    {
        public SaveManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            OnInitComplete();
        }
        
        public void DeleteDataFileByName<T>() where T : ISaveData
        {
            var typeName = typeof(T).FullName;
            var path = $"{Application.persistentDataPath}/{typeName}.Save";

            if (HasData(path))
            {
                File.Delete(path);
            }
        }

        public void ClearAllDataInAppPath()
        {
            var path = Application.persistentDataPath;
            var files = Directory.GetFiles(path);

            foreach (var fileName in files)
            {
                if (fileName.Contains(""))
                {
                    File.Delete(fileName);
                }
            }
        }
        
        public void SaveData(ISaveData saveData)
        {
            var typeName = saveData.GetType().FullName;
            var savePath = $"{Application.persistentDataPath}/{typeName}.Save";
            var dataText = JsonConvert.SerializeObject(saveData);
            File.WriteAllText(savePath, dataText);
        }
        
        public T LoadData<T>() where T : ISaveData
        {
            var typeName = typeof(T).FullName;
            var loadPath = $"{Application.persistentDataPath}/{typeName}.Save";
            if (HasData(loadPath))
            {
                var dataLoaded = File.ReadAllText(loadPath);
                return JsonConvert.DeserializeObject<T>(dataLoaded);
            }
            return default;
        }

        /// <summary>
        /// Load Data, if null will create new instance
        /// Also Save the new created data
        /// using reflection, should be used only when game is loaded
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadDataAndCreateIfNull<T>() where T : ISaveData
        {
            var data = LoadData<T>();

            if (data == null)
            {
                data = Activator.CreateInstance<T>();
                data.SaveData();
            }

            return data;
        }

        public bool HasData<T>() where T : ISaveData
        {
            var typeName = typeof(T).FullName;
            var loadPath = $"{Application.persistentDataPath}/{typeName}.Save";
            return HasData(loadPath);
        }
        
        public bool HasData(string path)
        {
            return File.Exists(path);
        }
    }
    
    public interface ISaveData
    {
        
    }

    public static class SaveExtensions
    {
        public static void SaveData(this ISaveData saveData)
        {
            GameManager.Instance.SaveManager.SaveData(saveData);
        }
        
        // TODO: 
        public static void LoadData<T>(this ISaveData saveData) where T : ISaveData
        {
            saveData =  GameManager.Instance.SaveManager.LoadData<T>();
        }
    }
}