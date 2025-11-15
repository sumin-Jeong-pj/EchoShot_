using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CoreDamageZone : MonoBehaviour
{
    public CoreHealth coreHealth;

    private void Awake()
    {
        if (coreHealth == null)
        {
            coreHealth = GetComponent<CoreHealth>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            Debug.Log("노트가 Core에 닿음! 체력 감소");
            if (coreHealth != null)
            {
                coreHealth.TakeHit();
            }

            Destroy(other.gameObject);  // 노트 제거
        }
    }
}

