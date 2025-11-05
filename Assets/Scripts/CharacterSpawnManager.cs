using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSpawnManager : MonoBehaviour
{
    public GameObject knightPrefab;
    public GameObject soldierPrefab;
    public Transform knightFloor;  // 씬에 있는 floorRenderer 오브젝트 (기사용)
    public Transform soldierFloor; // 씬에 있는 floorRenderer 오브젝트 (솔져용)

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "CharacterSelectScene")
        {
            SpawnCharacters();
        }
    }

    void SpawnCharacters()
    {
        if (knightPrefab == null || soldierPrefab == null)
        {
            Debug.LogError("CharacterSpawnManager: 프리팹이 할당되지 않음.");
            return;
        }
        if (knightFloor == null || soldierFloor == null)
        {
            Debug.LogError("CharacterSpawnManager: floor Transform이 할당되지 않음.");
            return;
        }

        // 이미 존재하는 선택용 인스턴스가 있으면 제거 (중복 방지)
        // (씬에 남아있는 이전 선택 인스턴스들을 정리)
        foreach (var old in Object.FindObjectsByType<CharacterSelector>(FindObjectsSortMode.None))
        {
            Destroy(old.gameObject);
        }

        // Knight 생성
        GameObject knight = Instantiate(knightPrefab, knightFloor.position + Vector3.up * 0.1f, Quaternion.Euler(0f, 110f, 0f));
        knight.name = "Knight_Select";
        knight.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

        // Soldier 생성
        GameObject soldier = Instantiate(soldierPrefab, soldierFloor.position + Vector3.up * 0.1f, Quaternion.Euler(0f, 100f, 0f));
        soldier.name = "Soldier_Select";
        soldier.transform.localScale = new Vector3(0.8f, 1.2f, 1f);

        // CharacterSelector 연결 & floorRenderer 연결
        CharacterSelector knightSel = knight.GetComponentInChildren<CharacterSelector>(); // 혹시 Prefab 구조상 자식에 있을 수 있으니
        CharacterSelector soldierSel = soldier.GetComponentInChildren<CharacterSelector>();

        if (knightSel == null || soldierSel == null)
        {
            Debug.LogError("CharacterSpawnManager: CharacterSelector 컴포넌트가 프리팹에 없음.");
            return;
        }

        // 서로 참조하도록 설정 (둘 다 동일한 배열을 참조)
        CharacterSelector[] both = new CharacterSelector[] { knightSel, soldierSel };
        knightSel.chars = both;
        soldierSel.chars = both;

        // floorRenderer 연결 (씬 오브젝트의 Renderer를 넣어줌)
        var knightRenderer = knightFloor.GetComponent<Renderer>();
        var soldierRenderer = soldierFloor.GetComponent<Renderer>();
        if (knightRenderer != null) knightSel.floorRenderer = knightRenderer;
        if (soldierRenderer != null) soldierSel.floorRenderer = soldierRenderer;

        Debug.Log("CharacterSpawnManager: Knight & Soldier spawned and linked.");
    }
}
