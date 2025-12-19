using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CoreHealth : MonoBehaviour
{
    [Header("하트 체력 설정")]
    public int maxHearts = 5;
    public int currentHearts;

    public Slider hpSlider;      
    public GameObject gameOverPanel;
    public AudioSource bgmAudioSource;

    public GameObject gameClearPanel;

    [Header("BGM")]
    public AudioSource gameOverBgmSource;   // 게임오버 BGM
    public AudioSource gameClearBgmSource;  // 클리어 BGM

    private bool isMusicStarted = false;
    private bool isGameEnded = false;

    void Start()
{
    if (gameOverBgmSource != null) gameOverBgmSource.Stop();
    if (gameClearBgmSource != null) gameClearBgmSource.Stop();
}

    void Awake()
    {
        currentHearts = maxHearts;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHearts;
            hpSlider.value = currentHearts;
        }

        Debug.Log($"[CoreHealth] 초기화: {currentHearts}/{maxHearts}");
    }

    void Update()
{
    if (isGameEnded || bgmAudioSource == null) return;

    // 곡이 실제로 0초 이상 진행되면 "시작했다"로 인정
    if (!isMusicStarted && bgmAudioSource.isPlaying && bgmAudioSource.time > 0.01f)
    {
        isMusicStarted = true;
    }

    // 시작한 이후에만 "끝남" 체크
    if (isMusicStarted && !bgmAudioSource.isPlaying)
    {
        OnGameClear();
    }
}


    public void TakeHit(int amount = 1)
    {
        currentHearts = Mathf.Max(0, currentHearts - amount);
        Debug.Log($"[CoreHealth] 피격! 현재 하트: {currentHearts}/{maxHearts}");

        if (hpSlider != null)
        {
            hpSlider.value = currentHearts;
        }

        if (currentHearts <= 0)
        {
            OnDead();
        }
    }

    void OnDead()
{
    if (isGameEnded) return;
    isGameEnded = true;

    Debug.Log("[CoreHealth] 체력 0! Game Over 처리 필요");

    // UI
    if (gameOverPanel != null)
    {
        ScoreManager.Instance.DisplayScore();
        gameOverPanel.SetActive(true);
    }

    // 음악: 플레이 곡 정지 -> 게임오버 브금 재생
    if (bgmAudioSource != null) bgmAudioSource.Stop();
    if (gameClearBgmSource != null) gameClearBgmSource.Stop();

    if (gameOverBgmSource != null)
    {
        gameOverBgmSource.time = 0f;
        gameOverBgmSource.Play();
    }

    Time.timeScale = 0f;
}

    void OnGameClear()
{
    if (isGameEnded) return;
    isGameEnded = true;

    Debug.Log("[CoreHealth] 노래 종료! Game Clear");

    // UI
    if (gameClearPanel != null)
    {
        ScoreManager.Instance.DisplayScore();
        gameClearPanel.SetActive(true);
    }

    // 음악: 플레이 곡은 이미 끝난 상태(또는 혹시 남아있으면 정지) -> 클리어 브금 재생
    if (bgmAudioSource != null) bgmAudioSource.Stop();
    if (gameOverBgmSource != null) gameOverBgmSource.Stop();

    if (gameClearBgmSource != null)
    {
        gameClearBgmSource.time = 0f;
        gameClearBgmSource.Play();
    }

    Time.timeScale = 0f;
}

    public void MoveToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
