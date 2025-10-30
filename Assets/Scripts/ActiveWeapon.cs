using UnityEngine;
using UnityEngine.Animations.Rigging;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

public class ActiveWeapon : MonoBehaviour
{
    private ProjectileWeapon currentWeapon; // 현재 장착된 무기
    public Rig handIKRig;                   // 손 IK 레이어 
    public Transform weaponHolder;          // 무기 장착 위치
    public Transform weaponLeftGrip;        // 왼손 그립
    public Transform weaponRightGrip;       // 오른손 그립

    // 애니메이션
    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorOverrideController aoc;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // 1) AOC를 확실하게 생성하고 다시 할당
        //var baseController = animator.runtimeAnimatorController;
        //if (baseController == null)
        //{
          //  Debug.LogError("ActiveWeapon: Animator에 RuntimeAnimatorController가 없습니다.");
            //return;
        //}

        // 기존이 AOC든 아니든, 새 AOC를 만들어 씌우는 게 가장 안전
        aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = aoc;

        // 시작 시 무기가 있으면 장착 처리
        ProjectileWeapon existingWeapon = GetComponentInChildren<ProjectileWeapon>();
        if (existingWeapon != null) EquipWeapon(existingWeapon);
        else Debug.LogWarning("ActiveWeapon: No ProjectileWeapon found in children.");
    }

    void Update()
    {
        if (currentWeapon != null)
        {
            // 현재 타입이 ProjectileWeapon이면 항상 1 (지금 구조상 true)
            handIKRig.weight = 1f;
            // 레이어 1이 존재하는지 확인 후 가중치 적용 (없으면 무시)
            if (animator.layerCount > 1) animator.SetLayerWeight(1, 1f);
        }
        else
        {
            handIKRig.weight = 0f;
            if (animator.layerCount > 1) animator.SetLayerWeight(1, 0f);
        }
    }

    public void EquipWeapon(ProjectileWeapon weapon)
    {
        // 기존 무기 제거
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        currentWeapon = weapon;

        // 홀더에 부착
        weapon.transform.SetParent(weaponHolder, worldPositionStays: false);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        // 레이어 가중치
        if (animator.layerCount > 1) animator.SetLayerWeight(1, 1f);


        // 아주 짧은 지연보다, 바로 오버라이드 + Rebind가 안정적
        //ApplyWeaponPoseClip();
        Invoke(nameof(SetAnimationonDelayer), 0.001f);
    }

    void SetAnimationonDelayer()
    {
        Debug.Log("ActiveWeapon: 무기 애니메이션 클립 설정 시작.");
        aoc["Weapon_anim_empty"] = currentWeapon.weponAnimationClip;
        Debug.Log(currentWeapon.weponAnimationClip.name);
         // 애니메이션 클립 이름 확인
         if (currentWeapon.weponAnimationClip == null)
         {
             Debug.LogError("ActiveWeapon: 현재 무기의 애니메이션 클립이 설정되지 않았습니다.");
             return;
         }
        animator.runtimeAnimatorController = aoc; // 오버라이드 컨트롤러 재할당
        animator.Rebind();
        animator.Update(0f);
    }

   
    [ContextMenu("Save Weapon Position")]
    public void SaveWeapOnPosition()
    {
        // Recorder의 루트는 '포즈를 적용하려는 애니메이터 루트'여야 하며,
        // 바인딩 대상(weaponHolder/그립들)은 반드시 이 루트의 하위여야 곡선이 기록됩니다.
        var recorder = new GameObjectRecorder(gameObject);

        recorder.BindComponentsOfType<Transform>(weaponHolder.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponLeftGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponRightGrip.gameObject, false);

        // 스냅샷 & 저장
        recorder.TakeSnapshot(0.0f);

        recorder.SaveToClip(currentWeapon.weponAnimationClip);
        Debug.Log("ActiveWeapon: 현재 포즈를 무기 포즈 클립에 저장 완료.");
    }
}
