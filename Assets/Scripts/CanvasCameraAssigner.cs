using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CanvasCameraAssigner : MonoBehaviour
{
    [Tooltip("화면 렌더용 카메라. 비워두면 Runtime에 Camera.main 또는 새 카메라로 자동 설정합니다.")]
    public Camera screenCamera;

    [Tooltip("게임 시작 시 강제로 worldCamera에 할당합니다.")]
    public bool assignOnStart = true;

    private Canvas _canvas;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    void Start()
    {
        if (assignOnStart)
            EnsureAssigned();
    }

    // 필요하면 다른 타이밍에서 호출 가능
    public void EnsureAssigned()
    {
        // Canvas가 Screen Space - Camera 여야만 worldCamera 적용
        if (_canvas.renderMode != RenderMode.ScreenSpaceCamera)
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;

        // 우선 Inspector에 지정된 카메라 사용
        if (screenCamera == null)
        {
            // 가능한 Main 카메라 우선
            if (Camera.main != null)
                screenCamera = Camera.main;
        }

        // Main 카메라도 없으면 새 카메라 생성 (최소한의 설정)
        if (screenCamera == null)
        {
            var go = new GameObject("Canvas_FallbackCamera");
            screenCamera = go.AddComponent<Camera>();
            screenCamera.clearFlags = CameraClearFlags.SolidColor;
            screenCamera.backgroundColor = Color.clear;
            screenCamera.cullingMask = ~0;
            Debug.LogWarning("CanvasCameraAssigner: Screen camera가 없어 Fallback 카메라를 생성했습니다.");
        }

        _canvas.worldCamera = screenCamera;
        Debug.Log($"CanvasCameraAssigner: Canvas.worldCamera가 '{screenCamera.name}'로 설정되었습니다.");
    }
}