using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool isPlayerNearby = false;

    // �÷��̾ ������ Trigger Collider�� �����ϸ�
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("�÷��̾ ���� ��ó�� �ֽ��ϴ�. 'E' Ű�� ���� ȹ���ϼ���.");
        }
    }

    // �÷��̾ ���� ������ �����
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    // �� ������ ������Ʈ
    private void Update()
    {
        // �÷��̾ ��ó�� �ְ�, 'E' Ű�� ������ ��
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // GameManager�� ���� �ν��Ͻ��� ���� AddCoin �Լ� ȣ��
            if (LSH.instance != null)
            {
                LSH.instance.AddCoin();
            }

            // ���� ������Ʈ ����
            Destroy(gameObject);
        }
    }
}