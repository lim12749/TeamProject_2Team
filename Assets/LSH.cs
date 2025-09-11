using UnityEngine;
using TMPro; // TMP_Text�� ����Ϸ��� �� ���̺귯���� �߰��ؾ� �մϴ�.

public class LSH : MonoBehaviour
{
    // ���� ����
    private int coinCount = 0;

    // ���� ������ ǥ���� UI �ؽ�Ʈ (����Ƽ �ν����Ϳ��� ����)
    public TextMeshProUGUI coinCountText;

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