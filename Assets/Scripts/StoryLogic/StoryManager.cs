using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }

    //the flags being used in the current level - reverted by reset
    Dictionary<string, bool> currentEventFlags;

    //the flags saved at the end of each level - allows for reseting story events if a level is restarted
    Dictionary<string, bool> savedEventFlags;

    public delegate void storyEventOccured(string eventName);
    public event storyEventOccured eventOccured;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (currentEventFlags == null || currentEventFlags.Count == 0)
            InitFlags();
    }

    private void InitFlags()
    {
        if (currentEventFlags == null)
        {
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
        }

        savedEventFlags = new Dictionary<string, bool>(currentEventFlags);
    }

    //Update the story state when a new event occurs
    public void StoryEvent(StoryEventInfo info) {
        Debug.Log("Story event: " + info.eventName);
        //must be valid event
        if (currentEventFlags.ContainsKey(info.eventName)) {
            //event must not have already happened
            if (!currentEventFlags[info.eventName])
            {
                currentEventFlags[info.eventName] = true;
                switch (info.eventName)
                {
                    case "1.1":
                        TextManager.Instance.QueuePrompt("evilNPC1-hi");
                        break;
                    //Handle any special cases for story events here
                    default:
                        break;
                }
                eventOccured?.Invoke(info.eventName);
            }
        }
    }

    //allows other scripts to check the state of a given story event
    public bool getEvent(string eventName) {
        if (Instance == null)
        {
            Debug.LogError("StoryManager instance is not initialized!");
            return false;
        }

        if (currentEventFlags != null && currentEventFlags.ContainsKey(eventName))
        {
            return currentEventFlags[eventName];
        }
        return false;
    }

    //reset event flags to where they were at the start of the level
    public void Reset()
    {
        if (savedEventFlags == null) return;

        foreach (var key in savedEventFlags.Keys)
        {
            currentEventFlags[key] = savedEventFlags[key];
        }
    }

    //save the event flags so they persist after reset - called at end of level
    public void Save()
    {
        foreach (var key in currentEventFlags.Keys)
        {
            savedEventFlags[key] = currentEventFlags[key];
        }
    }
}
