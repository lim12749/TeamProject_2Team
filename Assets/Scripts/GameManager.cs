using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject[] charPrefabs;
    public GameObject Player;

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

    private void Start()
    {
        int index = (int)DataManager.Instance.CurrentCharacter;
        Player = Instantiate(charPrefabs[index], Vector3.zero, Quaternion.identity);
        Player.transform.SetParent(null);
        Player.transform.localScale = Vector3.one;
    }
}
