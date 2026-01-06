using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("TextTrigger activated by Player trigger.");
            // Call the method in TextManager to complete the text prompt event
            TextManager.Instance.CompleteCurrentPrompt();
        }
    }
}