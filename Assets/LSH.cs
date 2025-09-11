using UnityEngine;
using TMPro;

public class LSH : MonoBehaviour
{
    // ����(static) �ν��Ͻ��� �����Ͽ� �ٸ� ��ũ��Ʈ���� ���� ������ �� �ְ� �մϴ�.
    public static LSH instance;

    // ���� ����
    private int coinCount = 0;

    // ���� ������ ǥ���� UI �ؽ�Ʈ (����Ƽ �ν����Ϳ��� ����)
    public TextMeshProUGUI coinCountText;

    // ���� ���� ��, �ν��Ͻ� �ʱ�ȭ
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ������ �߰��ϴ� �Լ�
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

    // ���� ���� �� UI �ʱ�ȭ
     void Start()
    {
        UpdateCoinUI();
    }
}