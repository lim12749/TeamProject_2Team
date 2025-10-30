using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    // 버튼에서 호출할 함수
    public void OnClickCharacterSelect()
    {
        Debug.Log("캐릭터 선택 버튼 클릭됨");
        // 캐릭터 선택 씬으로 이동
        SceneManager.LoadScene("CharacterSelectScene");
    }

    public void OnClickQuit()
    {
        // 게임 종료
        Application.Quit();

        // 에디터에서 테스트할 때 종료 확인용 로그
        Debug.Log("게임 종료");
    }
}
