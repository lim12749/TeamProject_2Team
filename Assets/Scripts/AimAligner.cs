using UnityEngine;

public class AimAligner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform bodyRoot;        // 몸통 회전 기준(보통 Player 루트). 비우면 this.transform
    [SerializeField] Transform weaponPivot;     // 무기 회전 기준(무기 루트나 손 밑에 빈 오브젝트)
    [SerializeField] Transform muzzle;          // 총구 끝(총알 나가는 곳)
    [SerializeField] Transform crosshairWorld;  // 월드 크로스헤어(있으면 최우선)
    [SerializeField] Camera   aimCamera;        // 메인카메라(없으면 Camera.main 사용)

    [Header("Ray Fallback")]
    [SerializeField] LayerMask aimMask = ~0;    // 플레이어 레이어는 제외
    [SerializeField] float maxAimDistance = 100f;

    [Header("Smoothing")]
    [SerializeField] float weaponAimSpeed = 1200f; // 무기 회전 속도(°/s)
    [SerializeField] float bodyYawSpeed  = 720f;   // 몸통 회전 속도(°/s)
    [SerializeField] float bodyDeadZone  = 1.0f;   // 너무 작은 각도는 무시(°)
    [SerializeField] bool   onlyWhenAiming = true; // 에임 중에만 적용할지

    [Header("Simple Gates (선택)")]
    [SerializeField] bool hasWeapon = true;        // 무기 있을 때만 적용하고 싶으면 체크/해제
    [SerializeField] bool isAiming  = true;        // 외부에서 우클릭 상태를 넘겨줄 때 쓸 수 있음

    void Awake()
    {
        if (!bodyRoot)   bodyRoot   = transform;
        if (!aimCamera)  aimCamera  = Camera.main;
        if (!weaponPivot && muzzle) weaponPivot = muzzle.parent; // 못 넣었으면 총구 부모를 피벗으로
    }
        // 외부에서 무기/머즐을 알려줄 때 호출
        public void BindWeapon(Transform weaponRoot, Transform muzzleTransform)
        {
            // 애니 본(WeaponRoot) 아래의 AimPivot을 찾아서 weaponPivot으로 사용
            Transform pivot = weaponRoot ? weaponRoot.Find("AimPivot") : null;
            weaponPivot = pivot ? pivot : weaponRoot; // AimPivot 있으면 그걸, 없으면 임시로 weaponRoot

            muzzle = muzzleTransform; // 없어도 됨(회전만 하니까)

            // 디버그용 (원하면 지워도 됨)
            // Debug.Log($"[AimAligner] bind => pivot:{weaponPivot?.name}, muzzle:{muzzle?.name}");
        }

        public void UnbindWeapon()
        {
            weaponPivot = null;
            muzzle = null;
        }
    void FixedUpdate()
    {
        // 에임 중에만 돌리고 싶다면
        if (onlyWhenAiming && !isAiming) return;
        if (!hasWeapon) return; // 무기 없으면 스킵(원하면 끄세요)

        Vector3 targetPos = GetAimWorldPoint();

        // 1) 무기(피벗)을 타깃으로 회전
        if (weaponPivot)
        {
            Vector3 dir =  weaponPivot.position- targetPos;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion desired = Quaternion.LookRotation(dir.normalized, Vector3.up);
                weaponPivot.rotation = Quaternion.RotateTowards(
                    weaponPivot.rotation, desired, weaponAimSpeed * Time.deltaTime
                );
            }
        }

        // 2) 몸통(Yaw)도 같은 타깃으로 회전 (수평면 기준)
        if (bodyRoot)
        {
            Vector3 flat = targetPos - bodyRoot.position; flat.y = 0f;
            if (flat.sqrMagnitude > 0.0001f)
            {
                Quaternion byaw = Quaternion.LookRotation(flat.normalized, Vector3.up);
                float angle = Quaternion.Angle(bodyRoot.rotation, byaw);
                if (angle > bodyDeadZone)
                {
                    bodyRoot.rotation = Quaternion.RotateTowards(
                        bodyRoot.rotation, byaw, bodyYawSpeed * Time.deltaTime
                    );
                }
            }
        }
    }

    Vector3 GetAimWorldPoint()
    {
        // 월드 크로스헤어가 있으면 그 위치가 가장 정확
        if (crosshairWorld) return crosshairWorld.position;

        // 없으면 카메라 중앙에서 레이를 쏴서 히트 포인트 사용
        Camera c = aimCamera ? aimCamera : Camera.main;
        if (c)
        {
            Ray r = c.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(r, out var hit, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore))
                return hit.point;

            return r.origin + r.direction * maxAimDistance; // 미스면 전방 고정거리
        }
        // 카메라 없으면 전방 5m
        return transform.position + transform.forward * 5f;
    }

    // 외부(입력/무기 상태)에서 간단히 세팅하고 싶을 때 쓸 메서드(선택)
    public void SetAiming(bool aiming)  => isAiming = aiming;
    public void SetHasWeapon(bool armed) => hasWeapon = armed;
}
