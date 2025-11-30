using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }

    public CharacterSelector[] platforms;

    private int selectedPlatformIndex = -1;

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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject knightObj = GameObject.Find("FloorCircle(knight)");
        GameObject soldierObj = GameObject.Find("FloorCircle(soldier)");

        platforms = new CharacterSelector[2];

        if (knightObj != null)
            platforms[0] = knightObj.GetComponent<CharacterSelector>();

        if (soldierObj != null)
            platforms[1] = soldierObj.GetComponent<CharacterSelector>();
    }

    public void SelectPlatform(int platformIndex)
    {
        if (platforms == null || platforms.Length == 0)
            return;

        if (platformIndex >= 0 && platformIndex < platforms.Length)
        {
            if (selectedPlatformIndex != -1 && platforms[selectedPlatformIndex] != null)
            {
                platforms[selectedPlatformIndex].resetSelected();
            }

            if (platforms[platformIndex] != null)
            {
                platforms[platformIndex].SetSelected();
                selectedPlatformIndex = platformIndex;
            }
        }
    }

    public void DeletePlatform(int platformIndex)
    {
        if (platformIndex >= 0 && platformIndex < platforms.Length)
        {
            if (platforms[platformIndex] != null)
            {
                Destroy(platforms[platformIndex].gameObject);
                platforms[platformIndex] = null;
            }
        }
    }
}
