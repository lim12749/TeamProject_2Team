// PlayerAnimationController.cs
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] InputReader input;
    [SerializeField] PlayerLocomotion locomotion;

    [Header("Params/Layers")]
    string hasWeaponParam = "HasWeapon";
    string isAimingParam  = "IsAiming";
    string upperBodyAimLayer = "UpperBody_Aim";
    [SerializeField] float  aimTransitionSpeed = 8f;

    int upperLayerIndex = -1;
    float aimWeight;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>(true);
        if (animator) {
            upperLayerIndex = animator.GetLayerIndex(upperBodyAimLayer);
            if (upperLayerIndex >= 0) animator.SetLayerWeight(upperLayerIndex, 0f);
        }
    }

    void Update()
    {
        if (!animator || input == null || locomotion == null) return;

        // 1) HasWeapon을 문자열로 바로 세팅 (가장 단순)
        animator.SetBool(hasWeaponParam, locomotion.HasWeapon);

        // 2) IsAiming도 문자열로 바로 세팅 (무기 있을 때만)
        bool aimingActive = input.AimHeld && locomotion.HasWeapon;
        animator.SetBool(isAimingParam, aimingActive);

        // 3) 상체 레이어 가중치
        if (upperLayerIndex >= 0)
        {
            float target = aimingActive ? 1f : 0f;
            aimWeight = Mathf.MoveTowards(aimWeight, target, aimTransitionSpeed * Time.deltaTime);
            animator.SetLayerWeight(upperLayerIndex, aimWeight);
        }

        // (선택) 이동 블렌드 값도 여기서 세팅한다면:
        animator.SetFloat("Speed01", locomotion.Speed01);
    }
}
