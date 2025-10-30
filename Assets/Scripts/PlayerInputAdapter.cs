using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputAdapter : MonoBehaviour
{
    [SerializeField] InputActionProperty move;       // WASD
    [SerializeField] InputActionProperty toggleWalk; // CapsLock (토글)
    [SerializeField] InputActionProperty sprint;     // LeftShift (홀드)

    public Vector2 Move => move.action?.ReadValue<Vector2>() ?? Vector2.zero;

    public bool SprintHeld => sprint.action != null && sprint.action.IsPressed();

    public bool IsWalkMode { get; private set; } // CapsLock 토글 상태

    void OnEnable(){ move.action?.Enable(); toggleWalk.action?.Enable(); sprint.action?.Enable(); }
    void OnDisable(){ move.action?.Disable(); toggleWalk.action?.Disable(); sprint.action?.Disable(); }

    void Update()
    {
        if (toggleWalk.action != null && toggleWalk.action.WasPressedThisFrame())
            IsWalkMode = !IsWalkMode; // CapsLock 누를 때마다 걷기 모드 토글
    }
}
