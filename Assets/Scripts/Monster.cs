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

    private Transform playerTransform;
    private bool isDead = false;

    void Start()
    {
        // 중앙 매니저가 있으면 기본값 적용
        if (MonsterManager.Instance != null)
            MonsterManager.Instance.ApplyDefaults(this);

        // MonsterHealth 컴포넌트가 있으면 그쪽을 우선 사용
        var mh = GetComponent<MonsterHealth>();
        if (mh != null)
        {
            // MonsterHealth 내부에서 currentHealth를 관리하므로 동기화는 필요 없음
            // mh.maxHealth 값이 이미 설정되어 있을 것임
        }
        else
        {
            // Monster 자체에서 체력을 관리
            currentHealth = maxHealth;
        }

        // 플레이어 찾기
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
        {
            Debug.LogWarning($"[{name}] 플레이어를 찾을 수 없습니다. 'Player' 태그를 확인하세요.");
            // 플레이어가 반드시 필요한 행동이면 스크립트를 disable
            // 여기서는 이동 로직에서 null 체크하므로 계속 둠
        }

        // 등록(매니저가 존재하면)
        MonsterManager.Instance?.RegisterMonster(this);
    }

    void Update()
    {
        if (isDead) return;
        if (playerTransform == null) return;

        MoveTowardPlayer();
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

        // 제거(애니메이션 재생 시간을 고려해 지연)
        Destroy(gameObject, 2f);
    }

    void OnDestroy()
    {
        // 안전하게 등록 해제
        MonsterManager.Instance?.UnregisterMonster(this);
    }
}
