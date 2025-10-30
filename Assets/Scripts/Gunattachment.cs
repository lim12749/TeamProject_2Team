// GunAttachment.cs
using UnityEngine;

[DisallowMultipleComponent]
public class GunAttachment : MonoBehaviour
{
    [Header("Muzzle & Bullet")]
    [SerializeField] Transform muzzle;           // 총구(없으면 "Muzzle" 찾고, 그래도 없으면 자기 Transform)
    [SerializeField] GameObject bulletPrefab;    // 생성할 총알 프리팹(필수: BulletProjectile 포함)

    [Header("Stats")]
    [SerializeField, Min(0.1f)] float fireRate = 6f; // 초당 발사수
    [SerializeField] float bulletSpeed = 60f;
    [SerializeField] float damage = 10f;
    [SerializeField] float bulletLife = 3f;
    [Tooltip("총알을 멈추게 할 레이어(몬스터+지형). Player 레이어는 제외 권장")]
    [SerializeField] LayerMask stopMask = ~0;

    public Transform Muzzle => muzzle ? muzzle : (transform.Find("Muzzle") ?? transform);
    public GameObject BulletPrefab => bulletPrefab;
    public float FireInterval => 1f / fireRate;
    public float BulletSpeed => bulletSpeed;
    public float Damage => damage;
    public float BulletLife => bulletLife;
    public LayerMask StopMask => stopMask;
}
