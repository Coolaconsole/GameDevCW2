using UnityEngine;

public class StoryEventTrigger : MonoBehaviour
{
    public StoryEventInfo info;
    public StoryManager manager;

    private void Start()
    {
        manager = (StoryManager)FindAnyObjectByType(typeof(StoryManager));
    }

    public void Trigger()
    {
        manager.StoryEvent(info);
    }

    public void Trigger(StoryEventInfo info)
    {
        this.info = info;
        Trigger();
    }
}
