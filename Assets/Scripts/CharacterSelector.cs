<<<<<<< Updated upstream
<<<<<<< Updated upstream
ï»¿using UnityEngine;
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
=======
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public Renderer floorRenderer;      // ¹Ù´ÚÀÇ Renderer
    public Color normalColor = Color.white;  // ±âº» »ö
    public Color selectedColor = Color.red;  // ¼±ÅÃ »ö
    public float normalScale = 1f;           // ±âº» Å©±â
    public float selectedScale = 1.2f;       // ¼±ÅÃ Å©±â
    public float transitionSpeed = 5f;       // º¯È­ ¼Óµµ

=======
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public Renderer floorRenderer;      // ¹Ù´ÚÀÇ Renderer
    public Color normalColor = Color.white;  // ±âº» »ö
    public Color selectedColor = Color.red;  // ¼±ÅÃ »ö
    public float normalScale = 1f;           // ±âº» Å©±â
    public float selectedScale = 1.2f;       // ¼±ÅÃ Å©±â
    public float transitionSpeed = 5f;       // º¯È­ ¼Óµµ

>>>>>>> Stashed changes
    [Header("ÀÌ Ä³¸¯ÅÍÀÇ °ÔÀÓ ³» ¼ÒÈ¯ ÇÁ¸®ÆÕ")]
    public GameObject inGamePrefab; // CharacterStats.inGamePrefab ´ë½Å Á÷Á¢ ÂüÁ¶ °¡´ÉÇÏµµ·Ï ³ëÃâ

    private bool isSelected = false;
<<<<<<< Updated upstream
>>>>>>> Stashed changes
    private Vector3 targetScale;

    void Awake()
    {
<<<<<<< Updated upstream
        mainCam = Camera.main;
        targetScale = Vector3.one * normalScale;
        // animatorë¥¼ ìê¸° ìì‹  ë˜ëŠ” ìì‹ì—ì„œ ìë™ìœ¼ë¡œ ì°¾ìŒ
        animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // ì´ˆê¸° ìƒíƒœëŠ” ë¹„ì„ íƒ(ì•ˆì „í•˜ê²Œ)
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
                // í´ë¦­í•œ ì˜¤ë¸Œì íŠ¸ê°€ ì´ ìºë¦­í„°(ë£¨íŠ¸)ì¸ì§€ í™•ì¸
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    DataManager.Instance.CurrentCharacter = character;
                    OnSelect();

                    // ë‹¤ë¥¸ ìºë¦­í„° ë¹„í™œì„±í™”
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

    // ì•ˆì „ ì´ˆê¸°í™” (ì™¸ë¶€ì—ì„œ í•œ ë²ˆë§Œ í˜¸ì¶œí•´ë„ ì•ˆì „)
    public void SafeDeselect()
    {
        if (SceneManager.GetActiveScene().name == "MainGameScene")
            return;

=======
>>>>>>> Stashed changes
=======
    private Vector3 targetScale;

    void Start()
    {
>>>>>>> Stashed changes
        if (floorRenderer != null)
        {
            // ëŸ°íƒ€ì„ì— ì”¬ ì˜¤ë¸Œì íŠ¸ë¼ë©´ .material ì•ˆì „, ì—ë””í„°ì—ì„œë§Œ sharedMaterial
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

<<<<<<< Updated upstream
<<<<<<< Updated upstream
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
=======
    void Update()
    {
        // ºÎµå·´°Ô Å©±â º¯È­
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
    }

    void OnMouseDown()
    {
        // CharacterSelectionManagerÀÇ ½Ì±ÛÅÏ ÀÎ½ºÅÏ½º¸¦ »ç¿ë
        var manager = CharacterSelectionManager.Instance;
        if (manager != null)
            manager.SelectCharacter(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (floorRenderer != null)
        {
            // material Á¢±Ù ½Ã ÀÎ½ºÅÏ½º°¡ »ı¼ºµÇ¹Ç·Î ÁÖÀÇ. ¿©±â¼± °£´ÜÈ÷ º¯°æ.
            floorRenderer.material.color = isSelected ? selectedColor : normalColor;
        }
>>>>>>> Stashed changes

=======
    void Update()
    {
        // ºÎµå·´°Ô Å©±â º¯È­
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
    }

    void OnMouseDown()
    {
        // CharacterSelectionManagerÀÇ ½Ì±ÛÅÏ ÀÎ½ºÅÏ½º¸¦ »ç¿ë
        var manager = CharacterSelectionManager.Instance;
        if (manager != null)
            manager.SelectCharacter(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (floorRenderer != null)
        {
            // material Á¢±Ù ½Ã ÀÎ½ºÅÏ½º°¡ »ı¼ºµÇ¹Ç·Î ÁÖÀÇ. ¿©±â¼± °£´ÜÈ÷ º¯°æ.
            floorRenderer.material.color = isSelected ? selectedColor : normalColor;
        }

>>>>>>> Stashed changes
        targetScale = Vector3.one * (isSelected ? selectedScale : normalScale);
    }
}
