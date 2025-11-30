using UnityEngine;

public class LevelUpUIManager : MonoBehaviour
{
    public static LevelUpUIManager instance;

    public GameObject levelUpCanvas;
    public LevelUpOption[] options;

    void Awake()
    {
        instance = this;

        if (levelUpCanvas != null)
            levelUpCanvas.SetActive(false);
    }

    public void ShowLevelUpUI()
    {
        Debug.Log("ShowLevelUpUI È£ÃâµÊ");
        Time.timeScale = 0;
        if (levelUpCanvas != null)
            levelUpCanvas.SetActive(true);

        foreach (var option in options)
            option?.SetupOption();
    }


    public void SelectOption(int index)
    {
        if (index >= 0 && index < options.Length)
        {
            options[index].ApplyUpgrade();
        }

        CloseUI();
    }

    void CloseUI()
    {
        if (levelUpCanvas != null)
            levelUpCanvas.SetActive(false);

        Time.timeScale = 1;
    }
}
