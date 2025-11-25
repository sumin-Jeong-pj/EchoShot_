using UnityEngine;

public enum JudgeResult
{
    None,
    Fast,
    Perfect,
    Slow
}

public class JudgeSystem : MonoBehaviour
{
    [Header("기준 오브젝트")]
    public Transform core;                // Core 위치
    public SphereCollider judgeRing;      // JudgeRing의 SphereCollider (Perfect 기준선)
    public CoreHealth coreHealth;         // Core의 체력

    [Header("판정 범위 (월드 단위)")]
    public float perfectWindow = 0.2f;    // 기준선에서 ±0.2 이내
    public float fastWindow = 0.4f;       // 기준선 바깥쪽 허용 범위
    public float slowWindow = 0.4f;       // 기준선 안쪽 허용 범위
    public float maxJudgeDistance = 1.0f; // 기준선에서 이 거리 이상 벗어나면 아예 판정 안함

    [Header("미스 처리")]
    public float missRadius = 0.5f;       // 코어에서 이 거리 이내로 들어오면 Miss

    void Update()
    {
        // 1) 키 입력 판정
        if (Input.anyKeyDown)
        {
            Judge();
        }

        // 2) Miss 판정 (놓친 노트가 코어에 닿았는지)
        CheckMiss();
    }

    void Judge()
    {
        Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        if (notes.Length == 0)
        {
            Debug.Log("[JudgeSystem] 판정할 노트 없음");
            return;
        }

        float ringRadius = judgeRing.radius;

        Note bestNote = null;
        float bestScore = float.MaxValue;
        float bestDist = 0f;

        // 기준선에 가장 가까운 노트 찾기
        foreach (var n in notes)
        {
            float dist = Vector3.Distance(core.position, n.transform.position);
            float score = Mathf.Abs(dist - ringRadius);

            if (score < bestScore)
            {
                bestScore = score;
                bestNote = n;
                bestDist = dist;
            }
        }

        if (bestNote == null)
        {
            Debug.Log("[JudgeSystem] 유효한 노트 없음");
            return;
        }

        // 기준선에서 너무 멀면 판정 안함
        if (bestScore > maxJudgeDistance)
        {
            Debug.Log($"[JudgeSystem] 판정 실패: 노트가 너무 멀다 (score={bestScore:F3})");
            return;
        }

        float delta = bestDist - ringRadius; // + = 바깥, - = 안쪽
        JudgeResult result = GetResult(delta);

        if (result == JudgeResult.None)
        {
            Debug.Log($"[JudgeSystem] 판정 범위 밖 (delta={delta:F3})");
            return;
        }

        Debug.Log($"[JudgeSystem] Judge: {result}, dist={bestDist:F3}, delta={delta:F3}");

        //점수 기록
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RegisterJudge(result);
        }

        // 노트에게 '맞았다' 이벤트 전달 (이펙트 + 사운드 + 삭제)
        bestNote.Hit();

    }

    void CheckMiss()
    {
        if (coreHealth == null) return;

        Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        if (notes.Length == 0) return;

        foreach (var n in notes)
        {
            float dist = Vector3.Distance(core.position, n.transform.position);

            if (dist <= missRadius)
            {
                Debug.Log($"[JudgeSystem] Miss! 노트가 코어에 도달 (dist={dist:F3})");
                coreHealth.TakeHit(1);
                //miss 기록
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.RegisterMiss();
                }
                Destroy(n.gameObject);
                // 여러 개 한 번에 맞으면 그만큼 깎이는 게 자연스러우니 계속 진행
            }
        }
    }

    JudgeResult GetResult(float delta)
    {
        float absDelta = Mathf.Abs(delta);

        if (absDelta <= perfectWindow)
            return JudgeResult.Perfect;

        if (delta > 0 && absDelta <= perfectWindow + fastWindow)
            return JudgeResult.Fast;

        if (delta < 0 && absDelta <= perfectWindow + slowWindow)
            return JudgeResult.Slow;

        return JudgeResult.None;
    }
}
