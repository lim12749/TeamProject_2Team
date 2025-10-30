using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputReader : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool AimHeld { get; private set; }
    public bool SprintHeld { get; private set; }

    // ★ 추가: 발사 상태(누르고 있는 동안 true)
    public bool FireHeld { get; private set; }
    public event Action InteractPressed;
    public event Action ReloadPressed;   // 필요시 사용
    public event Action FirePressed;     // 선택: 눌렀을 때 1회
    public event Action FireReleased;    // 선택: 뗐을 때 1회

    PlayerInput pi;
    InputAction move, look, aim, sprint, interact, reload, fire;

    void Awake()
    {
        pi = GetComponent<PlayerInput>();
        var a = pi.actions;
        move     = a.FindAction("Move",     true);
        look     = a.FindAction("Look",     true);
        aim      = a.FindAction("Aim",      true);
        sprint   = a.FindAction("Sprint",   true);
        interact = a.FindAction("Interact", true);
        //reload   = a.FindAction("Reload",   false);
        fire     = a.FindAction("Fire",     true);  // ★ 추가 (Button)
    }

    void OnEnable()
    {
        move.performed   += OnMovePerformed;
        move.canceled    += OnMoveCanceled;

        look.performed   += OnLookPerformed;
        look.canceled    += OnLookCanceled;

        aim.started      += OnAimStarted;
        aim.canceled     += OnAimCanceled;

        sprint.started   += OnSprintStarted;
        sprint.canceled  += OnSprintCanceled;

        interact.performed += OnInteractPerformed;
        //if (reload != null) reload.performed += OnReloadPerformed;

        if (fire != null)
        {
            // 참고: Press 인터랙션(Press And Release)일 때에만 release 콜백이 보장됨.
            // 아래 Update 폴링이 있으니 콜백 누락돼도 FireHeld는 정확함.
            fire.started  += OnFireStarted;
            fire.canceled += OnFireCanceled;
        }
    }

    void OnDisable()
    {
        move.performed   -= OnMovePerformed;
        move.canceled    -= OnMoveCanceled;

        look.performed   -= OnLookPerformed;
        look.canceled    -= OnLookCanceled;

        aim.started      -= OnAimStarted;
        aim.canceled     -= OnAimCanceled;

        sprint.started   -= OnSprintStarted;
        sprint.canceled  -= OnSprintCanceled;

        interact.performed -= OnInteractPerformed;
        //if (reload != null) reload.performed -= OnReloadPerformed;

        if (fire != null)
        {
            fire.started  -= OnFireStarted;
            fire.canceled -= OnFireCanceled;
        }
    }

    void Update()
    {
        // ★ 핵심: 매 프레임 폴링으로 눌림/뗌 상태를 확실히 반영
        if (fire != null)
        {
            bool pressedNow = fire.IsPressed(); // InputSystem 확장 메서드
            if (pressedNow != FireHeld)
            {
                FireHeld = pressedNow;
                if (pressedNow) FirePressed?.Invoke();
                else            FireReleased?.Invoke();
            }
        }
    }

    // --- Handlers ---
    void OnMovePerformed(InputAction.CallbackContext ctx) => Move = ctx.ReadValue<Vector2>();
    void OnMoveCanceled (InputAction.CallbackContext ctx) => Move = Vector2.zero;

    void OnLookPerformed(InputAction.CallbackContext ctx) => Look = ctx.ReadValue<Vector2>();
    void OnLookCanceled (InputAction.CallbackContext ctx) => Look = Vector2.zero;

    void OnAimStarted (InputAction.CallbackContext ctx) => AimHeld = true;
    void OnAimCanceled(InputAction.CallbackContext ctx) => AimHeld = false;

    void OnSprintStarted (InputAction.CallbackContext ctx) => SprintHeld = true;
    void OnSprintCanceled(InputAction.CallbackContext ctx) => SprintHeld = false;

    void OnInteractPerformed(InputAction.CallbackContext ctx) => InteractPressed?.Invoke();
    //void OnReloadPerformed  (InputAction.CallbackContext ctx) => ReloadPressed?.Invoke();

    void OnFireStarted (InputAction.CallbackContext ctx) { /* 이벤트 필요 시 사용 */ }
    void OnFireCanceled(InputAction.CallbackContext ctx) { /* 이벤트 필요 시 사용 */ }
}
