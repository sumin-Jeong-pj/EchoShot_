using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("전체 노트 수 (n)")]
    public int totalNotes = 0; 

    [Header("판정별 카운트")]
    [SerializeField] private int perfectCount = 0;
    [SerializeField] private int fastSlowCount = 0;
    [SerializeField] private int missCount = 0;

    public int PerfectCount => perfectCount;
    public int FastSlowCount => fastSlowCount;
    public int MissCount => missCount;

    public long TotalScore { get; private set; }

    public TMP_Text gameClearScoreText;
    public TMP_Text gameOverScoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 게임 시작 시 총 노트 개수 설정
    public void SetTotalNotes(int count)
    {
        totalNotes = count;
    }

    // JudgeSystem에서 호출: Fast / Perfect / Slow 판정 등록
    public void RegisterJudge(JudgeResult result)
    {
        switch (result)
        {
            case JudgeResult.Perfect:
                perfectCount++;
                break;

            case JudgeResult.Fast:
            case JudgeResult.Slow:
                fastSlowCount++;
                break;

            case JudgeResult.None:
            default:
                // 점수에 포함 안함
                break;
        }

        LogCurrentScore();
    }

    // Miss 발생 시 JudgeSystem에서 호출
    public void RegisterMiss()
    {
        missCount++;
        LogCurrentScore();
    }

    // 점수 계산: (perfectCount + fastSlowCount/2) / totalNotes * 100
    public float GetScorePercent()
    {
        if (totalNotes <= 0)
            return 0f;

        float a = perfectCount;
        float b = fastSlowCount;

        float score = (a + b * 0.5f) / totalNotes * 100f;
        return Mathf.Clamp(score, 0f, 100f);
    }

    //점수 띄우기
    public void DisplayScore()
    {
        Debug.Log($"[ScoreManager] Final Score: {GetScorePercent():F2}%");
        float scorePercent = GetScorePercent();
        gameClearScoreText.text = $"Score: {scorePercent:F2}%";
        gameOverScoreText.text = $"Score: {scorePercent:F2}%";
        ResetScore();


    }   

    // 점수/카운트 리셋
    public void ResetScore(int newTotalNotes = -1)
    {
        perfectCount = 0;
        fastSlowCount = 0;
        missCount = 0;

        if (newTotalNotes >= 0)
            totalNotes = newTotalNotes;
    }

    // 디버그용
    private void LogCurrentScore()
    {
        float score = GetScorePercent();
        Debug.Log(
            $"[ScoreManager] Perfect={perfectCount}, Fast/Slow={fastSlowCount}, Miss={missCount}, " +
            $"Total={totalNotes}, Score={score:F2}"
        );
    }
}