using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour
{
    public static SceneChange Instance { get; private set; }

    private Button targetButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameOverScene")
        {
            Button btn1 = GameObject.Find("Button1")?.GetComponent<Button>();
            Button btn2 = GameObject.Find("Button2")?.GetComponent<Button>();

            if (btn1 != null)
            {
                btn1.onClick.RemoveAllListeners();
                btn1.onClick.AddListener(GoToMainMenu);
            }

            if (btn2 != null)
            {
                btn2.onClick.RemoveAllListeners();
                btn2.onClick.AddListener(QuitGame);
            }

            return;
        }

        // Other scenes: only one button
        targetButton = GameObject.Find("Button")?.GetComponent<Button>();
        if (targetButton != null)
        {
            targetButton.onClick.RemoveAllListeners();
            targetButton.onClick.AddListener(OnSingleButtonClick);
        }
    }

    public void OnSingleButtonClick()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "MainMenuScene")
        {
            SceneManager.LoadScene("CharacterSelectScene");
        }
        else if (currentScene == "CharacterSelectScene")
        {
            SceneManager.LoadScene("MainGameScene");
        }
    }

    // Button1 action ¡æ Go back to MainMenuScene
    private void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    // Button2 action ¡æ Quit game
    private void QuitGame()
    {
        Application.Quit();
    }
}
