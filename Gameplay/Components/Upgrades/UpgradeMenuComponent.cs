using System.Collections.Generic;
using Base.Core.Components;
using Base.Core.Managers;
using DG.Tweening;
using UnityEngine;

namespace Base.Gameplay.Components
{
    public class UpgradeMenuComponent : MyMonoBehaviour, IUIComponent
    {
        [SerializeField] private List<UpgradeItemUIComponent> upgradeItems;
        private List<UpgradeableConfig> upgradeables;
            
        [SerializeField] private float lerpTime = 0.56f;
        [SerializeField] private Ease easeType;
            
        [SerializeField] public Sprite[] iconsArray;
        private void Start()
        {
            upgradeables = GameManager.UpgradeManager.GetAllUpgrades();
                
                
            GenerateUpgradeItems();
            InitializeUpgradeItems();
            GameManager.DateTimeManager.TimerLoop();
            GameManager.UIManager.RegisterUIComponent(this);

        }
        private void GenerateUpgradeItems()
        {
            var firstItem = upgradeItems[0];
            firstItem.gameObject.SetActive(false);

            for (var index = 1; index < upgradeables.Count; index++)
            {
                var tempItem = Instantiate(firstItem, firstItem.transform.parent, false);
                tempItem.gameObject.SetActive(false);
                upgradeItems.Add(tempItem);
            }
        }
        private void InitializeUpgradeItems()
        {
            for (var index = 0; index < upgradeables.Count; index++)
            {
                var upgradeable = upgradeables[index];
                    
                upgradeItems[index].upgradeIconsArray.Add(iconsArray[4]);
                upgradeItems[index].upgradeIconsArray.Add(iconsArray[index]);
                    
                upgradeItems[index].Init(upgradeable);
            }
        }
            
        private void OnDestroy()
        {
            GameManager.UIManager.UnRegisterUIComponent(this);
        }
            
        public string UITag { get; set; } = "MainMenu";
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