using UnityEngine;

public class DataManager : MonoBehaviour
{
    public enum character
    {
        warrior,
        soldier
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

    public character CurrentCharacter;
}
