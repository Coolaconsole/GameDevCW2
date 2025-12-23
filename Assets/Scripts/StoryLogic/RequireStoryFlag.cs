using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RequireStoryFlag : MonoBehaviour
{
    //Name of the required flag
    public string flag;

    //Required value of the flag
    public bool value = true;

    StoryManager manager;
    void Start()
    {
        manager = (StoryManager)FindAnyObjectByType(typeof(StoryManager));

        if (manager.getEvent(flag) != value)
        {
            Destroy(this.gameObject);
        }
    }
}
