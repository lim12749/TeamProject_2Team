using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance { get; private set; }

    [Header("몬스터 기본 스탯 (모든 몬스터의 기본값)")]
    public float defaultMoveSpeed = 3.0f;
    public float defaultMaxHealth = 100f;
    public int defaultExpReward = 10;

    // 초기값 백업 (런타임에서 변경된 값을 리셋하기 위함)
    private float initialDefaultMoveSpeed;
    private float initialDefaultMaxHealth;
    private int initialDefaultExpReward;

    // 활성 몬스터 추적(필요시 사용)
    private readonly List<Monster> activeMonsters = new List<Monster>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 초기값 캐시
            initialDefaultMoveSpeed = defaultMoveSpeed;
            initialDefaultMaxHealth = defaultMaxHealth;
            initialDefaultExpReward = defaultExpReward;
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

    // 런타임으로 변경된 기본값을 초기값으로 되돌림
    public void ResetDefaults()
    {
        defaultMoveSpeed = initialDefaultMoveSpeed;
        defaultMaxHealth = initialDefaultMaxHealth;
        defaultExpReward = initialDefaultExpReward;

        // 이미 존재하는 몬스터들에게도 기본값 재적용
        foreach (var m in activeMonsters)
        {
            if (m != null)
                ApplyDefaults(m);
        }

        Debug.Log("MonsterManager: 기본 스탯을 초기값으로 리셋했습니다.");
    }
}