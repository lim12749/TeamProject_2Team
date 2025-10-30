using Unity.Cinemachine;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private AimAligner aimAligner;        // ← 인스펙터 연결
    [SerializeField] private PlayerLocomotion locomotion;  // ← HasWeapon 읽기용 (인스펙터 연결)
    [Header("Refs")]
    [SerializeField] private InputReader input;              // ✅ 입력은 여기서만 읽는다
    [SerializeField] private CinemachineCamera freelookCam;
    [SerializeField] private CinemachineCamera aimCam;
    [SerializeField] private Camera mainCamera;              // 비워두면 Start에서 Camera.main
    [SerializeField] private GameObject crosshairUI;

    [Header("CM Input Controllers")]
    [SerializeField] private CinemachineInputAxisController freeLookAxis; // FreeLook에 붙은 것 (This Camera Only)
    [SerializeField] private bool aimUsesAxisController = false;          // AimCameraController를 쓰면 false

    private bool isAiming;
    private AimCameraController aimCamController;
    [SerializeField] PlayerAnimationController animCtrl; // 인스펙터 연결
    void Awake()
    {
        if (!mainCamera) mainCamera = Camera.main;
        if (!aimAligner) aimAligner = GetComponentInParent<AimAligner>();
        if (!locomotion) locomotion = GetComponentInParent<PlayerLocomotion>();
    }

    void Start()
    {
        aimCamController = aimCam ? aimCam.GetComponent<AimCameraController>() : null;

        // 참조 자동 보정
        if (!freeLookAxis && freelookCam) freeLookAxis = freelookCam.GetComponent<CinemachineInputAxisController>();

        // 축 입력 기본 상태
        if (freeLookAxis) freeLookAxis.enabled = true;

        SetCrosshair(false);
    }

    void Update()
    {
        if (input == null || freelookCam == null || aimCam == null) return;

        bool wantAim = input.AimHeld;                 // ✅ 우클릭 홀드
                                                      //Debug.Log("wantAim: " + wantAim);
            // ★ 여기! AimAligner로 상태 전달
        if (aimAligner)
        {
            aimAligner.SetAiming(wantAim);
            if (locomotion) aimAligner.SetHasWeapon(locomotion.HasWeapon);
        }

        if (wantAim && !isAiming)
            EnterAimMode(); // 에임 모드 진입
        else if (!wantAim && isAiming)
            ExitAimMode(); // 에임 모드 종료
    }

    private void EnterAimMode()
    {
        //Debug.Log("에임 활성화");
        isAiming = true;

        SnapAimCameraToCurrentView(); //

        // 우선순위 스왑
        aimCam.Priority = 100;
        freelookCam.Priority = 10;

        // 축 입력 토글
        if (freeLookAxis) freeLookAxis.enabled = false;

        SetCrosshair(true);
//        animCtrl?.RequestAim(true);
    }

    private void ExitAimMode()
    {
        isAiming = false;

        SnapFreeLookBehindPlayer();

        aimCam.Priority = 10;
        freelookCam.Priority = 100;

        if (freeLookAxis) freeLookAxis.enabled = true;

        SetCrosshair(false);
     //   animCtrl?.RequestAim(false);
    }

    private void SetCrosshair(bool show)
    {
        if (crosshairUI) crosshairUI.SetActive(show);
    }

    private void SnapFreeLookBehindPlayer()
    {
        if (!freelookCam) return;
        var orbital = freelookCam.GetComponent<CinemachineOrbitalFollow>();
        if (!orbital || !mainCamera) return;

        // 메인카메라의 평면 헤딩 기준으로 FreeLook 정렬
        Vector3 fwd = mainCamera.transform.forward; fwd.y = 0f;
        if (fwd.sqrMagnitude < 0.0001f) return;
        float yaw = Mathf.Atan2(fwd.x, fwd.z) * Mathf.Rad2Deg;
        orbital.HorizontalAxis.Value = yaw;
    }

    private void SnapAimCameraToCurrentView()
    {
        if (!aimCam) return;

        // AimCameraController를 쓰면 그 쪽 API로 정렬
        if (aimCamController)
        {
            var src = mainCamera ? mainCamera.transform : (freelookCam ? freelookCam.transform : null);
            if (src) aimCamController.SetYawPitchFromCameraForward(src);
            return;
        }

        // AxisController로 돌리는 세팅이라면(aimUsesAxisController=true) 굳이 스냅 불필요
        // 필요시 여기서 FOV/오프셋만 다르게 두면 줌 체감 확보
    }
}
