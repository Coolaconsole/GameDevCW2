using UnityEngine;

public class StoryEventTrigger : MonoBehaviour
{
    public StoryEventInfo info;
    StoryManager manager;

    private void Start()
    {
        //set manager - done this way as won't be assigned in scene as will be loaded across scenes
        manager = (StoryManager)FindAnyObjectByType(typeof(StoryManager));
    }

    public void Trigger()
    {
        if (manager != null) {
            manager.StoryEvent(info);
        }
    }

    public void Trigger(StoryEventInfo info)
    {
        this.info = info;
        Trigger();
    }
}
