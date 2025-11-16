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
        Debug.Log("[CoreHealth] 체력 0! Game Over 처리 필요");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void MoveToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
