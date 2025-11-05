using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    public LayerMask groundLayer; // 클릭 가능한 바닥 레이어
    private CharacterController controller;
    private Animator animator;
    private CharacterStats stats;

    private Vector3 targetPosition;
    private bool isMoving = false;

    // 중력 관련 변수
    private Vector3 velocity;          // 현재 중력 속도
    private float gravity = -9.81f;    // 중력 값 (Unity 기본값)
    private float groundCheckOffset = 0.2f; // 바닥 근처 오차 범위

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
        MoveToTarget();
    }

    void HandleMouseInput()
    {
        // 마우스 오른쪽 클릭
        if (Input.GetMouseButtonDown(1))
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

    void MoveToTarget()
    {
        // 중력 먼저 계산
        if (controller.isGrounded)
        {
            // 땅에 닿으면 y 속도 리셋
            velocity.y = -groundCheckOffset;
        }
        else
        {
            // 중력 적용
            velocity.y += gravity * Time.deltaTime;
        }

        // 이동 처리
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
                // 회전
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

                // 이동
                Vector3 move = direction.normalized * stats.moveSpeed;
                controller.Move(move * Time.deltaTime);
            }
        }

        // 중력 적용 (항상 마지막에 Move)
        controller.Move(velocity * Time.deltaTime);
    }
}
