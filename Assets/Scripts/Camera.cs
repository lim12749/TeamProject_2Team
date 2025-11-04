using UnityEngine;

public class camera : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 10, 0);
    private Transform target;

    private void LateUpdate()
    {
        if (target == null && GameManager.Instance != null && GameManager.Instance.Player != null)
        {
            target = GameManager.Instance.Player.transform;
        }

        if (!target) return;

        transform.position = target.position + offset;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
