using UnityEngine;

public class Gun : MonoBehaviour
{
    public static Gun instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("총알 프리팹")]
    public GameObject bulletPrefab;

    [Header("총구 위치")]
    public Transform firePoint;

    [Header("발사 간격 (초)")]
    public float fireRate = 0.2f;

    private float lastFireTime = 0f;

    public void TryFire()
    {
        if (Time.time - lastFireTime < fireRate)
            return;

        Fire();
        lastFireTime = Time.time;
    }

    void Fire()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("총알 프리팹 또는 발사 위치가 설정되지 않았습니다!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Debug.Log("🔫 총알 발사!");
    }
}
