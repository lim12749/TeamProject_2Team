using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class VolumeUI : MonoBehaviour
{
    [Header("Sliders (0~1)")]
    [Tooltip("BGM 볼륨 조절용 슬라이더 0 ~ 1 범위")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider mastarSlider;

    [SerializeField] TextMeshProUGUI bgmText;

    [Tooltip("True면 퍼센트 표시, false면 소수점 표시")]
    [SerializeField] bool showPercent = true;

    private void OnEnable()
    {
        var soundManager = SoundManagerManual_ye.Instance;
        if ((soundManager == null))
            return;
        if (bgmSlider)
            bgmSlider.SetValueWithoutNotify(soundManager.GetVolume01 (SoundManagerManual_ye.Bus.BGM));
    }
    public void OnBGMChanged(float _value)
    {
        var soundManager = SoundManagerManual_ye.Instance;
        if ((soundManager == null))
            return;

        soundManager.SetVolume01(SoundManagerManual_ye.Bus.BGM, _value);
    }
}
