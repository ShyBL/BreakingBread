using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Core.Managers
{
    // The EventsManager class manages events and their listeners.
    public class EventsManager : BaseManager
    {
        private Dictionary<EventType, EventListenersData> eventListenersData = new();
        public EventsManager(Action<BaseManager> onComplete) : base(onComplete)
        {
            OnInitComplete();
        }
        // Adds listener for the specified event type, and adds action to be invoked when the event is triggered
        public void AddListener(EventType eventType, Action<object> methodToInvoke)
        {
            if (eventListenersData.TryGetValue(eventType, out var value))
            {
                value.ActionsOnInvoke.Add(methodToInvoke);
            }
            else
            {
                eventListenersData[eventType] = new EventListenersData(methodToInvoke);
            }
        }

        // Removes listener for the specified event type
        public void RemoveListener(EventType eventType, Action<object> methodToInvoke)
        {
            if (!eventListenersData.TryGetValue(eventType, out var value))
            {
                return;
            }

            value.ActionsOnInvoke.Remove(methodToInvoke);

            if (!value.ActionsOnInvoke.Any())
            {
                eventListenersData.Remove(eventType);
            }
        }

        // Invokes all listeners registered for the specified event type, and the data to pass to the listeners when invoking the event
        public void InvokeEvent(EventType eventType, object dataToInvoke)
        {
            if (!eventListenersData.TryGetValue(eventType, out var value))
            {
                return;
            }

            foreach (var method in value.ActionsOnInvoke)
            {
                method.Invoke(dataToInvoke);
            }
        }
    }
    
    // Represents the data of listeners for a specific event type.
    public class EventListenersData
    {
        public List<Action<object>> ActionsOnInvoke;

        public EventListenersData(Action<object> additionalData)
        {
            ActionsOnInvoke = new List<Action<object>>
            {
                additionalData
            };
        }
    }
    
    // Enum representing different event types.
    public enum EventType
    {
        OnResearched,
        Tick,
        OnUpgraded,
        OnScoreChanged,
        OnMilestone,
        OnMessage,
        OnOfferMessage,
        OnSellClick
    }
}