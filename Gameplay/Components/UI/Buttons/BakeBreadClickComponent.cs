using Base.Core.Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Base.Gameplay.Components
{
    [RequireComponent(typeof(Button))]
    public class BakeBreadClickComponent : ItemUIComponent
    {
        private void Start()
        {
            audioComponent = FindObjectOfType<AudioComponent>();
            fxSource = audioComponent.SFXAudioSource;
            soundClip = audioComponent.menuClick;
        }
        
        public void OnClickBake()
        {
            GameManager.AudioManager.PlaySFX(soundClip,fxSource);
            
            var amount = Amount();
            GameManager.CurrencyManager.AddScore(amount, CurrencyType.Baked);
        }

        private int Amount()
        {
            var breadPerClick = GameManager.UpgradeManager.GetPowerByType(UpgradeType.GenerateDefaultAmount);
            var multi = GameManager.ResearchManager.GetPowerByType(ResearchType.GenerateDefaultAmount);
            var amount = multi * breadPerClick;
            return amount;
        }
    }
}