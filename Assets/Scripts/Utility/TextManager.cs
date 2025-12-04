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
    private Dictionary<string, (string, Vector3, UnityEvent, bool)> textPrompts = new Dictionary<string, (string, Vector3, UnityEvent, bool)>();
    private HashSet<string> completedEvents = new HashSet<string>();
    
    [Header("Text UI")]
    List<(string key, string text, Vector3 pos, bool pausesGame)> promptQueue = new List<(string, string, Vector3, bool)>();
    public GameObject promptObject;

    public float charsPerSecond = 25f;
    public float punctuationPause = 0.25f;

    private Coroutine typingRoutine;
    private TextMeshProUGUI tmp;
    private string currentText = "";
    private bool typingComplete = false;
    
    private string currentPromptKey = "";
    private bool currentPromptPausesGame = false;
    
    [Header("Others")]
    public static TextManager Instance { get; private set; }

    //Events that can be called from other scripts
    [HideInInspector] public UnityEvent onEvent = new UnityEvent();

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
        return (promptQueue.Count > 0) || promptObject.activeSelf;
    }

    private void Start()
    {
        // Starting Prompts
        textPrompts["name"] = ("Text", new Vector3(0, 0, 0), null, true);
    }

    private void Update()
    {
        if (promptObject.activeSelf && Time.timeScale != 0)
        {
            //Whether all prompts pause game or just this specific one pause game
            Time.timeScale = allPromptsPauseGame || currentPromptPausesGame ? 0f : 1f;
        }
        if (promptObject.activeSelf && Input.GetKeyDown(KeyCode.R))
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

        if (!promptObject.activeSelf && promptQueue.Count > 0)
        {
            ShowNextPrompt();
        }
    }

    private void ShowNextPrompt()
    {
        (string key, string text, Vector3 pos, bool pausesGame) = promptQueue[0];
        promptQueue.RemoveAt(0);

        currentPromptKey = key;
        currentPromptPausesGame = pausesGame;
        
        promptObject.SetActive(true);
        var rect = promptObject.GetComponent<RectTransform>();
        tmp = promptObject.GetComponentInChildren<TextMeshProUGUI>();
        rect.anchoredPosition3D = pos;

        //If the prompt is specified to pause the game or everyone does 
        if (pausesGame || allPromptsPauseGame)
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
    }

    public void ClosePrompt()
    {
        if (promptObject.activeSelf)
        {
            promptObject.SetActive(false);
            currentPromptKey = "";
        }
        if (promptQueue.Count == 0)
            Time.timeScale = 1f;
    }

    public void QueuePrompt(string key)
    {
        if (textPrompts.ContainsKey(key))
        {
            //Checks for event completion
            if (completedEvents.Contains(key))
            {
                textPrompts.Remove(key);
                return;
            }
            
            var (text, pos, unityEvent, pause) = textPrompts[key];
            promptQueue.Add((key, text, pos, pause));

            //If there is an event subscribe to it, if it goes off complete text prompt
            if (unityEvent != null)
            {
                unityEvent.AddListener(() => { OnEventTriggerd(key); });
            }
            
            textPrompts.Remove(key);  // so they arent shown again
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

    public void OnEventTriggerd(string eventKey)
    {
        //Turns this on to allow event triggering
        if (!eventBasedPromptCompletion) { return; }
        
        completedEvents.Add(eventKey);

        if (currentPromptKey == eventKey)
        {
            ClosePrompt();
        }
        
        //Removes it from the queue if it hasn't come up yet
        promptQueue.RemoveAll(p => p.key == eventKey);
    }


}
