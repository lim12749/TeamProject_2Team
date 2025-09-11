using UnityEngine;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static gamemanager Instance { get; private set; }

    // === ���� ���� ===
    public int playerScore = 0;
    public int playerHealth = 100;
    public string currentPlayerName = "Guest";

    // === �ʱ�ȭ ===
    private void Awake()
    {
        // �̱��� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // === ���� �Լ� ===
    public void AddScore(int amount)
    {
        playerScore += amount;
        Debug.Log("���� �߰���: " + playerScore);
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("�÷��̾� ���");
            RestartGame();
        }
    }

    public void RestartGame()
    {
        Debug.Log("���� �����");
        playerScore = 0;
        playerHealth = 100;
    }
}
