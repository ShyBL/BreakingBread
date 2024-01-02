using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Base.Core.Managers
{
    public class StoreManager : BaseManager
    {
        private readonly StoresConfig storesConfig;
        
        private Action<bool> onPurchaseComplete;
        private StoreOfferData currentOfferPurchase;
        private bool isProcessingPurchase;
        
        public StoreManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            storesConfig = GameManager.Instance.ConfigManager.GetConfig<StoresConfig>();
            OnInitComplete();
        }

        public void GetStoreByID(string storeID)
        {
            
        }

        public void TryPurchaseItem(StoreOfferData offerData, Action<bool> onResult)
        {
            if (isProcessingPurchase)
            {
                return;
            }
            
            isProcessingPurchase = true;
            
            onPurchaseComplete = onResult;
            currentOfferPurchase = offerData;
            
            GameManager.PurchaseManager.Purchase(offerData.SKU, OnPurchaseComplete);
        }

        private void OnPurchaseComplete(bool isSuccess)
        {
            if (isSuccess)
            {
                GiveItemsToUser();
            }
            
            onPurchaseComplete.Invoke(isSuccess);
            
            onPurchaseComplete = null;
            currentOfferPurchase = null;
            isProcessingPurchase = false;
        }

        private void GiveItemsToUser()
        {
            foreach (var productsData in currentOfferPurchase.StoreProductsData)
            {
                //example
                //event shoot with product data
            }
        }
    }

    public class StoresConfig : BaseConfig
    {
        [JsonProperty("Stores")]
        public Dictionary<string, StoreData> Stores { get; set; }
    }

    public class StoreData
    {
        [JsonProperty("StoreName")]
        public string StoreName { get; set; }

        [JsonProperty("PrefabName")]
        public string PrefabName { get; set; }

        [JsonProperty("StoreOffersData")]
        public List<StoreOfferData> StoreOffersData { get; set; }
    }

    public class StoreOfferData
    {
        [JsonProperty("SKU")]
        public string SKU { get; set; }
        
        [JsonProperty("PriceInDollars")]
        public string PriceInDollars { get; set; }

        [JsonProperty("PrefabName")]
        public string PrefabName { get; set; }

        [JsonProperty("Text")]
        public string Text { get; set; }

        [JsonProperty("StoreProductsData")]
        public List<StoreProductsData> StoreProductsData { get; set; }
    }

    public class StoreProductsData
    {
        [JsonProperty("ProductType")]
        public string ProductType { get; set; }

        [JsonProperty("ProductAmount")]
        public int ProductAmount { get; set; }

        [JsonProperty("ItemIconName")]
        public string ItemIconName { get; set; }
    }
}