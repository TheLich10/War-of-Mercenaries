using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class AreaTrap2D : MonoBehaviour
{
    [Header("Trap Settings")]
    [Tooltip("„ﬁœ«— «·÷—— «·–Ì ”Ì”»»Â «·›Œ")]
    public float damage = 50f;

    public GameObject trapAnime;

    public SpriteRenderer childSpriteRenderer;

    // „ €Ì— ·÷„«‰ ⁄„· «·›Œ „—… Ê«Õœ… ›ﬁÿ (ﬂ÷——)
    private bool hasTriggered = false;
    private Collider2D myCollider;

    // „ €Ì— ·· Õﬂ„ ›Ì ŸÂÊ— Ê«Œ ›«¡ «·›Œ
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //  ›⁄Ì· «· —ÌÃ— „»«‘—…
        if (myCollider != null)
            myCollider.isTrigger = true;

        // ≈Œ›«¡ «·›Œ ⁄‰œ »œ¡ «··⁄»… (·ﬂ‰Â ”Ì»ﬁÏ Ì⁄„· ·√‰Â ·„ Ì „  ⁄ÿÌ· «·√Ê»Ãﬂ  »«·ﬂ«„·)
        if (childSpriteRenderer != null)
        {
            if (SceneManager.GetActiveScene().name == "SampleScene")
            {
                // ≈–« ﬂ«‰ «·„‘Âœ ÂÊ SampleScene° Ì»ﬁÏ ’Ê—… «·‹ Child Ÿ«Â—…
                childSpriteRenderer.enabled = true;
            }
            else
            {
                // ›Ì √Ì „‘Âœ ¬Œ—° Ì „ ≈Œ›«¡ ’Ê—… «·‹ Child
                childSpriteRenderer.enabled = false;
            }
        }
    }

    private void Update()
    {
        if (spyButton.spyButtonState == true)
        {
            childSpriteRenderer.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ≈–« ﬂ«‰ «·›Œ ﬁœ «‰›Ã— „”»ﬁ«° ‰ Ã«Â· «·√„—
        if (hasTriggered) return;

        // «· Õﬁﬁ „‰ «· «€«  «·Œ«’… »«·√⁄œ«¡
        if (other.CompareTag("Enemy") ||
            other.CompareTag("monsterEnemy"))
        {
            ActivateTrap();
            trapAnime.SetActive(true);
            childSpriteRenderer.enabled = false;
            AudioManager.Instance.PlaySFX("otmATK");
            Debug.Log("is get touch");
        }
    }

    private void ActivateTrap()
    {
        // ≈Ìﬁ«› «·›Œ ›Ê—« Õ Ï ·« Ì”»» ÷—— „—… √Œ—Ï
        hasTriggered = true;

        // ≈ŸÂ«— «·›Œ »„Ã—œ √‰ Ì·„”Â «·⁄œÊ
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        //  ÕœÌœ „‰ÿﬁ… «·›Œ
        Vector2 center = myCollider.bounds.center;

        //  ﬂ»Ì— «·„‰ÿﬁ… »‰”»… »”Ìÿ… (20%) ·÷„«‰ «· ﬁ«ÿ ﬂ· „‰ Ìﬁ› ⁄·Ï «·Õ«›… ›Ì ‰›” «··ÕŸ…
        Vector2 size = myCollider.bounds.size * 1.6f;

        // «· ﬁ«ÿ Ã„Ì⁄ «·√⁄œ«¡ ›Ì Â–Â «·„”«Õ… œ›⁄… Ê«Õœ…
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(center, size, transform.eulerAngles.z);

        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag("Enemy") ||
                hit.CompareTag("monsterEnemy"))
            {
                ApplyDamage(hit.gameObject);
            }
        }

        //  „ Õ–› ”ÿ—  œ„Ì— «·√Ê»Ãﬂ  (Destroy) ·ﬂÌ Ì»ﬁÏ «·›Œ Ÿ«Â—« ›Ì «·„‘Âœ
    }

    private void ApplyDamage(GameObject target)
    {
        // «· Õﬁﬁ „‰ «·ﬂ·«”«  «·Œ«’… »ﬂ Ê ÿ»Ìﬁ «·÷——
        if (target.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
        }
        else if (target.TryGetComponent<EnemyRNG_Advanced>(out EnemyRNG_Advanced enemyRng))
        {
            enemyRng.TakeDamage(damage);
        }
        else if (target.TryGetComponent<sapperEnemy>(out sapperEnemy sapper))
        {
            sapper.TakeDamage(damage);
        }
    }
}