using UnityEngine;

public class MonsterController_KJH : MonoBehaviour
{
    // 몬스터의 이동 속도
    public float speed = 2.0f;

    // 몬스터가 좌우로 이동할 거리
    public float distance = 3.0f;

    // 몬스터의 시작 위치
    private Vector3 startPosition;

    // 몬스터의 이동 방향 (1: 오른쪽, -1: 왼쪽)
    private int direction = 1;

    // 스크립트가 처음 시작될 때 한 번 호출됩니다.
    void Start()
    {
        // 현재 위치를 시작 위치로 저장합니다.
        startPosition = transform.position;
    }

    // 매 프레임마다 호출됩니다.
    void Update()
    {
        // 몬스터를 좌우로 이동시킵니다.
        // Vector3.right는 (1, 0, 0) 방향을 의미합니다.
        // Time.deltaTime을 곱해 컴퓨터 성능과 상관없이 일정한 속도로 움직이게 합니다.
        transform.Translate(Vector3.right * speed * direction * Time.deltaTime);

        // 몬스터가 지정된 거리(distance) 이상으로 이동했는지 확인합니다.
        if (transform.position.x >= startPosition.x + distance)
        {
            // 오른쪽 최대 지점에 도달하면 방향을 왼쪽으로 바꿉니다.
            direction = -1;
        }
        else if (transform.position.x <= startPosition.x - distance)
        {
            // 왼쪽 최대 지점에 도달하면 방향을 오른쪽으로 바꿉니다.
            direction = 1;
        }
    }

    // 다른 오브젝트와 물리적으로 충돌했을 때 호출됩니다.
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트의 태그(Tag)가 "Player"인지 확인합니다.
        if (collision.gameObject.CompareTag("Player"))
        {
            // 충돌한 플레이어 오브젝트를 파괴(삭제)합니다.
            Destroy(collision.gameObject);
            Debug.Log("플레이어가 몬스터와 충돌하여 사망했습니다.");
        }
    }
}