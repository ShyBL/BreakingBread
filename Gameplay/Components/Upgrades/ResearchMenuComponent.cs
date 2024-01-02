using System.Collections.Generic;
using Base.Core.Components;
using Base.Core.Managers;
using DG.Tweening;
using UnityEngine;

namespace Base.Gameplay.Components
{
    public class ResearchMenuComponent : MyMonoBehaviour, IUIComponent
    { 
        [SerializeField] private List<ResearchItemUIComponent> researchItems;
        private List<ResearchableConfig> researchables;
        
        [SerializeField] private float lerpTime = 0.56f;
        [SerializeField] private Ease easeType;
        
        [SerializeField] public Sprite[] iconsArray;

        private void Start()
        {
            researchables = GameManager.ResearchManager.GetAllResearches();
            GenerateItems();
            InitializeItems();
            
            GameManager.UIManager.RegisterUIComponent(this);

        }

        private void InitializeItems()
        {
            for (var index = 0; index < researchables.Count; index++)
            {
                var researchable = researchables[index];
                
                researchItems[index].upgradeIconsArray.Add(iconsArray[5]);
                researchItems[index].upgradeIconsArray.Add(iconsArray[index]);
                
                researchItems[index].Init(researchable);
            }
        }

        private void GenerateItems()
        {
            //TODO: use poolables
            var firstItem = researchItems[0];
            firstItem.gameObject.SetActive(false);

            for (var index = 1; index < researchables.Count; index++)
            {
                var tempItem = Instantiate(firstItem, firstItem.transform.parent, false);
                tempItem.gameObject.SetActive(false);
                researchItems.Add(tempItem);
            }
        }

        private void OnDestroy()
        {
            GameManager.UIManager.UnRegisterUIComponent(this);
        }
        
        public string UITag { get; set; } = "ResearchMenu";
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