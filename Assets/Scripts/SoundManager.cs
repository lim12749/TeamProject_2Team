using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public enum Bus { Master, BGM, SFX, Enemy }

    public static SoundManager Instance { get; private set; }

    [Header("Mixer & Groups")]
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioMixerGroup masterGroup;
    [SerializeField] AudioMixerGroup bgmGroup;
    [SerializeField] AudioMixerGroup sfxGroup;
    [SerializeField] AudioMixerGroup enemyGroup;

    [Header("Exposed Param Names (Mixer)")]
    [SerializeField] string masterVolParam = "MasterVol";
    [SerializeField] string bgmVolParam    = "BGMVol";
    [SerializeField] string sfxVolParam    = "SFXVol";
    [SerializeField] string enemyVolParam  = "EnemyVol";

    [Header("BGM")]
    [SerializeField] AudioSource bgmSource; // 2D AudioSource(필수), Output=bgmGroup

    [Header("SFX Pool")]
    [SerializeField] int poolSize = 16;
    [SerializeField] float defaultMaxDistance = 40f;

    readonly List<AudioSource> pool = new List<AudioSource>();
    Transform poolRoot;

    // -------- LifeCycle --------
    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // BGM 소스 자동 생성
        if (!bgmSource)
        {
            var go = new GameObject("BGM_Source");
            go.transform.SetParent(transform);
            bgmSource = go.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            bgmSource.spatialBlend = 0f;
            bgmSource.outputAudioMixerGroup = bgmGroup;
        }

        // SFX 풀 루트
        poolRoot = new GameObject("SFX_Pool").transform;
        poolRoot.SetParent(transform);

        // 볼륨 로드
        LoadVolumes();
    }

    // -------- Public: BGM --------
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (!clip) return;
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        if (bgmGroup) bgmSource.outputAudioMixerGroup = bgmGroup;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();

    // -------- Public: SFX --------
    public void PlaySFX2D(AudioClip clip, Bus bus = Bus.SFX, float volume = 1f, float pitch = 1f)
    {
        if (!clip) return;
        var src = GetFreeSource(bus, spatial:false);
        ConfigureAndPlay(src, clip, volume, pitch, Vector3.zero, spatial:false);
    }

    public void PlaySFXAt(AudioClip clip, Vector3 position, Bus bus = Bus.SFX, float volume = 1f, float pitch = 1f)
    {
        if (!clip) return;
        var src = GetFreeSource(bus, spatial:true);
        ConfigureAndPlay(src, clip, volume, pitch, position, spatial:true);
    }

    // -------- Public: Volume (0~1) --------
    public void SetVolume01(Bus bus, float value01, bool save = true)
    {
        string param = GetParamName(bus);
        float dB = ToDecibel(value01);
        if (mixer) mixer.SetFloat(param, dB);
        if (save) PlayerPrefs.SetFloat(GetPrefKey(bus), Mathf.Clamp01(value01));
    }

    public float GetVolume01(Bus bus)
    {
        string param = GetParamName(bus);
        if (mixer && mixer.GetFloat(param, out float dB))
            return FromDecibel(dB);
        // 저장값이 있으면 반환
        if (PlayerPrefs.HasKey(GetPrefKey(bus)))
            return PlayerPrefs.GetFloat(GetPrefKey(bus), 1f);
        return 1f;
    }

    public void LoadVolumes()
    {
        SetVolume01(Bus.Master, PlayerPrefs.GetFloat(GetPrefKey(Bus.Master), 0.9f), save:false);
        SetVolume01(Bus.BGM,    PlayerPrefs.GetFloat(GetPrefKey(Bus.BGM),    0.8f), save:false);
        SetVolume01(Bus.SFX,    PlayerPrefs.GetFloat(GetPrefKey(Bus.SFX),    1.0f), save:false);
        SetVolume01(Bus.Enemy,  PlayerPrefs.GetFloat(GetPrefKey(Bus.Enemy),  1.0f), save:false);
    }

    // -------- Helpers --------
    AudioSource GetFreeSource(Bus bus, bool spatial)
    {
        // 빈 소스 찾기
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].isPlaying)
            {
                SetupGroup(pool[i], bus, spatial);
                return pool[i];
            }
        }
        // 풀 크기 미달이면 생성
        if (pool.Count < poolSize)
        {
            var go = new GameObject($"SFX_{pool.Count}");
            go.transform.SetParent(poolRoot);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.spatialBlend = spatial ? 1f : 0f;
            src.rolloffMode = AudioRolloffMode.Logarithmic;
            src.minDistance = 1f;
            src.maxDistance = defaultMaxDistance;
            SetupGroup(src, bus, spatial);
            pool.Add(src);
            return src;
        }
        // 모두 바쁠 경우 0번 재사용 (간단 모드)
        var reuse = pool[0];
        reuse.Stop();
        SetupGroup(reuse, bus, spatial);
        return reuse;
    }

    void SetupGroup(AudioSource src, Bus bus, bool spatial)
    {
        src.outputAudioMixerGroup = GetGroup(bus);
        src.spatialBlend = spatial ? 1f : 0f;
    }

    void ConfigureAndPlay(AudioSource src, AudioClip clip, float volume, float pitch, Vector3 pos, bool spatial)
    {
        if (spatial) src.transform.position = pos;
        src.volume = Mathf.Clamp01(volume);
        src.pitch  = Mathf.Clamp(pitch, 0.1f, 3f);
        src.clip   = clip;
        src.Play();
    }

    AudioMixerGroup GetGroup(Bus bus)
    {
        return bus switch
        {
            Bus.BGM   => bgmGroup ? bgmGroup : masterGroup,
            Bus.SFX   => sfxGroup ? sfxGroup : masterGroup,
            Bus.Enemy => enemyGroup ? enemyGroup : (sfxGroup ? sfxGroup : masterGroup),
            _         => masterGroup
        };
    }

    string GetParamName(Bus bus) => bus switch
    {
        Bus.Master => masterVolParam,
        Bus.BGM    => bgmVolParam,
        Bus.SFX    => sfxVolParam,
        Bus.Enemy  => enemyVolParam,
        _          => masterVolParam
    };

    string GetPrefKey(Bus bus) => $"vol.{bus}";

    // 0~1 -> dB, 0은 -80dB로 처리(음소거 근사)
    static float ToDecibel(float v01) => v01 <= 0.0001f ? -80f : Mathf.Log10(Mathf.Clamp01(v01)) * 20f;
    static float FromDecibel(float dB) => Mathf.Clamp01(Mathf.Pow(10f, dB / 20f));
}
