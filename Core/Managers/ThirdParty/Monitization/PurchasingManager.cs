using System;
using Newtonsoft.Json;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Base.Core.Managers
{
    public class PurchasingManager : BaseManager, IDetailedStoreListener
    {
        private IStoreController storeController;
        private IExtensionProvider extensionProvider;

        private Action<bool> purchaseCompleteAction;
        private IAPConfig iapConfig;
        
        public PurchasingManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            iapConfig = GameManager.Instance.ConfigManager.GetConfig<IAPConfig>();

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var item in iapConfig.SKUItems)
            {
                builder.products.Add(new ProductDefinition(item, ProductType.Consumable));
            }
            
            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            extensionProvider = extensions;
            
            OnInitComplete();
        }
        
        public void Purchase(string sku, Action<bool> onPurchaseResult)
        {
            purchaseCompleteAction = onPurchaseResult;
            storeController.InitiatePurchase(sku);
        }
        
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            GameManager.MonitorManager.ReportException(error.ToString());
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            GameManager.MonitorManager.ReportException(message);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            purchaseCompleteAction.Invoke(true);
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            purchaseCompleteAction.Invoke(false);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            purchaseCompleteAction.Invoke(false);
        }

    }
    
    [Serializable]
    public class IAPConfig : BaseConfig
    {
        [JsonProperty("SKUs")]
        public string[] SKUItems { get; set; }
    }


}