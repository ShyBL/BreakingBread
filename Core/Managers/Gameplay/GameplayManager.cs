using System;
using System.Collections.Generic;

namespace Base.Core.Managers
{
    public class GameplayManager : BaseManager
    {
        private int totalProductionAtPayout;
        public GameplayManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            OnInitComplete();
        }
        
        public void AdAwardBreadForProduction()
        {
            var timePassed = int.Parse(UpdateOfflineTimeOnce());
            CalculateCurrentTotalProduction(timePassed);
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
                    var currentLevelData = upgradeableConfig.UpgradeableLevelData[currentUpgradeLevel];
                    int multiplier = 1;
                    var power = currentLevelData.Power;
                    var production = (power * multiplier) * timePassed;

                    totalProduction += production;
                }
            }
            
            return totalProduction;
        }
    }
}
