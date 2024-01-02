using System;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Core.Managers
{
    // The CurrencyManager class manages the player's currency and provides methods to add, remove, and retrieve currency amounts.
    public class CurrencyManager : BaseManager
    {
        private PlayerCurrencyData allCurrencyData; // Stores the player's currency data.
        public CurrencyManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            allCurrencyData = GameManager.SaveManager.LoadData<PlayerCurrencyData>();

            if (allCurrencyData == null)
            {
                allCurrencyData = new PlayerCurrencyData();
                allCurrencyData.CurrencyList.Add(CurrencyType.Money, new CurrencyData
                    {
                        CurrencyType = CurrencyType.Money,
                        CurrencyAmount = 0
                    }
                );

                GameManager.SaveManager.SaveData(allCurrencyData);
            }
            
            OnInitComplete();
        }
        
        public void AddScore(int amount, CurrencyType scoreTypes) // Add amount of currency the given type 
        {
            if (!allCurrencyData.CurrencyList.TryGetValue(scoreTypes,
                    out var scoreData)) // Check if the currency type exists in the dictionary
            {
                // If the currency type doesn't exist, add a new entry to the dictionary with default amount (0)
                allCurrencyData.CurrencyList.Add(scoreTypes, new CurrencyData
                {
                    CurrencyType = scoreTypes,
                    CurrencyAmount = 0
                });
            }
            allCurrencyData.CurrencyList[scoreTypes].ChangeAmount(amount);
            
            GameManager.EventsManager.InvokeEvent(EventType.OnScoreChanged, allCurrencyData.CurrencyList[scoreTypes]);

            GameManager.SaveManager.SaveData(allCurrencyData);
        }
        
        public bool RemoveScore(int amount, CurrencyType scoreTypes)
        {
            if (allCurrencyData.CurrencyList.TryGetValue(scoreTypes, out var scoreData))
            {
                if (scoreData.CurrencyAmount >= amount)
                {
                    scoreData.ChangeAmount(-amount);

                    GameManager.EventsManager.InvokeEvent(EventType.OnScoreChanged, scoreData);

                    GameManager.SaveManager.SaveData(allCurrencyData);

                    return true;
                }
                
                MyDebug.Log($"Insufficient {scoreTypes} to remove. Current amount: {scoreData.CurrencyAmount}, Attempted to remove: {amount}");
                return false;
            }
            
            allCurrencyData.CurrencyList.Add(scoreTypes, new CurrencyData
            {
                CurrencyType = scoreTypes,
                CurrencyAmount = 0
            });
            
            GameManager.SaveManager.SaveData(allCurrencyData);
            return false;
        }
        
        public int GetScoreAsInt(CurrencyType scoreTypes)
        {
            if (this.allCurrencyData.CurrencyList.TryGetValue(scoreTypes, out var currencyData))
            {
                return currencyData.GetCurrencyAmountInt();
            }
            this.allCurrencyData.CurrencyList.Add(scoreTypes, new CurrencyData{ CurrencyType = scoreTypes,CurrencyAmount = 0} );
            MyDebug.Log($"Given key {scoreTypes} was not present in the dictionary, created new and set score to 0.");
            return 0;
        }
        
        public string GetScoreAsString(CurrencyType scoreTypes)
        {
            if (this.allCurrencyData.CurrencyList.TryGetValue(scoreTypes, out var currencyData))
            {
                if (scoreTypes == CurrencyType.Baked)
                {
                    return currencyData.GetBakedAmountString();
                }
                return currencyData.GetCoinAmountString();
                
            }
            this.allCurrencyData.CurrencyList.Add(scoreTypes, new CurrencyData{ CurrencyType = scoreTypes,CurrencyAmount = 0} );
            GameManager.MonitorManager.ReportException($"Given key {scoreTypes} was not present in the dictionary, Returned 0.");
            return "0";
        }
    }
    
    // Represents the player's currency data, containing a dictionary of CurrencyType and CurrencyData objects
    [Serializable]
    public class PlayerCurrencyData : ISaveData
    {
        public Dictionary<CurrencyType, CurrencyData> CurrencyList = new()
        {
            { CurrencyType.SpecialCurrency, new CurrencyData { CurrencyType = CurrencyType.SpecialCurrency, CurrencyAmount = 0 } },
            { CurrencyType.Baked, new CurrencyData { CurrencyType = CurrencyType.Baked, CurrencyAmount = 0 } }
        };
    }
    
    // Represents a specific currency type and its amount
    [Serializable]
    public class CurrencyData
    {
        public CurrencyType CurrencyType;
        public int CurrencyAmount;
        
        public void ChangeAmount(int amount)
        {
            int newAmount = CurrencyAmount + amount;
            if (newAmount < 0)
            {
                MyDebug.Log("Cannot decrease CurrencyAmount below 0.");
                return;
            }
            CurrencyAmount += amount;
        }
        
        public int GetCurrencyAmountInt() => CurrencyAmount;
        
        public string GetCoinAmountString()
        {
            if (CurrencyAmount >= 1000)
            {
                int power = Mathf.FloorToInt(Mathf.Log10(CurrencyAmount) / 3);
                float value = CurrencyAmount / Mathf.Pow(1000, power);
                string suffix = "kmbt"[power - 1].ToString();
                string formattedValue = value.ToString("F");
                return "$" + formattedValue + suffix;
            }
            return "$" + CurrencyAmount;
        }
        
        public string GetBakedAmountString()
        {
            if (CurrencyAmount >= 1000)
            {
                int power = Mathf.FloorToInt(Mathf.Log10(CurrencyAmount) / 3);
                float value = CurrencyAmount / Mathf.Pow(1000, power);
                char suffix = "abcdefghijklmnopqrstuv"[power - 1];
                string formattedValue = value.ToString("F");
                return formattedValue + suffix;
            }
            return CurrencyAmount.ToString();
        }
        
    }
    // Enum representing different currency types
    public enum CurrencyType
    {
        Money, SpecialCurrency, Baked 
    }
}