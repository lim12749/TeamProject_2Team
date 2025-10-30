using System;
using UnityEngine;

[Flags]
public enum ActionFlag
{
    None     = 0,
    Run      = 1 << 0, // 달리는 중
    Jump     = 1 << 1, // 점프 키 누름
    Attack   = 1 << 2, // 공격 중
}

public class PlayerActionFlags : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpPower = 6f;
    Rigidbody rb;
    ActionFlag flags = ActionFlag.None;
    bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
    }

    void Update()
    {
        // 이동 입력 → Run 비트 ON/OFF
        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (dir.sqrMagnitude > 0) flags |= ActionFlag.Run;
        else                      flags &= ~ActionFlag.Run;

        // 점프 입력 → Jump 비트 토글 느낌: 누를 때 ON, 땅에 닿으면 OFF
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            flags |= ActionFlag.Jump;
            rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }

        // 공격 입력 → Attack 비트 잠깐 ON
        if (Input.GetMouseButtonDown(0)) flags |= ActionFlag.Attack;

        // 이동 처리 (Run이 켜져 있으면 이동)
        if ((flags & ActionFlag.Run) != 0)
            rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);

        // Attack은 0.2초 뒤 자동 OFF (간단 타이머)
        if ((flags & ActionFlag.Attack) != 0)
            Invoke(nameof(EndAttack), 0.2f);

        // 땅에 닿으면 Jump 비트 OFF
        if (grounded && (flags & ActionFlag.Jump) != 0)
            flags &= ~ActionFlag.Jump;

        // 디버그 보기(2진수)
        if (Input.anyKeyDown)
        {
            string bin = Convert.ToString((int)flags, 2).PadLeft(4,'0');
            Debug.Log($"Flags: {flags}  bin={bin}");
        }
    }

    void EndAttack() => flags &= ~ActionFlag.Attack;

    void OnCollisionStay(Collision c) { grounded = true; }
    void OnCollisionExit(Collision c) { grounded = false; }
}
