// MonsterHealth.cs
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class MonsterHealth : MonoBehaviour
{
    [SerializeField] float maxHP = 50f;
    [SerializeField] GameObject deathFx; // 선택: 죽을 때 이펙트
    public float Current { get; private set; }
    public bool IsDead => Current <= 0f;

    public event Action OnDeath;
    public event Action<float, float> OnChanged; // (현재, 최대)

    void Awake() => Current = maxHP;

    public void TakeDamage(float dmg)
    {
        if (IsDead) return;
        Current -= dmg;
        OnChanged?.Invoke(Current, maxHP);

        if (Current <= 0f)
        {
            OnDeath?.Invoke();
            if (deathFx) Instantiate(deathFx, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
