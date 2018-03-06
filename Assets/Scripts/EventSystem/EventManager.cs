using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    /// <summary>
    /// TODO :Make this class generic and specialize for each type of message
    /// </summary>
    /// 
    public class UnityEventFloat : UnityEvent<float>
    {

    }


    private Dictionary<string, UnityEventFloat> eventDictionary;

    private static EventManager eventManager;

   
    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEventFloat>();
        }
    }

    public static void StartListening(string eventName, UnityAction<float> listener)
    {
        UnityEventFloat thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEventFloat();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<float> listener)
    {
        if (eventManager == null) return;
        UnityEventFloat thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    
    public static void TriggerEvent(string eventName, float value)
    {
        UnityEventFloat thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(value);
        }
    }
}