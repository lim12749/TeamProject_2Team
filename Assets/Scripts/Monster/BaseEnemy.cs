using UnityEngine;
using UnityEngine.AI;
public abstract class BaseEnemy : MonoBehaviour 
{
    [Header("순찰")]
    public Transform[] patrolPoints; // 🟠 순찰 지점 배열
    private int patrolIndex = 0;     // 현재 순찰 지점
    public float patrolWaitTime = 2f;
    private float patrolWaitTimer = 0f;
    [Header("시야 설정")]
    public float viewDistance = 10f;       // 감지 거리
    public float viewAngle = 90f;          // 감지 각도 (부채꼴 각도)
    public int rayCount = 15;              // 발사할 레이 개수
    public LayerMask targetMask;           // 플레이어 감지용 레이어
    public LayerMask obstacleMask;         // 장애물 레이어
    [Header("공통 스탯")]
    public int maxHP = 100;
    protected int currentHP;
    public float attackCooldown = 1f;
    protected float lastAttackTime;
    public GameObject expOrbPrefab;

    [Header("AI 컴포넌트")]
    protected NavMeshAgent agent;
    public Transform target;

    [Header("공격 범위 설정")]
    public float minAttackRange = 2f;  // 🟠 공격 최소 거리
    private Animator anim;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        currentHP = maxHP;
    }
    protected virtual void Update()
    {
        if (target != null)
        {
            TrackAndAttack(); // 추적 및 공격 처리
        }
        else
        {
            ScanForTarget();  // 부채꼴 감지
            Patrol();         // 타겟 없으면 패트롤
        }

        UpdateAnimation(); // ✅ 애니메이션 처리
    }

    protected virtual void UpdateAnimation()
    {
        if (anim != null && agent != null)
        {
            //Debug.Log("체크");
            anim.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    protected virtual void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        // 목표 지점 도달 확인
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            patrolWaitTimer += Time.deltaTime;

            if (patrolWaitTimer >= patrolWaitTime)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[patrolIndex].position);
                patrolWaitTimer = 0f;
            }
        }
        else
        {
            // 처음 목적지 설정
            if (agent.destination != patrolPoints[patrolIndex].position)
            {
                agent.SetDestination(patrolPoints[patrolIndex].position);
            }
        }
    }
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"{gameObject.name}이 피해를 입음: {amount}");

        if (currentHP <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} 사망");
        //애니메이션 실행'
        
        // 경험치 구슬 생성
        if (expOrbPrefab != null)
        {
            GameObject orb = Instantiate(expOrbPrefab, transform.position + Vector3.up, Quaternion.identity);

            Rigidbody rb = orb.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 force = Vector3.up * 4f + Random.insideUnitSphere * 1.5f;
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
        Destroy(gameObject);
    }

    // 타겟 설정
    public virtual void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    // 자식 클래스가 구현할 공격 함수
    protected abstract void Attack();

    protected virtual void ScanForTarget()
    {
        float halfAngle = viewAngle * 0.5f;

        for (int i = 0; i < rayCount; i++)
        {
            float t = i / (float)(rayCount - 1);
            float angle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            if (Physics.Raycast(transform.position + Vector3.up, dir, out RaycastHit hit, viewDistance, targetMask))
            {
                // 플레이어 감지
                SetTarget(hit.transform);
                break;
            }

            // 디버그 시각화 (에디터에서만 보임)
            Debug.DrawRay(transform.position + Vector3.up, dir * viewDistance, Color.yellow);
        }
    }
    protected abstract void TrackAndAttack();
}
