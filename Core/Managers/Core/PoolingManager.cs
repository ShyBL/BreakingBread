using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.Core.Managers
{
    public class PoolingManager : BaseManager
    {
        private Dictionary<string, PoolData> pools = new Dictionary<string, PoolData>();

        private GameObject poolsHolder;
        
        public PoolingManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            OnInitComplete();
        }
        
        public void InitializePools<T>(string originalName, int amount) where T : Component
        {
            poolsHolder = new GameObject("PoolsHolder");
            
            var generateObjects = GameManager.FactoryManager.CreateObjects<T>(originalName, amount);

            if (generateObjects == null || !generateObjects.Any())
            {
                GameManager.MonitorManager.ReportException("Failed to generate objects.");
                return;
            }

            var poolParent = new GameObject($"Pool_{originalName}");
            poolParent.transform.SetParent(poolsHolder.transform);
            
            pools[originalName] = new PoolData(generateObjects, poolParent);
        }

        public T TryGetFromPools<T>(string poolName) where T : Component
        {
            if (!pools.ContainsKey(poolName)) 
            {
                GameManager.MonitorManager.ReportException($"No pool found with the name {poolName}. Pool not initialized.");
                return null;
            }
            // Check if available, if not generate only 1, add it and later return it
            if (!pools[poolName].AvailableItems.Any())
            {
                var generateObject = GameManager.FactoryManager.CreateObject<T>(poolName);
                pools[poolName].AddNewToPool(generateObject);
            }
            // GetFromPool
            var availableItem = pools[poolName].AvailableItems[0];
            pools[poolName].AvailableItems.Remove(availableItem);
            pools[poolName].UnAvailableItems.Add(availableItem);

            availableItem.gameObject.SetActive(true);
            return (T) availableItem;
        }

        public void ReturnToPool<T>(string poolName, T returnedObject) where T : Component
        {
            pools[poolName].AvailableItems.Add(returnedObject);
            pools[poolName].UnAvailableItems.Remove(returnedObject);
            
            returnedObject.gameObject.SetActive(false);
        }
    }

    public class PoolData
    {
        public List<Component> TotalItems; // Backup list for everything
        public List<Component> AvailableItems;
        public List<Component> UnAvailableItems;

        private GameObject PoolHolder; // Parent holder for scene view
        
        public PoolData(Component[] generateObjects, GameObject poolHolder)
        {
            TotalItems = generateObjects.ToList();
            AvailableItems = generateObjects.ToList();
            UnAvailableItems = new List<Component>();

            PoolHolder = poolHolder;
            
            foreach (var generateObject in generateObjects)
            {
                generateObject.transform.SetParent(PoolHolder.transform);
                generateObject.gameObject.SetActive(false);
            }
        }

        public void AddNewToPool<T>(T generateObject) where T : Component
        {
            TotalItems.Add(generateObject);
            AvailableItems.Add(generateObject);
            generateObject.transform.SetParent(PoolHolder.transform);
            generateObject.gameObject.SetActive(false);
        }
    }
}