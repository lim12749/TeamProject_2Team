using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public int characterIndex = 0; // 0 = knight, 1 = soldier 등
    public Renderer floorRenderer;      // 바닥의 Renderer
    public Color normalColor = Color.white;  // 기본 색
    public Color selectedColor = Color.red;  // 선택 색
    public float normalScale = 1f;           // 기본 크기
    public float selectedScale = 1.2f;       // 선택 크기
    public float transitionSpeed = 5f;       // 변화 속도

    [Header("이 캐릭터의 게임 내 소환 프리팹")]
    public GameObject inGamePrefab; // Inspector에서 각 캐릭터의 소환용 프리팹을 할당

    // CharacterSelector 배열 참조 필드 추가
    public CharacterSelector[] chars;

    private bool isSelected = false;
    private Vector3 targetScale;

    void Start()
    {
        if (floorRenderer != null)
            floorRenderer.material.color = normalColor;

        targetScale = Vector3.one * normalScale;
    }

    void Update()
    {
        // 부드럽게 크기 변화
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
    }

    void OnMouseDown()
    {
        var manager = CharacterSelectionManager.Instance;
        if (manager != null)
            manager.SelectCharacter(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (floorRenderer != null)
        {
            floorRenderer.material.color = isSelected ? selectedColor : normalColor;
        }

        targetScale = Vector3.one * (isSelected ? selectedScale : normalScale);
    }
}
