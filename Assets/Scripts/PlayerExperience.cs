using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    public Slider expBar;
    public TextMeshProUGUI levelText;

    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    void Awake()
    {
        // 현재 씬이 MainGameScene일 때만 실행되도록 조건문 추가
        if (SceneManager.GetActiveScene().name == "MainGameScene")
        {
            if (expBar == null)
                expBar = GameObject.Find("Canvas/backgroundHalfCustom/ex")?.GetComponent<Slider>();

            if (levelText == null)
                levelText = GameObject.Find("Canvas/level/level(TMP)")?.GetComponent<TextMeshProUGUI>();
        }
    }


    void Start()
    {
        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    void LevelUp()
    {
        Debug.Log("레벨업");
        level++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f);
    }

    void UpdateUI()
    {
        if (expBar != null)
            expBar.value = (float)currentExp / expToNextLevel;

        if (levelText != null)
            levelText.text = $"{level}";
    }
}