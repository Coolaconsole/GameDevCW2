using System.Collections.Generic;
using UnityEngine;

public class SpawnReact : ExampleReactToEvent
{
    public List<GameObject> objectToSpawn;
    StoryManager manager;
    void Start()
    {
        //set manager - done this way as won't be assigned in scene as will be loaded across scenes
        manager = (StoryManager)FindAnyObjectByType(typeof(StoryManager));
        if (manager != null )
        {
            //bind delegate
            manager.eventOccured += StoryEventOccuered;
        }
    }
    void StoryEventOccuered(string eventName)
    {
        Debug.Log("SpawnReact detected event: " + eventName);
        if (eventName.Contains("Spawn"))
        {
            foreach (GameObject obj in objectToSpawn)
            {
                obj.SetActive(true);
            }
        }
    }
}
