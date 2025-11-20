using UnityEngine;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    [Header("채보 파일")]
    public string chartFileName = "echoshot_3"; // Resources/Charts/asdf.json

    [Header("필수 설정")]
    public Transform core;            // 중심 Core (없으면 태그로 자동 찾기)
    public SphereCollider judgeRing;     // JudgeRing (Perfect 기준선)

    [Header("스폰 설정")]
    public float spawnRadius = 8f;    // 코어 기준 얼마나 멀리에서 스폰할지
    //public float spawnInterval = 1f;  // 노트 생성 간격(초)

    [Header("노트 프리팹")]
    public List<NotePrefabEntry> notePrefabs; 

    private float travelTime = 0f;    // 노트가 스폰 지점에서 코어까지 도달하는 데 걸리는 시간

    private AudioSource audioSource;
    private ChartJsonData chart;
    private List<RuntimeNote> runtimeNotes;
    private int spawnIndex = 0;

    [System.Serializable]
    private class RuntimeNote
    {
        public NoteJsonData raw;
        public float hitTime;    // 판정선 도달 시각(초)
        public float spawnTime;  // 실제 스폰 시각(초)
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Core가 지정 안 되어 있으면 태그로 자동 찾기
        if (core == null)
        {
            GameObject coreObj = GameObject.FindWithTag("Core");
            if (coreObj != null)
                core = coreObj.transform;
            else
                Debug.LogError("[ChartNoteSpawner] Core 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void Start()
    {
        // 1) JSON 로드
        TextAsset jsonText = Resources.Load<TextAsset>($"Charts/{chartFileName}");
        if (jsonText == null)
        {
            Debug.LogError($"[ChartNoteSpawner] Charts/{chartFileName}.json 을 찾을 수 없습니다.");
            return;
        }

        // 2) JSON 파싱
        chart = JsonUtility.FromJson<ChartJsonData>(jsonText.text);
        if (chart == null || chart.notes == null || chart.notes.Count == 0)
        {
            Debug.LogError("[ChartNoteSpawner] 채보 데이터가 비어있습니다.");
            return;
        }

        // 3) 오디오 설정
        if (!string.IsNullOrEmpty(chart.audioClipName))
        {
            AudioClip clip = Resources.Load<AudioClip>($"Audio/{chart.audioClipName}");
            if (clip != null)
            {
                audioSource.clip = clip;
            }
            else
            {
                Debug.LogError($"[ChartNoteSpawner] Audio/{chart.audioClipName} 클립을 찾을 수 없습니다.");
            }
        }

        // 4) 이동 시간 계산
        float distance = spawnRadius - judgeRing.radius;
        float speed = chart.noteSpeed;
        travelTime = distance / speed;

        // 5) 런타임 노트 리스트 구성
        BuildRuntimeNotes();

        // 6) 음악 재생 시작
        audioSource.Play();
    }

    void Update()
    {
        if (chart == null || runtimeNotes == null) return;
        if (spawnIndex >= runtimeNotes.Count) return;

        float songTime = audioSource.time;

        // 현재 songTime이 spawnTime을 넘어간 노트들을 차례로 스폰
        while (spawnIndex < runtimeNotes.Count && songTime >= runtimeNotes[spawnIndex].spawnTime)
        {
            SpawnNote(runtimeNotes[spawnIndex]);
            spawnIndex++;
        }
    }

    void BuildRuntimeNotes() //노트 비트를 시간으로 변환
    {
        runtimeNotes = new List<RuntimeNote>();

        float secPerBeat = 60f / chart.bpm;

        foreach (var n in chart.notes)
        {
            RuntimeNote rn = new RuntimeNote();
            rn.raw = n;

            float beatsFromStart = n.bar * chart.beatsPerBar + n.beat;
            float hitTime = chart.offsetSeconds + beatsFromStart * secPerBeat;
            float spawnTime = hitTime - travelTime;

            rn.hitTime = hitTime;
            rn.spawnTime = spawnTime;

            runtimeNotes.Add(rn);
        }

        // time 순으로 정렬 (JSON이 시간 순서가 아니어도 문제 없게 함)
        runtimeNotes.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }

    void SpawnNote(RuntimeNote runtimeNote)
    {
        NoteJsonData n = runtimeNote.raw;

        // 1) 각도 결정 (JSON에 음수가 들어오면 랜덤)
        float angleDeg = n.angleDeg;
        if (angleDeg < 0f)
        {
            angleDeg = Random.Range(0f, 360f);
        }

        float rad = angleDeg * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
        Vector3 spawnPos = core.position + dir * spawnRadius;

        // 2) 타입별 프리팹 선택
        GameObject prefab = GetPrefabForType(n.type);
        if (prefab == null)
        {
            Debug.LogError($"[ChartNoteSpawner] {n.type} 타입에 해당하는 프리팹이 없습니다.");
            return;
        }

        // 3) 노트 생성
        GameObject noteObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        noteObj.transform.LookAt(core.position);

        // 4) 속도 세팅 (기존 NoteMover 사용)
        NoteMover mover = noteObj.GetComponent<NoteMover>();
        if (mover != null)
        {
            mover.speed = chart.noteSpeed;
        }

        // 5) HoldStart / HoldEnd / Heal / Bomb 등은
        //    이후에 Note 스크립트 확장해서 타입별 행동 넣으면 됨
        //    (예: Note.cs에 NoteType 필드 추가 후 세팅)
        Note note = noteObj.GetComponent<Note>();
        if (note != null)
        {
            // 여기에 note.type = n.type; 같은 필드 추가해서 넘겨도 됨
        }
    }

    GameObject GetPrefabForType(NoteType type) // 노트 타입에 맞는 프리팹 반환
    {
        foreach (var e in notePrefabs)
        {
            if (e.type == type)
                return e.prefab;
        }
        return null;
    }
}
