using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveSpawner : MonoBehaviour
{
    [Serializable]
    public class Wave
    {
        public GameObject prefab;     // 이 웨이브에서 소환할 몬스터 프리팹
        public int count = 5;         // 총 마릿수
        public float spawnInterval = 0.25f; // 개체 간 소환 간격
    }

    [Header("Spawn Points")]
    [SerializeField] Transform[] spawnPoints;     // 비워두면 자기 위치에서만 소환
    [SerializeField] Transform spawnParent;       // (선택) 여기에 자식이 곧 스폰 포인트

    [Header("Mode")]
    [SerializeField] bool useWaveList = false;    // true면 아래 waves 사용, 아니면 Auto 모드
    [SerializeField] Wave[] waves;                // 고정 웨이브 목록

    [Header("Auto Mode (useWaveList=false)")]
    [SerializeField] GameObject monsterPrefab;    // 자동 모드용 기본 프리팹
    [SerializeField] int firstCount = 4;          // 1웨이브 마릿수
    [SerializeField] int addPerWave = 2;          // 웨이브마다 증가 수
    [SerializeField] float autoSpawnInterval = 0.25f;
    [SerializeField] int totalWaves = 10;         // 0 이하면 무한

    [Header("Common")]
    [SerializeField] int maxAlive = 20;           // 동시에 살아있을 수 있는 최대치(과부하 방지)
    [SerializeField] float timeBetweenWaves = 5f; // 웨이브와 웨이브 사이 대기
    [SerializeField] bool autoStart = true;

    [Header("Optional UI")]
    [SerializeField] TMPro.TextMeshProUGUI waveText;
    [SerializeField] TMPro.TextMeshProUGUI aliveText;

    // 런타임
    public int CurrentWave { get; private set; }  // 1부터 시작
    public int Alive       { get; private set; }
    public bool Running    { get; private set; }

    public event Action<int> OnWaveStarted;   // 파라미터: wave index (1-based)
    public event Action<int> OnWaveCleared;   // 파라미터: wave index
    public event Action      OnAllWavesCleared;

    Coroutine runner;

    void OnValidate()
    {
        // spawnParent 지정 시 자식들을 자동으로 spawnPoints에 채워준다.
        if (spawnParent != null)
        {
            int n = spawnParent.childCount;
            var arr = new Transform[n];
            for (int i = 0; i < n; i++) arr[i] = spawnParent.GetChild(i);
            spawnPoints = arr;
        }
        if (maxAlive < 1) maxAlive = 1;
    }

    void Start()
    {
        if (autoStart) StartWaves();
        UpdateUI();
    }

    public void StartWaves()
    {
        if (Running) return;
        Running = true;
        runner = StartCoroutine(Run());
    }

    public void StopWaves()
    {
        if (!Running) return;
        StopCoroutine(runner);
        runner = null;
        Running = false;
    }

    IEnumerator Run()
    {
        CurrentWave = 0;
        while (true)
        {
            // 다음 웨이브 자료 구성
            GameObject prefab;
            int toSpawn;
            float interval;

            if (useWaveList)
            {
                if (waves == null || waves.Length == 0) yield break;
                if (CurrentWave >= waves.Length) break;

                var w = waves[CurrentWave];
                prefab = w.prefab ? w.prefab : monsterPrefab;
                toSpawn = Mathf.Max(0, w.count);
                interval = w.spawnInterval > 0f ? w.spawnInterval : autoSpawnInterval;
            }
            else
            {
                if (totalWaves > 0 && CurrentWave >= totalWaves) break;

                prefab = monsterPrefab;
                toSpawn = Mathf.Max(0, firstCount + addPerWave * CurrentWave);
                interval = autoSpawnInterval;
            }

            // 웨이브 증가 & 이벤트
            CurrentWave++;
            OnWaveStarted?.Invoke(CurrentWave);
            UpdateUI();

            // 스폰 코루틴
            yield return StartCoroutine(SpawnWave(prefab, toSpawn, interval));

            // 해당 웨이브가 모두 죽을 때까지 대기
            while (Alive > 0) { UpdateUI(); yield return null; }

            OnWaveCleared?.Invoke(CurrentWave);
            UpdateUI();

            // 다음 웨이브가 남았으면 휴식 시간
            bool wavesLeft = useWaveList
                ? CurrentWave < (waves?.Length ?? 0)
                : (totalWaves <= 0 || CurrentWave < totalWaves);

            if (!wavesLeft) break;
            if (timeBetweenWaves > 0f) yield return new WaitForSeconds(timeBetweenWaves);
        }

        Running = false;
        OnAllWavesCleared?.Invoke();
        UpdateUI();
    }

    IEnumerator SpawnWave(GameObject prefab, int count, float interval)
    {
        if (!prefab) yield break;
        int spawned = 0;

        while (spawned < count)
        {
            // 동시 생존 제한(서버/퍼포먼스 보호)
            while (Alive >= maxAlive) yield return null;

            // 스폰 지점 선택
            Transform sp = GetSpawnPoint();
            Vector3 pos  = sp ? sp.position : transform.position;
            Quaternion rot = sp ? sp.rotation : Quaternion.identity;

            // 생성
            var go = Instantiate(prefab, pos, rot);

            // 생존 수 관리: MonsterHealth 구독
            var mh = go.GetComponent<MonsterHealth>();
            if (mh != null)
            {
                Alive++;
                mh.OnDeath += () =>
                {
                    Alive = Mathf.Max(0, Alive - 1);
                    UpdateUI();
                };
            }
            else
            {
                // 안전장치: MonsterHealth가 없으면 바로 카운트하지 않음(관리 불가)
                Debug.LogWarning($"[WaveSpawner] '{prefab.name}'에 MonsterHealth가 없습니다. Alive 카운트 관리가 되지 않습니다.");
            }

            spawned++;
            UpdateUI();

            if (interval > 0f) yield return new WaitForSeconds(interval);
            else yield return null;
        }
    }

    Transform GetSpawnPoint()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        return null;
    }

    void UpdateUI()
    {
        if (waveText)  waveText.text  = Running ? $"Wave {CurrentWave}" : $"Wave -";
        if (aliveText) aliveText.text = $"Alive {Alive}";
    }

    void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        Gizmos.color = new Color(0, 1, 0, 0.35f);
        foreach (var t in spawnPoints)
        {
            if (!t) continue;
            Gizmos.DrawSphere(t.position, 0.3f);
        }
    }
}
