using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeUI : MonoBehaviour
{
    [Header("Sliders(0~1")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] TextMeshProUGUI bgmText;
    [SerializeField] bool showPercent = true;
    private void OnEnable()
    {
        var soundManager = SoundManagerManual.Instance;
        if(soundManager == null) { return; }
        if(bgmSlider)
            bgmSlider.SetValueWithoutNotify(soundManager.GetVolume01(SoundManagerManual.Bus.BGM));
    }
    public void OnBGMChanged(float _value)
    {
        var soundManager = SoundManagerManual.Instance;
        if (soundManager == null) { return; }
        soundManager.SetVolume01(SoundManagerManual.Bus.BGM, _value);
    }
    public void OnSFXChanged(float _value)
    {
        var soundManager = SoundManagerManual.Instance;
        if (soundManager == null) { return; }
        soundManager.SetVolume01(SoundManagerManual.Bus.SFX, _value);
    }
    
}
