using UnityEngine;
using System;

public class WeaponHandler : MonoBehaviour
{
    [Header("Sockets / Animator")]
    [SerializeField] Transform rightHandSocket;      // 오른손 소켓
    [SerializeField] Animator animator;              // 플레이어 Animator
    [SerializeField] string hasWeaponParam = "HasWeapon"; // Animator 파라미터 이름

    public GameObject CurrentWeapon { get; private set; }
    public bool HasWeapon { get; private set; }
    public bool CanEquip => CurrentWeapon == null;

    public event Action<bool> WeaponStateChanged;    // (선택) 외부 알림
    [SerializeField] AimAligner aimAligner;   // 플레이어에 붙은 AimAligner
    void Awake()
    {
        // 참조 자동 보정 (최대한 단순)
        if (!animator) animator = GetComponentInChildren<Animator>(true);
        if (!rightHandSocket && animator && animator.isHuman)
            rightHandSocket = animator.GetBoneTransform(HumanBodyBones.RightHand);

        // 시작 상태 동기화

        if (!aimAligner) aimAligner = GetComponentInParent<AimAligner>(); // ★ 추가
        if (animator) animator.SetBool(hasWeaponParam, HasWeapon);
    }

    public void EquipWorldWeapon(GameObject weaponGO, Vector3 localPos, Vector3 localEuler)
    {
        if (!CanEquip || !rightHandSocket || weaponGO == null) return;

        // 물리 끄기
        if (weaponGO.TryGetComponent<Rigidbody>(out var rb)) { rb.isKinematic = true; rb.detectCollisions = false; }
        foreach (var col in weaponGO.GetComponentsInChildren<Collider>()) col.enabled = false;

        // 손에 붙이기
        weaponGO.transform.SetParent(rightHandSocket, false);
        weaponGO.transform.localPosition = localPos;
        weaponGO.transform.localEulerAngles = localEuler;

        CurrentWeapon = weaponGO;
        SetHasWeapon(true);

        // 다시 주워지지 않게
        if (weaponGO.TryGetComponent<WeaponPickup>(out var pickup)) pickup.enabled = false;
        
        // ★ 여기 추가: AimPivot / Muzzle 찾아 바인딩
        Transform weaponRoot = weaponGO.transform;
        Transform muzzle     = weaponRoot.Find("Muzzle");           // 있으면
        aimAligner?.BindWeapon(weaponRoot, muzzle);                 // ← 한 줄
    }

    public void UnequipAndDrop(float dropForce = 3f)
    {
        if (!CurrentWeapon) return;

        CurrentWeapon.transform.SetParent(null);
        foreach (var col in CurrentWeapon.GetComponentsInChildren<Collider>()) col.enabled = true;
        if (CurrentWeapon.TryGetComponent<Rigidbody>(out var rb))
        { rb.isKinematic = false; rb.detectCollisions = true; rb.AddForce(transform.forward * dropForce, ForceMode.Impulse); }

        if (CurrentWeapon.TryGetComponent<WeaponPickup>(out var pickup)) pickup.enabled = true;

        CurrentWeapon = null;
        SetHasWeapon(false);
    }

    private void SetHasWeapon(bool value)
    {
        if (HasWeapon == value) return;
        HasWeapon = value;

        // ✅ 가장 단순: 문자열 SetBool로 바로 세팅
        if (animator) animator.SetBool(hasWeaponParam, HasWeapon);

        // (선택) 외부에 알림
        WeaponStateChanged?.Invoke(HasWeapon);
        //imAligner?.UnbindWeapon(); // ★ 추가
        // (디버그) 확인용
        Debug.Log($"[WeaponHandler] HasWeapon = {HasWeapon}");
    }
}
