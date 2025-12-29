using UnityEngine;
using UnityEngine.SceneManagement;

public class characterStats : MonoBehaviour
{
    [Header("기본 능력치")]
    public int maxHealth = 100;
    public int attackPower = 10;
    public float moveSpeed = 5f;

    public int currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        SceneManager.LoadScene("GameOverScene");
    }

    public void AddHealth(int value)
    {
        maxHealth += value;
        currentHealth += value;
    }

    public void AddAttack(int value)
    {
        attackPower += value;
    }

    public void AddMoveSpeed(float value)
    {
        moveSpeed += value;
    }
}
