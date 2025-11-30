using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 플레이어의 경험치와 레벨을 관리하는 컴포넌트입니다.
/// 경험치 획득, 레벨업, UI 갱신 기능을 제공합니다.
/// </summary>
public class PlayerExperience : MonoBehaviour
{
    // 경험치 바 UI
    public Slider expBar;
    // 레벨 텍스트 UI
    public TextMeshProUGUI levelText;

    // 현재 레벨
    public int level = 1;
    // 현재 경험치
    public int currentExp = 0;
    // 다음 레벨까지 필요한 경험치
    public int expToNextLevel = 100;

    /// <summary>
    /// 게임 시작 시 UI를 초기화합니다.
    /// </summary>
    void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// 경험치를 추가하고, 필요 시 레벨업을 처리합니다.
    /// </summary>
    /// <param name="amount">추가할 경험치 양</param>
    public void AddExperience(int amount)
    {
        currentExp += amount;

        // 경험치가 다음 레벨에 도달하면 레벨업 반복 처리
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    /// <summary>
    /// 레벨을 올리고, 다음 레벨까지 필요한 경험치를 증가시킵니다.
    /// </summary>
    void LevelUp()
    {
        Debug.Log("레벨업");
        level++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f);
    }

    /// <summary>
    /// 경험치 바와 레벨 텍스트 UI를 갱신합니다.
    /// </summary>
    void UpdateUI()
    {
        if (expBar != null)
            expBar.value = (float)currentExp / expToNextLevel;

        if (levelText != null)
            levelText.text = $"LV {level}";
    }
}
