using UnityEngine;

/// <summary>
/// 총알, 몬스터 간 전투를 관리하는 시스템 (Bullet + Monster 연동)
/// </summary>
public class MonsterCombatSystem : MonoBehaviour
{
    [Header("총알 프리팹 (Bullet.cs 포함)")]
    public GameObject bulletPrefab;

    [Header("총알 발사 위치 (총구 Transform)")]
    public Transform firePoint;

    [Header("발사 간격 (초)")]
    public float fireRate = 0.5f;

    [Header("탐지 반경 (몬스터 탐색 범위)")]
    public float detectionRadius = 15f;

    [Header("탐색할 레이어 (Monster)")]
    public LayerMask monsterLayer;

    private float lastFireTime = 0f;
    private Transform targetMonster;

    void Update()
    {
        // 가까운 몬스터 탐색
        FindNearestMonster();

        // 목표가 있을 때 발사
        if (targetMonster != null)
        {
            // 몬스터 바라보기
            LookAtTarget(targetMonster);

            // 일정 시간마다 총알 발사
            if (Time.time - lastFireTime >= fireRate)
            {
                Shoot();
                lastFireTime = Time.time;
            }
        }
    }

    /// <summary>
    /// 가장 가까운 몬스터 탐색
    /// </summary>
    void FindNearestMonster()
    {
        Collider[] monsters = Physics.OverlapSphere(transform.position, detectionRadius, monsterLayer);

        if (monsters.Length == 0)
        {
            targetMonster = null;
            return;
        }

        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider m in monsters)
        {
            float dist = Vector3.Distance(transform.position, m.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = m.transform;
            }
        }

        targetMonster = closest;
    }

    /// <summary>
    /// 몬스터 방향으로 회전
    /// </summary>
    void LookAtTarget(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }

    /// <summary>
    /// 총알 발사
    /// </summary>
    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("⚠️ BulletPrefab 또는 FirePoint가 설정되지 않았습니다!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Debug.Log($"🔫 총알 발사 → 목표: {targetMonster?.name ?? "없음"}");
    }

    /// <summary>
    /// 디버그용 Gizmo (탐지 범위)
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
