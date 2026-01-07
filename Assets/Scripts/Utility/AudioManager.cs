using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Required for automatic scene music detection


/// A persistent, singleton AudioManager that handles SFX pooling and 
/// automatic music transitions between scenes.

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEntry
    {
        public string name;
        public AudioClip clip;
    }

    [System.Serializable]
    public class SceneMusicMap
    {
        public string sceneName;
        public string trackName;
    }

    [Header("Audio Collections")]
    public List<SoundEntry> sounds = new();
    public List<SoundEntry> musicTracks = new();

    [Header("Automatic Scene Music")]
    [Tooltip("Map scene names to track names to change music automatically on load.")]
    public List<SceneMusicMap> sceneMusicMaps = new();

    [Header("Settings")]
    public int initialPoolSize = 10;
    public float defaultMasterVolume = 0.5f;

    // Dictionaries 
    private Dictionary<string, AudioClip> soundDict = new();
    private Dictionary<string, AudioClip> musicDict = new();

    // SFX Pooling
    private List<AudioSource> sfxPool = new();
    private Transform poolParent;

    // Music
    private AudioSource musicSource;

    private void Awake()
    {

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeManager();
    }


    /// Sets up the internal dictionaries, the SFX pool, and the music source.

    private void InitializeManager()
    {
        // Convert SFX list to dictionary for faster access
        foreach (var s in sounds)
            soundDict[s.name] = s.clip;

        // Convert Music list to dictionary
        foreach (var m in musicTracks)
            musicDict[m.name] = m.clip;

        // Create a parent object to keep the hierarchy clean
        poolParent = new GameObject("SFX_Pool").transform;
        poolParent.SetParent(transform);

        // Pre-warm the SFX pool
        for (int i = 0; i < initialPoolSize; i++)
            CreateNewSource();

        // Initialize the dedicated music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        
        // Global volume setting
        AudioListener.volume = defaultMasterVolume;
    }

    private void OnEnable()
    {
        // Tell Unity to call OnSceneLoaded whenever a new scene is ready
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Clean up the event subscription
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

 
    /// Checks if the current scene has a specific music track assigned.

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var map in sceneMusicMaps)
        {
            if (map.sceneName == scene.name)
            {
                // Only change if the music isn't already playing the correct track
                if (musicSource.clip == null || musicSource.clip.name != musicDict[map.trackName].name)
                {
                    PlayMusic(map.trackName);
                }
                return;
            }
        }
    }


    /// Instantiates a new AudioSource and adds it to the pool.

    private AudioSource CreateNewSource()
    {
        var src = poolParent.gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        sfxPool.Add(src);
        return src;
    }


    /// Finds a non-playing source. If none exist, it expands the pool.

    private AudioSource GetAvailableSource()
    {
        foreach (var s in sfxPool)
        {
            if (!s.isPlaying) return s;
        }

        // Dynamic Expansion: If the pool is full, create a new one on the fly
        return CreateNewSource();
    }

 
    /// Plays an SFX by name. Includes optional pitch randomization to prevent robotic sounds.

  public void PlaySFX(string name, float volume = 0.1f, float minPitch = 0.75f, float maxPitch = 1.25f)
{
    // Force the search term to lowercase to match the dictionary
    string searchKey = name.ToLower().Trim();

    if (soundDict.TryGetValue(searchKey, out var clip))
    {
        var src = GetAvailableSource();
        src.clip = clip;
        src.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        src.volume = volume;
        src.Play();
    }
    else
    {
        // This will print every key currently inside the dictionary so you can compare
        string allKeys = string.Join(", ", soundDict.Keys);
        Debug.LogWarning($"AudioManager: '{searchKey}' not found! Available keys: [{allKeys}]");
    }
}


    /// Plays background music by name. Automatically stops the previous track.

    public void PlayMusic(string name, float volume = 0.1f)
    {
        if (musicDict.TryGetValue(name, out var clip))
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"AudioManager: Music track '{name}' not found!");
        }
    }
    

    /// Stops the background music immediately.
 
    public void StopMusic()
    {
        musicSource.Stop();
    }
}