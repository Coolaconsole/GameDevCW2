using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Unity.Collections;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    //the default secene to load after level completion
    public string nextSceneName;

    //the story manager flag for is flag1Scene should be loaded
    public string flag1;
    //the secene to load after level completion if story flag1 is true
    public string flag1SceneName;

    //the story manager flag for is flag2Scene should be loaded
    public string flag2;
    //the secene to load after level completion if story flag2 is true
    public string flag2SceneName;

    string CurrentSceneName;

    StoryManager manager;
    void Start()
    {
        CurrentSceneName = SceneManager.GetActiveScene().name;
    }
    private void Awake()
    {
        manager = (StoryManager)FindAnyObjectByType(typeof(StoryManager));
    }

    public void ResetLevel()
    {
        CurrentSceneName = SceneManager.GetActiveScene().name;
        if (manager != null)
        {
            manager.Reset();
        }
        Debug.Log(CurrentSceneName);
        SceneManager.LoadScene(CurrentSceneName);
    }

    public void NextLevel(string eventName)
    {
        manager.Save();
        if (manager.getEvent(flag1) || eventName == flag1)
        {
            Debug.Log("flag1: " + flag1SceneName);
            SceneManager.LoadScene(flag1SceneName);
        }
        else if (manager.getEvent(flag2) || eventName == flag2)
        {
            Debug.Log("flag2: " + flag2SceneName);
            SceneManager.LoadScene(flag2SceneName);
        }
        else
        {
            Debug.Log("else: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
