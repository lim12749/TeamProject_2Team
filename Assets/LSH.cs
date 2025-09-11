using UnityEngine;
using TMPro; // TMP_Text를 사용하려면 이 라이브러리를 추가해야 합니다.

public class LSH : MonoBehaviour
{
    // 코인 개수
    private int coinCount = 0;

    // 코인 개수를 표시할 UI 텍스트 (유니티 인스펙터에서 연결)
    public TextMeshProUGUI coinCountText;

    // 코인을 추가하는 함수
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

    // 게임 시작 시 UI 초기화
    void Start()
    {
        UpdateCoinUI();
    }
}