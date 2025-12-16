using UnityEngine;

public class ExampleReactToEvent : MonoBehaviour
{
    StoryManager manager;

    SpriteRenderer spriteRenderer;

    public string eventName;

    void Start()
    {
        //set manager - done this way as won't be assigned in scene as will be loaded across scenes
        manager = (StoryManager)FindAnyObjectByType(typeof(StoryManager));
        if (manager != null )
        {
            //bind delegate
            manager.eventOccured += StoryEventOccuered;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //triggered by story manager delegate
    void StoryEventOccuered(string eventName)
    {
        if (eventName == this.eventName)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}
