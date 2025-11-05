using UnityEngine;

[RequireComponent(typeof(MonsterHealth))]
public class Monster : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Transform playerTransform;
    private MonsterHealth health;

    void Start()
    {
        health = GetComponent<MonsterHealth>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null || health == null || health.enabled == false)
            return;

        // 체력이 남아있을 때만 이동
        if (health != null && playerTransform != null)
        {
            Vector3 dir = playerTransform.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.LookRotation(dir);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }
}
