using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    // Singleton 패턴을 사용하여 CharacterSelectionManager 인스턴스를 쉽게 참조
    public static CharacterSelectionManager Instance { get; private set; }

    public CharacterSelector[] platforms;  // 발판들의 배열

    private int selectedPlatformIndex = -1;  // 현재 선택된 발판의 인덱스 (-1은 선택되지 않음)

    void Awake()
    {
        // Singleton 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 발판 선택 시 호출되는 함수
    public void SelectPlatform(int platformIndex)
    {
        // platformIndex가 유효한지 확인 (배열 범위 내에 있는지)
        if (platformIndex >= 0 && platformIndex <= platforms.Length)
        {
            // 이전에 선택된 발판의 상태를 리셋
            if (selectedPlatformIndex != -1)
            {
                platforms[selectedPlatformIndex].resetSelected();  // 이전 발판 비활성화
            }

            // 새로운 발판을 선택하고, 해당 발판의 상태를 활성화
            platforms[platformIndex].SetSelected();
            selectedPlatformIndex = platformIndex;  // 현재 선택된 발판 인덱스 저장
        }
        else
        {
            Debug.LogError("Invalid platform index: " + platformIndex);  // 인덱스 오류 발생 시 에러 로그
        }
    }

}
