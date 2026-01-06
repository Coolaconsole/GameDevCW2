using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cutscenecontroller : MonoBehaviour
{
    public float duration = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StoryManager manager = (StoryManager)FindAnyObjectByType(typeof(StoryManager));
        if (manager != null )
        {
            Destroy(manager.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0) {
            SceneManager.LoadScene("menu");
        }
    }



    //shouldnt be here but my bad n shi

    private void TriggerCutsceneAudio()
    {
        //only trigger if we are in the specific ending scene
        if (SceneManager.GetActiveScene().name == "neutral end")
        {
            // null check the Instance in case the manager didn't load properly
            if (AudioManager.Instance != null)
            {
                
                AudioManager.Instance.PlaySFX("heron");
            }
            else
            {
                Debug.LogWarning("AudioManager instance not found for cutscene!");
            }
        }
    }
}
