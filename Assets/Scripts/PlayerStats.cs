using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("기본 스탯")]
    public float maxHP = 100f;         // 최대 체력
    public float attackDamage = 10f;   // 공격력
    public float moveSpeed = 5f;       // 이동속도
    public float attackSpeed = 1f;     // 초당 공격 횟수

    [Header("현재 상태")]
    public float currentHP;

    void Awake()
    {
        currentHP = maxHP;
    }

    // === 체력 관련 ===
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }

    void Die()
    {
        Debug.Log("플레이어 사망!");
        // TODO: 나중에 사망 연출 추가
    }

    // === 외부에서 스탯 변경 ===
    public void AddAttackDamage(float value) => attackDamage += value;
    public void AddMoveSpeed(float value) => moveSpeed += value;
    public void AddAttackSpeed(float value) => attackSpeed += value;
    public void AddMaxHP(float value)
    {
        maxHP += value;
        currentHP = Mathf.Min(currentHP + value, maxHP);
    }
}
