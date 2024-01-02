using System.Collections.Generic;
using Base.Core.Components;
using Base.Core.Managers;
using DG.Tweening;
using UnityEngine;

namespace Base.Gameplay.Components
{
    public class MilestonesMenuComponent : MyMonoBehaviour, IUIComponent
    {
        [SerializeField] private List<MilestoneItemUIComponent> milestonesItems;
        private List<MilestoneableConfig> milestones;
        
        [SerializeField] private float lerpTime = 0.56f;
        [SerializeField] private Ease easeType;
        
        [SerializeField] public Sprite[] iconsArray;

        private void Start()
        {
            milestones = GameManager.MilestoneManager.GetAllMilestones();
            GenerateItems();
            InitializeItems();
            
            GameManager.UIManager.RegisterUIComponent(this);

        }

        public void InitializeItems()
        {
            for (var index = 0; index < milestones.Count; index++)
            {
                var milestoneable = milestones[index];
                
                milestonesItems[index].upgradeIconsArray.Add(iconsArray[5]);
                milestonesItems[index].upgradeIconsArray.Add(iconsArray[index]);
                
                milestonesItems[index].Init(milestoneable);
            }
        }

        public void GenerateItems()
        {
            //TODO: use poolables
            var firstItem = milestonesItems[0];
            firstItem.gameObject.SetActive(false);

            for (var index = 1; index < milestones.Count; index++)
            {
                var tempItem = Instantiate(firstItem, firstItem.transform.parent, false);
                tempItem.gameObject.SetActive(false);
                milestonesItems.Add(tempItem);
            }
        }

        private void OnDestroy()
        {
            GameManager.UIManager.UnRegisterUIComponent(this);
        }
        
        public string UITag { get; set; } = "MilestoneMenu";
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
            if (isOpen)
            {
                gameObject.transform.DOScale(Vector3.zero, lerpTime).SetEase(easeType);
            }
            else
            {
                gameObject.transform.DOScale(Vector3.one, lerpTime).SetEase(easeType);
            }        
        }
    }
}