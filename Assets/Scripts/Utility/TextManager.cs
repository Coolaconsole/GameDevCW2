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
        
        SetAllPrompts();
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
        waitingForEvent = eventBasedCompletion;
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
        if (!typingComplete)
                CompleteInstantly();

        Debug.Log("Completing current prompt");
        waitingForEvent = false;
        ClosePrompt();

        if (promptQueue.Count > 0)
        {
            ShowNextPrompt();
        }
    }

    public void ClearQueue()
    {
        textBox.SetActive(false);
        currentPromptKey = "";
        promptQueue.Clear();
    }

    private void SetAllPrompts()
    {
        //First village dialogue
        textPrompts["neutralNPC1-hi"] = ("Good morning. It seems there are people gathering in the square.", new Vector3(60, -170, 0), null, false);
        textPrompts["neutralNPC1-talk"] = ("Talk to other frogs by walking into them. Find out what's up.", new Vector3(60, -170, 0), null, false);
        textPrompts["neutralNPC1-walk"] = ("Talk with walk. Close text boxes with R.", new Vector3(60, -170, 0), null, false);
        textPrompts["neutralNPC1-kid"] = ("I'm just a kid, but I want a king.", new Vector3(220, 0, 0), null, false);
        textPrompts["neutralNPC1-king"] = ("We want a king!\nWe want a king!", new Vector3(220, -40, 0), null, false);
        textPrompts["neutralNPC1-seem"] = ("Seems everyone wants a king, I dig it.", new Vector3(220, -40, 0), null, false);
        textPrompts["neutralNPC1-idk"] = ("I don't know what's happening, but I want a king.", new Vector3(65, 65, 0), null, false);
        textPrompts["neutralNPC1-goodLuck"] = ("Good luck out there, dude. Glad I wasn't chosen.", new Vector3(-245, 10, 0), null, false);

        textPrompts["step-forth"] = ("We need an adventurer to seek Jupiter. We need a king to rule us.", new Vector3(245, 50, 0), null, false);
        textPrompts["volunteer"] = ("If you wish to volunteer, uhh please step forth... anyone?", new Vector3(245, 50, 0), null, false);
        textPrompts["callToAction"] = ("Aha! We thank you for your bravery. Please exit the village now.", new Vector3(245, 50, 0), null, false);
        textPrompts["walk-out"] = ("You! You're going the right way! You shall head out on this quest.", new Vector3(245, 50, 0), null, false);
        textPrompts["takeThis"] = ("Take this lily pad, surely Jupiter will reward our effort.", new Vector3(245, 50, 0), null, false);
        textPrompts["level"] = ("This lily pad must be placed at the pedestal at the end of each level.", new Vector3(245, 50, 0), null, false);

        //Level 1-2
        textPrompts["evilNPC1-hi"] = ("Hey you, come over here. I have something for ya...", new Vector3(-169, -135, 0), null, false);
        textPrompts["evilNPC1-king"] = ("You know... you don't have to ask for a king. We could do better.", new Vector3(-169, -135, 0), null, false);
        textPrompts["evilNPC1-offer"] = ("I took this from the village, Jupiter could be bargained with.", new Vector3(-169, -135, 0), null, false);
        textPrompts["evilNPC1-deal"] = ("Take it with you, Jupiter will give you what you want.", new Vector3(-169, -135, 0), "1.2 Spawn", true);

        //Level 1-3
        textPrompts["goodNPC1-hi"] = ("Hi, I'm glad I could catch you. I have a small request.", new Vector3(-230, -169, 0), null, false);
        textPrompts["goodNPC1-family"] = ("My family is starving, we wrote down a prayer for Jupiter.", new Vector3(-230, -169, 0), null, false);
        textPrompts["goodNPC1-offer"] = ("Can you please bring it to Jupiter?", new Vector3(-230, -169, 0), "1.3 Spawn", true);

        //Jupiter Dialogue
        textPrompts["jupiter-neutral1"] = ("Ah, an adventurer. How quaint. What is it you seek?", new Vector3(0, 30, 0), null, false);
        textPrompts["jupiter-neutral2"] = ("A king you say? Interesting... I can make it happen.", new Vector3(225, 95, 0), null, false);
        textPrompts["jupiter-neutral3"] = ("I grant you this log. It will tower over the land.", new Vector3(-225, 95, 0), null, false);
        textPrompts["jupiter-neutral4"] = ("Place the offering on the podium, and it shall be done.", new Vector3(0, -50, 0), null, false);

        textPrompts["jupiter-good1"] = ("Ah, an adventurer. What is it? You seem troubled.", new Vector3(0, 30, 0), null, false);
        textPrompts["jupiter-good2"] = ("The village want a king, but they can't sustain themselves?", new Vector3(225, 95, 0), null, false);
        textPrompts["jupiter-good3"] = ("Interesting... that's a lot to ask for. I can fulfill one request.", new Vector3(-225, 95, 0), null, false);
        textPrompts["jupiter-good4"] = ("Place the offering in the podium for your request:\n<color=yellow>A ruler</color> - <color=green>Plentiful harvests</color>", new Vector3(0, -50, 0), null, false);

        textPrompts["jupiter-evil1"] = ("Ah, an adventurer. How quaint. What is this you've brought?", new Vector3(0, 30, 0), null, false);
        textPrompts["jupiter-evil2"] = ("Oh, a request for a king. Or is this something else?", new Vector3(-225, 95, 0), null, false);
        textPrompts["jupiter-evil3"] = ("You could be this king they want. The choice is yours.", new Vector3(225, 95, 0), null, false);
        textPrompts["jupiter-evil4"] = ("Place the offering in the podium for your request:\n<color=yellow>A king</color> - <color=red>Become the king</color>", new Vector3(0, -50, 0), null, false);
        
        //First village dialogue
        textPrompts["v2n-hi"] = ("Oh hey, you're back. People are gathering in the square again.", new Vector3(60, -170, 0), null, false);
        textPrompts["v2n-talk"] = ("Am I repeating myself? Oh well.", new Vector3(60, -170, 0), null, false);
        textPrompts["v2n-walk"] = ("Just to remind you: Talk with walk. Close text boxes with R.", new Vector3(60, -170, 0), null, false);
        textPrompts["v2n-kid"] = ("I jumped off the new king, but I landed just fine!", new Vector3(220, 0, 0), null, false);
        textPrompts["v2n-king"] = ("This new king's rubbish! It's just a log!", new Vector3(220, -40, 0), null, false);
        textPrompts["v2n-seem"] = ("People seem angry at you dude, better head back out.", new Vector3(220, -40, 0), null, false);
        textPrompts["v2n-idk"] = ("I don't know what you did wrong, but Big Frog wants you.", new Vector3(65, 65, 0), null, false);
        textPrompts["v2n-goodLuck"] = ("Maybe get us a proper king next time.", new Vector3(-245, 10, 0), null, false);

        textPrompts["v2n-d1"] = ("You again! We want a better king, come here.", new Vector3(245, 50, 0), null, false);
        textPrompts["v2n-d2"] = ("A food offering is more likely to work. Take it to Jupiter!", new Vector3(245, 50, 0), null, false);
        textPrompts["v2n-d3"] = ("This offering works the same as the lily pad, but it's bigger.", new Vector3(245, 50, 0), null, false);
        textPrompts["v2n-d4"] = ("Off you go!", new Vector3(245, 50, 0), null, false);
        textPrompts["v2n-d5"] = ("I've never been hungry, so we clearly don't have a food shortage.", new Vector3(245, 50, 0), null, false);

        
        
        //Level 2-1
        textPrompts["evilNPC2-return"] = ("Hello again, you did well. Look at us now.", new Vector3(-230, -70, 0), null, false);
        textPrompts["evilNPC2-power"] = ("The position we're in. The power we have...", new Vector3(-230, -70, 0), null, false);
        textPrompts["evilNPC2-monitor"] = ("Leave that stupid offering behind! Lets go back to Jupiter.", new Vector3(-230, -70, 0), null, false);

        //Level 2-2
        textPrompts["evilNPC3-steal"] = ("Aha! That money will make a good offering.", new Vector3(225, 40, 0), null, false);
        textPrompts["evilNPC3-tempt"] = ("Take it... Jupiter will reward us for such an act.", new Vector3(225, 40, 0), null, false);
        textPrompts["goodNPC2-stolen"] = ("Hey! Please don't, that's all I have left.", new Vector3(-240, 60, 0), null, true);

        textPrompts["goodNPC2-plead"] = ("Oh hello, it's you. Can you please help me?", new Vector3(-240, 60, 0), null, false);
        textPrompts["goodNPC2-food"] = ("My family is out of food, could you spare some?", new Vector3(-240, 60, 0), null, true);
        textPrompts["goodNPC2-payment"] = ("Thank you! I don't have much to pay you back, but I do have this.", new Vector3(-240, 60, 0), null, false);

        //Jupiter Dialogue
        textPrompts["jupiter2-neutral1"] = ("You again, what is it you want this time?", new Vector3(0, 30, 0), null, false);
        textPrompts["jupiter2-neutral2"] = ("The people aren't happy with their king? Odd.", new Vector3(225, 95, 0), null, false);
        textPrompts["jupiter2-neutral3"] = ("Fine. I'll give you a ruler. A commanding eel you will get.", new Vector3(-225, 95, 0), null, false);
        textPrompts["jupiter2-neutral4"] = ("Place the offering on the podium, and I shall send it off.", new Vector3(0, -50, 0), null, false);

        textPrompts["jupiter2-good1"] = ("You're back, do your people want a king again?", new Vector3(0, 30, 0), null, false);
        textPrompts["jupiter2-good2"] = ("Where did you get this offering? You must've earned it.", new Vector3(225, 95, 0), null, false);
        textPrompts["jupiter2-good3"] = ("I can offer you a <color=yellow>commanding eel</color>, or <color=green>cooperative spider</color>.", new Vector3(-225, 95, 0), null, false);
        textPrompts["jupiter2-good4"] = ("Place the offering in the podium for your request:\n<color=yellow>The eel</color> - <color=green>The spider</color>", new Vector3(0, -50, 0), null, false);

        textPrompts["jupiter2-evil1"] = ("Welcome back, satisfied with your first request?", new Vector3(0, 30, 0), null, false);
        textPrompts["jupiter2-evil2"] = ("They want a different ruler? You should teach them a lesson.", new Vector3(-225, 95, 0), null, false);
        textPrompts["jupiter2-evil3"] = ("I can offer you a <color=yellow>commanding eel</color>, or <color=red>the power of fire</color>.", new Vector3(225, 95, 0), null, false);
        textPrompts["jupiter2-evil4"] = ("Place the offering in the podium for your request:\n<color=yellow>The eel</color> - <color=red>Power</color>", new Vector3(0, -50, 0), null, false);
        

        //Jupiter Dialogue
        textPrompts["jupiter3-neutral1"] = ("You. This is the last request I'll deal with.", new Vector3(0, 30, 0), null, false);
        textPrompts["jupiter3-neutral2"] = ("The people still aren't happy with their king? Hmm...", new Vector3(225, 95, 0), null, false);
        textPrompts["jupiter3-neutral3"] = ("Okay I've made up my mind. I'll give you the ruler you deserve.", new Vector3(-225, 95, 0), null, false);
        textPrompts["jupiter3-neutral4"] = ("Place the offering on the podium, it will arrive shortly.", new Vector3(0, -50, 0), null, false);

        textPrompts["jupiter3-good1"] = ("Oh hello again. I think I see what's happening...", new Vector3(0, 30, 0), null, false);
        textPrompts["jupiter3-good2"] = ("They'll never be satisfied, will they? Okay, let's make a deal.", new Vector3(225, 95, 0), null, false);
        textPrompts["jupiter3-good3"] = ("I can offer you <color=yellow>the most powerful ruler</color>, or I can <color=green>allow you to leave</color>.", new Vector3(-225, 95, 0), null, false);
        textPrompts["jupiter3-good4"] = ("Which will be your final decision:\n<color=yellow>Powerful ruler</color> - <color=green>Freedom</color>", new Vector3(0, -50, 0), null, false);

        textPrompts["jupiter3-evil1"] = ("Well well well, what have you brought now... a live sacrifice?", new Vector3(0, 30, 0), null, false);
        textPrompts["jupiter3-evil2"] = ("With this, I can grant you any one wish.", new Vector3(-225, 95, 0), null, false);
        textPrompts["jupiter3-evil3"] = ("Will you <color=yellow>share</color> this power with the people, or <color=red>keep it for yourself</color>?", new Vector3(225, 95, 0), null, false);
        textPrompts["jupiter3-evil4"] = ("<color=yellow>Wish for a ruler</color> - <color=red>Wish for infinite power</color>", new Vector3(0, -50, 0), null, false);
        

        //Npc who gives you a hammer
        textPrompts["ReceiveHammer"] = ("Before you leave, take this!", new Vector3(0, 0, 0), "ReceivedHammer", true);
    }
}
