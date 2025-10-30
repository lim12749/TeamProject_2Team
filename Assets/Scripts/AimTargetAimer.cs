using UnityEngine;

public class AimTargetAimer : MonoBehaviour
{
    public Transform aimTarget; // AimLookTarget 오브젝트
    public float maxDistance = 100f;
    public LayerMask aimLayer;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Vector3 targetPoint = ray.GetPoint(maxDistance);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, aimLayer))
        {
            targetPoint = hit.point;
        }

        aimTarget.position = targetPoint;

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
    }
}
