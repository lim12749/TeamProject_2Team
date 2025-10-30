using UnityEngine;
using Unity.Cinemachine;
public class teacam : MonoBehaviour
{
    public CinemachineCamera aimCam;
    public CinemachineCamera defaultCam;

    void Update()
    {
        bool rightClick = Input.GetMouseButton(1); // 우클릭 감지

        if (rightClick)
        {
            aimCam.Priority = 20;      // 우선순위를 높임 → Live 전환
            defaultCam.Priority = 0;
        }
        else
        {
            aimCam.Priority = 0;
            defaultCam.Priority = 20;  // 다시 기본 카메라 Live
        }
    }
}
