using UnityEngine;

public class UnifiedMove : MonoBehaviour
{
    public enum MoveType
    {
        Translate,
        Rigidbody,
        CharacterController
    }

    [Header("이동 방식 선택")]
    public MoveType moveType = MoveType.Translate;

    [Header("공통 이동 속도")]
    public float moveSpeed = 5f;

    // 🔶 필요한 컴포넌트 참조
    private Rigidbody rb;
    private CharacterController controller;

    void Start()
    {
        if (moveType == MoveType.Rigidbody)
            rb = GetComponent<Rigidbody>();

        if (moveType == MoveType.CharacterController)
            controller = GetComponent<CharacterController>();
    }

     void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0, v);

        switch (moveType)
        {
            case MoveType.Translate:
                transform.Translate(input * moveSpeed * Time.deltaTime);
                break;

            case MoveType.CharacterController:
                if (controller != null)
                    controller.SimpleMove(input * moveSpeed);
                break;
        }
    }

    void FixedUpdate()
    {
        if (moveType == MoveType.Rigidbody && rb != null)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 input = new Vector3(h, 0, v);
            rb.linearVelocity = input * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
        }
    }
}
