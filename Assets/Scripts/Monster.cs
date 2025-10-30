using UnityEngine;

public class Monster : MonoBehaviour
{
    public float health = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;// 받은데미지로 감소
        Debug.Log($"몬스터 피격 남은 체력 : {health}");

        if(health <=0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("몬스터 사망");
        Destroy(gameObject);
    }
}
