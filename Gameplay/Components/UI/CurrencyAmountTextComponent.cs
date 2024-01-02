using Base.Core.Components;
using Base.Core.Managers;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.UI;
using EventType = Base.Core.Managers.EventType;

namespace Base.Gameplay.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class CurrencyAmountTextComponent : MyMonoBehaviour
    {
        //Text
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private CurrencyType currencyType;
        // Animation
        [SerializeField] private Image currencyArt;
        [SerializeField] private float lerpTime = 0.5f;
        [SerializeField] private Ease easeType;
        [SerializeField] private float sizeMulti;
        
        private void Start()
        {
            GetTMPComponent();
            amountText.text = GameManager.CurrencyManager.GetScoreAsString(currencyType);
            
            Core.Managers.GameManager.Instance.EventsManager.AddListener(EventType.OnScoreChanged, OnCurrencyChanged);
        }
        
        private void OnCurrencyChanged(object eventData)
        {
            var type = ((CurrencyData)eventData).CurrencyType;
            
            if (type == currencyType)
            {
                amountText.text = (currencyType == CurrencyType.Baked) 
                    ? ((CurrencyData)eventData).GetBakedAmountString() 
                    : ((CurrencyData)eventData).GetCoinAmountString();
                
                DoVFX();
            }
        }
        
        private void DoVFX()
        {
            currencyArt.transform.DOScale(Vector3.one * sizeMulti, lerpTime).SetEase(easeType);
            currencyArt.transform.DOScale(Vector3.one, lerpTime);
        }
        
        private void GetTMPComponent()
        {
            amountText = GetComponent<TMP_Text>();
        }
        
        private void Reset()
        {
            GetTMPComponent();
        }
    }
}