using Base.Core.Managers;
using DG.Tweening;
using EventType = Base.Core.Managers.EventType;

namespace Base.Gameplay.Components
{
    public class ResearchItemUIComponent : ItemUIComponent
    {
        private ResearchableConfig researchableConfig;
        private ResearchableLevelData currentResearchableLevel;
        
        public void Init(ResearchableConfig researchable)
        {
            researchableConfig = researchable;
            
            SetUI();
            UpdateUI();
            UpdateIcon(0);
            gameObject.SetActive(true);
        }

        private void Start()
        {
            GameManager.EventsManager.AddListener(EventType.OnResearched, OnItemResearched);
            audioComponent = FindObjectOfType<AudioComponent>();
            fxSource = audioComponent.SFXAudioSource;
            soundClip = audioComponent.buyClick;
        }

        public void OnResearchClicked()
        {
            //Handheld.Vibrate();
            GameManager.AudioManager.PlaySFX(soundClip,fxSource);
            GameManager.ResearchManager.TryResearch(researchableConfig.ResearchType);
        }
        
        private void OnItemResearched(object researchData)
        {
            var researchEventData = (ResearchEventData) researchData;
            if (researchEventData.ResearchType == researchableConfig.ResearchType)
            {
                UpdateUI();
                UpdateIcon(1);
                DoTweenVFX(powerText.gameObject);
                DoTweenVFX(levelText.gameObject);
                DoTweenVFX(button.gameObject);
            }
        }
        
        private void UpdateUI()
        {
            var index = GameManager.ResearchManager.GetLevelByType(researchableConfig.ResearchType);
            
            currentResearchableLevel = researchableConfig.ResearchableLevelData[index];
            
            costText.text = ScoreToString(currentResearchableLevel.ScoreForNextLevel);
            powerText.text = $"X {currentResearchableLevel.Power}";
            levelText.text = currentResearchableLevel.CurrentLevel.ToString("N0");
            fillImage.DOFillAmount(currentResearchableLevel.CurrentLevel/50f, 0.1f).SetEase(Ease.Linear);
        }
        
        private void SetUI()
        {
            itemName.text = researchableConfig.ResearchableName;
            itemDescription.text = researchableConfig.ResearchableDescription;
        }
        
        private void OnApplicationQuit()
        {
            GameManager.EventsManager.RemoveListener(EventType.OnResearched, OnItemResearched);
            
        }
    }
}