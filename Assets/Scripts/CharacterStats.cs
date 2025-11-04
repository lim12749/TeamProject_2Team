using UnityEngine;

[System.Serializable]
public class CharacterStats : MonoBehaviour
{
    public static CharacterStats instance { get; private set; }

    [Header("기본 정보")]
    public string characterName = "기본 캐릭터";
    public Sprite portrait;

    [Header("기본 능력치")]
    public int maxHealth = 100;
    public int attackPower = 10;
    public float moveSpeed = 5f;
}
