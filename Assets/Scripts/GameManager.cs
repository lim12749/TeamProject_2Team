using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject[] charPrefabs; // 0 = knight, 1 = soldier 등
    public GameObject Player { get; private set; }

    private TextMeshProUGUI timeText;
    private float elapsedTime = 0f;
    private bool isTimerRunning = false;

    private float monsterUpgradeInterval = 30f;  // 30초마다
    private float nextMonsterUpgradeTime = 30f;  // 다음 강화 시간 기준

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
        {
            SpawnPlayerForGame();
            FindTimeTextInScene();
            StartTimer();
            ResetMonsterUpgradeTimer();
        }
        else
        {
            StopTimer();
        }
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

    void FindTimeTextInScene()
    {
        GameObject timeGO = GameObject.Find("Time");
        if (timeGO != null)
        {
            timeText = timeGO.GetComponent<TextMeshProUGUI>();
            if (timeText == null)
                Debug.LogWarning("GameManager: 'Time' 오브젝트에 TextMeshProUGUI 컴포넌트가 없습니다.");
        }
        else
        {
            Debug.LogWarning("GameManager: 씬에서 'Time' 이름의 오브젝트를 찾을 수 없습니다.");
        }
    }

    void StartTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = true;
    }

    void StopTimer()
    {
        isTimerRunning = false;
    }

    void ResetMonsterUpgradeTimer()
    {
        nextMonsterUpgradeTime = monsterUpgradeInterval;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;

            // 시간 텍스트 갱신
            if (timeText != null)
            {
                int minutes = (int)(elapsedTime / 60);
                int seconds = (int)(elapsedTime % 60);
                timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            // 30초마다 몬스터 능력치 강화
            if (elapsedTime >= nextMonsterUpgradeTime)
            {
                UpgradeMonsters();
                nextMonsterUpgradeTime += monsterUpgradeInterval;  // 다음 강화 시간 설정
            }
        }
    }

    void UpgradeMonsters()
    {
        if (MonsterManager.Instance != null)
        {
            // 1. 몬스터 기본 능력치 강화
            MonsterManager.Instance.defaultMoveSpeed *= 1.1f;
            MonsterManager.Instance.defaultMaxHealth *= 1.1f;
            MonsterManager.Instance.defaultExpReward = Mathf.CeilToInt(MonsterManager.Instance.defaultExpReward * 1.1f);

            // 3. 몬스터 스폰 속도 감소
            var spawners = FindObjectsOfType<MonsterSpawner>();
            foreach (var spawner in spawners)
            {
                spawner.IncreaseSpawnInterval(1.1f); // 10% 느리게
            }

            Debug.Log($"MonsterManager 기본 능력치 강화! 새 속도: {MonsterManager.Instance.defaultMoveSpeed:F2}, 새 체력: {MonsterManager.Instance.defaultMaxHealth:F2}, 새 경험치: {MonsterManager.Instance.defaultExpReward}");
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene("MainMenuScene");
        ResetGame();
    }

    private void ResetGame()
    {
        // 타이머 초기화
        StopTimer();
        elapsedTime = 0f;
        ResetMonsterUpgradeTimer();

        // MonsterManager 기본값 복원
        if (MonsterManager.Instance != null)
        {
            MonsterManager.Instance.ResetDefaults();
        }

        // 몬스터 스포너 상태(스폰 인터벌 등) 리셋 시도
        var spawners = FindObjectsOfType<MonsterSpawner>();
        foreach (var spawner in spawners)
        {
            // 가능하면 스포너에 Reset 관련 메서드가 있는지 호출
            var t = spawner.GetType();
            var resetMethod = t.GetMethod("ResetSpawnInterval") ?? t.GetMethod("ResetInterval") ?? t.GetMethod("Reset");
            if (resetMethod != null)
            {
                try { resetMethod.Invoke(spawner, null); }
                catch (Exception ex) { Debug.LogWarning($"GameManager: spawner reset 호출 실패: {ex.Message}"); }
            }
        }

        // 씬에 남아있는 몬스터들 제거 (초기화 목적)
        var monsters = FindObjectsOfType<Monster>();
        foreach (var m in monsters)
        {
            if (m != null)
                Destroy(m.gameObject);
        }

        Debug.Log("GameManager: 게임 상태 초기화 완료.");
    }
}
