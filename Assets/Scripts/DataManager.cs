using UnityEngine;

public class DataManager : MonoBehaviour
{
    public enum Character
    {
        knightPlayer,
        soldierPlayer
    }
    public static DataManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Character CurrentCharacter;
}
