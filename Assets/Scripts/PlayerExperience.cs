using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerExperience : MonoBehaviour
{
    public Slider expBar;
    public TextMeshProUGUI levelText;

    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

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
        Debug.Log("·¹º§¾÷");
        level++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f);
    }

    void UpdateUI()
    {
        if (expBar != null)
            expBar.value = (float)currentExp / expToNextLevel;

        if (levelText != null)
            levelText.text = $"LV {level}";
    }
}
