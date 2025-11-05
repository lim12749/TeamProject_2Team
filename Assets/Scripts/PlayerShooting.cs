using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint;     // 총알 나가는 위치
    public GameObject bulletPrefab; // 총알 프리팹
    public float fireRate = 0.3f;   // 발사 간격

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetMouseButton(0)) // 마우스 좌클릭 누르고 있으면
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogWarning("FirePoint 또는 BulletPrefab이 지정되지 않았습니다!");
            return;
        }

        // 총알 생성
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
