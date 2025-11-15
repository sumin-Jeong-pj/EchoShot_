using UnityEngine;

public class CoreHealth : MonoBehaviour
{
    [Header("하트 체력 설정")]
    public int maxHearts = 5;
    public int currentHearts;

    void Awake()
    {
        currentHearts = maxHearts;
    }

    public void TakeHit()
    {
        currentHearts = Mathf.Max(0, currentHearts - 1);
        Debug.Log($"Core Hearts: {currentHearts}/{maxHearts}");

        if (currentHearts <= 0)
        {
            OnDead();
        }
    }

    void OnDead()
    {
        Debug.Log("Core 체력 0! Game Over 처리");
        // TODO: 게임 오버 UI, 재시작 등
    }
}
