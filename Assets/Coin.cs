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
            // �÷��̾� ��Ʈ�ѷ��� ã�� AddCoin �Լ� ȣ��
            LSH playerController = FindFirstObjectByType<LSH>();
            if (playerController != null)
            {
                playerController.AddCoin();
            }

            // ���� ������Ʈ ����
            Destroy(gameObject);
        }
    }
}