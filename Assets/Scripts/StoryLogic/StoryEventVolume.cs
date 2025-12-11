using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(StoryEventTrigger))]
[RequireComponent(typeof(Collider2D))]

//triggers a story event when an object with StortEventTag enters or exits
public class StoryEventVolume : MonoBehaviour
{
    //if the tirgger should occur on enter or exit of the volume
    public bool onEnter = true;
    //if not null then will only react to the specific object specified - set to null if  doesn't have the StoryEventTag component
    public Object specificStoryObject;

    //The trigger to update the story engine
    StoryEventTrigger trigger;
    //Collision for the volume
    Collider2D volume;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trigger = GetComponent<StoryEventTrigger>();
        volume = GetComponent<Collider2D>();
        //force trigger
        volume.isTrigger = true;

        //set specificStoryObject to null if it has no tag
        if (specificStoryObject != null )
        {
            if (specificStoryObject.GetComponent<StoryEventTag>() == null) 
            {
                specificStoryObject = null;
            }
        }
    }

    void volumeTriggerCheck(Collider2D collider)
    {
        //get the tag containing story info
        StoryEventTag tag = collider.GetComponentInParent<StoryEventTag>();

        if (tag != null)
        {
            if (specificStoryObject != null)
            {
                if (tag != specificStoryObject.GetComponent<StoryEventTag>())
                {
                    //if the volume has a specifc object and this isn't it abort
                    return;
                }
            }

            //trigger the story event if has tag and can proceed
            //assume story engine handles duplicate tags
            trigger.Trigger(tag.info);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //only trigger if the volume is set to on enter
        if (onEnter) 
        {
            volumeTriggerCheck(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //only trigger if the volume is set to on exit
        if (!onEnter)
        {
            volumeTriggerCheck(collision);
        }
    }
}
