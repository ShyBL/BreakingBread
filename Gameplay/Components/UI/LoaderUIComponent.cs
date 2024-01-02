using System.Collections.Generic;
using Base.Core.Components;
using Base.Core.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Base.Gameplay.Components
{
    public class LoaderUIComponent : MyMonoBehaviour
    {
        
        [SerializeField] private TMP_Text timeValueText;
        [SerializeField] private TMP_Text timePayoutText;

        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject bg;
        [SerializeField] private GameObject welcome;
        [SerializeField] private GameObject logo;
        
        private bool canCloseWindow;
        private bool timeCheck = true;
        private int totalProductionAtPayout;
        
        public AudioComponent audioComponent;
        public AudioSource fxSource;
        public AudioClip soundClick;
        public AudioClip soundPopup;

        private void Start()
        {
            audioComponent = FindObjectOfType<AudioComponent>();
            fxSource = audioComponent.SFXAudioSource;
            soundClick = audioComponent.menuClick;
            soundPopup = audioComponent.popup;
            DontDestroyOnLoad(canvas);
        }

        private void Update()
        {
            if (timeCheck && 
                GameManager.DateTimeManager != null && GameManager.ResearchManager != null && GameManager.UIManager != null)
            {
                SetTimePayout();
                GetComponent<CanvasGroup>().alpha = 1;
            }
            
            if (SceneManager.GetActiveScene().name == "BB_Main" && canCloseWindow == false)
            {
                DoStartMainScene();
            }
        }

        public void CloseWindow()
        {
            if (canCloseWindow)
            {
                GameManager.AudioManager.PlaySFX(soundClick,fxSource);

                AwardBreadForProduction();
                Destroy(gameObject);
            }
        }
        
        private void DoStartMainScene()
        {
            GameManager.AudioManager.PlaySFX(soundPopup, fxSource);

            Destroy(bg);
            Destroy(welcome);
            Destroy(logo);

            canCloseWindow = true;
            GameManager.UIManager.HideAllAndOpenByTags("MainMenu", "BakeryTabs", true);
            GameManager.UIManager.LockByScenario(false, UIScenarios.Welcome);
        }

        private void SetTimePayout()
        {
            timeCheck = false;
            var timePassed = GameManager.DateTimeManager.OfflineSeconds;
            timeValueText.text = timePassed.ToString();
            timePayoutText.text = UpdateOfflineTimeOnce();
        }

        private void AwardBreadForProduction()
        {
            GameManager.CurrencyManager.AddScore(totalProductionAtPayout, CurrencyType.Baked);
        }

        private string UpdateOfflineTimeOnce()
        {
            var timePassed = GameManager.DateTimeManager.OfflineSeconds;
            var timeCap = GameManager.ResearchManager.GetPowerByType(ResearchType.OfflineLimit);
                
            if (timePassed >= timeCap)
            {
                    var totalProduction = CalculateCurrentTotalProduction(timeCap);
                    totalProductionAtPayout = totalProduction;
                    return $"{totalProduction} Bread, (Max Time Offline: {timeCap})";
            }
            else
            {
                    var totalProduction = CalculateCurrentTotalProduction(timePassed);
                    totalProductionAtPayout = totalProduction;
                    return $"{totalProduction} Bread, (Max Time Offline: {timeCap})";
            }
        }
        
        private int CalculateCurrentTotalProduction(int timePassed)
        {
            List<UpgradeableConfig> upgradesList = GameManager.UpgradeManager.GetAllUpgrades();
            var totalProduction = 0;

            foreach (var upgrade in upgradesList)
            {
                // Find the current level of the upgrade type
                if (GameManager.UpgradeManager.GetUpgradeConfigData(upgrade.UpgradeType, out var upgradeableConfig, out var currentUpgradeLevel))
                {
                    // Calculate production based on the current power of the upgrade
                    var production = CalculateCurrentProduction(timePassed, upgradeableConfig, currentUpgradeLevel);

                    totalProduction += production;
                }
            }
            
            return totalProduction;
        }

        private static int CalculateCurrentProduction(int timePassed, UpgradeableConfig upgradeableConfig, int currentUpgradeLevel)
        {
            var currentLevelData = upgradeableConfig.UpgradeableLevelData[currentUpgradeLevel];
            int multiplier = 1;
            var power = currentLevelData.Power;
            var production = (power * multiplier) * timePassed;
            return production;
        }

        private void OnDestroy()
        {
           GameManager.UIManager.LockByScenario(true, UIScenarios.Welcome);
        }
    }
}