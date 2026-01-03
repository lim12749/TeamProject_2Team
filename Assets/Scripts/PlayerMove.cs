using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    public LayerMask groundLayer; // 클릭 가능한 바닥 레이어
    public LayerMask monsterLayer; // 몬스터 레이어
    public float detectionRange = 10f;
    public float rotationSpeed = 10f;

    private CharacterController controller;
    public Animator animator;
    private characterStats stats;
    private PlayerShooting shooting;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private Vector3 velocity;
    private float gravity = -9.81f;
    private float groundCheckOffset = 0.2f;

    // 자동 공격 관련 변수
    public float attackRange = 10f;
    public float attackCooldown = 1f; // 공격 간격
    private float lastAttackTime = 0f;
    private Transform currentTarget;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        stats = GetComponent<characterStats>();
        shooting = GetComponent<PlayerShooting>();

        // RuntimeManager가 있으면 중앙 설정 참조
        if (RuntimeManager.Instance != null)
        {
            groundLayer = RuntimeManager.Instance.GetGroundLayer();
            monsterLayer = RuntimeManager.Instance.GetMonsterLayer();
            detectionRange = RuntimeManager.Instance.GetDetectionRange();
            rotationSpeed = RuntimeManager.Instance.GetRotationSpeed();

            if (stats != null)
                stats.moveSpeed = RuntimeManager.Instance.GetPlayerMoveSpeed();
        }

        // CharacterSelectScene 등에서 root motion 제어(여기서 순수하게 이동 가능하도록 false로 고정)
        if (animator != null)
            animator.applyRootMotion = false;

        targetPosition = transform.position;
    }

    void Update()
    {
        HandleMouseInput();

        DetectMonster();

        // 항상 이동 처리 실행하도록 변경 — 공격 중에도 이동이 계속됨
        MoveToTarget();

        // 공격은 이동과 별개로 실행 (공격 시에도 이동 멈추지 않음)
        if (currentTarget != null && IsTargetInRange())
        {
            AttackTarget();
        }

        ApplyGravity();

        // 몬스터가 감지 범위에 있으면 자동 사격 시도
        ShootIfMonsterInRange();
    }

    void HandleMouseInput()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        var cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            targetPosition = hit.point;
            isMoving = true;
            currentTarget = null;
            if (animator != null) animator.SetBool("isMoving", true);
        }
    }

    // 마우스 클릭한 목표 지점으로 이동
    void MoveToTarget()
    {
        if (controller == null || stats == null)
        {
            if (controller == null) Debug.LogError("❌ CharacterController가 없음!");
            if (stats == null) Debug.LogError("❌ CharacterStats가 없음!");
            return;
        }

        if (!isMoving)
        {
            if (animator != null)
            {
                animator.SetBool("isMoving", false);
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveZ", 0);
            }
            return;
        }

        Vector3 moveDir = targetPosition - transform.position;
        moveDir.y = 0f;

        if (moveDir.magnitude < 0.1f)
        {
            isMoving = false;
            if (animator != null) animator.SetBool("isMoving", false);
            return;
        }

        // 몬스터 감지 범위에 따라 몬스터가 있으면 몬스터를 바라보게 하고, 없으면 이동 방향으로 회전
        Transform lookTarget = FindNearestMonster(detectionRange);
        Vector3 lookDir = (lookTarget != null) ? (lookTarget.position - transform.position) : moveDir;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        Vector3 move = moveDir.normalized * stats.moveSpeed;
        controller.Move(move * Time.deltaTime);

        if (animator != null)
        {
            animator.SetBool("isMoving", true);
            UpdateDirectionalAnimation(moveDir);
        }
    }

    void UpdateDirectionalAnimation(Vector3 moveDir)
    {
        if (animator == null) return;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float forwardDot = Vector3.Dot(forward, moveDir.normalized);
        float rightDot = Vector3.Dot(right, moveDir.normalized);

        animator.SetFloat("MoveZ", forwardDot);
        animator.SetFloat("MoveX", rightDot);
    }

    void ApplyGravity()
    {
        if (controller == null) return;

        if (controller.isGrounded)
            velocity.y = -groundCheckOffset;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    // 몬스터 감지(자동공격용)
    void DetectMonster()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, monsterLayer);

        if (hits.Length > 0)
        {
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
        if (currentTarget == null) return;

        // 몬스터 바라보기 (공격 시 조준용 회전만 수행)
        Vector3 lookDir = currentTarget.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // 공격 애니메이션 및 발사 (이 로직은 이동을 멈추지 않음)
        if (Time.time - lastAttackTime > attackCooldown)
        {
            if (animator != null) animator.SetTrigger("Fire");
            lastAttackTime = Time.time;

            // 애니메이션이 공격 시 PlayerShooting을 트리거하도록 하거나 즉시 시도
            shooting?.TryShoot();
        }
    }

    // 유틸: 범위내 가장 가까운 몬스터 반환
    Transform FindNearestMonster(float range)
    {
        Collider[] monsters = Physics.OverlapSphere(transform.position, range, monsterLayer);
        if (monsters.Length == 0) return null;

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

        return nearest;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    void ShootIfMonsterInRange()
    {
        if (shooting == null) return;

        Collider[] monsters = Physics.OverlapSphere(transform.position, detectionRange, monsterLayer);
        if (monsters.Length > 0)
        {
            shooting.TryShoot();
        }
    }
}