using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Base.Core.Managers
{
    public class FactoryManager : BaseManager
    {
        public FactoryManager(Action<BaseManager>onComplete) : base(onComplete)    
        {
            OnInitComplete();
        }
        
        public T[] CreateObjects<T>(string originalName, int amount) where T : Component
        {
            var created = new List<T>();
            
            for (var i = 0; i < amount; i++)
            {
                var generated = CreateObject<T>(originalName);
                created.Add(generated);
            }
            return created.ToArray();
        }

        public T CreateObject<T>(string originalName) where T : Component
        {
            var original = Resources.Load<T>(originalName);
            return Object.Instantiate(original);
        }
    }
}