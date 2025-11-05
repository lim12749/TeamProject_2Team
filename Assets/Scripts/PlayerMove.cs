using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instanse { get; private set; }
    public LayerMask groundLayer; // 클릭 가능한 바닥 레이어
    public LayerMask monsterLayer; // 몬스터 레이어 추가

    private CharacterController controller;
    public Animator animator;
    private CharacterStats stats;

    private Vector3 targetPosition;
    private bool isMoving = false;

    // 중력 관련 변수
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

        // 선택씬일 땐 RootMotion 끄기
        if (SceneManager.GetActiveScene().name == "CharacterSelectScene")
            animator.applyRootMotion = false;
        else
            animator.applyRootMotion = false; // CharacterController로 이동하니까 계속 false 유지
    }


    void Update()
    {
        DetectMonster();

        // 공격 중이면 이동 중단
        if (currentTarget != null && IsTargetInRange())
        {
            AttackTarget();
        }
        else
        {
            MoveToTarget();
        }

        HandleMouseInput();
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
                animator.SetBool("isMoving", true);
                currentTarget = null; // 수동 이동 시 자동 공격 해제
            }
        }
    }

    void MoveToTarget()
    {
        if (controller == null)
            Debug.LogError("❌ CharacterController가 없음!");
        if (stats == null)
            Debug.LogError("❌ CharacterStats가 없음!");

        // 중력 계산
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
                animator.SetBool("isMoving", false);
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

                // 💥 문제 위치
                Vector3 move = direction.normalized * stats.moveSpeed;
                controller.Move(move * Time.deltaTime);
            }
        }

        controller.Move(velocity * Time.deltaTime);
    }


    void DetectMonster()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, monsterLayer);

        if (hits.Length > 0)
        {
            // 가장 가까운 몬스터 찾기
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
        // 몬스터 바라보기
        Vector3 lookDir = currentTarget.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // 공격 애니메이션 실행
        if (Time.time - lastAttackTime > attackCooldown)
        {
            animator.SetTrigger("Fire"); // UpperBodyLayer에서 사용할 공격 트리거
            lastAttackTime = Time.time;
        }
    }

    void OnDrawGizmosSelected()
    {
        // 공격 범위 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
