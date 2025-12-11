using UnityEngine;
using System.Collections.Generic;
using System;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;
    public List<GameObject> scenes;
    private int currentScene;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadScene(string sceneName)
    {
        // Code to load a scene by name
        switch(sceneName)
        {
            case "MainMenu":
                // Load Main Menu scene
                scenes[0].SetActive(true);
                break;
            case "Town1":
                // Load 1st town scene
                scenes[0].SetActive(false);
                scenes[1].SetActive(true);
                break;
            case "Level1":
                // Load Level 1 scene
                scenes[1].SetActive(false);
                scenes[2].SetActive(true);
                currentScene = 2;
                break;
            case "Jupiter":
                // Load Jupiter scene
                scenes[currentScene].SetActive(false);
                scenes[3].SetActive(true);
                break;


            case "Town2":
                // Load 2nd town scene
                scenes[3].SetActive(false);
                scenes[4].SetActive(true);
                break;

            case "Level2":
                // Load Level 2 scene
                scenes[4].SetActive(false);
                scenes[5].SetActive(true);
                currentScene = 5;
                break;
            case "Level2-Greed":
                // Load Level 2 greed scene
                scenes[4].SetActive(false);
                scenes[6].SetActive(true);
                currentScene = 6;
                break;
            case "Level2-Kind":
                // Load Level 2 kind scene
                scenes[4].SetActive(false);
                scenes[7].SetActive(true);
                currentScene = 7;
                break;


            case "Town3":
                // Load 3rd town scene
                scenes[3].SetActive(false);
                scenes[8].SetActive(true);
                break;

            case "Level3-Eel":
                // Load Level 3 eel scene
                scenes[8].SetActive(false);
                scenes[9].SetActive(true);
                currentScene = 9;
                break;
            case "Level3-Fire":
                // Load Level 3 fire scene
                scenes[8].SetActive(false);
                scenes[10].SetActive(true);
                currentScene = 10;
                break;
            case "Level3-Spider":
                // Load Level 3 spider scene
                scenes[8].SetActive(false);
                scenes[11].SetActive(true);
                currentScene = 11;
                break;
            default:
                Debug.LogWarning("Scene not found: " + sceneName);
                break;
        }
    }
}
