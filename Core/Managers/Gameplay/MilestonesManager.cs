using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Core.Managers
{
    public class MilestonesManager : BaseManager
    {
        private MilestoneablesConfig milestoneConfig = new();
        private PlayerMilestoneData milestoneData = new();
            
        public MilestonesManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            milestoneConfig = GameManager.ConfigManager.GetConfig<MilestoneablesConfig>();
            LoadMilestoneData();
            OnInitComplete();   
        }
            
        // Try to perform an milestone for a specific milestone type.
        public bool TryMilestone(MilestoneType milestoneType, CurrencyType currencyType)
        {
            if (!GetMilestoneConfigData(milestoneType, out var milestoneableConfig, out var currentMilestoneLevel))
            {
                return false;
            }
                
            var scoreNeededForNextLevel = milestoneableConfig.MilestoneableLevelData[currentMilestoneLevel].ScoreForNextLevel;
            var currentUserScore = GameManager.CurrencyManager.GetScoreAsInt(currencyType);
            var awardAmount = milestoneableConfig.MilestoneableLevelData[currentMilestoneLevel].AwardAmount;

            if (currentUserScore < scoreNeededForNextLevel)
            {
                GameManager.UIManager.OpenPopupWindow(currentUserScore, currentUserScore, "PopupWindow");
                return false;
            }
            
            currentMilestoneLevel++;

            milestoneData.SavedMilestoneLevel[milestoneType] = currentMilestoneLevel;
            GameManager.CurrencyManager.AddScore(awardAmount, CurrencyType.SpecialCurrency);
            GameManager.EventsManager.InvokeEvent(EventType.OnMilestone, new MilestoneEventData(milestoneType, milestoneableConfig.MilestoneableLevelData[currentMilestoneLevel]));
            
            GameManager.SaveManager.SaveData(milestoneData);
            GameManager.AnalyticsManager.SendAnalytics(AnalyticsEventName.Milestone,
                new Dictionary<string, object>
                {
                    {"milestone_type", milestoneType.ToString()},
                    {"new_level", currentMilestoneLevel}
                });
                
            return true;
        }

        private bool GetMilestoneConfigData(MilestoneType milestoneType, out MilestoneableConfig milestoneableConfig, out int currentMilestoneLevel)
        {
            // Try to find the milestoneable configuration based on the milestone type.
            milestoneableConfig = milestoneConfig.AllMilestoneAbles.FirstOrDefault(x => x.MilestoneType == milestoneType);

            if (milestoneableConfig == null)
            {
                // If the milestone type doesn't exist in the configuration, report an exception, and return false.
                GameManager.MonitorManager.ReportException($"Tried to milestone type that does not exist in config {milestoneType}");
                currentMilestoneLevel = 0;
                return false;
            }
            // Retrieve the current milestone level for the specified type.
            currentMilestoneLevel = milestoneData.SavedMilestoneLevel[milestoneType];
            return true;
        }
        // Get the level of an milestone by its type.
        public int GetLevelByType(MilestoneType milestoneType)
        {
            // Check if the milestone level exists for the specified type.
            if (!milestoneData.SavedMilestoneLevel.TryGetValue(milestoneType, out var level))
            {
                // If not, set the level to 0 and add it to the saved milestone levels.
                level = 0;
                milestoneData.SavedMilestoneLevel.Add(milestoneType, 0);
            }

            // Return the level.
            return level;
        }
        
            
        // Get the power of an milestone by its type.
        public int GetAwardAmountByType(MilestoneType milestoneType)
        {
            // Check if the milestone level exists for the specified type.
            if (!milestoneData.SavedMilestoneLevel.TryGetValue(milestoneType, out var level))
            {
                // If not, set the level to 0 and add it to the saved milestone levels.
                level = 0;
                milestoneData.SavedMilestoneLevel.Add(milestoneType, 0);
            }
        
            // Retrieve the milestoneable configuration and return its power if found.
            if (GetMilestoneConfigData(milestoneType, out var milestoneableConfig, out _))
            {
                return milestoneableConfig.MilestoneableLevelData[level].AwardAmount;
            }
        
            // Return 0 if the milestoneable configuration is not found.
            return 0;
        }
        
            
        // method returns the list of all MilestoneableConfig
        public List<MilestoneableConfig> GetAllMilestones() 
        {
            return milestoneConfig.AllMilestoneAbles;
        }
            
        // Load Helper Methods
        private void LoadMilestoneData()
        {
            milestoneData = GameManager.SaveManager.LoadDataAndCreateIfNull<PlayerMilestoneData>();
        }
    }
        
    // Other classes and enumerations related to milestones and their configurations.
    public class PlayerMilestoneData : ISaveData
    {
        public Dictionary<MilestoneType, int> SavedMilestoneLevel = new()
        {
            { MilestoneType.Baked, 0 },
            { MilestoneType.Money, 0 },
            { MilestoneType.GenerateBasicAmount, 0 },
            { MilestoneType.GenerateMidAmount, 0 },
            { MilestoneType.GenerateBestAmount, 0 },
        };
    }

    public enum MilestoneType
    { 
        Baked, 
        Money, 
        GenerateBasicAmount, 
        GenerateMidAmount,
        GenerateBestAmount
    }
        
    // This class represents a configuration for milestoneable items.
    public class MilestoneablesConfig : BaseConfig
    {
        public List<MilestoneableConfig> AllMilestoneAbles = new();
    }
        
    // This class represents a specific milestoneable item's configuration.
    public class MilestoneableConfig
    {
        public MilestoneType MilestoneType;
        public string MilestoneableName;
        public string MilestoneableDescription;
        public List<MilestoneableLevelData> MilestoneableLevelData;
    }
        
    // This class represents data for a specific milestoneable item at different levels.
    public class MilestoneableLevelData
    {
        public int ScoreForNextLevel; // Next Cost
        public int AwardAmount;
        public int CurrentLevel;
            
        public MilestoneableLevelData(int scoreForNextLevel, int awardAmount, int currentLevel)
        {
            ScoreForNextLevel = scoreForNextLevel;
            AwardAmount = awardAmount;
            CurrentLevel = currentLevel;
        }
    }
        
    // A class representing an event used to pass data to subscribers.
    public class MilestoneEventData
    {
        public MilestoneType MilestoneType;
        public MilestoneableLevelData MilestoneableLevelEventData;

        public MilestoneEventData(MilestoneType milestoneType, MilestoneableLevelData milestoneableLevelEventData)
        {
            MilestoneType = milestoneType;
            MilestoneableLevelEventData = milestoneableLevelEventData;
        }
    }
}