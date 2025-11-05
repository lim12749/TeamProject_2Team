using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해

public class CountdownTimer : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_Text timerText; // 시간을 표시할 TextMeshPro UI

    // 내부 변수
    private float timeElapsed = 0f; // 경과 시간 (초)

    void Start()
    {
        // 시작할 때 0초로 초기화
        timeElapsed = 0f;
        timerText.text = "00:00";
    }

    void Update()
    {
        // 1. 매 프레임마다 경과 시간을 더함
        timeElapsed += Time.deltaTime;

        // 2. 시간 표시 업데이트
        DisplayTime(timeElapsed);
    }

    // 시간을 "분:초" (MM:SS) 형식으로 변환하여 텍스트에 표시
    void DisplayTime(float timeToDisplay)
    {
        // 총 경과 시간을 분과 초로 계산
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // "f0"은 소수점 없는 정수, "00"은 두 자리(예: 5초 -> "05")
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}