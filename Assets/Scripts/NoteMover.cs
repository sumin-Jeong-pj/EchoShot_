using UnityEngine;

public class NoteMover : MonoBehaviour
{
    public float speed = 3f;      // 노트 이동 속도
    private Transform core;       // 목표(코어) 위치

    void Start()
    {
        // "Core" 태그를 가진 오브젝트 찾아서 Transform 가져오기
        GameObject coreObj = GameObject.FindWithTag("Core");
        if (coreObj != null)
        {
            core = coreObj.transform;
        }
        else
        {
            Debug.LogError("Core 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        if (core == null) return;

        // Core 쪽으로 이동
        transform.position = Vector3.MoveTowards(
            transform.position,
            core.position,
            speed * Time.deltaTime
        );
    }
}
