<<<<<<< Updated upstream
using UnityEngine;

// ½ºÅ©¸³Æ® ÆÄÀÏ ÀÌ¸§°ú Å¬·¡½º ÀÌ¸§ÀÌ "Monster"·Î µ¿ÀÏÇØ¾ß ÇÕ´Ï´Ù.
public class Monster : MonoBehaviour
{
    // 1. ¸ó½ºÅÍÀÇ ÀÌµ¿ ¼Óµµ (ÀÎ½ºÆåÅÍ¿¡¼­ Á¶Àı °¡´É)
    public float moveSpeed = 3.0f;

    // 2. ÃßÀûÇÒ ´ë»ó (ÇÃ·¹ÀÌ¾î)ÀÇ Transform
    private Transform playerTransform;

    void Start()
    {
        // 3. "Player" ÅÂ±×¸¦ °¡Áø °ÔÀÓ ¿ÀºêÁ§Æ®¸¦ Ã£¾Æ Transform ÄÄÆ÷³ÍÆ®¸¦ ÀúÀå
=======
ï»¿using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("ì´ë™ ì„¤ì •")]
    public float moveSpeed = 3.0f;

    [Header("ì²´ë ¥ ì„¤ì •")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("ì£½ì„ ë•Œ ì´í™íŠ¸ (ì„ íƒì‚¬í•­)")]
    public GameObject deathEffect; // í­ë°œ, íŒŒí¸ ë“± ì´í™íŠ¸ í”„ë¦¬íŒ¹

    [Header("ì• ë‹ˆë©”ì´í„° (ì„ íƒì‚¬í•­)")]
    public Animator animator;

    private Transform playerTransform;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // í”Œë ˆì´ì–´ íƒìƒ‰
>>>>>>> Stashed changes
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
<<<<<<< Updated upstream
            Debug.LogError("ÇÃ·¹ÀÌ¾î¸¦ Ã£À» ¼ö ¾ø½À´Ï´Ù. 'Player' ÅÂ±×¸¦ È®ÀÎÇÏ¼¼¿ä.");
            // ÇÃ·¹ÀÌ¾î¸¦ ¸øÃ£À¸¸é ½ºÅ©¸³Æ® ºñÈ°¼ºÈ­
            this.enabled = false;
=======
            Debug.LogError("í”Œë ˆì´ì–´ë¥¼ ì°¾ì„ ìˆ˜ ì—†ìŠµë‹ˆë‹¤. 'Player' íƒœê·¸ë¥¼ í™•ì¸í•˜ì„¸ìš”.");
            enabled = false;
>>>>>>> Stashed changes
        }
    }

    void Update()
    {
<<<<<<< Updated upstream
        // ÇÃ·¹ÀÌ¾î TransformÀÌ ÇÒ´çµÇ¾ú´ÂÁö È®ÀÎ
        if (playerTransform == null) return;

        // 4. ¸ó½ºÅÍ°¡ ÇÃ·¹ÀÌ¾î¸¦ ¹Ù¶óº¸°Ô ¸¸µê (Áï½Ã È¸Àü)
        transform.LookAt(playerTransform);

        // 5. ¸ó½ºÅÍÀÇ ¾Õ ¹æÇâÀ¸·Î ÃÊ´ç moveSpeed¸¸Å­ ÀÌµ¿
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
=======
        if (isDead || playerTransform == null) return;

        // í”Œë ˆì´ì–´ë¥¼ í–¥í•´ ì´ë™
        MoveTowardPlayer();
    }

    void MoveTowardPlayer()
    {
        // ë°”ë¼ë³´ê¸° (ìˆ˜í‰ë§Œ)
        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    // ğŸ”¥ ì²´ë ¥ ê°ì†Œ í•¨ìˆ˜ (ì´ì•Œì´ ë§ì•˜ì„ ë•Œ í˜¸ì¶œ)
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{name} í”¼ê²©! ë‚¨ì€ ì²´ë ¥: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ğŸ’€ ì‚¬ë§ ì²˜ë¦¬
    void Die()
    {
        isDead = true;
        Debug.Log($"{name} ì‚¬ë§!");

        // ì£½ëŠ” ì• ë‹ˆë©”ì´ì…˜ì´ ìˆìœ¼ë©´ ì‹¤í–‰
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // ì£½ì„ ë•Œ ì´í™íŠ¸ (ìˆìœ¼ë©´ ìƒì„±)
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // ì¼ì • ì‹œê°„ ë’¤ ì œê±° (ì• ë‹ˆë©”ì´ì…˜ ì‹œê°„ ê³ ë ¤)
        Destroy(gameObject, 2f);
    }
}
>>>>>>> Stashed changes
