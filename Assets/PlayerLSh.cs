using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 5f; // �̵� �ӵ� ���� �߰�

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate�� ������ ���� �����Ӹ��� ȣ��˴ϴ�.
    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(x, 0, z);
        rb.linearVelocity = moveDirection * speed; // �ӵ� ������ �����ݴϴ�.
    }
}