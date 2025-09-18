/// <summary>
/// 게임의 모든 사운드를 관리하는 매니저 클래스
/// BGM, 효과음, 적 사운드를 분류하여 관리하고 볼륨 조절 기능을 제공합니다.
/// 싱글톤 패턴을 사용하여 어디서든 접근 가능합니다.
/// </summary>
using UnityEngine;
using UnityEngine.Audio;

public class SoundManagerManual_ye : MonoBehaviour
{
    /// <summary>
    /// 사운드 버스(채널) 종류를 정의하는 열거형
    /// 각각 다른 볼륨으로 제어할 수 있습니다.
    /// </summary>
    public enum Bus { Master, BGM, SFX, Enemy }

    /// <summary>
    /// 사운드 채널이 가득 찼을 때의 처리 방식
    /// DropNewest: 새로운 사운드를 재생하지 않음
    /// StealOldest: 가장 오래된 사운드를 중지하고 새 사운드 재생
    /// </summary>
    public enum OverflowPolicy { DropNewest, StealOldest }

    /// <summary>
    /// 싱글톤 인스턴스 - 게임 전체에서 하나만 존재
    /// 어디서든 SoundManagerManual.Instance로 접근 가능
    /// </summary>
    public static SoundManagerManual_ye Instance { get; private set; }

    [Header("Mixer & Groups (옵션)")]
    [Tooltip("오디오 믹서 - 볼륨 조절을 위해 사용")]
    [SerializeField] AudioMixer mixer;
    [Tooltip("마스터 볼륨 파라미터 이름")]
    [SerializeField] string masterVolParam = "MasterVol";
    [Tooltip("BGM 볼륨 파라미터 이름")]
    [SerializeField] string bgmVolParam = "BGMVol";
    [Tooltip("효과음 볼륨 파라미터 이름")]
    [SerializeField] string sfxVolParam = "SFXVol";
    [Tooltip("적 사운드 볼륨 파라미터 이름")]
    [SerializeField] string enemyVolParam = "EnemyVol";

    [Header("Audio Sources (직접 드래그)")]
    [Tooltip("배경음악용 오디오 소스 (2D, 하나만 사용)")]
    [SerializeField] AudioSource bgmSource;        // 2D, Output=BGM 그룹
    [Tooltip("2D 효과음용 오디오 소스 배열 (여러 개 동시 재생 가능)")]
    [SerializeField] AudioSource[] sfx2DSources;   // 2D, Output=SFX 그룹
    [Tooltip("3D 효과음용 오디오 소스 배열 (공간감 있는 사운드)")]
    [SerializeField] AudioSource[] sfx3DSources;   // 3D, Output=SFX 그룹
    [Tooltip("적 전용 3D 오디오 소스 배열")]
    [SerializeField] AudioSource[] enemy3DSources; // 3D, Output=Enemy 그룹

    [Header("3D 기본 설정")]
    [Tooltip("3D 사운드가 최대 볼륨으로 들리는 최소 거리")]
    [SerializeField] float defaultMinDistance = 1f;
    [Tooltip("3D 사운드가 들리지 않게 되는 최대 거리")]
    [SerializeField] float defaultMaxDistance = 40f;

    [Header("동시재생 가득 찼을 때 정책")]
    [Tooltip("모든 오디오 소스가 사용 중일 때의 처리 방식")]
    [SerializeField] OverflowPolicy overflowPolicy = OverflowPolicy.StealOldest;

    // 라운드 로빈 방식으로 오디오 소스를 순환 사용하기 위한 인덱스
    int _idxSfx2D, _idxSfx3D, _idxEnemy3D;

    /// <summary>
    /// 컴포넌트 초기화 - 싱글톤 설정 및 기본 설정 적용
    /// </summary>
    void Awake()
    {
        // 싱글톤 패턴 구현 - 이미 인스턴스가 있으면 자신을 제거
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // 씬이 바뀌어도 이 오브젝트는 파괴되지 않음 (BGM 연속 재생을 위해)
        DontDestroyOnLoad(gameObject);

        // 3D 사운드 소스들의 기본 설정 적용
        Apply3DDefaults(sfx3DSources);
        Apply3DDefaults(enemy3DSources);

        // 저장된 볼륨 설정 불러오기
        LoadVolumes();
    }

    /// <summary>
    /// 3D 오디오 소스 배열에 기본 3D 설정을 적용합니다
    /// </summary>
    /// <param name="arr">설정할 오디오 소스 배열</param>
    void Apply3DDefaults(AudioSource[] arr)
    {
        if (arr == null) return;

        for (int i = 0; i < arr.Length; i++)
        {
            var a = arr[i];
            if (!a) continue; // null 체크

            a.spatialBlend = 1f; // 완전한 3D 사운드 (0=2D, 1=3D)
            a.rolloffMode = AudioRolloffMode.Logarithmic; // 거리에 따른 볼륨 감소 방식

            // 거리 설정이 안 되어 있으면 기본값 적용
            if (a.minDistance <= 0f) a.minDistance = defaultMinDistance;
            if (a.maxDistance <= defaultMaxDistance) a.maxDistance = defaultMaxDistance;
        }
    }

    // ================= BGM (배경음악) 관련 메서드 =================

    /// <summary>
    /// 배경음악을 재생합니다
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    /// <param name="loop">반복 재생 여부 (기본: true)</param>
    /// <param name="volume">볼륨 (0~1, 기본: 1)</param>
    /// <param name="pitch">피치 (0.1~3, 기본: 1)</param>
    public void PlayBGM(AudioClip clip, bool loop = true, float volume = 1f, float pitch = 1f)
    {
        // 오디오 소스나 클립이 없으면 종료
        if (!bgmSource || !clip) return;

        bgmSource.clip = clip; // 재생할 클립 설정
        bgmSource.loop = loop; // 반복 재생 설정
        bgmSource.volume = Mathf.Clamp01(volume); // 볼륨을 0~1 범위로 제한
        bgmSource.pitch = Mathf.Clamp(pitch, 0.1f, 3f); // 피치를 0.1~3 범위로 제한

        // 이미 재생 중이면 중지하고 새로 재생, 아니면 바로 재생
        if (!bgmSource.isPlaying)
            bgmSource.Play();
        else
        {
            bgmSource.Stop();
            bgmSource.Play();
        }
    }

    /// <summary>
    /// 배경음악을 중지합니다
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource) bgmSource.Stop();
    }

    // ================= SFX (2D 효과음) 관련 메서드 =================

    /// <summary>
    /// 2D 효과음을 재생합니다 (공간감 없는 UI 사운드 등에 사용)
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    /// <param name="volume">볼륨 (0~1, 기본: 1)</param>
    /// <param name="pitch">피치 (0.1~3, 기본: 1)</param>
    public void PlaySFX2D(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        // 클립이나 오디오 소스 배열이 없으면 종료
        if (clip == null || sfx2DSources == null || sfx2DSources.Length == 0) return;

        // 사용 가능한 오디오 소스를 찾음
        AudioSource src = GetFreeFromArray(sfx2DSources, ref _idxSfx2D);
        if (src == null) return; // DropNewest 정책일 때 모두 바쁘면 재생하지 않음

        // 오디오 소스 설정 후 재생
        ConfigureAndPlay(src, clip, volume, pitch, Vector3.zero, is3D: false);
    }

    // ================= SFX (3D 효과음) 관련 메서드 =================

    /// <summary>
    /// 특정 위치에서 3D 효과음을 재생합니다 (발소리, 총소리 등)
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    /// <param name="position">사운드가 재생될 월드 좌표</param>
    /// <param name="volume">볼륨 (0~1, 기본: 1)</param>
    /// <param name="pitch">피치 (0.1~3, 기본: 1)</param>
    public void PlaySFXAt(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || sfx3DSources == null || sfx3DSources.Length == 0) return;

        AudioSource src = GetFreeFromArray(sfx3DSources, ref _idxSfx3D);
        if (src == null) return;

        ConfigureAndPlay(src, clip, volume, pitch, position, is3D: true);
    }

    // ================= Enemy (적 전용 사운드) 관련 메서드 =================

    /// <summary>
    /// 특정 위치에서 적 전용 3D 사운드를 재생합니다 (적의 공격, 울음소리 등)
    /// 적 사운드는 별도 볼륨으로 조절 가능합니다
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    /// <param name="position">사운드가 재생될 월드 좌표</param>
    /// <param name="volume">볼륨 (0~1, 기본: 1)</param>
    /// <param name="pitch">피치 (0.1~3, 기본: 1)</param>
    public void PlayEnemyAt(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || enemy3DSources == null || enemy3DSources.Length == 0) return;

        AudioSource src = GetFreeFromArray(enemy3DSources, ref _idxEnemy3D);
        if (src == null) return;

        ConfigureAndPlay(src, clip, volume, pitch, position, is3D: true);
    }

    // ================= 볼륨 조절 관련 메서드 =================

    /// <summary>
    /// 특정 버스의 볼륨을 설정합니다 (0~1 범위)
    /// </summary>
    /// <param name="bus">설정할 사운드 버스 (Master, BGM, SFX, Enemy)</param>
    /// <param name="value01">볼륨 값 (0~1)</param>
    /// <param name="save">PlayerPrefs에 저장할지 여부 (기본: true)</param>
    public void SetVolume01(Bus bus, float value01, bool save = true)
    {
        if (!mixer) return; // 믹서가 없으면 종료

        string p = GetParam(bus); // 버스에 해당하는 파라미터 이름 가져오기
        float dB = ToDecibel(value01); // 0~1 값을 데시벨로 변환
        mixer.SetFloat(p, dB); // 믹서에 데시벨 값 적용

        // 설정 저장이 필요하면 PlayerPrefs에 저장
        if (save) PlayerPrefs.SetFloat(GetKey(bus), Mathf.Clamp01(value01));
    }

    /// <summary>
    /// 특정 버스의 현재 볼륨 값을 가져옵니다 (0~1 범위)
    /// </summary>
    /// <param name="bus">조회할 사운드 버스</param>
    /// <returns>현재 볼륨 값 (0~1)</returns>
    public float GetVolume01(Bus bus)
    {
        // 저장된 설정이 있으면 그것을 사용
        if (PlayerPrefs.HasKey(GetKey(bus)))
            return PlayerPrefs.GetFloat(GetKey(bus), 1f);

        // 믹서에서 현재 값을 가져와서 0~1로 변환
        if (mixer && mixer.GetFloat(GetParam(bus), out float dB))
            return FromDecibel(dB);

        // 기본값 반환
        return 1f;
    }

    /// <summary>
    /// PlayerPrefs에서 저장된 볼륨 설정을 불러와서 적용합니다
    /// 게임 시작 시 자동으로 호출됩니다
    /// </summary>
    public void LoadVolumes()
    {
        // 각 버스의 저장된 볼륨 설정 불러오기 (기본값도 함께 설정)
        SetVolume01(Bus.Master, PlayerPrefs.GetFloat(GetKey(Bus.Master), 0.9f), false);
        SetVolume01(Bus.BGM, PlayerPrefs.GetFloat(GetKey(Bus.BGM), 0.8f), false);
        SetVolume01(Bus.SFX, PlayerPrefs.GetFloat(GetKey(Bus.SFX), 1.0f), false);
        SetVolume01(Bus.Enemy, PlayerPrefs.GetFloat(GetKey(Bus.Enemy), 1.0f), false);
    }

    // ================= 내부 유틸리티 메서드들 =================

    /// <summary>
    /// 오디오 소스 배열에서 사용 가능한 소스를 찾아 반환합니다
    /// 모든 소스가 사용 중이면 정책에 따라 처리합니다
    /// </summary>
    /// <param name="arr">검색할 오디오 소스 배열</param>
    /// <param name="roundIndex">라운드 로빈용 인덱스 (참조로 전달)</param>
    /// <returns>사용 가능한 오디오 소스 또는 null</returns>
    AudioSource GetFreeFromArray(AudioSource[] arr, ref int roundIndex)
    {
        int len = arr.Length;
        int i;

        // 1단계: 재생 중이 아닌 소스를 먼저 찾기
        for (i = 0; i < len; i++)
        {
            if (arr[i] != null && !arr[i].isPlaying)
                return arr[i]; // 비어있는 소스 발견 시 바로 반환
        }

        // 2단계: 모든 소스가 사용 중일 때 정책에 따른 처리
        if (overflowPolicy == OverflowPolicy.DropNewest)
        {
            return null; // 새로운 사운드를 재생하지 않음
        }

        // 3단계: StealOldest 정책 - 라운드 로빈으로 기존 사운드를 중지하고 재사용
        if (len > 0)
        {
            roundIndex = (roundIndex + 1) % len; // 다음 인덱스로 순환
            var src = arr[roundIndex];
            if (src != null) src.Stop(); // 기존 사운드 중지
            return src; // 재사용할 소스 반환
        }
        return null;
    }

    /// <summary>
    /// 오디오 소스를 설정하고 사운드를 재생합니다
    /// </summary>
    /// <param name="src">사용할 오디오 소스</param>
    /// <param name="clip">재생할 오디오 클립</param>
    /// <param name="volume">볼륨 (0~1)</param>
    /// <param name="pitch">피치 (0.1~3)</param>
    /// <param name="pos">3D 사운드일 경우의 위치</param>
    /// <param name="is3D">3D 사운드 여부</param>
    void ConfigureAndPlay(AudioSource src, AudioClip clip, float volume, float pitch, Vector3 pos, bool is3D)
    {
        if (src == null) return;

        // 3D 사운드면 위치 설정
        if (is3D) src.transform.position = pos;

        // 볼륨과 피치 설정 (안전한 범위로 제한)
        src.volume = Mathf.Clamp01(volume);
        src.pitch = Mathf.Clamp(pitch, 0.1f, 3f);

        // 클립 설정 후 재생
        // PlayOneShot 대신 clip 설정 후 Play를 사용하는 이유:
        // 3D 사운드에서 위치가 바뀔 때 이전 사운드 위치가 함께 움직이는 것을 방지
        src.clip = clip;
        src.Play();
    }

    /// <summary>
    /// 사운드 버스에 해당하는 믹서 파라미터 이름을 반환합니다
    /// </summary>
    /// <param name="bus">사운드 버스</param>
    /// <returns>해당하는 믹서 파라미터 이름</returns>
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
    /// 사운드 버스에 해당하는 PlayerPrefs 키를 생성합니다
    /// </summary>
    /// <param name="bus">사운드 버스</param>
    /// <returns>PlayerPrefs에 사용할 키 (예: "vol.BGM")</returns>
    string GetKey(Bus bus) { return "vol." + bus; }

    /// <summary>
    /// 0~1 범위의 볼륨 값을 데시벨로 변환합니다
    /// 오디오 믹서는 데시벨 단위를 사용하므로 변환이 필요합니다
    /// </summary>
    /// <param name="v01">0~1 범위의 볼륨 값</param>
    /// <returns>데시벨 값 (-80dB ~ 0dB)</returns>
    static float ToDecibel(float v01)
    {
        return (v01 <= 0.0001f) ? -80f : Mathf.Log10(Mathf.Clamp01(v01)) * 20f;
    }

    /// <summary>
    /// 데시벨 값을 0~1 범위의 볼륨 값으로 변환합니다
    /// </summary>
    /// <param name="dB">데시벨 값</param>
    /// <returns>0~1 범위의 볼륨 값</returns>
    static float FromDecibel(float dB)
    {
        return Mathf.Clamp01(Mathf.Pow(10f, dB / 20f));
    }
}
