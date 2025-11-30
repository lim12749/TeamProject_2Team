using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject[] monsterPrefabs;
    public float spawnInterval = 15.0f;
    public float spawnRadius = 3.0f;

    private float currentSpawnInterval;
    private float nextSpawnTime;

    void Start()
    {
        currentSpawnInterval = spawnInterval;
        nextSpawnTime = Time.time + currentSpawnInterval;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnMonster();
            nextSpawnTime = Time.time + currentSpawnInterval;
        }
    }

    void SpawnMonster()
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
        {
            Debug.LogError("MonsterPrefabs 배열이 비어있습니다. 스포너 인스펙터를 확인하세요.");
            return;
        }

        int randomIndex = Random.Range(0, monsterPrefabs.Length);
        GameObject prefabToSpawn = monsterPrefabs[randomIndex];

        Vector3 randomPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPosition.y = transform.position.y;

        Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
    }

    // 30초마다 호출: 스폰 간격 증가 (즉, 스폰 속도 감소)
    public void IncreaseSpawnInterval(float multiplier)
    {
        currentSpawnInterval *= multiplier;
        Debug.Log($"몬스터 스폰 간격 증가: {currentSpawnInterval:F2}초");
    }
}
