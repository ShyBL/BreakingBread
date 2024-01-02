using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Core.Managers
{
    public class ResearchsManager : BaseManager
    {
        private ResearchablesConfig researchConfig = new();
        private PlayerResearchData researchData = new();
        
        public ResearchsManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            researchConfig = GameManager.ConfigManager.GetConfig<ResearchablesConfig>();
            LoadResearchData();
            OnInitComplete();
        }
        
        public bool TryResearch(ResearchType researchType)
        {
            if (!GetResearchConfigData(researchType, out var researchableConfig, out var currentResearchLevel))
            {
                return false;
            }
            
            var scoreNeededForNextLevel = researchableConfig.ResearchableLevelData[currentResearchLevel].ScoreForNextLevel;
            var currentUserScore = GameManager.CurrencyManager.GetScoreAsInt(CurrencyType.Money);
            
            if (currentUserScore < scoreNeededForNextLevel)
            {
                GameManager.UIManager.OpenPopupWindow(currentUserScore, scoreNeededForNextLevel, "PopupWindow");
                MyDebug.Log($"Tried to research type {researchType.ToString()} had score {currentUserScore} and needed {scoreNeededForNextLevel}");
                return false;
            }
            
            currentResearchLevel++;
            
            researchData.SavedResearchLevel[researchType] = currentResearchLevel;
            
            GameManager.CurrencyManager.RemoveScore(scoreNeededForNextLevel, CurrencyType.Money);
            
            GameManager.EventsManager.InvokeEvent(EventType.OnResearched, new ResearchEventData(researchType, researchableConfig.ResearchableLevelData[currentResearchLevel]));
            
            GameManager.SaveManager.SaveData(researchData);
            
            GameManager.AnalyticsManager.SendAnalytics(AnalyticsEventName.Research,
                new Dictionary<string, object>
                {
                    {"research_type", researchType.ToString()},
                    {"new_level", currentResearchLevel}
                });
            
            return true;
        }

        private bool GetResearchConfigData(ResearchType researchType, out ResearchableConfig researchableConfig, out int currentResearchLevel)
        {
            researchableConfig = researchConfig.AllResearchAbles.FirstOrDefault(x => x.ResearchType == researchType);

            if (researchableConfig == null)
            {
                GameManager.MonitorManager.ReportException($"Tried to research type that not exist in config {researchType}");
                currentResearchLevel = 0;
                return false;
            }

            currentResearchLevel = researchData.SavedResearchLevel[researchType];
            return true;
        }
        
        public int GetPowerByType(ResearchType researchType)
        {
            if (!researchData.SavedResearchLevel.TryGetValue(researchType, out var level))
            {
                level = 0;
                researchData.SavedResearchLevel.Add(researchType, 0);
            }

            if (GetResearchConfigData(researchType, out var researchableConfig, out _))
            {
                return researchableConfig.ResearchableLevelData[level].Power;
            }
            return 1;
        }
        
        public int GetLevelByType(ResearchType researchType)
        {
            if (!researchData.SavedResearchLevel.TryGetValue(researchType, out var level))
            {
                level = 0;
                researchData.SavedResearchLevel.Add(researchType, 0);
            }
            
            return level;
        }
        
        public List<ResearchableConfig> GetAllResearches() // method returns the list of all ResearchableConfig
        {
            return researchConfig.AllResearchAbles;
        }
             
        // Load Helper Methods
        private void LoadResearchData()
        {
            researchData = GameManager.SaveManager.LoadDataAndCreateIfNull<PlayerResearchData>();
        }
    }
    
    public class PlayerResearchData : ISaveData
    {
        public Dictionary<ResearchType, int> SavedResearchLevel = new();
    }
    
    public enum ResearchType
    {
        GenerateDefaultAmount,
        GenerateBasicAmount,
        GenerateMidAmount,
        GenerateBestAmount,
        OfflineLimit
    }
    
    public class ResearchablesConfig : BaseConfig
    {
        public List<ResearchableConfig> AllResearchAbles = new();
    }
    
    public class ResearchableConfig
    {
        public ResearchType ResearchType;
        public string ResearchableName;
        public string ResearchableDescription;
        public List<ResearchableLevelData> ResearchableLevelData;
    }
    
    public class ResearchableLevelData
    {
        public int ScoreForNextLevel;
        public int Power;
        public int CurrentLevel;
        
        public ResearchableLevelData(int scoreForNextLevel, int power, int currentLevel)
        {
            ScoreForNextLevel = scoreForNextLevel;
            Power = power;
            CurrentLevel = currentLevel;
        }
    }
    
    public class ResearchEventData
    {
        public ResearchType ResearchType;
        public ResearchableLevelData ResearchableLevelEventData;

        public ResearchEventData(ResearchType researchType, ResearchableLevelData researchableLevelEventData)
        {
            ResearchType = researchType;
            ResearchableLevelEventData = researchableLevelEventData;
        }
    }
}