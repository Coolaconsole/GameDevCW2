using UnityEngine;
using UnityEngine.UI;

public class ResetButtonOnClick : MonoBehaviour
{
    GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = (GameManager)FindAnyObjectByType(typeof(GameManager));
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        gameManager = (GameManager)FindAnyObjectByType(typeof(GameManager));
        if (gameManager != null)
        {
            gameManager.ResetLevel();
        }
    }
}
