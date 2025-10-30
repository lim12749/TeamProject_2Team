using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerLocomotion : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] InputReader input;         // 이동/우클릭 값
    [SerializeField] Transform cam;             // Main Camera Transform
    [SerializeField] WeaponHandler weaponHandler; // 무기 상태 소스

    [Header("Move")]
    [SerializeField] float walkSpeed = 2.4f;
    [SerializeField] float runSpeed  = 6f;
    [SerializeField] float aimMoveSpeed = 1.5f;
    [SerializeField] float turnSpeed = 720f;
    [SerializeField] bool faceMoveDir = true;

    [Header("Gravity")]
    [SerializeField] float gravity = -20f;
    [SerializeField] LayerMask groundMask = ~0;

    // 외부(애니 컨트롤러)가 읽는 값
    public bool HasWeapon { get; private set; }
    public float Speed01 { get; private set; }
    public bool IsAiming => input && input.AimHeld;

    CharacterController cc;
    bool grounded; float vY;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!cam && Camera.main) cam = Camera.main.transform;
        if (!weaponHandler) weaponHandler = GetComponentInChildren<WeaponHandler>(true);
        if (!input) input = GetComponentInParent<InputReader>();

        // 시작 시 무기 보유 상태 동기화
        if (weaponHandler) HasWeapon = weaponHandler.HasWeapon;
    }

    void OnEnable()
    {
        if (weaponHandler) weaponHandler.WeaponStateChanged += OnWeaponStateChanged;
    }
    void OnDisable()
    {
        if (weaponHandler) weaponHandler.WeaponStateChanged -= OnWeaponStateChanged;
    }
    void OnWeaponStateChanged(bool armed) => HasWeapon = armed;

    void Update()
    {
        GroundCheck();

        Vector3 moveDir = CalcMoveDir();
        float speed = CalcSpeed();

        // 이동 + 중력
        if (grounded && vY < 0f) vY = -2f;
        vY += gravity * Time.deltaTime;
        Vector3 delta = moveDir * speed * Time.deltaTime + Vector3.up * vY * Time.deltaTime;
        cc.Move(delta);

        // 회전: 에임일 때는 회전하지 않음 (AimAligner가 담당)
        if ((!IsAiming || !HasWeapon) && faceMoveDir && moveDir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(moveDir),
                turnSpeed * Time.deltaTime
            );
        }

        // Speed01 계산(0=idle, 0.5=walk, 1=run)
        float t01 = 0f;
        if (speed > 0f) t01 = Mathf.Approximately(speed, runSpeed) ? 1f : 0.5f;
        Speed01 = Mathf.MoveTowards(Speed01, t01, 8f * Time.deltaTime);
    }

    Vector3 CalcMoveDir()
    {
        Vector2 mv = input ? input.Move : Vector2.zero;

        if (IsAiming && HasWeapon)
        {
            // 조준 중: 플레이어 기준
            Vector3 f = transform.forward; f.y = 0; f.Normalize();
            Vector3 r = transform.right;   r.y = 0; r.Normalize();
            Vector3 d = f * mv.y + r * mv.x;
            if (d.sqrMagnitude > 1f) d.Normalize();
            return d;
        }
        else
        {
            // 일반: 카메라 기준
            Vector3 f = cam ? cam.forward : Vector3.forward; f.y = 0;
            if (f.sqrMagnitude < 0.001f) f = transform.forward;
            f.Normalize();
            Vector3 r = cam ? cam.right   : Vector3.right; r.y = 0;
            if (r.sqrMagnitude < 0.001f) r = transform.right;
            r.Normalize();

            Vector3 d = f * mv.y + r * mv.x;
            if (d.sqrMagnitude > 1f) d.Normalize();
            return d;
        }
    }

    float CalcSpeed()
    {
        if (!input || input.Move.sqrMagnitude <= 0.0001f) return 0f;
        if (IsAiming && HasWeapon) return aimMoveSpeed;
        return input.SprintHeld ? runSpeed : walkSpeed;
    }

    void GroundCheck()
    {
        Vector3 bottom = transform.position + cc.center
                         + Vector3.down * (cc.height * 0.5f - cc.radius + 0.02f);
        grounded = cc.isGrounded || Physics.CheckSphere(bottom, cc.radius * 0.95f, groundMask);
    }
}
