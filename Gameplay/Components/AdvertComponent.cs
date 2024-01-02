using System;
using Base.Core.Components;

namespace Base.Gameplay.Components
{
    public class AdvertComponent : MyMonoBehaviour
    {
        private void Start()
        {
            GameManager.AdsManager.TryShowRewardAd(GiveReward());
        }

        private Action<bool> GiveReward()
        {
            GameManager.GameplayManager.AdAwardBreadForProduction();
            return null;
        }
        
    }
}