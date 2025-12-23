using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StoryManager : MonoBehaviour
{
    //the flags being used in the current level - reverted by reset
    Dictionary<string, bool> currentEventFlags;

    //the flags saved at the end of each level - allows for reseting story events if a level is restarted
    Dictionary<string, bool> savedEventFlags;

    public delegate void storyEventOccured(string eventName);
    public event storyEventOccured eventOccured;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        currentEventFlags = new Dictionary<string, bool>() {
            {"Default Story Event", false},
            {"1.1", false},
            {"1.2 Neutral", false},
            {"1.2 Evil", false},
            {"1.3 Neutral", false},
            {"1.3 Evil", false},
            {"1.3 Good", false},
            {"Ask rich", false},
            {"Ask log", false},
            {"Ask food", false},
            {"2.1 Offering", false},
            {"2.1 NoOffering", false},
            {"2.2 Steal", false},
            {"2.2 Give", false},
            {"2.2 Neutral", false},
            {"2.2 Evil", false},
            {"2.2 Good", false},
            {"RecivedFire", false},
            {"Eel", false},
            {"Spider", false},
            {"RecivedHammer", false},
            {"3 Neutral", false},
            {"3 Evil", false},
            {"3 Good", false},
            {"3.1 Neutral", false},
            {"3.1 Evil", false},
            {"3.1 Good", false},
            {"3.2 Neutral", false},
            {"3.2 Evil", false},
            {"3.2 Good", false},
            {"3.3 Neutral", false},
            {"3.3 Evil", false},
            {"3.3 Good", false},
        };

        savedEventFlags = currentEventFlags;
    }

    //Update the story state when a new event occurs
    public void StoryEvent(StoryEventInfo info) {
        Debug.Log("Story event: " + info.eventName);
        //must be valid event
        if (currentEventFlags.ContainsKey(info.eventName)) {
            //event must not have already happened
            if (currentEventFlags[info.eventName] == false)
            {
                currentEventFlags[info.eventName] = true;
                eventOccured?.Invoke(info.eventName);
            }
        }
    }

    //allows other scripts to check the state of a given story event
    public bool getEvent(string eventName) {
        if (currentEventFlags.ContainsKey(eventName)) {
            return currentEventFlags[eventName];
        }
        return false;
    }

    //reset event flags to where they were at the start of the level
    public void Reset()
    {
        currentEventFlags = savedEventFlags;
    }

    //save the event flags so they persist after reset - called at end of level
    public void Save()
    {
        savedEventFlags = currentEventFlags;
    }
}
