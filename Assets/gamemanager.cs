using UnityEngine;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static gamemanager Instance { get; private set; }

    // === 공통 변수 ===
    public int playerScore = 0;
    public int playerHealth = 100;
    public string currentPlayerName = "Guest";

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

    // === 공통 함수 ===
    public void AddScore(int amount)
    {
        playerScore += amount;
        Debug.Log("점수 추가됨: " + playerScore);
    }

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
        playerScore = 0;
        playerHealth = 100;
    }
}
