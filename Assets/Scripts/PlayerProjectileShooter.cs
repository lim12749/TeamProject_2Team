using UnityEngine;
using UnityEngine.InputSystem; // Mouse fallback 용

// PlayerInput를 여기서 강제 요구하지 않습니다.
// PlayerInput은 InputReader 쪽에만 있으면 됩니다.
public class PlayerProjectileShooter : MonoBehaviour
{
    [SerializeField] WeaponHandler weaponHandler;
    [SerializeField] InputReader   input;   // ★ 추가: InputReader에서 FireHeld 읽음
    [SerializeField] Camera        cam;     // 월드 크로스헤어 = 카메라 중앙

    float cooldown;

    void Awake()
    {
        if (!weaponHandler) weaponHandler = GetComponentInChildren<WeaponHandler>(true);
        if (!input)         input         = GetComponentInParent<InputReader>();
        if (!cam)           cam           = Camera.main;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        // ★ 눌렀을 때만 발사. 떼면 즉시 중지.
        bool fireHeld = input ? input.FireHeld : (Mouse.current?.leftButton.isPressed ?? false);
        if (fireHeld) TryFire();
    }

    void TryFire()
    {
        if (!weaponHandler || !weaponHandler.HasWeapon) return;
        var gunObj = weaponHandler.CurrentWeapon;
        if (!gunObj) return;

        var gun = gunObj.GetComponent<GunAttachment>();
        if (!gun) { Debug.LogWarning("GunAttachment가 무기에 없습니다."); return; }

        if (cooldown > 0f) return;
        cooldown = gun.FireInterval;

        // === 월드 크로스헤어 방향 계산 ===
        Vector3 aimOrigin = cam ? cam.transform.position : transform.position + Vector3.up * 1.5f;
        Vector3 aimDir    = cam ? cam.transform.forward  : transform.forward;

        Vector3 aimPoint;
        if (cam && Physics.Raycast(aimOrigin, aimDir, out var hit, 1000f, gun.StopMask, QueryTriggerInteraction.Ignore))
            aimPoint = hit.point;
        else
            aimPoint = aimOrigin + aimDir * 1000f;

        // 총구 → AimPoint로 보정
        Transform muzzle = gun.Muzzle;
        Vector3 dir = (aimPoint - muzzle.position).normalized;

        // === 총알 생성 ===
        var prefab = gun.BulletPrefab;
        if (!prefab) { Debug.LogWarning("BulletPrefab이 지정되지 않았습니다."); return; }

        var bulletGO = Instantiate(prefab, muzzle.position, Quaternion.LookRotation(dir));
        if (bulletGO.TryGetComponent<BulletProjectile>(out var projectile))
        {
            projectile.Init(
                dir,
                gun.BulletSpeed,
                gun.Damage,
                gun.BulletLife,
                weaponHandler.transform,   // owner root(자가 피격 무시)
                gun.StopMask
            );
        }
        else
        {
            Debug.LogWarning("BulletProjectile 컴포넌트가 필요합니다.");
        }
    }
}
