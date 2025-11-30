using UnityEngine;

public class RuntimeManager : MonoBehaviour
{
    public static RuntimeManager Instance { get; private set; }

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask monsterLayer;

    [Header("Player Settings")]
    public float playerMoveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float detectionRange = 10f;

    [Header("Shooting Settings")]
    public float fireRate = 0.3f;

    [Header("Monster Settings")]
    public int monsterMaxHealth = 50;
    public int monsterExpReward = 20;

    // 내부 관리용
    private float nextFireTime = 0f;

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
            return;
        }
    }

    // 레이어/설정 접근용 유틸
    public LayerMask GetGroundLayer() => groundLayer;
    public LayerMask GetMonsterLayer() => monsterLayer;
    public float GetDetectionRange() => detectionRange;
    public float GetRotationSpeed() => rotationSpeed;
    public float GetPlayerMoveSpeed() => playerMoveSpeed;

    // 발사 쿨다운 관리 (싱글턴 기반 전역 쿨다운)
    public bool CanFire()
    {
        return Time.time >= nextFireTime;
    }

    public void RegisterFire()
    {
        nextFireTime = Time.time + fireRate;
    }

    // 몬스터 기본값 반환
    public int GetMonsterMaxHealth() => monsterMaxHealth;
    public int GetMonsterExpReward() => monsterExpReward;
}