using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public int expAmount = 10;
    public float moveSpeed = 5f;
    public float absorbDistance = 1f;
    public float destroyAfterSeconds = 10f;

    private Transform target;
    private bool isFollowing = false;

    private void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isFollowing && other.CompareTag("Player"))
        {
            SetTarget(other.transform);
        }
    }

    public void SetTarget(Transform playerTransform)
    {
        target = playerTransform;
        isFollowing = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (!isFollowing || target == null) return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < absorbDistance)
        {
            Absorb();
        }
    }
    
    private void Absorb()
    {
        // 인터페이스 기반으로 경험치 부여
        //ILevelable levelable = target.GetComponent<ILevelable>();
       //if (levelable != null)
      //  {
       //     levelable.GainExp(expAmount);
      //  }

        // TODO: 사운드/파티클 추가 가능

        Destroy(gameObject);
    }
}
