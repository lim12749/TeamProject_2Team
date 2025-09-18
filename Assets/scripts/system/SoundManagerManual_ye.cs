/// <summary>
/// ������ ��� ���带 �����ϴ� �Ŵ��� Ŭ����
/// BGM, ȿ����, �� ���带 �з��Ͽ� �����ϰ� ���� ���� ����� �����մϴ�.
/// �̱��� ������ ����Ͽ� ��𼭵� ���� �����մϴ�.
/// </summary>
using UnityEngine;
using UnityEngine.Audio;

public class SoundManagerManual_ye : MonoBehaviour
{
    /// <summary>
    /// ���� ����(ä��) ������ �����ϴ� ������
    /// ���� �ٸ� �������� ������ �� �ֽ��ϴ�.
    /// </summary>
    public enum Bus { Master, BGM, SFX, Enemy }

    /// <summary>
    /// ���� ä���� ���� á�� ���� ó�� ���
    /// DropNewest: ���ο� ���带 ������� ����
    /// StealOldest: ���� ������ ���带 �����ϰ� �� ���� ���
    /// </summary>
    public enum OverflowPolicy { DropNewest, StealOldest }

    /// <summary>
    /// �̱��� �ν��Ͻ� - ���� ��ü���� �ϳ��� ����
    /// ��𼭵� SoundManagerManual.Instance�� ���� ����
    /// </summary>
    public static SoundManagerManual_ye Instance { get; private set; }

    [Header("Mixer & Groups (�ɼ�)")]
    [Tooltip("����� �ͼ� - ���� ������ ���� ���")]
    [SerializeField] AudioMixer mixer;
    [Tooltip("������ ���� �Ķ���� �̸�")]
    [SerializeField] string masterVolParam = "MasterVol";
    [Tooltip("BGM ���� �Ķ���� �̸�")]
    [SerializeField] string bgmVolParam = "BGMVol";
    [Tooltip("ȿ���� ���� �Ķ���� �̸�")]
    [SerializeField] string sfxVolParam = "SFXVol";
    [Tooltip("�� ���� ���� �Ķ���� �̸�")]
    [SerializeField] string enemyVolParam = "EnemyVol";

    [Header("Audio Sources (���� �巡��)")]
    [Tooltip("������ǿ� ����� �ҽ� (2D, �ϳ��� ���)")]
    [SerializeField] AudioSource bgmSource;        // 2D, Output=BGM �׷�
    [Tooltip("2D ȿ������ ����� �ҽ� �迭 (���� �� ���� ��� ����)")]
    [SerializeField] AudioSource[] sfx2DSources;   // 2D, Output=SFX �׷�
    [Tooltip("3D ȿ������ ����� �ҽ� �迭 (������ �ִ� ����)")]
    [SerializeField] AudioSource[] sfx3DSources;   // 3D, Output=SFX �׷�
    [Tooltip("�� ���� 3D ����� �ҽ� �迭")]
    [SerializeField] AudioSource[] enemy3DSources; // 3D, Output=Enemy �׷�

    [Header("3D �⺻ ����")]
    [Tooltip("3D ���尡 �ִ� �������� �鸮�� �ּ� �Ÿ�")]
    [SerializeField] float defaultMinDistance = 1f;
    [Tooltip("3D ���尡 �鸮�� �ʰ� �Ǵ� �ִ� �Ÿ�")]
    [SerializeField] float defaultMaxDistance = 40f;

    [Header("������� ���� á�� �� ��å")]
    [Tooltip("��� ����� �ҽ��� ��� ���� ���� ó�� ���")]
    [SerializeField] OverflowPolicy overflowPolicy = OverflowPolicy.StealOldest;

    // ���� �κ� ������� ����� �ҽ��� ��ȯ ����ϱ� ���� �ε���
    int _idxSfx2D, _idxSfx3D, _idxEnemy3D;

    /// <summary>
    /// ������Ʈ �ʱ�ȭ - �̱��� ���� �� �⺻ ���� ����
    /// </summary>
    void Awake()
    {
        // �̱��� ���� ���� - �̹� �ν��Ͻ��� ������ �ڽ��� ����
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // ���� �ٲ� �� ������Ʈ�� �ı����� ���� (BGM ���� ����� ����)
        DontDestroyOnLoad(gameObject);

        // 3D ���� �ҽ����� �⺻ ���� ����
        Apply3DDefaults(sfx3DSources);
        Apply3DDefaults(enemy3DSources);

        // ����� ���� ���� �ҷ�����
        LoadVolumes();
    }

    /// <summary>
    /// 3D ����� �ҽ� �迭�� �⺻ 3D ������ �����մϴ�
    /// </summary>
    /// <param name="arr">������ ����� �ҽ� �迭</param>
    void Apply3DDefaults(AudioSource[] arr)
    {
        if (arr == null) return;

        for (int i = 0; i < arr.Length; i++)
        {
            var a = arr[i];
            if (!a) continue; // null üũ

            a.spatialBlend = 1f; // ������ 3D ���� (0=2D, 1=3D)
            a.rolloffMode = AudioRolloffMode.Logarithmic; // �Ÿ��� ���� ���� ���� ���

            // �Ÿ� ������ �� �Ǿ� ������ �⺻�� ����
            if (a.minDistance <= 0f) a.minDistance = defaultMinDistance;
            if (a.maxDistance <= defaultMaxDistance) a.maxDistance = defaultMaxDistance;
        }
    }

    // ================= BGM (�������) ���� �޼��� =================

    /// <summary>
    /// ��������� ����մϴ�
    /// </summary>
    /// <param name="clip">����� ����� Ŭ��</param>
    /// <param name="loop">�ݺ� ��� ���� (�⺻: true)</param>
    /// <param name="volume">���� (0~1, �⺻: 1)</param>
    /// <param name="pitch">��ġ (0.1~3, �⺻: 1)</param>
    public void PlayBGM(AudioClip clip, bool loop = true, float volume = 1f, float pitch = 1f)
    {
        // ����� �ҽ��� Ŭ���� ������ ����
        if (!bgmSource || !clip) return;

        bgmSource.clip = clip; // ����� Ŭ�� ����
        bgmSource.loop = loop; // �ݺ� ��� ����
        bgmSource.volume = Mathf.Clamp01(volume); // ������ 0~1 ������ ����
        bgmSource.pitch = Mathf.Clamp(pitch, 0.1f, 3f); // ��ġ�� 0.1~3 ������ ����

        // �̹� ��� ���̸� �����ϰ� ���� ���, �ƴϸ� �ٷ� ���
        if (!bgmSource.isPlaying)
            bgmSource.Play();
        else
        {
            bgmSource.Stop();
            bgmSource.Play();
        }
    }

    /// <summary>
    /// ��������� �����մϴ�
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource) bgmSource.Stop();
    }

    // ================= SFX (2D ȿ����) ���� �޼��� =================

    /// <summary>
    /// 2D ȿ������ ����մϴ� (������ ���� UI ���� � ���)
    /// </summary>
    /// <param name="clip">����� ����� Ŭ��</param>
    /// <param name="volume">���� (0~1, �⺻: 1)</param>
    /// <param name="pitch">��ġ (0.1~3, �⺻: 1)</param>
    public void PlaySFX2D(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        // Ŭ���̳� ����� �ҽ� �迭�� ������ ����
        if (clip == null || sfx2DSources == null || sfx2DSources.Length == 0) return;

        // ��� ������ ����� �ҽ��� ã��
        AudioSource src = GetFreeFromArray(sfx2DSources, ref _idxSfx2D);
        if (src == null) return; // DropNewest ��å�� �� ��� �ٻڸ� ������� ����

        // ����� �ҽ� ���� �� ���
        ConfigureAndPlay(src, clip, volume, pitch, Vector3.zero, is3D: false);
    }

    // ================= SFX (3D ȿ����) ���� �޼��� =================

    /// <summary>
    /// Ư�� ��ġ���� 3D ȿ������ ����մϴ� (�߼Ҹ�, �ѼҸ� ��)
    /// </summary>
    /// <param name="clip">����� ����� Ŭ��</param>
    /// <param name="position">���尡 ����� ���� ��ǥ</param>
    /// <param name="volume">���� (0~1, �⺻: 1)</param>
    /// <param name="pitch">��ġ (0.1~3, �⺻: 1)</param>
    public void PlaySFXAt(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || sfx3DSources == null || sfx3DSources.Length == 0) return;

        AudioSource src = GetFreeFromArray(sfx3DSources, ref _idxSfx3D);
        if (src == null) return;

        ConfigureAndPlay(src, clip, volume, pitch, position, is3D: true);
    }

    // ================= Enemy (�� ���� ����) ���� �޼��� =================

    /// <summary>
    /// Ư�� ��ġ���� �� ���� 3D ���带 ����մϴ� (���� ����, �����Ҹ� ��)
    /// �� ����� ���� �������� ���� �����մϴ�
    /// </summary>
    /// <param name="clip">����� ����� Ŭ��</param>
    /// <param name="position">���尡 ����� ���� ��ǥ</param>
    /// <param name="volume">���� (0~1, �⺻: 1)</param>
    /// <param name="pitch">��ġ (0.1~3, �⺻: 1)</param>
    public void PlayEnemyAt(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || enemy3DSources == null || enemy3DSources.Length == 0) return;

        AudioSource src = GetFreeFromArray(enemy3DSources, ref _idxEnemy3D);
        if (src == null) return;

        ConfigureAndPlay(src, clip, volume, pitch, position, is3D: true);
    }

    // ================= ���� ���� ���� �޼��� =================

    /// <summary>
    /// Ư�� ������ ������ �����մϴ� (0~1 ����)
    /// </summary>
    /// <param name="bus">������ ���� ���� (Master, BGM, SFX, Enemy)</param>
    /// <param name="value01">���� �� (0~1)</param>
    /// <param name="save">PlayerPrefs�� �������� ���� (�⺻: true)</param>
    public void SetVolume01(Bus bus, float value01, bool save = true)
    {
        if (!mixer) return; // �ͼ��� ������ ����

        string p = GetParam(bus); // ������ �ش��ϴ� �Ķ���� �̸� ��������
        float dB = ToDecibel(value01); // 0~1 ���� ���ú��� ��ȯ
        mixer.SetFloat(p, dB); // �ͼ��� ���ú� �� ����

        // ���� ������ �ʿ��ϸ� PlayerPrefs�� ����
        if (save) PlayerPrefs.SetFloat(GetKey(bus), Mathf.Clamp01(value01));
    }

    /// <summary>
    /// Ư�� ������ ���� ���� ���� �����ɴϴ� (0~1 ����)
    /// </summary>
    /// <param name="bus">��ȸ�� ���� ����</param>
    /// <returns>���� ���� �� (0~1)</returns>
    public float GetVolume01(Bus bus)
    {
        // ����� ������ ������ �װ��� ���
        if (PlayerPrefs.HasKey(GetKey(bus)))
            return PlayerPrefs.GetFloat(GetKey(bus), 1f);

        // �ͼ����� ���� ���� �����ͼ� 0~1�� ��ȯ
        if (mixer && mixer.GetFloat(GetParam(bus), out float dB))
            return FromDecibel(dB);

        // �⺻�� ��ȯ
        return 1f;
    }

    /// <summary>
    /// PlayerPrefs���� ����� ���� ������ �ҷ��ͼ� �����մϴ�
    /// ���� ���� �� �ڵ����� ȣ��˴ϴ�
    /// </summary>
    public void LoadVolumes()
    {
        // �� ������ ����� ���� ���� �ҷ����� (�⺻���� �Բ� ����)
        SetVolume01(Bus.Master, PlayerPrefs.GetFloat(GetKey(Bus.Master), 0.9f), false);
        SetVolume01(Bus.BGM, PlayerPrefs.GetFloat(GetKey(Bus.BGM), 0.8f), false);
        SetVolume01(Bus.SFX, PlayerPrefs.GetFloat(GetKey(Bus.SFX), 1.0f), false);
        SetVolume01(Bus.Enemy, PlayerPrefs.GetFloat(GetKey(Bus.Enemy), 1.0f), false);
    }

    // ================= ���� ��ƿ��Ƽ �޼���� =================

    /// <summary>
    /// ����� �ҽ� �迭���� ��� ������ �ҽ��� ã�� ��ȯ�մϴ�
    /// ��� �ҽ��� ��� ���̸� ��å�� ���� ó���մϴ�
    /// </summary>
    /// <param name="arr">�˻��� ����� �ҽ� �迭</param>
    /// <param name="roundIndex">���� �κ�� �ε��� (������ ����)</param>
    /// <returns>��� ������ ����� �ҽ� �Ǵ� null</returns>
    AudioSource GetFreeFromArray(AudioSource[] arr, ref int roundIndex)
    {
        int len = arr.Length;
        int i;

        // 1�ܰ�: ��� ���� �ƴ� �ҽ��� ���� ã��
        for (i = 0; i < len; i++)
        {
            if (arr[i] != null && !arr[i].isPlaying)
                return arr[i]; // ����ִ� �ҽ� �߰� �� �ٷ� ��ȯ
        }

        // 2�ܰ�: ��� �ҽ��� ��� ���� �� ��å�� ���� ó��
        if (overflowPolicy == OverflowPolicy.DropNewest)
        {
            return null; // ���ο� ���带 ������� ����
        }

        // 3�ܰ�: StealOldest ��å - ���� �κ����� ���� ���带 �����ϰ� ����
        if (len > 0)
        {
            roundIndex = (roundIndex + 1) % len; // ���� �ε����� ��ȯ
            var src = arr[roundIndex];
            if (src != null) src.Stop(); // ���� ���� ����
            return src; // ������ �ҽ� ��ȯ
        }
        return null;
    }

    /// <summary>
    /// ����� �ҽ��� �����ϰ� ���带 ����մϴ�
    /// </summary>
    /// <param name="src">����� ����� �ҽ�</param>
    /// <param name="clip">����� ����� Ŭ��</param>
    /// <param name="volume">���� (0~1)</param>
    /// <param name="pitch">��ġ (0.1~3)</param>
    /// <param name="pos">3D ������ ����� ��ġ</param>
    /// <param name="is3D">3D ���� ����</param>
    void ConfigureAndPlay(AudioSource src, AudioClip clip, float volume, float pitch, Vector3 pos, bool is3D)
    {
        if (src == null) return;

        // 3D ����� ��ġ ����
        if (is3D) src.transform.position = pos;

        // ������ ��ġ ���� (������ ������ ����)
        src.volume = Mathf.Clamp01(volume);
        src.pitch = Mathf.Clamp(pitch, 0.1f, 3f);

        // Ŭ�� ���� �� ���
        // PlayOneShot ��� clip ���� �� Play�� ����ϴ� ����:
        // 3D ���忡�� ��ġ�� �ٲ� �� ���� ���� ��ġ�� �Բ� �����̴� ���� ����
        src.clip = clip;
        src.Play();
    }

    /// <summary>
    /// ���� ������ �ش��ϴ� �ͼ� �Ķ���� �̸��� ��ȯ�մϴ�
    /// </summary>
    /// <param name="bus">���� ����</param>
    /// <returns>�ش��ϴ� �ͼ� �Ķ���� �̸�</returns>
    string GetParam(Bus bus)
    {
        switch (bus)
        {
            case Bus.BGM: return bgmVolParam;   // "BGMVol"
            case Bus.SFX: return sfxVolParam;   // "SFXVol"  
            case Bus.Enemy: return enemyVolParam; // "EnemyVol"
            default: return masterVolParam; // "MasterVol"
        }
    }

    /// <summary>
    /// ���� ������ �ش��ϴ� PlayerPrefs Ű�� �����մϴ�
    /// </summary>
    /// <param name="bus">���� ����</param>
    /// <returns>PlayerPrefs�� ����� Ű (��: "vol.BGM")</returns>
    string GetKey(Bus bus) { return "vol." + bus; }

    /// <summary>
    /// 0~1 ������ ���� ���� ���ú��� ��ȯ�մϴ�
    /// ����� �ͼ��� ���ú� ������ ����ϹǷ� ��ȯ�� �ʿ��մϴ�
    /// </summary>
    /// <param name="v01">0~1 ������ ���� ��</param>
    /// <returns>���ú� �� (-80dB ~ 0dB)</returns>
    static float ToDecibel(float v01)
    {
        return (v01 <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Clamp01(v01)) * 20f;
    }

    /// <summary>
    /// ���ú� ���� 0~1 ������ ���� ������ ��ȯ�մϴ�
    /// </summary>
    /// <param name="dB">���ú� ��</param>
    /// <returns>0~1 ������ ���� ��</returns>
    static float FromDecibel(float dB)
    {
        return Mathf.Clamp01(Mathf.Pow(10f, dB / 20f));
    }
}
