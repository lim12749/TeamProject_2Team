using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;      // 총알 속도
    public float lifetime = 3f;    // 몇 초 뒤에 자동 삭제
    public float damage = 10f;     // 공격력

    private void Start()
    {
        Destroy(gameObject, lifetime); // 일정 시간 후 자동 파괴
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // "Monster" 레이어 번호 가져오기
        int monsterLayer = LayerMask.NameToLayer("Monster");

        // 충돌한 오브젝트가 몬스터 레이어에 속하면 실행
        if (other.gameObject.layer == monsterLayer)
        {
            Debug.Log($"{other.name} 피격! 데미지: {damage}");

            // 몬스터 스크립트가 있다면 데미지 주기
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
            }

            // 총알 제거
            Destroy(gameObject);
        }
    }

}
