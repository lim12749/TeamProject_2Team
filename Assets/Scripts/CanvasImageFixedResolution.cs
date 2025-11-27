using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class NamedCameraRenderToCanvas : MonoBehaviour
{
    private RawImage rawImage;
    private RenderTexture rt;
    private Camera targetCamera;

    private const int width = 2240;
    private const int height = 1400;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
        FindCameraByName("MainCamera");
        CreateRenderTexture();
    }

    void OnDestroy()
    {
        if (rt != null)
        {
            rt.Release();
            Destroy(rt);
        }
    }

    void FindCameraByName(string cameraName)
    {
        GameObject camObj = GameObject.Find(cameraName);
        if (camObj != null)
        {
            targetCamera = camObj.GetComponent<Camera>();
        }

        if (targetCamera == null)
        {
            Debug.LogWarning($"씬에서 이름이 '{cameraName}'인 카메라를 찾을 수 없습니다.");
        }
    }

    void CreateRenderTexture()
    {
        if (targetCamera == null) return;

        if (rt != null)
        {
            rt.Release();
            Destroy(rt);
        }

        // 2240x1400 RenderTexture 생성
        rt = new RenderTexture(width, height, 24);
        rt.name = "NamedCameraFixedRT";

        // 카메라에 RenderTexture 연결
        targetCamera.targetTexture = rt;

        // RawImage에 RenderTexture 연결
        rawImage.texture = rt;

        // RectTransform 크기 맞춤
        RectTransform rectTransform = rawImage.rectTransform;
        rectTransform.sizeDelta = new Vector2(width, height);
    }
}
