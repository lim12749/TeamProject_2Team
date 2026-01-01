using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI 필수

[RequireComponent(typeof(Collider))]
public class Monster : MonoBehaviour
{
    [Header("기본 설정")]
    public float moveSpeed = 3.0f;

    [Header("체력 및 UI")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthSlider;        // 체력바 슬라이더 (하나만 사용)
    public GameObject healthBarCanvas; // 캔버스 (빌보드용)

    [Header("사망 이펙트")]
    public GameObject deathEffect;

    [Header("애니메이터")]
    public Animator animator;

    [Header("플레이어 공격")]
    public int damageToPlayer = 10;
    public float attackDistance = 1.5f;
    public float attackCooldown = 1.0f;

    [Header("시간 경과 강화 (1분마다)")]
    public bool enableScaling = true;
    public float healthIncreasePerMinute = 20f;
    public int damageIncreasePerMinute = 5;

    private float lastAttackTime = 0f;
    private Transform playerTransform;
    private bool isDead = false;
    private Camera mainCamera;

    void Start()
    {
        if (MonsterManager.Instance != null)
            MonsterManager.Instance.ApplyDefaults(this);

        currentHealth = maxHealth;
        mainCamera = Camera.main;

        // UI 초기화: 최대 체력에 맞춰 슬라이더 설정
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        MonsterManager.Instance?.RegisterMonster(this);

        if (enableScaling) StartCoroutine(ScaleStatsRoutine());
    }

    void Update()
    {
        if (isDead) return;

        // 체력바가 항상 카메라를 바라보게 함 (빌보드)
        if (healthBarCanvas != null && mainCamera != null)
        {
            healthBarCanvas.transform.rotation =
                Quaternion.LookRotation(healthBarCanvas.transform.position - mainCamera.transform.position);
        }

        if (playerTransform == null) return;

        // 이동 및 공격 로직
        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > attackDistance * attackDistance)
            MoveTowardPlayer();
        else
            TryAttackPlayer();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        var mh = GetComponent<MonsterHealth>();
        if (mh != null) { mh.TakeDamage(Mathf.RoundToInt(damage)); return; }

        currentHealth -= damage;

        // [수정됨] 지연 없이 즉시 UI 반영
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        Debug.Log($"{name} 피해: {damage} 남은 체력: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    // 1분마다 강해지는 로직
    IEnumerator ScaleStatsRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(60f);

            // 능력치 상승
            maxHealth += healthIncreasePerMinute;
            currentHealth += healthIncreasePerMinute;
            damageToPlayer += damageIncreasePerMinute;

            // UI의 최대값도 같이 늘려줌
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }
    }

    // ... (이동, 공격, 사망 로직은 기존과 동일) ...
    void MoveTowardPlayer()
    {
        if (playerTransform == null) return;
        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude <= 0.01f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void TryAttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;
        var playerStats = playerTransform.GetComponent<characterStats>();
        if (playerStats != null) playerStats.TakeDamage(damageToPlayer);
        if (animator != null) animator.SetTrigger("Attack");
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        if (animator != null) animator.SetTrigger("Die");

        // 죽으면 체력바 숨김
        if (healthBarCanvas != null) healthBarCanvas.SetActive(false);

        if (deathEffect != null) Instantiate(deathEffect, transform.position, Quaternion.identity);
        MonsterManager.Instance?.UnregisterMonster(this);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        MonsterManager.Instance?.UnregisterMonster(this);
    }
}