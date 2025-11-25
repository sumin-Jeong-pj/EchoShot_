using UnityEngine;

public class Note : MonoBehaviour
{
    [Header("히트 이펙트 프리팹")]
    public GameObject hitEffectPrefab;

    [Header("히트 사운드 (선택)")]
    public AudioClip hitSound;

    public void Hit()
    {
        // 이펙트 생성
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // 사운드 재생
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // 노트 삭제
        Destroy(gameObject);
    }
}
