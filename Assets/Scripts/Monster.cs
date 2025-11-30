using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Monster : MonoBehaviour
{
    [Header("움직임")]
    public float moveSpeed = 3.0f;

    [Header("체력")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("사망 이펙트")]
    public GameObject deathEffect;

    [Header("애니메이터")]
    public Animator animator;

    [Header("플레이어 공격")]
    public int damageToPlayer = 10;
    public float attackDistance = 1.5f;
    public float attackCooldown = 1.0f;

    private float lastAttackTime = 0f;

    private Transform playerTransform;
    private bool isDead = false;

    void Start()
    {
        if (MonsterManager.Instance != null)
            MonsterManager.Instance.ApplyDefaults(this);

        currentHealth = maxHealth;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning($"[{name}] 플레이어를 찾을 수 없습니다. 'Player' 태그를 확인하세요.");

        MonsterManager.Instance?.RegisterMonster(this);
    }

    void Update()
    {
        if (isDead || playerTransform == null) return;

        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > attackDistance * attackDistance)
            MoveTowardPlayer();
        else
            TryAttackPlayer();
    }

    void TryAttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;

        var playerStats = playerTransform.GetComponent<characterStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(damageToPlayer);
            Debug.Log($"{name}가 플레이어에게 {damageToPlayer} 피해를 입힘.");
        }

        if (animator != null)
            animator.SetTrigger("Attack");
    }




    void MoveTowardPlayer()
    {
        if (playerTransform == null) return;

        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude <= 0.01f) return;

        // 자연스러운 회전
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);

        // 전진
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    // 외부에서 데미지 호출 시(일부 다른 시스템과 호환되도록 float)
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // MonsterHealth 컴포넌트가 있으면 위임
        var mh = GetComponent<MonsterHealth>();
        if (mh != null)
        {
            // MonsterHealth.TakeDamage는 int로 되어있다면 오버로드되지 않으므로 형변환(정수 데미지 사용 권장)
            mh.TakeDamage(Mathf.RoundToInt(damage));
            return;
        }

        // 자체 체력 사용
        currentHealth -= damage;
        Debug.Log($"{name} 피해: {damage} 남은 체력: {currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // 매니저에서 등록 해제
        MonsterManager.Instance?.UnregisterMonster(this);

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // 안전하게 등록 해제
        MonsterManager.Instance?.UnregisterMonster(this);
    }
}
