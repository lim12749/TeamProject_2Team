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
        level++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f);

        UpdateUI();

        if (LevelUpManager.Instance != null)
        {
            LevelUpManager.Instance.ShowLevelUpOptions();
        }
        else
        {
            var player = gameObject;
            var stats = player.GetComponent<CharacterStats>();
            if (stats != null)
            {
                stats.maxHealth += 20;
                Debug.Log("LevelUp: LevelUpManager 없음 ? 기본 보상 적용 (+20 체력).");
            }
        }
    }

    void UpdateUI()
    {
        if (expBar != null)
            expBar.value = (float)currentExp / expToNextLevel;

        if (levelText != null)
            levelText.text = $"LV {level}";
    }
}