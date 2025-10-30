using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] WaveSpawner spawner;
    [SerializeField] PlayerHealth player;

    [Header("UI (Text)")]
    [SerializeField] TMPro.TextMeshProUGUI waveText;
    [SerializeField] TMPro.TextMeshProUGUI aliveText;
    [SerializeField] TMPro.TextMeshProUGUI hpText;

    [Header("UI (Panels)")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject victoryPanel;

    bool paused;
    bool ended;

    void Awake()
    {
        if (!spawner)
            spawner = FindFirstObjectByType<WaveSpawner>();
        if (!player)
            player  = FindFirstObjectByType<PlayerHealth>();

        HideAllPanels();
        Time.timeScale = 1f;
    }

    void OnEnable()
    {
        if (spawner != null)
        {
            spawner.OnWaveStarted      += OnWaveStarted;
            spawner.OnWaveCleared      += OnWaveCleared;
            spawner.OnAllWavesCleared  += OnAllWavesCleared;
        }
        if (player != null)
        {
            player.OnDeath   += OnPlayerDeath;
            player.OnChanged += OnPlayerHpChanged;
        }
    }

    void OnDisable()
    {
        if (spawner != null)
        {
            spawner.OnWaveStarted      -= OnWaveStarted;
            spawner.OnWaveCleared      -= OnWaveCleared;
            spawner.OnAllWavesCleared  -= OnAllWavesCleared;
        }
        if (player != null)
        {
            player.OnDeath   -= OnPlayerDeath;
            player.OnChanged -= OnPlayerHpChanged;
        }
    }

    void Start()
    {
        // 스포너 자동 시작
        if (spawner && !spawner.Running) spawner.StartWaves();
        RefreshHUD();
    }

    void Update()
    {
        if (ended) return;

        
        // ESC만 사용해 일시정지 토글
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        RefreshHUD();
    }

    // ====== 이벤트 핸들러 ======
    void OnWaveStarted(int wave)
    {
        RefreshHUD();
    }

    void OnWaveCleared(int wave)
    {
        RefreshHUD();
    }

    void OnAllWavesCleared()
    {
        ended = true;
        ShowPanel(victoryPanel, true);
        Time.timeScale = 0f;
    }

    void OnPlayerDeath()
    {
        ended = true;
        ShowPanel(gameOverPanel, true);
        Time.timeScale = 0f;
    }

    void OnPlayerHpChanged(float current, float max)
    {
        if (hpText) hpText.text = max > 0 ? $"HP {Mathf.Ceil(current)}/{Mathf.Ceil(max)}" : $"HP {Mathf.Ceil(current)}";
    }

    // ====== UI/도움 메서드 ======
    void RefreshHUD()
    {
        if (spawner)
        {
            if (waveText)  waveText.text  = spawner.Running ? $"Wave {spawner.CurrentWave}" : "Wave -";
            if (aliveText) aliveText.text = $"Alive {spawner.Alive}";
        }
        if (player && hpText && player.Current >= 0f)
        {
            // PlayerHealth.OnChanged로도 갱신되지만, 안전하게 1프레임마다 보정
            hpText.text = $"HP {Mathf.Ceil(player.Current)}";
        }
    }

    void TogglePause()
    {
        if (ended) return;
        paused = !paused;
        Time.timeScale = paused ? 0f : 1f;
        ShowPanel(pausePanel, paused);
    }

    void HideAllPanels()
    {
        ShowPanel(pausePanel,   false);
        ShowPanel(gameOverPanel,false);
        ShowPanel(victoryPanel, false);
    }

    void ShowPanel(GameObject go, bool show)
    {
        if (!go) return;
        go.SetActive(show);
    }

    // ====== UI 버튼용 ======
    public void UI_Resume()
    {
        if (!paused) return;
        TogglePause();
    }

    public void UI_Restart()
    {
        Time.timeScale = 1f;
        Restart();
    }

    public void UI_Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Restart()
    {
        var buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(buildIndex);
    }
}