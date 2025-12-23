using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(StoryEventTrigger))]
[RequireComponent(typeof(Collider2D))]

//triggers a story event when an object with StortEventTag enters or exits
public class StealEventVolume : StoryEventVolume
{

    protected virtual void volumeTriggerCheck(Collider2D collider)
    {
        //get the tag containing story info
        StoryEventTag tag = collider.GetComponentInParent<StoryEventTag>();

        if (tag != null)
        {
            if (specificStoryTags.Count != 0)
            {
                if (!specificStoryTags.Contains(tag))
                {
                    //if the volume has a specifc object and this isn't it abort
                    return;
                }
            }
            //trigger the story event if has tag and can proceed
            //assume story engine handles duplicate tags
            StoryEventInfo info = tag.info;
            if (info.eventName == "2.2 Evil") { info.eventName = "2.2 Steal"; }
            else if (info.eventName == "2.2 Neutral") { info.eventName = "2.2 Give"; }
            trigger.Trigger(info);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //only trigger if the volume is set to on enter
        StoryEventTag tag = collision.GetComponentInParent<StoryEventTag>();

        if (tag != null)
        {
            if (tag.info.eventName == "2.2 Neutral")
            {
                volumeTriggerCheck(collision);
            }   
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //only trigger if the volume is set to on enter
        StoryEventTag tag = collision.GetComponentInParent<StoryEventTag>();

        if (tag != null)
        {
            if (tag.info.eventName == "2.2 Evil")
            {
                volumeTriggerCheck(collision);
            }
        }
    }
}
