using UnityEngine;

// 스크립트 파일 이름과 클래스 이름이 "Monster"로 동일해야 합니다.
public class Monster : MonoBehaviour
{
    // 1. 몬스터의 이동 속도 (인스펙터에서 조절 가능)
    public float moveSpeed = 3.0f;

    // 2. 추적할 대상 (플레이어)의 Transform
    private Transform playerTransform;

    void Start()
    {
        // 3. "Player" 태그를 가진 게임 오브젝트를 찾아 Transform 컴포넌트를 저장
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없습니다. 'Player' 태그를 확인하세요.");
            // 플레이어를 못찾으면 스크립트 비활성화
            this.enabled = false;
        }
    }

    void Update()
    {
        // 플레이어 Transform이 할당되었는지 확인
        if (playerTransform == null) return;

        // 4. 몬스터가 플레이어를 바라보게 만듦 (즉시 회전)
        transform.LookAt(playerTransform);

        // 5. 몬스터의 앞 방향으로 초당 moveSpeed만큼 이동
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}