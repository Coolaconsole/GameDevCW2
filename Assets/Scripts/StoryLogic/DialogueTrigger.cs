using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string dialogueEventName;
    private bool eventTriggered = false;
    public GameObject altChoice;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !eventTriggered)
        {
            eventTriggered = true;
            Debug.Log("Dialogue triggered: " + dialogueEventName);
            TextManager.Instance.ClearQueue();
            // Call the method in TextManager to trigger the text prompt event
            TextManager.Instance.QueuePrompt(dialogueEventName);

            if (altChoice != null)
            {
                altChoice.SetActive(false);
            }
        }
    }
}
