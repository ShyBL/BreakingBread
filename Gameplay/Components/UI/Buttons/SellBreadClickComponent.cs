using Base.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Gameplay.Components
{
    [RequireComponent(typeof(Button))]
    public class SellBreadClickComponent : ItemUIComponent
    {
        private void Start()
        {
            audioComponent = FindObjectOfType<AudioComponent>();
            fxSource = audioComponent.SFXAudioSource;
            soundClip = audioComponent.menuClick;
        }
        public void OnClickSell()
        {
            //Handheld.Vibrate();
            GameManager.AudioManager.PlaySFX(soundClip,fxSource);
            
            var amount = GameManager.CurrencyManager.GetScoreAsInt(CurrencyType.Baked);
            
            GameManager.CurrencyManager.RemoveScore(amount, CurrencyType.Baked);
            
            GameManager.CurrencyManager.AddScore(amount, CurrencyType.Money);
        }
    }
}