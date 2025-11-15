using UnityEngine;

public class NoteHitDetector : MonoBehaviour
{
    public float coreHitRadius = 0.5f;   // 코어에 닿았다고 보는 거리
    private Transform core;
    private CoreHealth coreHealth;
    private bool alreadyHit = false;     // 중복 히트 방지

    void Start()
    {
        GameObject coreObj = GameObject.FindWithTag("Core");
        if (coreObj != null)
        {
            core = coreObj.transform;
            coreHealth = coreObj.GetComponent<CoreHealth>();
        }
        else
        {
            Debug.LogError("[NoteHitDetector] Core 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (alreadyHit || core == null) return;

        float dist = Vector3.Distance(transform.position, core.position);

        if (dist <= coreHitRadius)
        {
            alreadyHit = true;
            Debug.Log($"[NoteHitDetector] 노트가 코어에 도달! dist={dist:F3}");

            if (coreHealth != null)
            {
                coreHealth.TakeHit(1);
            }

            Destroy(gameObject);
        }
    }
}
