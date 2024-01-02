using System;
using System.Threading.Tasks;

namespace Base.Core.Managers
{
    public class BaseManager
    {
        protected GameManager GameManager => GameManager.Instance;
        private Action<BaseManager> onCompleteAction;

        protected BaseManager(Action<BaseManager> onComplete)
        {
            onCompleteAction = onComplete;
        }

        protected async void OnInitComplete()
        {
            await Task.Delay(500);
            onCompleteAction.Invoke(this);
        }
        
        
    }
}