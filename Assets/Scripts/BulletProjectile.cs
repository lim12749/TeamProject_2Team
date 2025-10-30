// BulletProjectile.cs
using UnityEngine;

[DisallowMultipleComponent]
public class BulletProjectile : MonoBehaviour
{
    Vector3 dir;
    float speed, damage, life, alive;
    Transform ownerRoot;
    LayerMask stopMask;

    public void Init(Vector3 dir, float speed, float damage, float life, Transform ownerRoot, LayerMask stopMask)
    {
        this.dir = dir.normalized;
        this.speed = speed;
        this.damage = damage;
        this.life = life;
        this.ownerRoot = ownerRoot;
        this.stopMask = stopMask;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        alive += dt;
        if (alive >= life) { Destroy(gameObject); return; }

        float dist = speed * dt;
        // 이동 경로 상에 맞는 것이 있는지 검사
        if (Physics.Raycast(transform.position, dir, out var hit, dist + 0.05f, stopMask, QueryTriggerInteraction.Ignore))
        {
            // 자기 자신(플레이어) 무시
            if (ownerRoot && hit.transform.root == ownerRoot)
            {
                // 한 프레임 뒤로 넘겨서 다시 진행
                transform.position = hit.point + dir * 0.02f;
                return;
            }

            // === 몬스터만 데미지 ===
            // 방법 A: 컴포넌트 존재 체크(권장)
            var monster = hit.collider.GetComponentInParent<MonsterHealth>();
            if (monster) monster.TakeDamage(damage);

            // (원하면 방법 B: Tag == "Monster" 도 병행 가능)
            // if (hit.collider.CompareTag("Monster")) { ... }

            // 히트 시 총알 소멸
            Destroy(gameObject);
            return;
        }

        // 충돌 없으면 전진
        transform.position += dir * dist;
        transform.rotation = Quaternion.LookRotation(dir);
    }
}
