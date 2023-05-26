using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomAnimationEventReceiver : MonoBehaviour
{
    [Serializable] public class EventDefinition
    {
        [SerializeField] private string name = string.Empty;
        public string Name { get { return name; } }

        [SerializeField] private UnityEvent Callback = new UnityEvent();

        public void Invoke()
        {
            Callback.Invoke();
        }
    }

    [SerializeField] private List<EventDefinition> events = new List<EventDefinition>();
    private Dictionary<string, EventDefinition> eventsDictionary = new Dictionary<string, EventDefinition>();

    private void Awake()
    {
        foreach (var @event in events)
            eventsDictionary.Add(@event.Name, @event);
    }

    public void Invoke(string name)
    {
        EventDefinition eventDefinition = null;
        if (eventsDictionary.TryGetValue(name, out eventDefinition))
            eventDefinition.Invoke();
        else
            Debug.Log(string.Format("No event named {0} related to object {1}.", name, this.gameObject.name), this);
    }
}
