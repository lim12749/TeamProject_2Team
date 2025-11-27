using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

<<<<<<< Updated upstream
    public GameObject[] charPrefabs; // 0 = knight, 1 = soldier
    public GameObject Player { get; private set; }
=======
    public GameObject[] charPrefabs; // ¿¹ºñ ÇÁ¸®ÆÕ ¹è¿­ (¿É¼Ç)
    public GameObject Player;
>>>>>>> Stashed changes

    void Awake()
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

<<<<<<< Updated upstream
    private void OnDestroy()
    {
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

            if (DataManager.Instance == null)
            {
                Debug.LogError("DataManager.Instanceï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½Ï´ï¿½! DataManagerï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½Îµï¿½Ç¾ï¿½ï¿½ï¿½ï¿½ï¿½ È®ï¿½ï¿½ï¿½Ï¼ï¿½ï¿½ï¿½.");
                return;
            }

            int index = (int)DataManager.Instance.CurrentCharacter;

            if (charPrefabs == null || charPrefabs.Length == 0)
            {
                Debug.LogError("charPrefabs ï¿½è¿­ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½Ï´ï¿½!");
                return;
            }

            if (index < 0 || index >= charPrefabs.Length)
            {
                Debug.LogError($"ï¿½ï¿½È¿ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ Ä³ï¿½ï¿½ï¿½ï¿½ ï¿½Îµï¿½ï¿½ï¿½: {index}");
                return;
            }

            Player = Instantiate(charPrefabs[index], Vector3.zero, Quaternion.identity);
            Player.name = charPrefabs[index].name + "_Player";
            Player.transform.localScale = Vector3.one;
        }

=======
    void Start()
    {
        GameObject prefabToSpawn = null;

        // ¿ì¼± RuntimeManagerÀÇ ¼±ÅÃµÈ ÇÁ¸®ÆÕ »ç¿ë
        if (RuntimeManager.Instance != null)
        {
            prefabToSpawn = RuntimeManager.Instance.GetSelectedCharacterPrefab();

            // ÂüÁ¶°¡ ¾øÀ¸¸é selectedCharacterName(»ç¿ëÀÚ Ç¥½Ã ÀÌ¸§ ¶Ç´Â ÇÁ¸®ÆÕ ÀÌ¸§)À» ÀÌ¿ëÇØ °Ë»ö
            string selName = RuntimeManager.Instance.selectedCharacterName;
            if (prefabToSpawn == null && !string.IsNullOrEmpty(selName))
            {
                // 1) ¸ÕÀú Resources¿¡¼­ ÇÁ¸®ÆÕ ÀÚ»ê ÀÌ¸§À¸·Î ½Ãµµ
                prefabToSpawn = Resources.Load<GameObject>(selName);
                if (prefabToSpawn != null)
                {
                    Debug.Log($"GameManager: Resources¿¡¼­ '{selName}' ÀÌ¸§À¸·Î ÇÁ¸®ÆÕÀ» ·ÎµåÇß½À´Ï´Ù.");
                }
                else
                {
                    // 2) ½ÇÆÐÇÏ¸é ResourcesÀÇ ¸ðµç GameObject ÇÁ¸®ÆÕÀ» °Ë»öÇÏ¿©
                    //    CharacterStats.component.characterName ÇÊµå¿Í ÀÏÄ¡ÇÏ´Â °ÍÀ» Ã£À½
                    var all = Resources.LoadAll<GameObject>("");
                    foreach (var g in all)
                    {
                        if (g == null) continue;
                        var stats = g.GetComponent<CharacterStats>();
                        if (stats != null && stats.characterName == selName)
                        {
                            prefabToSpawn = g;
                            Debug.Log($"GameManager: CharacterStats.characterName '{selName}' À¸·Î Resources¿¡¼­ ÇÁ¸®ÆÕ '{g.name}'À» Ã£¾Ò½À´Ï´Ù.");
                            break;
                        }
                    }

                    if (prefabToSpawn == null)
                        Debug.LogWarning($"GameManager: Resources¿¡¼­ '{selName}' ÀÌ¸§ÀÇ ÇÁ¸®ÆÕÀ» Ã£Áö ¸øÇß½À´Ï´Ù.");
                }
            }
        }

        // ¾øÀ¸¸é charPrefabs ¹è¿­ÀÇ Ã¹ ¹øÂ°¸¦ »ç¿ë(¹é¾÷)
        if (prefabToSpawn == null && charPrefabs != null && charPrefabs.Length > 0)
        {
            prefabToSpawn = charPrefabs[0];
            Debug.Log("GameManager: fallback charPrefabs[0] »ç¿ë");
        }

        if (prefabToSpawn != null)
        {
            Player = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity);
            Player.transform.SetParent(null);
            Player.transform.localScale = Vector3.one;
            Debug.Log($"GameManager: ÇÃ·¹ÀÌ¾î ÇÁ¸®ÆÕ '{prefabToSpawn.name}' ¼ÒÈ¯ ¿Ï·á.");
        }
        else
        {
            Debug.LogWarning("GameManager: ¼ÒÈ¯ÇÒ Ä³¸¯ÅÍ ÇÁ¸®ÆÕÀÌ ¾ø½À´Ï´Ù. RuntimeManager ¶Ç´Â charPrefabs ¸¦ È®ÀÎÇÏ¼¼¿ä.");
        }
>>>>>>> Stashed changes
    }
}
