using UnityEngine;
using TMPro;

public enum UpgradeType
{
    Health,
    Attack,
    MoveSpeed
}

public class LevelUpOption : MonoBehaviour
{
    public UpgradeType upgradeType;
    public int bonusValue;

    public characterStats characterStats;

    public TextMeshProUGUI descriptionText;

    void Awake()
    {
        // 씬에서 플레이어 캐릭터가 생성된 직후에 찾아서 연결
        if (characterStats == null)
        {
            characterStats = FindObjectOfType<characterStats>();

            if (characterStats == null)
            {
                Debug.LogWarning("LevelUpOption Awake: CharacterStats 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }

    public void SetupOption()
    {
        if (descriptionText != null)
        {
            string statName = "";
            string valueText = bonusValue.ToString();

            switch (upgradeType)
            {
                case UpgradeType.Health:
                    statName = "health";
                    break;
                case UpgradeType.Attack:
                    statName = "power";
                    break;
                case UpgradeType.MoveSpeed:
                    statName = "speed";
                    valueText = "1"; // MoveSpeed는 항상 1 증가
                    break;
            }

            descriptionText.text = statName + " +" + valueText;
        }
    }


    public void ApplyUpgrade()
    {
        if (characterStats == null)
        {
            characterStats = FindObjectOfType<characterStats>();

            if (characterStats == null)
            {
                Debug.LogWarning("ApplyUpgrade: CharacterStats 컴포넌트를 찾을 수 없습니다.");
                return;
            }
        }

        switch (upgradeType)
        {
            case UpgradeType.Health:
                characterStats.AddHealth(bonusValue);
                break;
            case UpgradeType.Attack:
                characterStats.AddAttack(bonusValue);
                break;
            case UpgradeType.MoveSpeed:
                // MoveSpeed는 항상 0.5만큼 증가
                characterStats.AddMoveSpeed(1f);
                break;
        }
    }


    public void OnSelectButton()
    {
        LevelUpUIManager.instance.SelectOption(transform.GetSiblingIndex());
    }
}