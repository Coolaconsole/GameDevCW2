using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StoryManager : MonoBehaviour
{
    // do we do this?: [SerializeField]
    Dictionary<string, bool> eventFlags;

    public delegate void storyEventOccured(string eventName);
    public event storyEventOccured eventOccured;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        eventFlags = new Dictionary<string, bool>() {
            {"Default Story Event", false},
            {"test", false}
        };
    }

    //Update the story state when a new event occurs
    public void StoryEvent(StoryEventInfo info) {
        Debug.Log("Story event: " + info.eventName);
        //must be valid event
        if (eventFlags.ContainsKey(info.eventName)) {
            //event must not have already happened
            if (eventFlags[info.eventName] == false)
            {
                eventFlags[info.eventName] = true;
                eventOccured?.Invoke(info.eventName);
            }
        }
        foreach(string key in eventFlags.Keys)
        {
            Debug.Log(key + ": " + eventFlags[key]);
        }
    }

    //allows other scripts to check the state of a given story event
    public bool getEvent(string eventName) {
        if (eventFlags.ContainsKey(eventName)) {
            return eventFlags[eventName];
        }
        return false;
    }
}
