using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{
    public CharacterSelector[] chars;
    public DataManager.Character character;
    public Renderer floorRenderer;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.red;
    public float normalScale = 1f;
    public float selectedScale = 1.2f;
    public float transitionSpeed = 5f;

    private Vector3 targetScale;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        targetScale = Vector3.one * normalScale;

        // 시작 시 전체 캐릭터 비선택 처리
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i].OnDeselect();
        }
    }

    void Update()
    {
        HandleClick();
        SmoothScale();
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonUp(0)) // 좌클릭
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 클릭한 오브젝트가 자신일 때
                if (hit.transform == transform)
                {
                    DataManager.Instance.CurrentCharacter = character;
                    OnSelect();

                    // 다른 캐릭터 비활성화
                    foreach (var c in chars)
                    {
                        if (c != this)
                            c.OnDeselect();
                    }
                }
            }
        }
    }

    void SmoothScale()
    {
        // 부드럽게 스케일 변화
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
    }

    public void OnDeselect()
    {
        // 메인 게임 씬이면 색상/스케일 변경 막기
        if (SceneManager.GetActiveScene().name == "MainGameScene")
            return;

        if (floorRenderer != null)
            floorRenderer.material.color = normalColor;

        targetScale = Vector3.one * normalScale;
    }

    public void OnSelect()
    {
        // 메인 게임 씬이면 색상/스케일 변경 막기
        if (SceneManager.GetActiveScene().name == "MainGameScene")
            return;

        if (floorRenderer != null)
            floorRenderer.material.color = selectedColor;

        targetScale = Vector3.one * selectedScale;
    }
}
