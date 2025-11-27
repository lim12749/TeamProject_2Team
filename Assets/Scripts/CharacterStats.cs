using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("기본 정보")]
    public string characterName = "기본 이름";
    public Sprite portrait;

    [Header("기본 능력치")]
    public int maxHealth = 100;
    public int attackPower = 10;
    public float moveSpeed = 5f;

    [Header("인게임 소환용 프리팹")]
    public GameObject inGamePrefab; // Inspector에서 각 캐릭터의 소환용 프리팹을 할당
}
