using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    public static GameObject SelectedCharacterPrefab;

    private CharacterSelector currentSelected;
    public Button selectButton;

    // Canvas(혹은 CharacterSelect UI 루트) 참조 — Inspector에서 할당
    public GameObject canvasRoot;

    public static CharacterSelectionManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        if (canvasRoot == null)
        {
            var parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas != null)
                canvasRoot = parentCanvas.gameObject;
            else
            {
                var anyCanvas = FindObjectOfType<Canvas>();
                if (anyCanvas != null)
                    canvasRoot = anyCanvas.gameObject;
            }
        }
    }

    void Start()
    {
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelectButtonClick);
            selectButton.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (canvasRoot != null)
        {
            bool shouldActive = scene.name == "CharacterSelectScene";
            canvasRoot.SetActive(shouldActive);
        }
    }

    // 선택 시 호출
    public GameObject SelectCharacter(CharacterSelector selector)
    {
        if (selector == null)
            return GetSelectedCharacterPrefab();

        if (currentSelected != null && currentSelected != selector)
            currentSelected.SetSelected(false);

        currentSelected = selector;
        currentSelected.SetSelected(true);

        // 선택 인덱스 저장 (0,1 등)
        PlayerPrefs.SetInt("SelectedCharacterIndex", selector.characterIndex);
        PlayerPrefs.Save();

        // prefab 확보: selector.inGamePrefab 우선, 없으면 CharacterStats에서 폴백
        GameObject prefab = selector.inGamePrefab;
        if (prefab == null)
        {
            var stats = selector.GetComponent<CharacterStats>();
            if (stats != null && stats.inGamePrefab != null)
                prefab = stats.inGamePrefab;
        }

        if (prefab != null)
        {
            // 씬 인스턴스가 아닌 에셋 프리팹을 사용해야 안전합니다.
            if (prefab.scene.IsValid())
                Debug.LogWarning($"CharacterSelectionManager: 선택된 inGamePrefab '{prefab.name}'이 씬 인스턴스입니다. 가능한 Prefab Asset을 지정하세요.");

            SelectedCharacterPrefab = prefab;
        }
        else
        {
            Debug.LogWarning($"선택한 캐릭터 '{selector.name}'에 할당된 inGamePrefab이 없습니다.");
            SelectedCharacterPrefab = null;
        }

        if (selectButton != null)
            selectButton.gameObject.SetActive(SelectedCharacterPrefab != null);

        return SelectedCharacterPrefab;
    }

    public GameObject GetSelectedCharacterPrefab()
    {
        return SelectedCharacterPrefab;
    }

    private void OnSelectButtonClick()
    {
        if (SelectedCharacterPrefab != null)
            SceneManager.LoadScene("MainGameScene");
        else
            Debug.LogWarning("캐릭터가 선택되지 않았습니다.");
    }
}