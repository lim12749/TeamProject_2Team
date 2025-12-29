using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class MonsterHealth : MonoBehaviour
{
    public int maxHealth = 50;  // monster max health
    private int currentHealth;

    public int expReward = 20;  // experience reward on death

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    // called when hit by a bullet
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
            // optional hit animation
            if (animator != null)
                animator.SetTrigger("Hit");
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // death animation (if any)
        if (animator != null)
            animator.SetTrigger("Die");

        // award experience only in MainGameScene
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

        // destroy monster immediately
        Destroy(gameObject);
    }
}
