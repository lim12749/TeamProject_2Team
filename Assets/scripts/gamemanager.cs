using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class gamemanager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static gamemanager Instance { get; private set; }
    public TextMeshProUGUI coinCountText;
    // === 공통 변수 ===
    public int playerScore = 0;
    public int playerHealth = 100;
    public string currentPlayerName = "Guest";
    public int coinCount = 0;

    // === 초기화 ===
    private void Awake()
    {
        // 싱글톤 보장
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        playerScore = 0;
        playerHealth = 100;
        coinCount = 0;
        UpdateCoinUI();
        Debug.Log("게임 시작! 초기화 완료");
    }


    // === 공통 함수 ===

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("플레이어 사망");
            RestartGame();
        }
    }

    public void RestartGame()
    {
        Debug.Log("게임 재시작");
        coinCount = 0;
        playerHealth = 100;
    }
    public void AddCoin()
    {
        coinCount++;
        Debug.Log("코인 획득! 현재 코인 개수: " + coinCount);
        UpdateCoinUI();
    }

    // UI를 업데이트하는 함수
    private void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = "Coins: " + coinCount.ToString();
        }
    }
}
