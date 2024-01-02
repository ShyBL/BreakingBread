using Base.Core.Managers;
using DG.Tweening;
using UnityEngine;
using EventType = Base.Core.Managers.EventType;

namespace Base.Gameplay.Components
{
    public class MilestoneItemUIComponent : ItemUIComponent
    {
        //Data
        private MilestoneableConfig milestoneConfig;
        private MilestoneableLevelData currentMilestoneLevel;

        // Animation
        [SerializeField] private GameObject notificationGameObject;
        private bool notificationDone = true;
        
        public void Init(MilestoneableConfig milestone)
        {
            milestoneConfig = milestone;
            
            InitInternalVariables();
            UpdateUI();
            UpdateIcon(0);
            
            SetDisabled(false);
            
            gameObject.SetActive(true);
        }

        private void Start()
        {
            AddListeners();
            audioComponent = FindObjectOfType<AudioComponent>();
            fxSource = audioComponent.SFXAudioSource;
            soundClip = audioComponent.buyClick;
        }

        public void OnMilestoneClicked()
        {
            //Handheld.Vibrate();
            GameManager.AudioManager.PlaySFX(soundClip,fxSource);
            
            GameManager.MilestoneManager.TryMilestone(milestoneConfig.MilestoneType, currencyType);
        }
        
        private void OnItemClaim(object milestoneData)
        {
            var milestoneEventData = (MilestoneEventData) milestoneData;
            if (milestoneEventData.MilestoneType == milestoneConfig.MilestoneType)
            {
                DoNotification(false);
                notificationDone = true;
                UpdateUI();
                SetDisabled(false);
                //DoTweenVFX(powerText.gameObject);
                //DoTweenVFX(levelText.gameObject);
                DoTweenVFX(button.gameObject);
            }
        }
        
        
        private void CheckCurrencyMilestone(object eventData)
        {
            var currencyData = (CurrencyData)eventData;
            
            if (currencyData.CurrencyType.ToString() == milestoneConfig.MilestoneType.ToString())
            {
                if (currencyData.CurrencyAmount >= currentMilestoneLevel.ScoreForNextLevel)
                {
                    DoNotification(true);
                    
                    SetFinalLevel();
                    
                    SetDisabled(true);
                    DoTweenVFX(milestoneLevel.gameObject);
                }
                else
                {
                    SetCurrentLevel();
                    
                    fillImage.DOFillAmount(GameManager.CurrencyManager.GetScoreAsInt(currencyType) / (float) currentMilestoneLevel.ScoreForNextLevel, 0.1f).SetEase(Ease.Linear);
                    
                    SetDisabled(false);
                    //DoTweenVFX(milestoneLevel.gameObject);
                }
            }
        }
        
        private void CheckUpgradesMilestone(object upgradeData)
        {
            var eventData = (UpgradeEventData)upgradeData;
            
            if (eventData.UpgradeType.ToString() == milestoneConfig.MilestoneType.ToString())
            {
                if (eventData.UpgradeableLevelEventData.CurrentLevel >= currentMilestoneLevel.ScoreForNextLevel)
                {
                    DoNotification(true);
                    
                    SetFinalLevel();
                    
                    SetDisabled(true);
                    DoTweenVFX(milestoneLevel.gameObject);
                }
                else
                {
                    SetCurrentLevel();
                    
                    fillImage.DOFillAmount(eventData.UpgradeableLevelEventData.CurrentLevel / currentMilestoneLevel.ScoreForNextLevel, 0.1f).SetEase(Ease.Linear);
                    
                    SetDisabled(false);
                    //DoTweenVFX(milestoneLevel.gameObject);
                }
            }
        }

        private void SetCurrentLevel()
        {
            switch (milestoneConfig.MilestoneType)
            {
                case MilestoneType.Baked:
                case MilestoneType.Money:
                    milestoneLevel.text = GameManager.CurrencyManager.GetScoreAsString(currencyType);
                    break;
                case MilestoneType.GenerateBasicAmount:
                case MilestoneType.GenerateMidAmount:
                case MilestoneType.GenerateBestAmount:
                    milestoneLevel.text = GameManager.UpgradeManager.GetLevelByType(upgradeType).ToString();
                    break;
            }
        }

        private void SetFinalLevel()
        {
            switch (milestoneConfig.MilestoneType)
            {
                case MilestoneType.Baked:
                case MilestoneType.GenerateBasicAmount:
                case MilestoneType.GenerateMidAmount:
                case MilestoneType.GenerateBestAmount:
                    milestoneLevel.text = BreadAmountToString(currentMilestoneLevel.ScoreForNextLevel);
                    break;
                case MilestoneType.Money:
                    milestoneLevel.text = ScoreToString(currentMilestoneLevel.ScoreForNextLevel);
                    break;
            }
        }
        
        private void DoNotification(bool isNotif)
        {
            notificationGameObject.SetActive(isNotif);
            if (notificationDone)
            {
                DoTweenVFX(notificationGameObject);
                //Handheld.Vibrate();
                notificationDone = false;
            }
        }
        
        private void InitInternalVariables()
        {
            switch (milestoneConfig.MilestoneType)
            {
                case MilestoneType.Baked:
                    currencyType = CurrencyType.Baked;
                    milestoneLevel.text = GameManager.CurrencyManager.GetScoreAsString(currencyType);
                    break;
                case MilestoneType.Money:
                    currencyType = CurrencyType.Money;
                    milestoneLevel.text = GameManager.CurrencyManager.GetScoreAsString(currencyType);
                    break;
                case MilestoneType.GenerateBasicAmount:
                    upgradeType = UpgradeType.GenerateBasicAmount;
                    milestoneLevel.text = GameManager.UpgradeManager.GetLevelByType(upgradeType).ToString();
                    break;
                case MilestoneType.GenerateMidAmount:
                    upgradeType = UpgradeType.GenerateMidAmount;
                    milestoneLevel.text = GameManager.UpgradeManager.GetLevelByType(upgradeType).ToString();
                    break;
                case MilestoneType.GenerateBestAmount:
                    upgradeType = UpgradeType.GenerateBestAmount;
                    milestoneLevel.text = GameManager.UpgradeManager.GetLevelByType(upgradeType).ToString();
                    break;
            }
            DoNotification(false);
        }
        
        private void SetDisabled(bool isInteractable)
        {
            if (isInteractable)
            {
                claimCurrency.SetActive(!isInteractable);
                button.interactable = isInteractable;
                button.targetGraphic.color = isInteractable ? Color.white : Color.grey;

            }
            else
            {
                claimCurrency.SetActive(!isInteractable);
                button.interactable = isInteractable;
                button.targetGraphic.color = isInteractable ? Color.white : Color.grey;
            }
        }
        
        private void UpdateUI()
        {
            var index = GameManager.MilestoneManager.GetLevelByType(milestoneConfig.MilestoneType);
            
            currentMilestoneLevel = milestoneConfig.MilestoneableLevelData[index];
            
            itemName.text = milestoneConfig.MilestoneableName;
            
            switch (milestoneConfig.MilestoneType)
            {
                case MilestoneType.Baked:
                case MilestoneType.GenerateBasicAmount:
                case MilestoneType.GenerateMidAmount:
                case MilestoneType.GenerateBestAmount:
                    milestoneLevelNeeded.text = BreadAmountToString(currentMilestoneLevel.ScoreForNextLevel);
                    break;
                case MilestoneType.Money:
                    milestoneLevelNeeded.text = ScoreToString(currentMilestoneLevel.ScoreForNextLevel);
                    break;
            }
            
            switch (milestoneConfig.MilestoneType)
            {
                case MilestoneType.Baked:
                case MilestoneType.Money:
                    milestoneLevel.text = GameManager.CurrencyManager.GetScoreAsString(currencyType);
                    break;
                case MilestoneType.GenerateBasicAmount:
                case MilestoneType.GenerateMidAmount:
                case MilestoneType.GenerateBestAmount:
                    BreadAmountToString(currentMilestoneLevel.ScoreForNextLevel);
                    break;
            }
        }
        private void AddListeners()
        {
            GameManager.EventsManager.AddListener(EventType.OnScoreChanged, CheckCurrencyMilestone);
            GameManager.EventsManager.AddListener(EventType.OnUpgraded, CheckUpgradesMilestone);
            GameManager.EventsManager.AddListener(EventType.OnMilestone, OnItemClaim);
        }
    }
}