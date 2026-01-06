using System.Collections.Generic;
using UnityEngine;

public class SpawnReact : ExampleReactToEvent
{
    public List<GameObject> objectToSpawn;
    StoryManager manager;
    void Awake()
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
            List<int> nullIndices = new List<int>();
            foreach (GameObject obj in objectToSpawn)
            {
                if (obj != null)
                    obj.SetActive(true);
                else
                    nullIndices.Add(objectToSpawn.IndexOf(obj));
            }
            nullIndices.Reverse();
            //remove null entries from list to avoid issues on future spawns
            foreach (int index in nullIndices)
            {
                objectToSpawn.RemoveAt(index);
            }
        }
    }
}
