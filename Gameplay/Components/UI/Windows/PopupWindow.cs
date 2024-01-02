using Base.Core.Components;
using Base.Core.Managers;
using TMPro;
using UnityEngine;
using EventType = Base.Core.Managers.EventType;

namespace Base.Gameplay.Components
{
    public class PopupWindow : MyMonoBehaviour, IUIComponent
    {
        public TMP_Text messageText;
        public TMP_Text missingAmountText;
        private int missingAmount;
        
        public AudioComponent audioComponent;
        public AudioSource fxSource;
        public AudioClip soundClip;
        
        private void Start()
        {
            audioComponent = FindObjectOfType<AudioComponent>();
            fxSource = audioComponent.SFXAudioSource;
            soundClip = audioComponent.popup;
            
            GameManager.AudioManager.PlaySFX(soundClip,fxSource);
            
            GameManager.UIManager.RegisterUIComponent(this);
            
            GameManager.EventsManager.AddListener(EventType.OnMessage, ShowMessage);
            GameManager.EventsManager.AddListener(EventType.OnOfferMessage, ShowOfferMessage);
        }
        
        private void OnDestroy()
        {
            GameManager.UIManager.UnRegisterUIComponent(this);
        }

        public void OnClose()
        {
            GameManager.UIManager.HideOne("PopupWindow", false);
        }
        
        public void OnOffer()
        {
            var amount = missingAmount;

            var specialCurrencyType = CurrencyType.SpecialCurrency;
            var moneyCurrencyType = CurrencyType.Money;

            switch (amount)
            {
                case <= 100:
                    if (GameManager.CurrencyManager.RemoveScore(5, specialCurrencyType))
                    {
                        GameManager.CurrencyManager.AddScore(100, moneyCurrencyType);
                        
                        GameManager.UIManager.HideOne("PopupWindow", false);
                    }
                    GameManager.UIManager.OpenPopupWindow(GameManager.CurrencyManager.
                        GetScoreAsInt(CurrencyType.SpecialCurrency), 5, "PopupWindow");
                    break;

                case <= 1000:
                    if (GameManager.CurrencyManager.RemoveScore(10, specialCurrencyType))
                    {
                        GameManager.CurrencyManager.AddScore(1000, moneyCurrencyType);
                        
                        GameManager.UIManager.HideOne("PopupWindow", false);
                    }
                    GameManager.UIManager.OpenPopupWindow(GameManager.CurrencyManager.
                        GetScoreAsInt(specialCurrencyType), 10, "PopupWindow");
                    break;
                
                case <= 10000:
                    if (GameManager.CurrencyManager.RemoveScore(50, specialCurrencyType))
                    {
                        GameManager.CurrencyManager.AddScore(10000, moneyCurrencyType);
                        
                        GameManager.UIManager.HideOne("PopupWindow", false);
                    }
                    GameManager.UIManager.OpenPopupWindow(GameManager.CurrencyManager.
                        GetScoreAsInt(specialCurrencyType), 50, "PopupWindow");
                    break;

                case <= 100000:
                    if (GameManager.CurrencyManager.RemoveScore(1000, specialCurrencyType))
                    {
                        GameManager.CurrencyManager.AddScore(100000, moneyCurrencyType);
                        
                        GameManager.UIManager.HideOne("PopupWindow", false);
                    }
                    GameManager.UIManager.OpenPopupWindow(GameManager.CurrencyManager.
                        GetScoreAsInt(specialCurrencyType), 1000, "PopupWindow");
                    break;
            }
        }
        
        private void ShowMessage(object msg)
        {
            messageText.text = (string)msg;
        }
        
        private void ShowOfferMessage(object msg)
        {
            missingAmount = (int)msg;
        }
        
        public string UITag { get; set; } = "PopupWindow";
        public UIScenarios[] UIScenarios { get; set; } = { Core.Managers.UIScenarios.Welcome };
        public void HideUI(bool isHide)
        {
            gameObject.SetActive(isHide);
        }

        public void LockUI(bool isLock)
        {
            GetComponent<CanvasGroup>().interactable = isLock;
        }

        public void DoTweenOpenClose(bool isOpen)
        {
            
        }
    }
}