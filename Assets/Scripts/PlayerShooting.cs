using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint;     
    public GameObject bulletPrefab; 
    public float fireRate = 0.3f;   

    private float nextFireTime = 0f;

    void Start()
    {
        if (RuntimeManager.Instance != null)
        {
            fireRate = RuntimeManager.Instance.fireRate;
        }
    }

    public void Shoot()
    {
        if (firePoint == null || bulletPrefab == null)
        {
            Debug.LogWarning("FirePoint 또는 BulletPrefab이 지정되지 않았습니다!");
            return;
        }

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    public bool TryShoot()
    {
        float effectiveFireRate = fireRate;
        if (RuntimeManager.Instance != null)
            effectiveFireRate = RuntimeManager.Instance.fireRate;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + effectiveFireRate;
            return true;
        }

        return false;
    }
}
