using UnityEngine;
using UnityEngine.UI;

public class RenderTextureResizer : MonoBehaviour
{
    public Camera characterCamera;
    public RawImage rawImage;

    private RenderTexture rt;

    // 고정 해상도 상수 (원하면 Inspector로 노출)
    private const int Width = 2240;
    private const int Height = 1400;
    private const int Depth = 24;

    void Start()
    {
        UpdateRenderTexture();
    }

    void Update()
    {
        // RenderTexture가 없거나 해상도가 바뀌었으면 다시 생성
        if (rt == null || rt.width != Width || rt.height != Height)
        {
            UpdateRenderTexture();
            return;
        }

        // 카메라가 존재하지만 targetTexture가 올바르지 않으면 재할당
        if (characterCamera != null && characterCamera.targetTexture != rt)
        {
            characterCamera.targetTexture = rt;
        }

        // rawImage가 지정되어 있고 텍스처가 다르면 갱신
        if (rawImage != null && rawImage.texture != rt)
        {
            rawImage.texture = rt;
        }
    }

    void UpdateRenderTexture()
    {
        // 기존 RT 안전 정리
        if (rt != null)
        {
            if (characterCamera != null && characterCamera.targetTexture == rt)
                characterCamera.targetTexture = null;

            rt.Release();
            Destroy(rt);
            rt = null;
        }

        // 새 RenderTexture 생성
        rt = new RenderTexture(Width, Height, Depth);
        rt.name = "CharacterRenderFixed";

        // rawImage에만 먼저 연결해도 UI는 동작
        if (rawImage != null)
            rawImage.texture = rt;

        // 카메라가 없으면 할당 시도 후 없으면 경고 (예외 방지)
        if (characterCamera == null)
        {
            characterCamera = Camera.main;
        }

        if (characterCamera != null)
        {
            characterCamera.targetTexture = rt;
        }
        else
        {
            Debug.LogWarning($"RenderTextureResizer: characterCamera가 할당되어 있지 않습니다. 나중에 카메라가 할당되면 targetTexture가 설정됩니다. (GameObject: {gameObject.name})");
        }
    }

    void OnDisable()
    {
        // 비활성화 시 카메라의 targetTexture 제거 및 RT 정리
        if (characterCamera != null && characterCamera.targetTexture == rt)
            characterCamera.targetTexture = null;

        if (rt != null)
        {
            rt.Release();
            Destroy(rt);
            rt = null;
        }
    }

    void OnDestroy()
    {
        // 안전을 위해 OnDisable과 동일한 정리
        if (characterCamera != null && characterCamera.targetTexture == rt)
            characterCamera.targetTexture = null;

        if (rt != null)
        {
            rt.Release();
            Destroy(rt);
            rt = null;
        }
    }
}
