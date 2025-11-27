using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class MonsterHealth : MonoBehaviour
{
    public int maxHealth = 50;  
    private int currentHealth;

    public int expReward = 20;  

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        if (RuntimeManager.Instance != null)
        {
            maxHealth = RuntimeManager.Instance.GetMonsterMaxHealth();
            expReward = RuntimeManager.Instance.GetMonsterExpReward();
        }

        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

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
            if (animator != null)
                animator.SetTrigger("Hit");
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

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

        Destroy(gameObject, 1.5f);
    }
}
