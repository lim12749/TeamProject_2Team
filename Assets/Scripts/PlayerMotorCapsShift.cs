using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerMotorCapsShift : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform cam;
    [SerializeField] Animator animator;
    [SerializeField] Transform yawTarget;

    [Header("Animation")]
    [SerializeField] private string isAimingParam = "IsAiming";
    //[SerializeField] private string upperBodyAimLayer = "UpperBody_Aim";
    [SerializeField] private float aimTransitionSpeed = 5f;

    [Header("Move")]
    [SerializeField] float walkSpeed = 2.4f;
    [SerializeField] float runSpeed = 6.0f;
    [SerializeField] float aimMoveSpeed = 1.5f;
    [SerializeField] float turnSpeed = 720f;
    [SerializeField] bool shouldFaceMoveDirection = true;

    [Header("Gravity/Ground")]
    [SerializeField] float gravity = -20f;
    [SerializeField] LayerMask groundMask = ~0;

    CharacterController cc;
    PlayerInput playerInput;
    InputAction sprintAction;

    Vector2 moveInput;
    public bool IsAiming;            // CameraSwitcher가 실시간 갱신
    public bool HasWeapon = false;   // WeaponHandler가 갱신
    bool sprintHeld;
    bool grounded;
    float vY, speed01;

    // anim
    int isAimingParamHash;
    int upperBodyAimLayerIndex;
    float currentAimWeight;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();

        playerInput = GetComponent<PlayerInput>();                  // ★ PlayerInput에서 액션 가져오기
        sprintAction = playerInput.actions.FindAction("Sprint", true);

        isAimingParamHash = Animator.StringToHash(isAimingParam);
        //upperBodyAimLayerIndex = animator ? animator.GetLayerIndex(upperBodyAimLayer) : -1;

        if (upperBodyAimLayerIndex >= 0) animator.SetLayerWeight(upperBodyAimLayerIndex, 0f);

        // 예: Awake/Start/임시 버튼 등 원하는 곳에
        if (animator == null) { Debug.Log("Animator == null"); }
        else
        {
            bool exists = false;
            foreach (var p in animator.parameters)
                if (p.name == "HasWeapon") { exists = true; break; }

            Debug.Log($"Has param 'HasWeapon'? {exists}");
        }
        DebugAnimatorLayers();
    }


    void Update()
    {
        sprintHeld = sprintAction != null && sprintAction.IsPressed();   // ★ 폴링

        GroundCheck();

        Vector3 moveDirection = CalculateMoveDirection();
        float targetSpeed = CalculateTargetSpeed();

        ApplyMovement(moveDirection, targetSpeed);
        HandleRotation(moveDirection);
        UpdateAnimator(targetSpeed);
        UpdateAimingAnimation(); //
    }

    Vector3 CalculateMoveDirection()
    {
        Vector3 moveDirection;

        if (IsAiming && HasWeapon)    // ★ 에임 이동은 무기 보유 시에만
        {
            Vector3 f = transform.forward; f.y = 0; f.Normalize();
            Vector3 r = transform.right; r.y = 0; r.Normalize();
            moveDirection = f * moveInput.y + r * moveInput.x;
        }
        else
        {
            Vector3 f = cam ? cam.forward : Vector3.forward; f.y = 0; if (f.sqrMagnitude < 0.001f) f = transform.forward; f.Normalize();
            Vector3 r = cam ? cam.right : Vector3.right; r.y = 0; if (r.sqrMagnitude < 0.001f) r = transform.right; r.Normalize();
            moveDirection = f * moveInput.y + r * moveInput.x;
        }

        if (moveDirection.sqrMagnitude > 1f) moveDirection.Normalize();
        return moveDirection;
    }

    float CalculateTargetSpeed()
    {
        if (moveInput.sqrMagnitude <= 0.0001f) return 0f;
        if (IsAiming && HasWeapon) return aimMoveSpeed;          // ★ 게이팅
        return sprintHeld ? runSpeed : walkSpeed;
    }

    void ApplyMovement(Vector3 moveDirection, float targetSpeed)
    {
        if (grounded && vY < 0f) vY = -2f;
        vY += gravity * Time.deltaTime;

        Vector3 delta = moveDirection * targetSpeed * Time.deltaTime + Vector3.up * vY * Time.deltaTime;
        cc.Move(delta);
    }

    void HandleRotation(Vector3 moveDirection)
    {
        if (IsAiming && HasWeapon)    // ★ 게이팅
        {
            if (yawTarget)
            {
                Vector3 look = yawTarget.forward; look.y = 0;
                if (look.sqrMagnitude > 0.01f)
                {
                    Quaternion target = Quaternion.LookRotation(look);
                    transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 10f);
                }
            }
        }
        else if (shouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
        }
    }

    void UpdateAnimator(float targetSpeed)
    {
        if (!animator) return;

        float t01 = 0f;
        if (targetSpeed > 0f) t01 = Mathf.Approximately(targetSpeed, runSpeed) ? 1f : 0.5f;
        speed01 = Mathf.MoveTowards(speed01, t01, 8f * Time.deltaTime);
        animator.SetFloat("Speed01", speed01);
    }

    void UpdateAimingAnimation()
    {
        if (!animator) return;

        bool aimingActive = IsAiming && HasWeapon;     // ★ 핵심: 무기 보유 시에만 에임 애니
        animator.SetBool(isAimingParamHash, aimingActive);

        if (upperBodyAimLayerIndex >= 0)
        {
            float targetWeight = aimingActive ? 1f : 0f;
            currentAimWeight = Mathf.MoveTowards(currentAimWeight, targetWeight, aimTransitionSpeed * Time.deltaTime);
            animator.SetLayerWeight(upperBodyAimLayerIndex, currentAimWeight);
        }
    }

    // PlayerInput (Send Messages)
    void OnMove(InputValue v) { moveInput = v.Get<Vector2>(); }
    // Sprint는 폴링으로 처리하므로 이 메서드는 더이상 필요 없음(있어도 무해)
    // void OnSprint(InputValue v) => sprintHeld = v.isPressed;

    void GroundCheck()
    {
        Vector3 bottom = transform.position + (cc ? cc.center : Vector3.zero)
                         + Vector3.down * ((cc ? cc.height : 2f) * 0.5f - (cc ? cc.radius : 0.25f) + 0.02f);
        float rad = (cc ? cc.radius : 0.25f) * 0.95f;
        grounded = (cc ? cc.isGrounded : false) || Physics.CheckSphere(bottom, rad, groundMask, QueryTriggerInteraction.Ignore);
    }
// Animator 레이어 전부 덤프 + 특정 레이어 존재/인덱스 확인
    void DebugAnimatorLayers()
    {
        if (!animator) { Debug.Log("Animator == null"); return; }

        Debug.Log($"[Layers] Controller='{animator.runtimeAnimatorController?.name}', count={animator.layerCount}");
        for (int i = 0; i < animator.layerCount; i++)
        {
            string name = animator.GetLayerName(i);
            float w = animator.GetLayerWeight(i);
            Debug.Log($"  [{i}] {name}  (weight={w:0.00})");
        }

        // 특정 레이어 확인 (예: UpperBody_Aim)
        string target = "UpperBody_Aim";
        int idx = animator.GetLayerIndex(target);
        if (idx >= 0)
            Debug.Log($"FOUND layer '{target}' at index {idx}, weight={animator.GetLayerWeight(idx):0.00}");
        else
            Debug.Log($"NOT FOUND layer '{target}'");
    }

}


