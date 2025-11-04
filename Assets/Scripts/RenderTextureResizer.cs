using UnityEngine;
using UnityEngine.UI;

public class RenderTextureResizer : MonoBehaviour
{
    public Camera characterCamera;
    public RawImage rawImage;

    private RenderTexture rt;

    void Start()
    {
        UpdateRenderTexture();
    }

    void Update()
    {
        // 해상도 바뀔 때마다 다시 생성
        if (rt == null || Screen.width != rt.width || Screen.height != rt.height)
        {
            UpdateRenderTexture();
        }
    }

    void UpdateRenderTexture()
    {
        if (rt != null)
        {
            rt.Release();
            Destroy(rt);
        }

        // RawImage의 RectTransform 비율 기준
        RectTransform rect = rawImage.rectTransform;
        float aspect = rect.rect.width / rect.rect.height;

        int height = Screen.height / 2; // 원하는 크기 비율 (화면 절반 크기 예시)
        int width = Mathf.RoundToInt(height * aspect);

        rt = new RenderTexture(width, height, 24);
        rt.name = "CharacterRenderDynamic";

        characterCamera.targetTexture = rt;
        rawImage.texture = rt;
    }
}
