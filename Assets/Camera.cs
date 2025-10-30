using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, 0);

    void LateUpdate()
    {
        if (!target) return;

        transform.position = target.position + offset;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
