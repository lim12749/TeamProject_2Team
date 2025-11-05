using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
<<<<<<< Updated upstream
    public static PlayerMove Instanse { get; private set; }
    public LayerMask groundLayer; // í´ë¦­ ê°€ëŠ¥í•œ ë°”ë‹¥ ë ˆì´ì–´
    public LayerMask monsterLayer; // ëª¬ìŠ¤í„° ë ˆì´ì–´ ì¶”ê°€
=======
    [Header("¹Ù´Ú ·¹ÀÌ¾î")]
    public LayerMask groundLayer;
>>>>>>> Stashed changes

    private CharacterController controller;
    public Animator animator;
    private CharacterStats stats;

    private Vector3 targetPosition;
    private bool isMoving = false;

<<<<<<< Updated upstream
    // ì¤‘ë ¥ ê´€ë ¨ ë³€ìˆ˜
=======
    // Áß·Â °ü·Ã º¯¼ö
>>>>>>> Stashed changes
    private Vector3 velocity;
    private float gravity = -9.81f;
    private float groundCheckOffset = 0.2f;

    // ìë™ ê³µê²© ê´€ë ¨ ë³€ìˆ˜
    public float attackRange = 10f;
    public float attackCooldown = 1f; // ê³µê²© ê°„ê²©
    private float lastAttackTime = 0f;
    private Transform currentTarget;

    private void Awake()
    {
        if(Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        stats = GetComponent<CharacterStats>();

        // ì„ íƒì”¬ì¼ ë• RootMotion ë„ê¸°
        if (SceneManager.GetActiveScene().name == "CharacterSelectScene")
            animator.applyRootMotion = false;
        else
            animator.applyRootMotion = false; // CharacterControllerë¡œ ì´ë™í•˜ë‹ˆê¹Œ ê³„ì† false ìœ ì§€
    }


    void Update()
    {
        DetectMonster();

        // ê³µê²© ì¤‘ì´ë©´ ì´ë™ ì¤‘ë‹¨
        if (currentTarget != null && IsTargetInRange())
        {
            AttackTarget();
        }
        else
        {
            MoveToTarget();
        }

        HandleMouseInput();
<<<<<<< Updated upstream
=======
        MoveToTarget();
        ApplyGravity();
>>>>>>> Stashed changes
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                targetPosition = hit.point;
                isMoving = true;
<<<<<<< Updated upstream
                animator.SetBool("isMoving", true);
                currentTarget = null; // ìˆ˜ë™ ì´ë™ ì‹œ ìë™ ê³µê²© í•´ì œ
=======
>>>>>>> Stashed changes
            }
        }
    }

    void MoveToTarget()
    {
<<<<<<< Updated upstream
        if (controller == null)
            Debug.LogError("âŒ CharacterControllerê°€ ì—†ìŒ!");
        if (stats == null)
            Debug.LogError("âŒ CharacterStatsê°€ ì—†ìŒ!");

        // ì¤‘ë ¥ ê³„ì‚°
=======
        if (!isMoving)
        {
            animator.SetBool("isMoving", false);
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveZ", 0);
            return;
        }

        Vector3 moveDir = targetPosition - transform.position;
        moveDir.y = 0f;

        if (moveDir.magnitude < 0.1f)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
            return;
        }

        Vector3 move = moveDir.normalized * stats.moveSpeed;
        controller.Move(move * Time.deltaTime);

        animator.SetBool("isMoving", true);
        UpdateDirectionalAnimation(moveDir);
    }

    void UpdateDirectionalAnimation(Vector3 moveDir)
    {
        // ÇöÀç ¹Ù¶óº¸´Â ¹æÇâ (MonsterDetector°¡ µ¹·ÁÁØ ¹æÇâ)
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // ÀÌµ¿ º¤ÅÍ¸¦ ÇöÀç Ä³¸¯ÅÍ ·ÎÄÃ ±âÁØÀ¸·Î º¯È¯
        float forwardDot = Vector3.Dot(forward, moveDir.normalized); // ¾ÕµÚ
        float rightDot = Vector3.Dot(right, moveDir.normalized);     // ÁÂ¿ì

        // Blend Tree¿¡ Àü´Ş
        animator.SetFloat("MoveZ", forwardDot);
        animator.SetFloat("MoveX", rightDot);
    }

    void ApplyGravity()
    {
>>>>>>> Stashed changes
        if (controller.isGrounded)
            velocity.y = -groundCheckOffset;
        else
            velocity.y += gravity * Time.deltaTime;

<<<<<<< Updated upstream
        if (isMoving)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.magnitude < 0.1f)
            {
                isMoving = false;
                animator.SetBool("isMoving", false);
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

                // ğŸ’¥ ë¬¸ì œ ìœ„ì¹˜
                Vector3 move = direction.normalized * stats.moveSpeed;
                controller.Move(move * Time.deltaTime);
            }
        }

=======
>>>>>>> Stashed changes
        controller.Move(velocity * Time.deltaTime);
    }


    void DetectMonster()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, monsterLayer);

        if (hits.Length > 0)
        {
            // ê°€ì¥ ê°€ê¹Œìš´ ëª¬ìŠ¤í„° ì°¾ê¸°
            float minDistance = Mathf.Infinity;
            Transform nearest = null;

            foreach (Collider hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = hit.transform;
                }
            }

            currentTarget = nearest;
        }
        else
        {
            currentTarget = null;
        }
    }

    bool IsTargetInRange()
    {
        if (currentTarget == null) return false;
        return Vector3.Distance(transform.position, currentTarget.position) <= attackRange;
    }

    void AttackTarget()
    {
        // ëª¬ìŠ¤í„° ë°”ë¼ë³´ê¸°
        Vector3 lookDir = currentTarget.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // ê³µê²© ì• ë‹ˆë©”ì´ì…˜ ì‹¤í–‰
        if (Time.time - lastAttackTime > attackCooldown)
        {
            animator.SetTrigger("Fire"); // UpperBodyLayerì—ì„œ ì‚¬ìš©í•  ê³µê²© íŠ¸ë¦¬ê±°
            lastAttackTime = Time.time;
        }
    }

    void OnDrawGizmosSelected()
    {
        // ê³µê²© ë²”ìœ„ ì‹œê°í™”
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
