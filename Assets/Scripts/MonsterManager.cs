using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance { get; private set; }

    [Header("몬스터 기본 스탯 (모든 몬스터의 기본값)")]
    public float defaultMoveSpeed = 3.0f;
    public float defaultMaxHealth = 100f;
    public int defaultExpReward = 10;

    // 활성 몬스터 추적(필요시 사용)
    private readonly List<Monster> activeMonsters = new List<Monster>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 몬스터가 씬에서 활성화될 때 등록
    public void RegisterMonster(Monster m)
    {
        if (m == null) return;
        if (!activeMonsters.Contains(m))
            activeMonsters.Add(m);
    }

    // 몬스터가 파괴될 때 등록 해제
    public void UnregisterMonster(Monster m)
    {
        if (m == null) return;
        activeMonsters.Remove(m);
    }

    // 몬스터 인스턴스에 기본값을 적용 (몬스터가 개별값을 가지고 있지 않을 때 호출)
    public void ApplyDefaults(Monster m)
    {
        if (m == null) return;

        if (m.moveSpeed <= 0f)
            m.moveSpeed = defaultMoveSpeed;

        // Monster가 MonsterHealth 컴포넌트를 가지고 있으면 그쪽 maxHealth를, 아니면 필드에 적용
        var mh = m.GetComponent<MonsterHealth>();
        if (mh != null)
        {
            if (mh.maxHealth <= 0f)
                mh.maxHealth = (int)Mathf.Max(1f, defaultMaxHealth);
        }
        else
        {
            if (m.maxHealth <= 0f)
                m.maxHealth = Mathf.Max(1f, defaultMaxHealth);
        }
    }

    // 활성 몬스터 목록 반환(읽기 전용)
    public IReadOnlyList<Monster> GetActiveMonsters() => activeMonsters.AsReadOnly();
}
