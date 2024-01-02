using Base.Core.Managers;
using DG.Tweening;
using EventType = Base.Core.Managers.EventType;

namespace Base.Gameplay.Components
{
    public class UpgradeItemUIComponent : ItemUIComponent
    {
        // Data
        private UpgradeableConfig upgradeableConfig;
        private UpgradeableLevelData currentUpgradeableLevel;
        
        public void Init(UpgradeableConfig upgradeable)
        {
            upgradeableConfig = upgradeable;
            
            InitInternalVariables();
            
            SetUI();
            UpdateIcon(0);
            gameObject.SetActive(true);
        }

        private void Start()
        {
            AddListeners();
            audioComponent = FindObjectOfType<AudioComponent>();
            fxSource = audioComponent.SFXAudioSource;
            soundClip = audioComponent.buyClick;
        }

        public void OnUpgradeClicked()
        {
            //Handheld.Vibrate();
            GameManager.AudioManager.PlaySFX(soundClip,fxSource);
            GameManager.UpgradeManager.TryUpgrade(upgradeableConfig.UpgradeType);
        }
        
        private void Ticker(object objectToPass = null)
        {
            if ((upgradeableConfig.UpgradeType == UpgradeType.GenerateBasicAmount ||
                 upgradeableConfig.UpgradeType == UpgradeType.GenerateMidAmount ||
                 upgradeableConfig.UpgradeType == UpgradeType.GenerateBestAmount)
                && currentUpgradeableLevel.CurrentLevel > 0)
            {
                MakeBread();
            }
        }
        private void OnItemResearched(object reasearchData)
        {
            var upgradeEventData = (ResearchEventData) reasearchData;
            if (upgradeEventData.ResearchType.ToString() == upgradeableConfig.UpgradeType.ToString())
            {
                breadPerTick = GameManager.UpgradeManager.CalculateTotalBreadPerTick(upgradeableConfig,researchType);
                
                UpdateUI();
                    
                DoTweenVFX(powerText.gameObject);
                DoTweenVFX(levelText.gameObject);
                DoTweenVFX(button.gameObject);
            }
        }
        
        private void OnItemUpgraded(object upgradeData)
        {
            var upgradeEventData = (UpgradeEventData) upgradeData;
            if (upgradeEventData.UpgradeType == upgradeableConfig.UpgradeType)
            {
                breadPerTick = GameManager.UpgradeManager.CalculateTotalBreadPerTick(upgradeableConfig,researchType);
                UpdateIcon(1);
                UpdateUI();
                
                DoTweenVFX(powerText.gameObject);
                DoTweenVFX(levelText.gameObject);
                DoTweenVFX(button.gameObject);
            }
        }
        
        private void UpdateUI()
        {
            var index = GameManager.UpgradeManager.GetLevelByType(upgradeableConfig.UpgradeType);
            
            currentUpgradeableLevel = upgradeableConfig.UpgradeableLevelData[index];
            
            costText.text = ScoreToString(currentUpgradeableLevel.ScoreForNextLevel);
            powerText.text = BreadPowerToString(currentUpgradeableLevel.Power);
            levelText.text = currentUpgradeableLevel.CurrentLevel.ToString("N0");
            
            fillImage.DOFillAmount(currentUpgradeableLevel.CurrentLevel/50f, 0.1f).SetEase(Ease.Linear);
        }
        
        private void SetUI()
        {
            itemName.text = upgradeableConfig.UpgradeableName;
            var index = GameManager.UpgradeManager.GetLevelByType(upgradeableConfig.UpgradeType);
            
            currentUpgradeableLevel = upgradeableConfig.UpgradeableLevelData[index];
            
            costText.text = ScoreToString(currentUpgradeableLevel.ScoreForNextLevel);
            powerText.text = BreadPowerToString(currentUpgradeableLevel.Power);
            levelText.text = currentUpgradeableLevel.CurrentLevel.ToString("N0");
            
            fillImage.DOFillAmount(currentUpgradeableLevel.CurrentLevel/50f, 0.1f).SetEase(Ease.Linear);
        }
        
        private void MakeBread()
        {
            var amount = GameManager.UpgradeManager.CalculateTotalBreadPerTick(upgradeableConfig, researchType);
            GameManager.CurrencyManager.AddScore(amount, CurrencyType.Baked);
            //DoVisualFX();
        }

        private void InitInternalVariables()
        {
            switch (upgradeableConfig.UpgradeType)
            {
                case UpgradeType.GenerateDefaultAmount:
                    researchType = ResearchType.GenerateDefaultAmount;
                    break;
                case UpgradeType.GenerateBasicAmount:
                    researchType = ResearchType.GenerateBasicAmount;
                    break;
                case UpgradeType.GenerateMidAmount:
                    researchType = ResearchType.GenerateMidAmount;
                    break;
                case UpgradeType.GenerateBestAmount:
                    researchType = ResearchType.GenerateBestAmount;
                    break;
            }
            
            breadPerTick = GameManager.UpgradeManager.CalculateTotalBreadPerTick(upgradeableConfig, researchType);
        }
        
        private void AddListeners()
        {
            GameManager.EventsManager.AddListener(EventType.OnUpgraded, OnItemUpgraded);
            GameManager.EventsManager.AddListener(EventType.OnResearched, OnItemResearched);
            GameManager.EventsManager.AddListener(EventType.Tick, Ticker);
        }
        
        private void OnApplicationQuit()
        {
            GameManager.EventsManager.RemoveListener(EventType.OnUpgraded, OnItemUpgraded);
            GameManager.EventsManager.RemoveListener(EventType.OnResearched, OnItemResearched);
            GameManager.EventsManager.RemoveListener(EventType.Tick, Ticker);
        }
    }
}