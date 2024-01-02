using Base.Core.Components;
using UnityEngine;

namespace Base.Core.Managers
{
    public class TabsComponent : MyMonoBehaviour, IUIComponent
    {
        [SerializeField] private string uiTagName;
        void Start()
        {
            UITag = uiTagName;
            GameManager.UIManager.RegisterUIComponent(this);
        }
        
        void OnDestroy()
        {
            GameManager.UIManager.UnRegisterUIComponent(this);
        }

        public string UITag { get; set; }
        public UIScenarios[] UIScenarios { get; set; } = { Managers.UIScenarios.Welcome };
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
