using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    // 1. [수정됨] 단일 프리팹 대신 '프리팹 배열'을 사용합니다.
    //    인스펙터 창에서 여기에 2개의 몬스터 프리팹을 연결합니다.
    public GameObject[] monsterPrefabs;

    // 몬스터 스폰 간격 (초)
    public float spawnInterval = 5.0f;

    // 스폰될 위치의 반경
    public float spawnRadius = 3.0f;

    void Start()
    {
        // 2초 뒤에 SpawnMonster를 처음 호출하고, 그 후 5초마다 반복 호출
        InvokeRepeating("SpawnMonster", 2.0f, spawnInterval);
    }

    void SpawnMonster()
    {
        // 2. [수정됨] 프리팹 배열이 비어있거나, 아무것도 할당되지 않았는지 확인
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
        {
            Debug.LogError("MonsterPrefabs 배열이 비어있습니다. 스포너 인스펙터를 확인하세요.");
            return;
        }

        // 3. [추가됨] 몬스터 배열(monsterPrefabs) 중에서 랜덤으로 하나를 선택
        // Random.Range(0, monsterPrefabs.Length)는 0부터 (배열 길이 - 1) 까지의 정수를 반환합니다.
        // 배열 길이가 2라면, 0 또는 1을 반환합니다.
        int randomIndex = Random.Range(0, monsterPrefabs.Length);

        // 4. [추가됨] 랜덤 인덱스에 해당하는 프리팹을 가져옵니다.
        GameObject prefabToSpawn = monsterPrefabs[randomIndex];

        // 5. 스포너 위치 기준으로 무작위 위치 계산
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPosition.y = transform.position.y; // Y축 높이는 스포너와 동일하게

        // 6. [수정됨] 선택된 랜덤 몬스터(prefabToSpawn)를 생성합니다.
        Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
    }
}