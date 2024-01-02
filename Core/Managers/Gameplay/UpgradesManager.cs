using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Core.Managers
{
    // This is the main class for managing upgrades in the game.
    public class UpgradesManager : BaseManager
    {
        private UpgradeablesConfig upgradeConfig = new();
        private PlayerUpgradeData upgradeData = new();
            
        public UpgradesManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            upgradeConfig = GameManager.ConfigManager.GetConfig<UpgradeablesConfig>();
            LoadUpgradeData();
            OnInitComplete();   
        }
            
        // Try to perform an upgrade for a specific upgrade type.
        public bool TryUpgrade(UpgradeType upgradeType)
        {
            // Check if the upgrade is possible and the player has enough currency. If not, return false.
            if (!GetUpgradeConfigData(upgradeType, out var upgradeableConfig, out var currentUpgradeLevel))
            {
                return false;
            }
                
            // Calculate the score needed for the next level of the upgrade.
            var scoreNeededForNextLevel = upgradeableConfig.UpgradeableLevelData[currentUpgradeLevel].ScoreForNextLevel;
            var currentUserScore = GameManager.CurrencyManager.GetScoreAsInt(CurrencyType.Money);
                
            // If the player doesn't have enough score, return false.
            if (currentUserScore < scoreNeededForNextLevel)
            {
                GameManager.UIManager.OpenPopupWindow(currentUserScore, scoreNeededForNextLevel, "PopupWindow");                     
                return false;
            }
                
            // Updates the current upgrade level 
            currentUpgradeLevel++;
            // Updates the internal data structure
            upgradeData.SavedUpgradeLevel[upgradeType] = currentUpgradeLevel;
            // Deduct the required score
            GameManager.CurrencyManager.RemoveScore(scoreNeededForNextLevel, CurrencyType.Money);
                
            // Invoke an upgrade event
            GameManager.EventsManager.InvokeEvent(EventType.OnUpgraded, new UpgradeEventData(upgradeType, upgradeableConfig.UpgradeableLevelData[currentUpgradeLevel]));
                
            // Save all the data on device
            GameManager.SaveManager.SaveData(upgradeData);
            // Sends analytics.
            GameManager.AnalyticsManager.SendAnalytics(AnalyticsEventName.Upgrade,
                new Dictionary<string, object>
                {
                    {"upgrade_type", upgradeType.ToString()},
                    {"new_level", currentUpgradeLevel}
                });
                
            return true;
        }
            
        public bool GetUpgradeConfigData(UpgradeType upgradeType, out UpgradeableConfig upgradeableConfig, out int currentUpgradeLevel)
        {
            // Try to find the upgradeable configuration based on the upgrade type.
            upgradeableConfig = upgradeConfig.AllUpgradeAbles.FirstOrDefault(x => x.UpgradeType == upgradeType);

            if (upgradeableConfig == null)
            {
                // If the upgrade type doesn't exist in the configuration, report an exception, and return false.
                GameManager.MonitorManager.ReportException($"Tried to upgrade type that does not exist in config {upgradeType}");
                currentUpgradeLevel = 0;
                return false;
            }
            // Retrieve the current upgrade level for the specified type.
            currentUpgradeLevel = upgradeData.SavedUpgradeLevel[upgradeType];
            return true;
        }
            
        // Get the power of an upgrade by its type.
        public int GetPowerByType(UpgradeType upgradeType)
        {
            // Check if the upgrade level exists for the specified type.
            if (!upgradeData.SavedUpgradeLevel.TryGetValue(upgradeType, out var level))
            {
                // If not, set the level to 0 and add it to the saved upgrade levels.
                level = 0;
                upgradeData.SavedUpgradeLevel.Add(upgradeType, 0);
            }

            // Retrieve the upgradeable configuration and return its power if found.
            if (GetUpgradeConfigData(upgradeType, out var upgradeableConfig, out _))
            {
                return upgradeableConfig.UpgradeableLevelData[level].Power;
            }

            // Return 0 if the upgradeable configuration is not found.
            return 0;
        }
            
            
        // Get the level of an upgrade by its type.
        public int GetLevelByType(UpgradeType upgradeType)
        {
            // Check if the upgrade level exists for the specified type.
            if (!upgradeData.SavedUpgradeLevel.TryGetValue(upgradeType, out var level))
            {
                // If not, set the level to 0 and add it to the saved upgrade levels.
                level = 0;
                upgradeData.SavedUpgradeLevel.Add(upgradeType, 0);
            }

            // Return the level.
            return level;
        }
            
        // method returns the list of all UpgradeableConfig
        public List<UpgradeableConfig> GetAllUpgrades() 
        {
            return upgradeConfig.AllUpgradeAbles;
        }
            
        // Load Helper Methods
        private void LoadUpgradeData()
        {
            upgradeData = GameManager.SaveManager.LoadDataAndCreateIfNull<PlayerUpgradeData>();
        }
            
        public int CalculateTotalBreadPerTick(UpgradeableConfig upgradeableConfig, ResearchType multiType)
        {
            var power = GameManager.UpgradeManager.GetPowerByType(upgradeableConfig.UpgradeType);
            var powerMulti = GameManager.ResearchManager.GetPowerByType(multiType);
            var totalBreadPerTick = power * powerMulti;
            return totalBreadPerTick;
        }
    }
        
    // Other classes and enumerations related to upgrades and their configurations.
    public class PlayerUpgradeData : ISaveData
    {
        public Dictionary<UpgradeType, int> SavedUpgradeLevel = new()
        {
            { UpgradeType.GenerateDefaultAmount, 0 },
            { UpgradeType.GenerateBasicAmount, 0 },
            { UpgradeType.GenerateMidAmount, 0 },
            { UpgradeType.GenerateBestAmount, 0 }
        };
    }

    public enum UpgradeType
    {
        GenerateDefaultAmount,
        GenerateBasicAmount,
        GenerateMidAmount,
        GenerateBestAmount
    }
        
    // This class represents a configuration for upgradeable items.
    public class UpgradeablesConfig : BaseConfig
    {
        public List<UpgradeableConfig> AllUpgradeAbles = new();
    }
        
    // This class represents a specific upgradeable item's configuration.
    public class UpgradeableConfig
    {
        public UpgradeType UpgradeType;
        public string UpgradeableName;
        public string UpgradeableDescription;
        public List<UpgradeableLevelData> UpgradeableLevelData;
    }

    // This class represents data for a specific upgradeable item at different levels.
    public class UpgradeableLevelData
    {
        public int ScoreForNextLevel; // Next Cost
        public int Power; // Bread Per Tick
        public int CurrentLevel; // Amount Owned
            
        public UpgradeableLevelData(int scoreForNextLevel, int power, int currentLevel)
        {
            ScoreForNextLevel = scoreForNextLevel;
            Power = power;
            CurrentLevel = currentLevel;
        }
    }
        
    // A class representing an event used to pass data to subscribers.
    public class UpgradeEventData
    {
        public UpgradeType UpgradeType;
        public UpgradeableLevelData UpgradeableLevelEventData;

        public UpgradeEventData(UpgradeType upgradeType, UpgradeableLevelData upgradeableLevelEventData)
        {
            UpgradeType = upgradeType;
            UpgradeableLevelEventData = upgradeableLevelEventData;
        }
    }
}