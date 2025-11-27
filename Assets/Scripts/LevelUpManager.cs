using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance { get; private set; }

    [Header("Panel 방식 (Canvas 안에 3개의 Panel)")]
    public GameObject[] optionPanels = new GameObject[3]; // Inspector에서 3개 Panel을 순서대로 할당

    // 옵션 정의
    public enum StatType { MaxHealth, AttackPower, MoveSpeed }

    [Serializable]
    public struct Option
    {
        public StatType stat;
        public float amount; // 정수/실수 허용
        public string title; // 버튼에 표시할 텍스트 (예: "+20 체력")
        public string description; // 상세 설명 (선택적, Panel 내 두번째 TMP에 표시)
    }

    private List<Option> optionPool;
    private Option[] currentOptions = new Option[3];

    private float previousTimeScale = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 필요시 유지, UI가 씬별이면 제거 가능
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Panel 초기 상태 비활성화
        if (optionPanels != null)
        {
            for (int i = 0; i < optionPanels.Length; i++)
            {
                if (optionPanels[i] != null)
                    optionPanels[i].SetActive(false);
            }
        }

        BuildOptionPool();
    }

    void BuildOptionPool()
    {
        optionPool = new List<Option>
        {
            new Option { stat = StatType.MaxHealth, amount = 20f, title = "+20 체력", description = "최대 체력을 20 증가시킵니다." },
            new Option { stat = StatType.MaxHealth, amount = 40f, title = "+40 체력", description = "최대 체력을 40 증가시킵니다." },
            new Option { stat = StatType.AttackPower, amount = 5f, title = "+5 공격력", description = "공격력이 5 증가합니다." },
            new Option { stat = StatType.AttackPower, amount = 10f, title = "+10 공격력", description = "공격력이 10 증가합니다." },
            new Option { stat = StatType.MoveSpeed, amount = 0.5f, title = "+0.5 이동속도", description = "이동속도가 0.5 증가합니다." },
            new Option { stat = StatType.MoveSpeed, amount = 1f, title = "+1 이동속도", description = "이동속도가 1.0 증가합니다." },
        };
    }

    // 레벨업 시 호출: Panel 세팅 및 게임 일시정지
    public void ShowLevelUpOptions()
    {
        // optionPanels가 정확히 3개 이상 할당되어 있어야 함
        if (optionPanels == null || optionPanels.Length < 3)
        {
            Debug.LogWarning("LevelUpManager: optionPanels가 3개 할당되어 있지 않습니다.");
            return;
        }

        // 무작위로 3개 선택 (중복 없음)
        var indices = new List<int>();
        while (indices.Count < 3 && indices.Count < optionPool.Count)
        {
            int i = UnityEngine.Random.Range(0, optionPool.Count);
            if (!indices.Contains(i)) indices.Add(i);
        }

        for (int j = 0; j < 3; j++)
        {
            currentOptions[j] = optionPool[indices[j]];

            var panelObj = optionPanels[j];
            if (panelObj == null)
            {
                Debug.LogWarning($"LevelUpManager: optionPanels[{j}]가 null 입니다.");
                continue;
            }

            panelObj.SetActive(true);

            // Panel 내부의 Button 찾기
            var btn = panelObj.GetComponentInChildren<Button>();
            if (btn == null)
            {
                Debug.LogWarning($"LevelUpManager: optionPanels[{j}]에 Button이 없습니다.");
                continue;
            }

            // 버튼 텍스트(TMP) 설정: Button 자식의 TMP 우선 사용
            TextMeshProUGUI buttonLabel = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonLabel != null)
                buttonLabel.text = currentOptions[j].title;
            else
            {
                // Panel 내의 첫 TMP를 찾아서 텍스트로 사용
                var tmps = panelObj.GetComponentsInChildren<TextMeshProUGUI>();
                if (tmps.Length > 0)
                    tmps[0].text = currentOptions[j].title;
            }

            // Panel 내 두번째 TMP가 있으면 description으로 설정
            var allTmps = panelObj.GetComponentsInChildren<TextMeshProUGUI>();
            if (allTmps.Length > 1)
                allTmps[1].text = currentOptions[j].description;

            // 클릭 리스너 설정
            int captured = j;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnCardSelected(captured));
        }

        // 시간 저장 후 일시정지
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    void OnCardSelected(int index)
    {
        if (index < 0 || index >= currentOptions.Length)
            return;

        ApplyOption(currentOptions[index]);

        // 모든 panel 비활성화 및 시간 복구
        if (optionPanels != null)
        {
            foreach (var p in optionPanels)
            {
                if (p != null) p.SetActive(false);
            }
        }

        Time.timeScale = previousTimeScale;
    }

    void ApplyOption(Option opt)
    {
        // 선택된 플레이어 찾기 (Tag = "Player" 권장)
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("LevelUpManager: Player를 찾을 수 없습니다. 옵션 적용 불가.");
            return;
        }

        var stats = player.GetComponent<CharacterStats>();
        if (stats == null)
        {
            Debug.LogWarning("LevelUpManager: Player에 CharacterStats가 없습니다.");
            return;
        }

        switch (opt.stat)
        {
            case StatType.MaxHealth:
                stats.maxHealth = Mathf.RoundToInt(stats.maxHealth + opt.amount);
                break;
            case StatType.AttackPower:
                stats.attackPower = Mathf.RoundToInt(stats.attackPower + opt.amount);
                break;
            case StatType.MoveSpeed:
                stats.moveSpeed += opt.amount;
                break;
        }

        Debug.Log($"레벨업 보상 적용: {opt.title} -> {stats.characterName}");
    }
}