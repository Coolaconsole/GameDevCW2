using UnityEngine;
using static StoryManager;

public class EndLevelVolume : StoryEventVolume
{
    GameManager gameManager;

    protected override void Start()
    {
        base.Start();

        gameManager = (GameManager)FindAnyObjectByType(typeof(GameManager));
    }

    protected override void volumeTriggerCheck(Collider2D collider)
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
            trigger.Trigger(tag.info);

            TextManager.Instance?.CompleteCurrentPrompt();
            //then tell to load next level
            gameManager.NextLevel(tag.info.eventName);
        }
    }
}
