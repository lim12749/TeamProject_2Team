using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject[] charPrefabs; // 0 = knight, 1 = soldier 등
    public GameObject Player { get; private set; }

    void Awake()
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

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainGameScene")
            SpawnPlayerForGame();
    }

    void SpawnPlayerForGame()
    {
        if (Player != null) return;

        int index = PlayerPrefs.GetInt("SelectedCharacterIndex", 0);
        index = Mathf.Clamp(index, 0, (charPrefabs != null ? charPrefabs.Length - 1 : 0));

        GameObject prefabToSpawn = null;
        if (charPrefabs != null && charPrefabs.Length > 0)
            prefabToSpawn = charPrefabs[index];

        if (prefabToSpawn != null)
        {
            Player = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
            Player.name = prefabToSpawn.name + "_Player";
            Player.transform.SetParent(null);
            Player.transform.localScale = Vector3.one;
            Debug.Log($"GameManager: 플레이어 프리팹 '{prefabToSpawn.name}' (index {index}) 소환 완료.");
        }
        else
        {
            Debug.LogWarning("GameManager: charPrefabs가 설정되어 있지 않아 플레이어를 소환할 수 없습니다.");
        }
    }
}
