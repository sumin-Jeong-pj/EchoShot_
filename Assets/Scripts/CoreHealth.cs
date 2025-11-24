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

    private bool isMusicStarted = false;
    private bool isGameEnded = false;

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

        if (!isMusicStarted && bgmAudioSource.isPlaying)
        {
            isMusicStarted = true;
        }

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
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
        }
        Time.timeScale = 0f;
    }

    void OnGameClear()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        Debug.Log("[CoreHealth] 노래 종료! Game Clear");

        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(true);
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
