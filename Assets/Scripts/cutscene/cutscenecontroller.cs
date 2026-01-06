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
}
