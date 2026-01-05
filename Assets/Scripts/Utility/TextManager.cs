using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static Unity.Burst.Intrinsics.X86.Avx;
using Vector3 = UnityEngine.Vector3;

public class TextManager : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] private bool allPromptsPauseGame = false;
    [SerializeField] private bool eventBasedPromptCompletion = false;

    StoryManager sManager;

// Text prompts stored with key, text, position, optional story event to trigger, optional unity event to trigger
    private Dictionary<string, (string text, Vector3 pos, string tag, bool evnt)> textPrompts = new Dictionary<string, (string, Vector3, string, bool)>();
    // Tracks text prompts that have been completed via events so they don't show again
    private HashSet<string> completedEvents = new HashSet<string>();
    
    [Header("Text UI")]
    List<(string key, string text, Vector3 pos, bool eventBasedCompletion, string storyTrigger)> promptQueue = new List<(string, string, Vector3, bool, string)>();
    public GameObject textBox;

    public float charsPerSecond = 25f;
    public float punctuationPause = 0.25f;

    private Coroutine typingRoutine;
    private TextMeshProUGUI tmp;
    private string currentText = "";
    private bool typingComplete = true;
    
    private string currentPromptKey = "";
    private bool currentPromptPausesGame = false;
    
    [Header("Others")]
    public static TextManager Instance { get; private set; }

    private bool waitingForEvent = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasUnclosedPrompts()
    {
        return (promptQueue.Count > 0) || textBox.activeSelf;
    }

    private void Start()
    {
        sManager = (StoryManager)FindAnyObjectByType(typeof(StoryManager));
        


        textPrompts["evilNPC1-hi"] = ("Hey you, come over here. I have something for ya...", new Vector3(-169, -135, 0), null, false);
        textPrompts["evilNPC1-king"] = ("You know... you don't have to ask for a king. We could do better.", new Vector3(-169, -135, 0), null, false);
        textPrompts["evilNPC1-offer"] = ("I took this from the village, Jupiter could be bargained with.", new Vector3(-169, -135, 0), null, false);
        textPrompts["evilNPC1-deal"] = ("Take it with you, Jupiter will give you what you want.", new Vector3(-169, -135, 0), null, true);

        //Npc who gives you a hammer
        textPrompts["RecieveHammer"] = ("Before you leave, take this!", new Vector3(0, 0, 0), "RecivedHammer", true);
    }

    private void Update()
    {
        if (textBox.activeSelf && Time.timeScale != 0)
        {
            //Whether all prompts pause game or just this specific one pause game
            Time.timeScale = allPromptsPauseGame || currentPromptPausesGame ? 0f : 1f;
        }
        if (textBox.activeSelf && Input.GetKeyDown(KeyCode.R) && !waitingForEvent)
        {
            if (!typingComplete)
                CompleteInstantly();
            else
            {
                AudioManager.Instance.PlaySFX("click", 0.1f, 1f, 1f);
                ClosePrompt();
            }
            return;
        }

        if (!textBox.activeSelf && promptQueue.Count > 0)
        {
            ShowNextPrompt();
        }
    }

    private void ShowNextPrompt()
    {
        (string key, string text, Vector3 pos, bool eventBasedCompletion, string storyTrigger) = promptQueue[0];
        promptQueue.RemoveAt(0);

        currentPromptKey = key;
        eventBasedPromptCompletion = eventBasedCompletion;
        Debug.Log("EventBasedCompletion: " + eventBasedCompletion);
        // currentPromptPausesGame = true; // Currently stay false unless set otherwise
        textBox.SetActive(true);
        var rect = textBox.GetComponent<RectTransform>();
        tmp = textBox.GetComponentInChildren<TextMeshProUGUI>();
        rect.anchoredPosition3D = pos;

        //If the prompt is specified to pause the game or everyone does 
        if (currentPromptPausesGame || allPromptsPauseGame)
        {
            //Cannot have typing animation so just show text
            StartTyping(text);
            CompleteInstantly();
            
            Time.timeScale = 0;
        }
        else //Start Typing Animation
        {
            Time.timeScale = 1f;
            StartTyping(text);
        }

        //If there is an event, set the flag in the story manager
        if (storyTrigger != null)
        {
            sManager.StoryEvent(new StoryEventInfo(storyTrigger));
        }
    }

    public bool ClosePrompt()
    {
        if (textBox.activeSelf && !waitingForEvent && typingComplete)
        {
            textBox.SetActive(false);
            currentPromptKey = "";
            return true;
        }
        if (promptQueue.Count == 0)
            Time.timeScale = 1f;
        return !textBox.activeSelf && typingComplete;
    }

    public void QueuePrompt(string key)
    {
        if (textPrompts.ContainsKey(key))
        {
            //Checks for event completion
            if (completedEvents.Contains(key))
            {
                //textPrompts.Remove(key);
                return;
            }
            
            var (text, pos, storyEvent, unityEvent) = textPrompts[key];
            promptQueue.Add((key, text, pos, unityEvent, storyEvent));
            
            //textPrompts.Remove(key);  // so they arent shown again
        }
    }

    private void StartTyping(string fullText)
    {
        StopTyping();
        typingRoutine = StartCoroutine(TypeRoutine(fullText));
    }

    private void StopTyping()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);
        typingRoutine = null;
    }

    private IEnumerator TypeRoutine(string fullText)
    {
        tmp.text = "";
        currentText = fullText;
        typingComplete = false;

        float delay = 1f / Mathf.Max(1f, charsPerSecond);
        int i = 0;

        while (i < fullText.Length)
        {
            // if theres a markup tag, add the entire thing to prevent mess
            if (fullText[i] == '<')
            {
                int closingIndex = fullText.IndexOf('>', i);
                if (closingIndex != -1)
                {
                    tmp.text += fullText.Substring(i, closingIndex - i + 1);
                    i = closingIndex + 1;
                    continue;
                }
            }

            tmp.text += fullText[i];
            char c = fullText[i];
            i++;

            if (c == '.' || c == ',' || c == '!' || c == '?')
                yield return new WaitForSeconds(punctuationPause);
            else
                yield return new WaitForSeconds(delay);
        }

        typingComplete = true;
        typingRoutine = null;
    }

    private void CompleteInstantly()
    {
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        tmp.text = currentText;
        typingComplete = true;
    }

    public void CompleteCurrentPrompt()
    {
        if (!textBox.activeSelf) return;
        if (!waitingForEvent) return;

        waitingForEvent = false;
        ClosePrompt();

        if (promptQueue.Count > 0)
        {
            ShowNextPrompt();
        }
    }


}
