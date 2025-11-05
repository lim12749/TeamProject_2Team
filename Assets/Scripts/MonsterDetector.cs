using UnityEngine;

public class MonsterDetector : MonoBehaviour
{
    public static MonsterDetector instance { get; private set; }

    [Header("탐지 반경 (기본값 10)")]
    public float detectionRadius = 10f;

    [Header("탐지할 몬스터 레이어")]
    public LayerMask monsterLayer; // Unity Inspector에서 Monster 레이어 선택

    [Header("Fire 트리거를 가진 Animator")]
    public Animator animator;

    [Header("회전 속도")]
    public float rotationSpeed = 5f;

    private Transform targetMonster; // 가장 가까운 몬스터 저장

    void Update()
    {
        if (animator == null)
            return;

        DetectMonsters();

        // 몬스터가 있으면 바라보게 회전
        if (targetMonster != null)
        {
            LookAtMonster();
        }
    }

    void DetectMonsters()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, monsterLayer);

        if (colliders == null || colliders.Length == 0)
        {
            targetMonster = null;
            return;
        }

        // 가장 가까운 몬스터 찾기
        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider c in colliders)
        {
            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = c.transform;
            }
        }

        targetMonster = closest;

        // Fire 트리거 발동
        if (targetMonster != null)
        {
            TriggerFire();
        }
    }

    void LookAtMonster()
    {
        // 방향 계산 (수평만)
        Vector3 dir = targetMonster.position - transform.position;
        dir.y = 0;

        if (dir == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    void TriggerFire()
    {
        if (animator != null)
        {
            animator.SetTrigger("Fire");
            Gun.instance.TryFire();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
