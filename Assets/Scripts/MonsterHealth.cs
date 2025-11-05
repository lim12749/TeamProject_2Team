using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class MonsterHealth : MonoBehaviour
{
    public int maxHealth = 50;  // 몬스터 최대 체력
    private int currentHealth;

    public int expReward = 20;  // 처치 시 주는 경험치

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    // 총알에 맞았을 때 데미지 받기
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 피격 애니메이션 (선택)
            if (animator != null)
                animator.SetTrigger("Hit");
        }
    }

    void Die()
    {
        isDead = true;

        // 사망 애니메이션 (있다면)
        if (animator != null)
            animator.SetTrigger("Die");

        // 경험치 지급 (MainGameScene에서만)
        if (SceneManager.GetActiveScene().name == "MainGameScene")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerExperience exp = player.GetComponent<PlayerExperience>();
                if (exp != null)
                    exp.AddExperience(expReward);
            }
        }

        // 잠시 후 몬스터 삭제
        Destroy(gameObject, 1.5f);
    }
}
