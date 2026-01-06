using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menumanager : MonoBehaviour
{
    public Button PlayGameButton;
    public Button QuitGameButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayGameButton.onClick.AddListener(StartGame);
        QuitGameButton.onClick.AddListener(Quit);
    }

    void StartGame()
    {
        SceneManager.LoadScene("village1");
    }

    void Quit()
    {
        Application.Quit();
    }
}
