using UnityEngine;

public class CoreHealth : MonoBehaviour
{
    [Header("하트 체력 설정")]
    public int maxHearts = 5;
    public int currentHearts;

    void Awake()
    {
        currentHearts = maxHearts;
        Debug.Log($"[CoreHealth] 초기화: {currentHearts}/{maxHearts}");
    }

    public void TakeHit(int amount = 1)
    {
        currentHearts = Mathf.Max(0, currentHearts - amount);
        Debug.Log($"[CoreHealth] 피격! 현재 하트: {currentHearts}/{maxHearts}");

        if (currentHearts <= 0)
        {
            OnDead();
        }
    }

    void OnDead()
    {
        Debug.Log("[CoreHealth] 체력 0! Game Over 처리 필요");
        // TODO: 게임 오버 UI or 씬 전환
    }
}
