using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("필수 설정")]
    public GameObject notePrefab;     // 노트 프리팹
    public Transform core;            // 중심 Core (없으면 태그로 자동 찾기)

    [Header("스폰 설정")]
    public float spawnRadius = 8f;    // 코어 기준 얼마나 멀리에서 스폰할지
    public float spawnInterval = 1f;  // 노트 생성 간격(초)

    private float timer = 0f;

    void Start()
    {
        // Core가 지정 안 되어 있으면 태그로 자동 찾기
        if (core == null)
        {
            GameObject coreObj = GameObject.FindWithTag("Core");
            if (coreObj != null)
                core = coreObj.transform;
            else
                Debug.LogError("Core 태그를 가진 오브젝트를 찾을 수 없습니다! NoteSpawner에서 core를 수동으로 지정하세요.");
        }
    }

    void Update()
    {
        if (core == null || notePrefab == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnNote();
        }
    }

    void SpawnNote()
    {
        // 0 ~ 2π 사이의 랜덤 각도 (사방에서 날아오게)
        float angle = Random.Range(0f, Mathf.PI * 2f);

        // XZ 평면에서 방향 벡터
        Vector3 dir = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

        // 코어에서 spawnRadius 만큼 떨어진 위치
        Vector3 spawnPos = core.position + dir * spawnRadius;

        // 노트 생성
        GameObject noteObj = Instantiate(notePrefab, spawnPos, Quaternion.identity);

        // 선택: 노트가 Core 쪽을 바라보게 회전
        noteObj.transform.LookAt(core.position);
    }
}
