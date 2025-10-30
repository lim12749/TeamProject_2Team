using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeUI : MonoBehaviour
{
 [Header("Sliders (0~1)")]

    [SerializeField] Slider bgmSlider;


    [Header("Labels (선택)")]

    [SerializeField] TextMeshProUGUI bgmLabel;

    [SerializeField] bool showPercent = true; // true면 0~100%

    void OnEnable()
    {
        var sm = SoundManagerManual.Instance;
        if (sm == null) return;

        // 슬라이더는 UI 표시만 갱신. (SetValueWithoutNotify: OnValueChanged 트리거 안 함)
        if (bgmSlider)  
              bgmSlider   .SetValueWithoutNotify(sm.GetVolume01(SoundManagerManual.Bus.BGM));


        // 라벨도 갱신
        RefreshLabels();
    }

    // ===== 인스펙터에서 Slider.OnValueChanged(float)로 직접 연결할 메서드들 =====
    public void OnBGMChanged(float v)
    {
        var sm = SoundManagerManual.Instance; if (sm == null) return;
        sm.SetVolume01(SoundManagerManual.Bus.BGM, v);
        UpdateLabel(bgmLabel, v);
    }
    public void OnEnemyChanged(float v)
    {
        var sm = SoundManagerManual.Instance; if (sm == null) return;
        sm.SetVolume01(SoundManagerManual.Bus.Enemy, v);

    }

    void RefreshLabels()
    {

        if (bgmSlider)    UpdateLabel(bgmLabel,    bgmSlider.value);

    }

    void UpdateLabel(TextMeshProUGUI label, float v01)
    {
        if (!label) return;
        if (showPercent) label.text = $"{Mathf.RoundToInt(v01 * 100f)}%";
        else             label.text = v01.ToString("0.00");
    }
}
