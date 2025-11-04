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
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 전환 감지
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

    // 씬이 로드될 때마다 버튼을 새로 찾아서 연결
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        targetButton = GameObject.Find("Button")?.GetComponent<Button>();
        if (targetButton != null)
        {
            targetButton.onClick.RemoveAllListeners();
            targetButton.onClick.AddListener(OnMouseUpAsButton);
            Debug.Log($"[{scene.name}] 버튼 이벤트 연결 완료!");
        }
        else
        {
            Debug.LogWarning($"[{scene.name}] 버튼을 찾을 수 없습니다.");
        }
    }

    public void OnMouseUpAsButton()
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
}
