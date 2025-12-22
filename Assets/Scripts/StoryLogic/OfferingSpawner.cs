using UnityEngine;

public class OfferingSpawner : MonoBehaviour
{
    StoryManager manager;

    public string eventName;

    public GameObject prefab;

    void Start()
    {
        manager = (StoryManager)FindAnyObjectByType(typeof(StoryManager));
        if (manager != null)
        {
            //bind delegate
            manager.eventOccured += StoryEventOccuered;
        }
    }

    void StoryEventOccuered(string eventName)
    {
        if (eventName == this.eventName)
        {
            Instantiate(prefab,transform.position, transform.rotation);
        }
    }
}
