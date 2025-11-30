using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public Renderer floorRenderer;      // 발판의 Renderer
    public Color normalColor = Color.white;  // 기본 색
    public Color selectedColor = Color.red;  // 선택 색
    public Vector3 normalScale = new Vector3(0.3f, 1f, 0.2f);    // 기본 크기
    public float selectedScale = 1.2f;       // 선택 크기
    public float transitionSpeed = 5f;       // 변화 속도

    private Vector3 targetScale;

    void Awake()
    {
        // Renderer가 할당되지 않았으면 찾아서 할당
        if (floorRenderer == null)
        {
            floorRenderer = GetComponent<Renderer>();
        }

        // floorRenderer가 null이 아닌 경우 색상 설정
        if (floorRenderer != null)
        {
            floorRenderer.material.color = normalColor;
        }

        targetScale = normalScale;
    }

    void Update()
    {
        // 부드럽게 크기 변화
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
    }

    // 외부에서 선택 상태를 변경할 때 호출
    public void SetSelected()
    {
        if (floorRenderer != null)
        {
            // 'new Material()'을 사용하여 각 객체에 대해 독립적인 재질을 생성
            floorRenderer.material = new Material(floorRenderer.material);
            floorRenderer.material.color = selectedColor;
            targetScale = normalScale * selectedScale;
        }
    }

    public void resetSelected()
    {
        if (floorRenderer != null)
        {
            // 'new Material()'을 사용하여 원본 재질을 변경
            floorRenderer.material = new Material(floorRenderer.material);
            floorRenderer.material.color = normalColor;
            targetScale = normalScale;
        }
    }
}
