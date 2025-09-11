using UnityEngine;

public class MonsterController_KJH : MonoBehaviour
{
    // ������ �̵� �ӵ�
    public float speed = 2.0f;

    // ���Ͱ� �¿�� �̵��� �Ÿ�
    public float distance = 3.0f;

    // ������ ���� ��ġ
    private Vector3 startPosition;

    // ������ �̵� ���� (1: ������, -1: ����)
    private int direction = 1;

    // ��ũ��Ʈ�� ó�� ���۵� �� �� �� ȣ��˴ϴ�.
    void Start()
    {
        // ���� ��ġ�� ���� ��ġ�� �����մϴ�.
        startPosition = transform.position;
    }

    // �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        // ���͸� �¿�� �̵���ŵ�ϴ�.
        // Vector3.right�� (1, 0, 0) ������ �ǹ��մϴ�.
        // Time.deltaTime�� ���� ��ǻ�� ���ɰ� ������� ������ �ӵ��� �����̰� �մϴ�.
        transform.Translate(Vector3.right * speed * direction * Time.deltaTime);

        // ���Ͱ� ������ �Ÿ�(distance) �̻����� �̵��ߴ��� Ȯ���մϴ�.
        if (transform.position.x >= startPosition.x + distance)
        {
            // ������ �ִ� ������ �����ϸ� ������ �������� �ٲߴϴ�.
            direction = -1;
        }
        else if (transform.position.x <= startPosition.x - distance)
        {
            // ���� �ִ� ������ �����ϸ� ������ ���������� �ٲߴϴ�.
            direction = 1;
        }
    }

    // �ٸ� ������Ʈ�� ���������� �浹���� �� ȣ��˴ϴ�.
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� �±�(Tag)�� "Player"���� Ȯ���մϴ�.
        if (collision.gameObject.CompareTag("Player"))
        {
            // �浹�� �÷��̾� ������Ʈ�� �ı�(����)�մϴ�.
            Destroy(collision.gameObject);
            Debug.Log("�÷��̾ ���Ϳ� �浹�Ͽ� ����߽��ϴ�.");
        }
    }
}