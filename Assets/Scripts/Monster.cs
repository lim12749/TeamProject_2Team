using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("?대룞 ?ㅼ젙")]
    public float moveSpeed = 3.0f;

    [Header("泥대젰 ?ㅼ젙")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("二쎌쓣 ???댄럺??(?좏깮?ы빆)")]
    public GameObject deathEffect; 

    [Header("?좊땲硫붿씠??(?좏깮?ы빆)")]
    public Animator animator;

    private Transform playerTransform;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("플레이어를 찾을 수 없습니다. 'Player' 태그를 확인하세요.");
            // 플레이어를 못찾으면 스크립트 비활성화
            this.enabled = false;

            Debug.LogError("?뚮젅?댁뼱瑜?李얠쓣 ???놁뒿?덈떎. 'Player' ?쒓렇瑜??뺤씤?섏꽭??");
            enabled = false;
        }
    }

    void Update()
    {
        // 플레이어 Transform이 할당되었는지 확인
        if (playerTransform == null) return;

        // 4. 몬스터가 플레이어를 바라보게 만듦 (즉시 회전)
        transform.LookAt(playerTransform);

        // 5. 몬스터의 앞 방향으로 초당 moveSpeed만큼 이동
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
        if (isDead || playerTransform == null)
{

}

MoveTowardPlayer();

    void MoveTowardPlayer()
    {
        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{name} ?쇨꺽! ?⑥? 泥대젰: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        isDead = true;
        Debug.Log($"{name} ?щ쭩!");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, 2f);
    }
}
