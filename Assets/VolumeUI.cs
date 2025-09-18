using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class VolumeUI : MonoBehaviour
{
    [Header("Sliders (0~1)")]
    [Tooltip("BGM ���� ������ �����̴� 0 ~ 1 ����")]
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider mastarSlider;

    [SerializeField] TextMeshProUGUI bgmText;

    [Tooltip("True�� �ۼ�Ʈ ǥ��, false�� �Ҽ��� ǥ��")]
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
