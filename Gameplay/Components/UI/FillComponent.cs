using Base.Core.Components;
using Base.Core.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using EventType = Base.Core.Managers.EventType;

namespace Base.Gameplay.Components
{
    public class FillComponent : MyMonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private GameObject button;
        
        [SerializeField] private int dayHours;
        [SerializeField] private int maxHours = 10;
        [SerializeField] private float fillAmountDuration = 0.1f;
        
        private bool startTicker;
        private void Awake()
        {
            GameManager.EventsManager.AddListener(Core.Managers.EventType.Tick, Ticker);
            GameManager.EventsManager.AddListener(Core.Managers.EventType.OnSellClick, StartDay);
        }

        private void Start()
        {
            dayHours = 0;
            startTicker = false;
        }
        
        private void StartDay(object objectToPass = null)
        {
            startTicker = true;
        }
        
        private void Ticker(object objectToPass = null)
        {
            if (dayHours <= maxHours && startTicker)
            {
                dayHours += 1;
                var amount = GameManager.CurrencyManager.GetScoreAsInt(CurrencyType.Baked);
                GameManager.CurrencyManager.RemoveScore(amount, CurrencyType.Baked);
                GameManager.CurrencyManager.AddScore(amount, CurrencyType.Money);
                fillImage.DOFillAmount(amount/100f, fillAmountDuration).SetEase(Ease.Linear);
            }
            else if (dayHours >= maxHours && startTicker)
            {
                fillImage.DOFillAmount(1, fillAmountDuration).SetEase(Ease.Linear);
                button.SetActive(true);
            }
        }
    }
}
