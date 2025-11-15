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

    [Header("판정 범위 (월드 단위)")]
    public float perfectWindow = 0.1f;    // 기준선에서 ±0.1 이내
    public float fastWindow = 0.4f;       // 기준선 바깥쪽
    public float slowWindow = 0.4f;       // 기준선 안쪽

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Judge();
        }
    }

    void Judge()
    {
        Note closestNote = FindClosestNote();
        if (closestNote == null) return;

        float dist = Vector3.Distance(core.position, closestNote.transform.position);
        float ringRadius = judgeRing.radius;           // Perfect 기준선
        float delta = dist - ringRadius;               // + = 바깥, - = 안쪽

        JudgeResult result = GetResult(delta);

        if (result == JudgeResult.None)
        {
            Debug.Log("판정 범위 밖 (노트 안 맞음)");
            return;
        }

        Debug.Log($"Judge: {result}");

        // TODO: 점수 처리, 이펙트 등
        Destroy(closestNote.gameObject);   // 맞춘 노트 제거
    }

    Note FindClosestNote()
    {
        Note[] notes = FindObjectsOfType<Note>();
        if (notes.Length == 0) return null;

        Note best = null;
        float bestScore = float.MaxValue;

        float ringRadius = judgeRing.radius;

        foreach (var n in notes)
        {
            float d = Vector3.Distance(core.position, n.transform.position);
            float score = Mathf.Abs(d - ringRadius); // 기준선에서 얼마나 떨어졌는지
            if (score < bestScore)
            {
                bestScore = score;
                best = n;
            }
        }

        return best;
    }

    JudgeResult GetResult(float delta)
    {
        // delta = (노트 거리) - (기준선 반지름)
        float absDelta = Mathf.Abs(delta);

        if (absDelta <= perfectWindow)
            return JudgeResult.Perfect;

        // 바깥쪽 = 빠름
        if (delta > 0 && absDelta <= perfectWindow + fastWindow)
            return JudgeResult.Fast;

        // 안쪽 = 느림
        if (delta < 0 && absDelta <= perfectWindow + slowWindow)
            return JudgeResult.Slow;

        return JudgeResult.None;
    }
}
