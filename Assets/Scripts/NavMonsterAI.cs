// NavMonsterAI.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMonsterAI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform target;
    [SerializeField] PlayerHealth targetHealth;
    [SerializeField] MonsterHealth myHealth;
    [SerializeField] Animator animator;

    [Header("Detect/Combat")]
    [SerializeField] float detectRadius = 14f;
    [SerializeField] float attackRange = 1.8f;
    [SerializeField] float attackCooldown = 1.2f;
    [SerializeField] float contactDamage = 10f;
    [SerializeField] LayerMask losMask = ~0;
    [SerializeField] bool requireLineOfSight = true;

    [Header("Move")]
    [SerializeField] float chaseSpeed = 3.5f;

    NavMeshAgent agent;
    float cooldown;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!myHealth) myHealth = GetComponent<MonsterHealth>();
        if (!target)
        {
            var t = GameObject.FindWithTag("Player");
                target = t.transform;
        }
        
        if (!targetHealth && target)
            targetHealth = target.GetComponentInParent<PlayerHealth>();
        if (!animator)
            animator = GetComponentInChildren<Animator>();

        agent.stoppingDistance = attackRange - 0.1f;
        agent.updateRotation = true;
        agent.speed = chaseSpeed;
        TryAutoBind(); // ★ 추가
    }

    void OnEnable()
    {
        // 생성 프레임 순서 문제 대비(플레이어가 늦게 뜨는 케이스)
        if (!target || !targetHealth) StartCoroutine(LateBind());
    }
    void Update()
    {
        if (myHealth && myHealth.IsDead) { agent.isStopped = true; return; }
        if (!target) return;

        cooldown -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, target.position);
        bool inSight = true;
        if (requireLineOfSight)
        {
            Vector3 eye = transform.position + Vector3.up * 1.2f;
            Vector3 tgt = target.position + Vector3.up * 1.2f;
            if (Physics.Linecast(eye, tgt, out var hit, losMask, QueryTriggerInteraction.Ignore))
                inSight = hit.transform == target || hit.transform.IsChildOf(target);
        }

        if (dist <= detectRadius && inSight)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);

            //if (animator) animator.SetFloat("Speed", agent.velocity.magnitude);

            if (dist <= attackRange)
            {
                agent.isStopped = true;
                TryAttack();
            }
        }
        else
        {
            //if (animator) animator.SetFloat("Speed", 0f);
            if (agent.hasPath) agent.ResetPath();
        }
    }
    IEnumerator LateBind()
    {
        yield return null; // 한 프레임 대기
        TryAutoBind();
    }
    void TryAutoBind()
    {
        if (!target)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) target = p.transform;
        }
        if (!targetHealth && target)
            targetHealth = target.GetComponentInParent<PlayerHealth>();
    }
     public void InjectTarget(Transform t, PlayerHealth h)
    {
        target = t;
        targetHealth = h;
    }
    void TryAttack()
    {
        if (cooldown > 0f) return;
        cooldown = attackCooldown;

        //if (animator) animator.SetTrigger("Attack");
        if (targetHealth && !targetHealth.IsDead)
        {
            targetHealth.TakeDamage(contactDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
