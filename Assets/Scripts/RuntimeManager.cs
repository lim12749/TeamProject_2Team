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

    [Header("Selection")]
    public GameObject selectedCharacterPrefab;
    public GameObject defaultCharacterPrefab;

    // 선택된 프리팹 이름(복구용)
    public string selectedCharacterName;

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

    // 선택된 캐릭터 프리팹 접근자
    public GameObject GetSelectedCharacterPrefab()
    {
        // 우선 저장된 참조가 살아있으면 반환
        if (selectedCharacterPrefab != null)
            return selectedCharacterPrefab;

        // 참조가 null이면 이름으로 Resources에서 복구 시도
        if (!string.IsNullOrEmpty(selectedCharacterName))
        {
            var loaded = Resources.Load<GameObject>(selectedCharacterName);
            if (loaded != null)
            {
                selectedCharacterPrefab = loaded;
                return selectedCharacterPrefab;
            }
        }

        // 최종 fallback
        return defaultCharacterPrefab;
    }

    public void SetSelectedCharacterPrefab(GameObject prefab)
    {
        selectedCharacterPrefab = prefab;
        selectedCharacterName = prefab != null ? prefab.name : null;
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