using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    public static GameObject SelectedCharacterPrefab;

    private CharacterSelector currentSelected;
    public Button selectButton;

    // Canvas(혹은 CharacterSelect UI 루트) 참조 — Inspector에서 할당하거나 자동으로 찾아 할당됨
    public GameObject canvasRoot;

    public static CharacterSelectionManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 씬 로드 이벤트 구독 (씬 전환 시 Canvas 활성/비활성 처리)
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        // canvasRoot가 비어있으면 가능한 자동 할당 시도
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
        // 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로드 시 호출
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // CharacterSelectScene일 때만 선택 UI 보여주고, 다른 씬(예: MainGameScene)으로 갈 때는 비활성화
        if (canvasRoot != null)
        {
            bool shouldActive = scene.name == "CharacterSelectScene";
            canvasRoot.SetActive(shouldActive);
        }
    }

    // 선택 시 호출 (UI/Selector에서 호출)
    public GameObject SelectCharacter(CharacterSelector selector)
    {
        if (selector == null)
            return GetSelectedCharacterPrefab();

        // 같은 객체를 다시 선택하면 토글 동작 없이 그대로 유지
        if (currentSelected != null && currentSelected != selector)
            currentSelected.SetSelected(false);

        currentSelected = selector;
        currentSelected.SetSelected(true);

        // selector에서 직접 prefab을 가져오도록 변경 (null 검사)
        GameObject prefab = selector.inGamePrefab;
        if (prefab == null)
        {
            // CharacterStats가 존재하면 fallback으로 사용
            var stats = selector.GetComponent<CharacterStats>();
            if (stats != null && stats.inGamePrefab != null)
                prefab = stats.inGamePrefab;
        }

        // 만약 prefab이 씬 인스턴스(프리팹 에셋이 아닌 씬 오브젝트)라면 경고 및 Resources에서 같은 이름의 자산 복구 시도
        if (prefab != null && prefab.scene.IsValid())
        {
            Debug.LogWarning($"CharacterSelectionManager: 선택한 '{selector.name}'의 inGamePrefab이 씬 인스턴스입니다. 프리팹 에셋을 할당하세요. Resources에서 동일 이름 자산을 시도합니다: {prefab.name}");
            var loaded = Resources.Load<GameObject>(prefab.name);
            if (loaded != null)
            {
                prefab = loaded;
                Debug.Log($"CharacterSelectionManager: Resources에서 '{prefab.name}' 프리팹을 복구했습니다.");
            }
            else
            {
                Debug.LogWarning($"CharacterSelectionManager: Resources에서 '{prefab.name}'을 찾지 못했습니다. 씬 전환 시 참조가 사라질 수 있습니다.");
            }
        }

        if (prefab != null)
        {
            SelectedCharacterPrefab = prefab;

            // RuntimeManager가 있으면 중앙에도 저장
            if (RuntimeManager.Instance != null)
                RuntimeManager.Instance.SetSelectedCharacterPrefab(SelectedCharacterPrefab);
        }
        else
        {
            Debug.LogWarning($"선택한 캐릭터 '{selector.name}'에 할당된 inGamePrefab이 없습니다.");
        }

        if (selectButton != null)
            selectButton.gameObject.SetActive(SelectedCharacterPrefab != null);

        return SelectedCharacterPrefab;
    }

    // 외부에서 선택된 프리팹을 얻을 수 있게 함
    public GameObject GetSelectedCharacterPrefab()
    {
        if (RuntimeManager.Instance != null)
            return RuntimeManager.Instance.GetSelectedCharacterPrefab();

        return SelectedCharacterPrefab;
    }

    private void OnSelectButtonClick()
    {
        // CharacterPrefab이 선택된 상태에서만 다음 씬으로 전환
        if (SelectedCharacterPrefab != null)
        {
            // 예시: 실제 게임 씬으로 전환 (씬 이름은 프로젝트에 맞게 조정)
            SceneManager.LoadScene("MainGameScene");
        }
        else
        {
            Debug.LogWarning("캐릭터가 선택되지 않았습니다. 캐릭터를 선택한 후 진행해주세요.");
        }
    }
}