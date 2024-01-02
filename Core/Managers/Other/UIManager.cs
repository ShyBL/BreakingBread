using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Core.Managers
{
    public class UIManager : BaseManager
    {
        private List<IUIComponent> menuComponents = new();
        private Dictionary<UIScenarios, ScenarioStatus> scenarioStatusMap = new();
        
        public UIManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            PopulateScenarioStatusMap();
            OnInitComplete();
        }

        public List<IUIComponent> GetAllMenuComponentsList()
        {
            return menuComponents;
        }
        
        public void OpenPopupWindow(int currentUserScore, int scoreNeededForNextLevel, string tagName)
        {
            string msg = String.Empty;
            int offerMsg = 0;
            
            if (tagName == "PopupWindow")
            {
                msg = $"Tried to buy, had {currentUserScore} Currency and needed {scoreNeededForNextLevel}";
                offerMsg = scoreNeededForNextLevel - currentUserScore;
            }
            
            OpenWindowWithMessageByTag(tagName, true, msg, offerMsg);
        }
        private void OpenWindowWithMessageByTag(string tagName, bool isShow, string msg, int offerMsg)
        {
            foreach (var comp in menuComponents.Where(comp => comp.UITag == tagName))
            {
                comp.HideUI(isShow);
                
                GameManager.EventsManager.InvokeEvent(EventType.OnOfferMessage, offerMsg);
                GameManager.EventsManager.InvokeEvent(EventType.OnMessage, msg);
            }
        }
        public void HideAllAndOpenByTags(string menuTagName, string tabsTagName, bool isShow)
        {
            foreach (var comp in menuComponents)
            {
                if (comp.UITag == menuTagName || comp.UITag == tabsTagName)
                {
                    //comp.DoTweenOpenClose(false);
                    comp.HideUI(isShow);
                }
                else
                {
                    //comp.DoTweenOpenClose(true);
                    comp.HideUI(!isShow);
                }
            }
            
        }
        
        public void HideOne(string tagName, bool isHide)
        {
            foreach (var comp in menuComponents.Where(comp => comp.UITag == tagName))
            {
                comp.HideUI(isHide);
            }
        }

        public void LockAll(bool isLock)
        {
            foreach (var comp in menuComponents)
            {
                comp.LockUI(isLock);
            }
        }
        
        public void LockByTag(bool isLock, string tagName)
        {
            foreach (var comp in menuComponents.Where(comp => comp.UITag == tagName))
            {
                comp.LockUI(isLock);
            }
        }
        
        public void LockByScenario(bool isLock, UIScenarios scenario)
        {
            scenarioStatusMap[scenario].IsLock = isLock;
            
            foreach (var comp in menuComponents)
            {
                var shouldLock = false; 
                var componentScenarios = comp.UIScenarios ?? Array.Empty<UIScenarios>();
        
                if (componentScenarios.Any(compScenario => scenarioStatusMap[compScenario].IsLock))
                {
                    shouldLock = true;
                }
        
                comp.LockUI(shouldLock);
            }
        }
        
        public void RegisterUIComponent(IUIComponent comp)
        {
            if (!menuComponents.Contains(comp))
            {
                menuComponents.Add(comp);
            }
        }

        public void UnRegisterUIComponent(IUIComponent comp)
        {
            if (!menuComponents.Contains(comp))
            {
                menuComponents.Remove(comp);
            }
        }
        
        
        private void PopulateScenarioStatusMap()
        {
            var enumValues = Enum.GetValues(typeof(UIScenarios));

            foreach (UIScenarios enumValue in enumValues)
            {
                scenarioStatusMap[enumValue] = new ScenarioStatus();
            }
        }
    }
    
    public interface IUIComponent
    {
        string UITag { get; set; }
        UIScenarios[] UIScenarios { get; set; }
        void HideUI(bool isHide);
        void LockUI(bool isLock);
        void DoTweenOpenClose(bool isOpen);
    }

    public enum UIScenarios
    {
        Welcome,
        GeneralAnimation,
        GameClosing,
        GameLoading,
        Rewarding
    }

    public class ScenarioStatus
    {
        public bool IsLock;
        public bool IsHide;
    }
}