using System.Collections.Generic;
using Base.Core.Components;
using Base.Core.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Gameplay.Components
{
    public class ItemUIComponent : MyMonoBehaviour
    {
        [Header("General Text")]
        [SerializeField] public TMP_Text itemName;
        [SerializeField] public TMP_Text itemDescription;

        [Header("Text that changes")]
        [SerializeField] public TMP_Text powerText;
        [SerializeField] public TMP_Text costText;
        [SerializeField] public TMP_Text levelText;
        [SerializeField] public TMP_Text multiText;

        [Header("Milestone Text")]
        [SerializeField] public TMP_Text milestoneLevel;
        [SerializeField] public TMP_Text milestoneLevelNeeded;

        [Header("Animation")]
        [SerializeField] public float lerpTime = 0.5f;
        [SerializeField] public Ease easeType;
        [SerializeField] public float sizeMulti;
        [SerializeField] public int popUpDistanceMultiplier;

        [Header("Action Button")]
        [SerializeField] public Button button;
        [SerializeField] public GameObject claimCurrency;

        [Header("Bar")]
        [SerializeField] public Image fillImage;
        [SerializeField] public Image image;

        [Header("Internal Variables")]
        public int breadPerTick;
        public UpgradeType upgradeType;
        public ResearchType researchType;
        public MilestoneType milestoneType;
        public CurrencyType currencyType;
        public List<Sprite> upgradeIconsArray;

        [Header("Sound")]
        public AudioComponent audioComponent;
        public AudioSource fxSource;
        public AudioClip soundClip;

        public void UpdateIcon(int index)
        {
            image.sprite = upgradeIconsArray[index];
        }

        public string ScoreToString(int amount)
        {
            if (amount >= 1000)
            {
                int power = Mathf.FloorToInt(Mathf.Log10(amount) / 3);
                float value = amount / Mathf.Pow(1000, power);
                string suffix = "kmbt"[power - 1].ToString();
                string formattedValue = value.ToString("F1");
                return "$" + formattedValue + suffix;
            }
            return "$" + amount;
        }

        public string BreadPowerToString(float amount)
        {
            if (amount >= 1000)
            {
                int power = Mathf.FloorToInt(Mathf.Log10(amount) / 3);
                float value = amount / Mathf.Pow(1000, power);
                char suffix = "abcdefghijklmnopqrstuv"[power - 1];
                string formattedValue = value.ToString("F1");
                return $"{formattedValue} {suffix} /s";
            }

            return $"{amount} /s";
        }

        public string BreadAmountToString(float amount)
        {
            if (amount >= 1000)
            {
                int power = Mathf.FloorToInt(Mathf.Log10(amount) / 3);
                float value = amount / Mathf.Pow(1000, power);
                char suffix = "abcdefghijklmnopqrstuv"[power - 1];
                string formattedValue = value.ToString("F1");
                return $"{formattedValue} {suffix}";
            }

            return $"{amount}";
        }

        public void DoTweenVFX(GameObject objectToTween)
        {
            objectToTween.transform.DOScale(Vector3.one * sizeMulti, lerpTime).SetEase(easeType);
            objectToTween.transform.DOScale(Vector3.one, lerpTime);
        }

        public void DoSpritePopupVFX()
        {
            for (int i = 0; i < breadPerTick; i++)
            {
                var bread = GameManager.PoolManager.TryGetFromPools<SpritePopUpComponent>("SpritePopUp");
                var newPos = transform.position + (Vector3)Random.insideUnitCircle * popUpDistanceMultiplier;
                bread.Init(transform.position, newPos);
            }
        }
    }
}