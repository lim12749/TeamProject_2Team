using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelector : MonoBehaviour
{
    [HideInInspector] public CharacterSelector[] chars;
    public DataManager.Character character;
    public Renderer floorRenderer;
    public Animator animator;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.red;
    public float normalScale = 1f;
    public float selectedScale = 1.2f;
    public float transitionSpeed = 5f;

    private Animator animator;
    private Vector3 targetScale;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
        targetScale = Vector3.one * normalScale;
        // animator를 자기 자신 또는 자식에서 자동으로 찾음
        animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // 초기 상태는 비선택(안전하게)
        SafeDeselect();
    }

    void Update()
    {
        HandleClick();
        SmoothScale();
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 클릭한 오브젝트가 이 캐릭터(루트)인지 확인
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    DataManager.Instance.CurrentCharacter = character;
                    OnSelect();

                    // 다른 캐릭터 비활성화
                    if (chars != null)
                    {
                        foreach (var c in chars)
                        {
                            if (c != null && c != this)
                                c.OnDeselect();
                        }
                    }
                }
            }
        }
    }

    void SmoothScale()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
    }

    // 안전 초기화 (외부에서 한 번만 호출해도 안전)
    public void SafeDeselect()
    {
        if (SceneManager.GetActiveScene().name == "MainGameScene")
            return;

        if (floorRenderer != null)
        {
            // 런타임에 씬 오브젝트라면 .material 안전, 에디터에서만 sharedMaterial
            if (Application.isPlaying)
                floorRenderer.material.color = normalColor;
            else
                floorRenderer.sharedMaterial.color = normalColor;
        }

        if (animator != null)
            animator.SetBool("isMoving", false);

        animator.SetBool("isMoving", false);

        targetScale = Vector3.one * normalScale;
    }

    public void OnDeselect()
    {
        SafeDeselect();
    }

    public void OnSelect()
    {
        if (SceneManager.GetActiveScene().name == "MainGameScene")
            return;

        if (floorRenderer != null)
        {
            if (Application.isPlaying)
                floorRenderer.material.color = selectedColor;
            else
                floorRenderer.sharedMaterial.color = selectedColor;
        }

        if (animator != null)
            animator.SetBool("isMoving", true);

        animator.SetBool("isMoving", true);

        targetScale = Vector3.one * selectedScale;
    }
}
