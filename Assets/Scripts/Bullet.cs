using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f; //시간되면 삭제
    public int damage = 10;

    public AudioClip ricochetClip;
    private AudioSource audioSource;
    public GameObject impactEffectPrefab; // ✅ 이펙트 프리팹

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        Destroy(gameObject, lifeTime); //3초 뒤에 삭제
    }
    /*
        void OnCollisionEnter(Collision other)
    {
        if (other.collider.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(damage);
        }
                // 2. 이펙트 재생 위치 계산
        ContactPoint contact = other.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;

        // 3. 이펙트 생성
        if (impactEffectPrefab != null)
        {
            GameObject impact = Instantiate(impactEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
            Destroy(impact, 1.5f); // 자동 제거
        }
            // 🔊 위치기반 리코쳇 사운드
        if (ricochetClip != null)
        {
            AudioSource.PlayClipAtPoint(ricochetClip, transform.position);
        }
        Destroy(gameObject);
    }*/
}
