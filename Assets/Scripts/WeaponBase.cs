using UnityEngine;

//추상클래스로  모든 무기의 공통 부모 클래스로 (공통 인터페이스)
public abstract class WeaponBase : MonoBehaviour
{
    public string weaponName; //무기이름
    public float fireRate = 1f; // 발사 속도 (초당 발사 횟수)
    protected float nextFireTime = 0f; // 다음 발사 가능 시간

    public virtual bool CanFire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            return true;
        }
        return false;
    }

    public abstract void Fire(); //총알 발사 추상 메서드
}
