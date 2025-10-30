// Scripts/Player/ThirdPersonMotor.cs
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMotor : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform cam;          // Main Camera (Cinemachine Brain가 구동)
    [SerializeField] Transform yawTarget;    // 조준 시 바라볼 기준(에임 피벗/카메라 리그의 yaw)
    [SerializeField] Animator animator;      // 선택

    [Header("Move")]
    [SerializeField] float walkSpeed = 5f;      // 속도 증가
    [SerializeField] float runSpeed  = 10f;     // 속도 증가
    [SerializeField] float aimMoveSpeed = 3f;   // 속도 증가
    [SerializeField] float turnSpeed = 720f;        // deg/sec
    [SerializeField] bool  shouldFaceMoveDirection = true;

    [Header("Jump / Gravity / Ground")]
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = -20f;
    [SerializeField] LayerMask groundMask = ~0;

    CharacterController cc;
    Vector2 moveInput;
    bool sprintHeld;
    public bool isAiming;

    float vY;
    bool grounded;
    float speed01;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!cam && Camera.main) cam = Camera.main.transform;
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!cam && Camera.main) cam = Camera.main.transform;

        // 임시: 키보드 입력으로 테스트 (OnMove가 안 될 때)
        if (moveInput.magnitude < 0.1f)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            moveInput = new Vector2(h, v);
            if (moveInput.magnitude > 0.1f)
            {
                Debug.Log($"키보드 입력 감지: {moveInput}");
            }
        }

        GroundCheck();

        // 카메라 기준 평면 방향(카메라가 수직이면 자기 forward로 폴백)
        Vector3 f = cam ? cam.forward : Vector3.forward;
        f.y = 0;
        if (f.sqrMagnitude < 0.001f) f = transform.forward;
        f.Normalize();
        
        Vector3 r = cam ? cam.right : Vector3.right;
        r.y = 0;
        if (r.sqrMagnitude < 0.001f) r = transform.right;
        r.Normalize();

        Vector3 dir = f * moveInput.y + r * moveInput.x;
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        float targetSpeed = 0f;
        if (moveInput.sqrMagnitude > 0.0001f)
            targetSpeed = isAiming ? aimMoveSpeed : (sprintHeld ? runSpeed : walkSpeed);

        if (grounded && vY < 0f) vY = -2f; // 지면 붙이기
        vY += gravity * Time.deltaTime;

        Vector3 delta = dir * targetSpeed * Time.deltaTime + Vector3.up * vY * Time.deltaTime;
        cc.Move(delta);

        // 방향전환: 이동 중일 때만 이동 방향으로 회전
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
        }

        // 애니메이터(선택)
        if (animator)
        {
            float t01 = 0f;
            if (targetSpeed > 0f) t01 = Mathf.Approximately(targetSpeed, runSpeed) ? 1f : 0.5f;
            speed01 = Mathf.MoveTowards(speed01, t01, 8f * Time.deltaTime);
            animator.SetFloat("Speed01", speed01);
        }
    }

    void GroundCheck()
    {
        // 캡슐 바닥 구체 체크(컨트롤러 규격 기반)
        Vector3 bottom = transform.position + cc.center
                         + Vector3.down * (cc.height * 0.5f - cc.radius + 0.02f);
        float rad = cc.radius * 0.95f;
        grounded = cc.isGrounded || Physics.CheckSphere(bottom, rad, groundMask, QueryTriggerInteraction.Ignore);
    }

    // ===== Input Callbacks =====
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        Debug.Log($"OnMove 호출됨: {moveInput}, ctx.phase: {ctx.phase}");
    }
    public void OnSprint(InputAction.CallbackContext ctx)  { if (ctx.performed) sprintHeld = true; else if (ctx.canceled) sprintHeld = false; }
    public void OnAim(InputAction.CallbackContext ctx)     { if (ctx.performed) isAiming = true;  else if (ctx.canceled) isAiming = false; }
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (grounded || cc.isGrounded)
            vY = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!cc) cc = GetComponent<CharacterController>();
        Vector3 bottom = transform.position + cc.center
                         + Vector3.down * (cc.height * 0.5f - cc.radius + 0.02f);
        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(bottom, cc.radius * 0.95f);
    }
#endif
}
