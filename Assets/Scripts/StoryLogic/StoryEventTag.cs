using UnityEngine;

//Component to contain story info - could set up with getter to force only use once if needed
public class StoryEventTag : MonoBehaviour
{
    public StoryEventInfo info;
}

//stores all infor needed for a story event - can be expanded later if needed
[System.Serializable]
public struct StoryEventInfo
{
    public string eventName;

    public StoryEventInfo(string eventName)
    {
        this.eventName = eventName;
    }
}