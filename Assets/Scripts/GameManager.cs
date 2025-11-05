using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject[] charPrefabs; // 0 = knight, 1 = soldier
    public GameObject Player { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
<<<<<<< Updated upstream
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainGameScene")
        {
            SpawnPlayerForGame();
        }
    }

    void SpawnPlayerForGame()
    {
        if (Player != null) // ì´ë¯¸ ìŠ¤í°ë˜ì–´ ìžˆìœ¼ë©´ ë˜ ì•ˆë§Œë“¦
            return;

        if (DataManager.Instance == null)
        {
            Debug.LogWarning("DataManagerê°€ ì—†ìŒ: CurrentCharacter ê¸°ë³¸ê°’ ì‚¬ìš©");
            // ì•ˆì „ ìž¥ì¹˜: ì¸ë±ìŠ¤ 0 ì‚¬ìš©
            InstantiatePlayer(0);
            return;
        }

        int index = Mathf.Clamp((int)DataManager.Instance.CurrentCharacter, 0, charPrefabs.Length - 1);
        InstantiatePlayer(index);
    }

    void InstantiatePlayer(int index)
    {
        if (charPrefabs == null || charPrefabs.Length == 0)
        {
            Debug.LogError("charPrefabsê°€ ë¹„ì–´ìžˆìŒ!");
=======
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager.Instance°¡ ¾ø½À´Ï´Ù! DataManager°¡ ¸ÕÀú ·ÎµåµÇ¾ú´ÂÁö È®ÀÎÇÏ¼¼¿ä.");
            return;
        }

        int index = (int)DataManager.Instance.CurrentCharacter;

        if (charPrefabs == null || charPrefabs.Length == 0)
        {
            Debug.LogError("charPrefabs ¹è¿­ÀÌ ºñ¾ú½À´Ï´Ù!");
            return;
        }

        if (index < 0 || index >= charPrefabs.Length)
        {
            Debug.LogError($"À¯È¿ÇÏÁö ¾ÊÀº Ä³¸¯ÅÍ ÀÎµ¦½º: {index}");
>>>>>>> Stashed changes
            return;
        }

        Player = Instantiate(charPrefabs[index], Vector3.zero, Quaternion.identity);
        Player.name = charPrefabs[index].name + "_Player";
        Player.transform.localScale = Vector3.one;
    }

}
