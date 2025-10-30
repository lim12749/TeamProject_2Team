using UnityEngine;

/// <summary>
/// 에임컨트롤러는 카메를 회전 하게는 스크립트
/// </summary>
public class PlayerAimController : MonoBehaviour
{
    public float sensitivityX = 150f;
    public float sensitivityY = 1.5f;
    public float minPitch = -60f;
    public float maxPitch = 70f;

    private float pitch = 0f; // X축 상하 회전용
    public PlayerInputReader input;

    void Update()
    {
        float mouseX = input.MouseX * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        // Y축: 좌우 회전 (상대 회전으로 처리)
        transform.Rotate(Vector3.up, mouseX, Space.World);

        // X축: 상하 회전 (Pitch만 Clamp로 유지)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Vector3 currentEuler = transform.localEulerAngles;
        currentEuler.x = pitch;
        transform.localEulerAngles = currentEuler;
    }
}
