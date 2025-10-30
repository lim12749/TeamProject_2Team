// PlayerHealth.cs
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHP = 100f;
    public float Current { get; private set; }
    public bool IsDead => Current <= 0f;

    public event Action<float, float> OnChanged;
    public event Action OnDeath;

    void Awake() => Current = maxHP;

    public void TakeDamage(float dmg)
    {
        if (IsDead) return;
        Current -= dmg;
        OnChanged?.Invoke(Current, maxHP);
        if (Current <= 0f)
        {
            OnDeath?.Invoke();
            Debug.Log("Player Died");
            // TODO: 리스폰/게임오버 처리
        }
    }
}
