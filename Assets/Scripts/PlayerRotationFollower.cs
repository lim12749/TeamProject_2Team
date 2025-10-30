using UnityEngine;

public class PlayerRotationFollower : MonoBehaviour
{
    public Transform aimTarget; // AimTarget을 Drag & Drop
    public float rotateSpeed = 10f;

    void Update()
    {
        Vector3 targetForward = aimTarget.forward;
        targetForward.y = 0; // y축은 회전 안 하게

        if (targetForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetForward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
}
