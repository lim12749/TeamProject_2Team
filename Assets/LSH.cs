using UnityEngine;
using TMPro;

public class LSH : MonoBehaviour
{
    // 정적(static) 인스턴스를 선언하여 다른 스크립트에서 쉽게 접근할 수 있게 합니다.
    public static LSH instance;

    // 코인 개수
    private int coinCount = 0;

    // 코인 개수를 표시할 UI 텍스트 (유니티 인스펙터에서 연결)
    public TextMeshProUGUI coinCountText;

    // 게임 시작 시, 인스턴스 초기화
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