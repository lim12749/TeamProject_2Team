using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
<<<<<<< Updated upstream
<<<<<<< Updated upstream
    public static PlayerMove Instanse { get; private set; }
    public LayerMask groundLayer; // í´ë¦­ ê°€ëŠ¥í•œ ë°”ë‹¥ ë ˆì´ì–´
    public LayerMask monsterLayer; // ëª¬ìŠ¤í„° ë ˆì´ì–´ ì¶”ê°€

    [Header("ï¿½Ù´ï¿½ ï¿½ï¿½ï¿½Ì¾ï¿½")]
    public LayerMask groundLayer;
=======
=======
>>>>>>> Stashed changes
    public LayerMask groundLayer;
    public LayerMask monsterLayer;
    public float detectionRange = 10f;
    public float rotationSpeed = 10f;
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

    private CharacterController controller;
    public Animator animator;
    private CharacterStats stats;
    private PlayerShooting shooting;

    private Vector3 targetPosition;
    private bool isMoving = false;

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
        shooting = GetComponent<PlayerShooting>();
<<<<<<< Updated upstream
=======

        // RuntimeManager°¡ ÀÖÀ¸¸é Áß¾Ó ¼³Á¤°ªÀ¸·Î ÃÊ±âÈ­ (ÇÁ·ÎÁ§Æ® Àü¿ª ÅëÁ¦)
        if (RuntimeManager.Instance != null)
        {
            groundLayer = RuntimeManager.Instance.GetGroundLayer();
            monsterLayer = RuntimeManager.Instance.GetMonsterLayer();
            detectionRange = RuntimeManager.Instance.GetDetectionRange();
            rotationSpeed = RuntimeManager.Instance.GetRotationSpeed();

            if (stats != null)
                stats.moveSpeed = RuntimeManager.Instance.GetPlayerMoveSpeed();
        }
>>>>>>> Stashed changes

        // RuntimeManager°¡ ÀÖÀ¸¸é Áß¾Ó ¼³Á¤°ªÀ¸·Î ÃÊ±âÈ­ (ÇÁ·ÎÁ§Æ® Àü¿ª ÅëÁ¦)
        if (RuntimeManager.Instance != null)
        {
            groundLayer = RuntimeManager.Instance.GetGroundLayer();
            monsterLayer = RuntimeManager.Instance.GetMonsterLayer();
            detectionRange = RuntimeManager.Instance.GetDetectionRange();
            rotationSpeed = RuntimeManager.Instance.GetRotationSpeed();

            if (stats != null)
                stats.moveSpeed = RuntimeManager.Instance.GetPlayerMoveSpeed();
        }

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
        MoveToTarget();
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        ApplyGravity();
=======
        ShootIfMonsterInRange();
>>>>>>> Stashed changes
=======
        ShootIfMonsterInRange();
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
<<<<<<< Updated upstream
                animator.SetBool("isMoving", true);
                currentTarget = null; // ìˆ˜ë™ ì´ë™ ì‹œ ìë™ ê³µê²© í•´ì œ
=======
                if (animator != null) animator.SetBool("isMoving", true);
>>>>>>> Stashed changes
=======
                if (animator != null) animator.SetBool("isMoving", true);
>>>>>>> Stashed changes
            }
        }
    }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
    // 10f °Å¸® ³» °¡Àå °¡±î¿î ¸ó½ºÅÍ¸¦ ¹Ù¶óº½ (ºñÁÖÇà½Ã¿¡µµ µ¿ÀÛ °¡´É)
    void LookAtNearestMonster()
    {
        Collider[] monsters = Physics.OverlapSphere(transform.position, detectionRange, monsterLayer);

        if (monsters.Length == 0) return;

        Transform nearest = monsters[0].transform;
        float minDist = Vector3.Distance(transform.position, nearest.position);

        foreach (var m in monsters)
        {
            float dist = Vector3.Distance(transform.position, m.transform.position);
            if (dist < minDist)
            {
                nearest = m.transform;
                minDist = dist;
            }
        }

        Vector3 lookDir = nearest.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }

    // ¸¶¿ì½º Å¬¸¯ÇÑ ¸ñÇ¥ ÁöÁ¡À¸·Î ÀÌµ¿
>>>>>>> Stashed changes
    void MoveToTarget()
    {
        if (controller == null)
            Debug.LogError("âŒ CharacterControllerê°€ ì—†ìŒ!");
        if (stats == null)
            Debug.LogError("âŒ CharacterStatsê°€ ì—†ìŒ!");

        // ì¤‘ë ¥ ê³„ì‚°

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
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float forwardDot = Vector3.Dot(forward, moveDir.normalized); 
        float rightDot = Vector3.Dot(right, moveDir.normalized);     

        animator.SetFloat("MoveZ", forwardDot);
        animator.SetFloat("MoveX", rightDot);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded)
            velocity.y = -groundCheckOffset;
        else
            velocity.y += gravity * Time.deltaTime;

        if (isMoving)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.magnitude < 0.1f)
            {
                isMoving = false;
                if (animator != null) animator.SetBool("isMoving", false);
            }
            else
            {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

                // ğŸ’¥ ë¬¸ì œ ìœ„ì¹˜
                Vector3 move = direction.normalized * stats.moveSpeed;
=======
                // ÁÖº¯ ¸ó½ºÅÍ Å½»ö
                Collider[] monsters = Physics.OverlapSphere(transform.position, detectionRange, monsterLayer);
                Transform lookTarget = null;

                if (monsters.Length > 0)
                {
                    // °¡Àå °¡±î¿î ¸ó½ºÅÍ Ã£±â
                    lookTarget = monsters[0].transform;
                    float minDist = Vector3.Distance(transform.position, lookTarget.position);
                    foreach (var m in monsters)
                    {
                        float dist = Vector3.Distance(transform.position, m.transform.position);
                        if (dist < minDist)
                        {
                            lookTarget = m.transform;
                            minDist = dist;
                        }
                    }
                }

                // È¸Àü Ã³¸®: ¸ó½ºÅÍ°¡ ÀÖÀ¸¸é ¸ó½ºÅÍ ¹æÇâ, ¾øÀ¸¸é ÀÌµ¿ ¹æÇâ
                Vector3 lookDir;
                if (lookTarget != null)
                {
                    lookDir = lookTarget.position - transform.position;
                    lookDir.y = 0f;
                }
                else
                {
                    lookDir = direction;
                }

                if (lookDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }

                // ÀÌµ¿ Ã³¸®
                float moveSpeed = stats != null ? stats.moveSpeed : 0f;
                Vector3 move = direction.normalized * moveSpeed;
>>>>>>> Stashed changes
=======
                // ÁÖº¯ ¸ó½ºÅÍ Å½»ö
                Collider[] monsters = Physics.OverlapSphere(transform.position, detectionRange, monsterLayer);
                Transform lookTarget = null;

                if (monsters.Length > 0)
                {
                    // °¡Àå °¡±î¿î ¸ó½ºÅÍ Ã£±â
                    lookTarget = monsters[0].transform;
                    float minDist = Vector3.Distance(transform.position, lookTarget.position);
                    foreach (var m in monsters)
                    {
                        float dist = Vector3.Distance(transform.position, m.transform.position);
                        if (dist < minDist)
                        {
                            lookTarget = m.transform;
                            minDist = dist;
                        }
                    }
                }

                // È¸Àü Ã³¸®: ¸ó½ºÅÍ°¡ ÀÖÀ¸¸é ¸ó½ºÅÍ ¹æÇâ, ¾øÀ¸¸é ÀÌµ¿ ¹æÇâ
                Vector3 lookDir;
                if (lookTarget != null)
                {
                    lookDir = lookTarget.position - transform.position;
                    lookDir.y = 0f;
                }
                else
                {
                    lookDir = direction;
                }

                if (lookDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }

                // ÀÌµ¿ Ã³¸®
                float moveSpeed = stats != null ? stats.moveSpeed : 0f;
                Vector3 move = direction.normalized * moveSpeed;
>>>>>>> Stashed changes
                controller.Move(move * Time.deltaTime);
            }
        }

        controller.Move(velocity * Time.deltaTime);
    }

<<<<<<< Updated upstream
<<<<<<< Updated upstream

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
=======
=======
>>>>>>> Stashed changes
    void ShootIfMonsterInRange()
    {
        if (shooting == null) return;

        Collider[] monsters = Physics.OverlapSphere(transform.position, detectionRange, monsterLayer);
        if (monsters.Length > 0)
        {
            // PlayerShootingÀÌ ÀÚÃ¼ Äğ´Ù¿îÀ» °ü¸®ÇÏ´Â TryShoot »ç¿ë
            shooting.TryShoot();
        }
    }
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
}
