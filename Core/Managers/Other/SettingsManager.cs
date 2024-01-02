using System;
using System.Collections.Generic;

namespace Base.Core.Managers
{
    public class SettingsManager : BaseManager
    {
        private PlayerSettingsData playerSettings; // Stores the player's settings data.
        
        public SettingsManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            playerSettings = GameManager.SaveManager.LoadData<PlayerSettingsData>();

            if (playerSettings == null)
            {
                playerSettings = new PlayerSettingsData();
                GameManager.SaveManager.SaveData(playerSettings);
            }
            OnInitComplete();
        }
        
        public void ChangeValue(float value, SettingsType scoreTypes) // Add value of settings the given type 
        {
            if (!playerSettings.SettingsList.TryGetValue(scoreTypes, out var scoreData)) // Check if the settings type exists in the dictionary
            {
                // If the settings type doesn't exist, add a new entry to the dictionary with default value (0)
                playerSettings.SettingsList.Add(scoreTypes, new SettingsData
                {
                    SettingsType = scoreTypes,
                    SettingsAmount = 0
                });
            }
            
            playerSettings.SettingsList[scoreTypes].ChangeAmount(value);
            GameManager.SaveManager.SaveData(playerSettings);
        }
        
        public float GetValueAsFloat(SettingsType scoreTypes)
        {
            if (playerSettings.SettingsList.TryGetValue(scoreTypes, out var settingsData))
            {
                return settingsData.GetSettingsAmountInt();
            }
            playerSettings.SettingsList.Add(scoreTypes, new SettingsData{ SettingsType = scoreTypes,SettingsAmount = 0} );
            MyDebug.Log($"Given key {scoreTypes} was not present in the dictionary, created new and set score to 0.");
            return 0;
        }
    }
    
    // Represents the player's settings data, containing a dictionary of SettingsType and SettingsData objects
    [Serializable]
    public class PlayerSettingsData : ISaveData
    {
        public Dictionary<SettingsType, SettingsData> SettingsList = new()
        {
            { SettingsType.MasterVolume, new SettingsData { SettingsType = SettingsType.MasterVolume, SettingsAmount = 0.5f } },
            { SettingsType.MusicVolume, new SettingsData { SettingsType = SettingsType.MusicVolume, SettingsAmount = 0.5f } },
            { SettingsType.SFXVolume, new SettingsData { SettingsType = SettingsType.SFXVolume, SettingsAmount = 0.5f } }
        };

    }
    
    // Represents a specific settings type and its value
    [Serializable]
    public class SettingsData
    {
        public SettingsType SettingsType;
        public float SettingsAmount;
        
        public void ChangeAmount(float value)
        {
            float newAmount = value;
            if (newAmount < 0)
            {
                MyDebug.Log("Cannot decrease SettingsAmount below 0.");
                return;
            }
            SettingsAmount = value;
        }
        
        public float GetSettingsAmountInt() => SettingsAmount;
        
        
    }
    // Enum representing different settings types
    public enum SettingsType
    {
        MasterVolume, MusicVolume, SFXVolume 
    
    }
}