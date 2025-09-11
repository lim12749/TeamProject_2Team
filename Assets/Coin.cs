using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool isPlayerNearby = false;

    // 플레이어가 코인의 Trigger Collider에 진입하면
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("플레이어가 코인 근처에 있습니다. 'E' 키를 눌러 획득하세요.");
        }
    }

    // 플레이어가 코인 영역을 벗어나면
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    // 매 프레임 업데이트
    private void Update()
    {
        // 플레이어가 근처에 있고, 'E' 키가 눌렸을 때
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // 플레이어 컨트롤러를 찾아 AddCoin 함수 호출
            LSH playerController = FindFirstObjectByType<LSH>();
            if (playerController != null)
            {
                playerController.AddCoin();
            }

            // 코인 오브젝트 제거
            Destroy(gameObject);
        }
    }
}