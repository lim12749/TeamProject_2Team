using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public characterStats playerStats;

    void Awake()
    {
        // 캐릭터 Stats 연결
        if (playerStats == null)
            playerStats = FindObjectOfType<characterStats>();

        // 슬라이더 연결
        if (healthSlider == null)
            healthSlider = GameObject.Find("PlayerUI/backgroundHalfCustom/HealthBar_LR")?.GetComponent<Slider>();

        // 슬라이더 최대값 세팅
        if (healthSlider != null && playerStats != null)
            healthSlider.maxValue = playerStats.maxHealth;
    }

    void Update()
    {
        if (playerStats != null && healthSlider != null)
            healthSlider.value = playerStats.currentHealth;
    }
}
