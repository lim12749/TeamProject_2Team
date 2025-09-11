using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class gamemanager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static gamemanager Instance { get; private set; }
    public TextMeshProUGUI coinCountText;
    // === ���� ���� ===
    public int playerScore = 0;
    public int playerHealth = 100;
    public string currentPlayerName = "Guest";
    public int coinCount = 0;

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
    private void Start()
    {
        playerScore = 0;
        playerHealth = 100;
        coinCount = 0;
        UpdateCoinUI();
        Debug.Log("���� ����! �ʱ�ȭ �Ϸ�");
    }


    // === ���� �Լ� ===

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
        coinCount = 0;
        playerHealth = 100;
    }
    public void AddCoin()
    {
        coinCount++;
        Debug.Log("���� ȹ��! ���� ���� ����: " + coinCount);
        UpdateCoinUI();
    }

    // UI�� ������Ʈ�ϴ� �Լ�
    private void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = "Coins: " + coinCount.ToString();
        }
    }
}
