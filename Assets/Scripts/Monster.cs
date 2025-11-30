using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Monster Settings")]
    public float moveSpeed = 3.0f;

    // 1. 몬스터가 줄 경험치 양
    public int expReward = 20;

    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            // 플레이어가 없으면 에러 로그가 너무 많이 뜨지 않게 스크립트만 끔
            this.enabled = false;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 플레이어 따라가기
        transform.LookAt(playerTransform);
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    // 2. 테스트용: 몬스터를 마우스로 클릭하면 죽음 (공격 기능이 아직 없으므로)
    private void OnMouseDown()
    {
        Die();
    }

    // 3. 몬스터 사망 처리 함수
    public void Die()
    {
        // 플레이어 오브젝트가 존재하는지 확인
        if (playerTransform != null)
        {
            // 플레이어 오브젝트에서 PlayerExperience 컴포넌트를 가져옴
            PlayerExperience playerExp = playerTransform.GetComponent<PlayerExperience>();

            // 컴포넌트가 있다면 경험치 추가 함수 실행
            if (playerExp != null)
            {
                playerExp.AddExperience(expReward);
                Debug.Log($"경험치 {expReward} 획득!");
            }
        }

        // 몬스터 오브젝트 삭제
        Destroy(gameObject);
    }
}