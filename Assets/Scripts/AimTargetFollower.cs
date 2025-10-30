using UnityEngine;

public class AimTargetFollower : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    void LateUpdate()
    {
        transform.position = player.position + offset;
        // 회전은 유지, 위치만 따라갑니다.
    }
}
