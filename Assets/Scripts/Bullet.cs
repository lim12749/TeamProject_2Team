using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            // 몬스터에 체력 스크립트가 있다면 데미지 전달
            MonsterHealth mh = other.GetComponent<MonsterHealth>();
            if (mh != null)
                mh.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
