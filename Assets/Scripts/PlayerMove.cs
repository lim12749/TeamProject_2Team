using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public LayerMask groundLayer;   // 클릭 가능한 바닥 레이어
    public LayerMask monsterLayer;  // 몬스터 레이어 지정
    public float detectionRange = 10f; // 몬스터 감지 거리
    public float rotationSpeed = 10f;  // 회전 속도

    private CharacterController controller;
    private Animator animator;
    private CharacterStats stats;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private Vector3 velocity;
    private float gravity = -9.81f;
    private float groundCheckOffset = 0.2f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        stats = GetComponent<CharacterStats>();

        targetPosition = transform.position;
    }

    void Update()
    {
        HandleMouseInput();
        LookAtNearestMonster();
        MoveToTarget();
    }

    // 마우스 우클릭으로 이동 목표 지정
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1)) // 오른쪽 클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                targetPosition = hit.point;
                isMoving = true;
                animator.SetBool("isMoving", true);
            }
        }
    }

    // 10f 거리 내 가장 가까운 몬스터를 바라봄
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

    // 마우스 클릭한 목표 지점으로 이동
    void MoveToTarget()
    {
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
                // 도착
                isMoving = false;
                animator.SetBool("isMoving", false);
            }
            else
            {
                // 이동 방향으로 회전 (몬스터 바라보는 것과 별개)
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

                // 이동 처리
                Vector3 move = direction.normalized * stats.moveSpeed;
                controller.Move(move * Time.deltaTime);
            }
        }

        controller.Move(velocity * Time.deltaTime);
    }
}
