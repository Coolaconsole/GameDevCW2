using UnityEngine;

public class StoryManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void StoryEvent(StoryEventInfo info) {
        Debug.Log("Story event: " + info.eventName);
    }
}
