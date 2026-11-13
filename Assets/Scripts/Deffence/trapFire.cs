using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class trapFire : MonoBehaviour
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

        // ‰ Õﬁﬁ „„« ≈–« ﬂ«‰ «·ﬂ«∆‰ «·–Ì ·„” «·›Œ „‰ «·√Âœ«› «·„”„ÊÕ ·Â« » ›⁄Ì· «·›Œ
        if (IsValidTarget(other.gameObject))
        {
            ActivateTrap();
            trapAnime.SetActive(true);
            childSpriteRenderer.enabled = false;
            AudioManager.Instance.PlaySFX("flameBuildATk");
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
        Vector2 size = myCollider.bounds.size * 1.1f;

        // «· ﬁ«ÿ Ã„Ì⁄ «·√⁄œ«¡ ›Ì Â–Â «·„”«Õ… œ›⁄… Ê«Õœ…
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(center, size, transform.eulerAngles.z);

        foreach (Collider2D hit in hitColliders)
        {
            //  ÿ»Ìﬁ «·÷—— ›ﬁÿ ⁄·Ï «·√Âœ«› «·’«·Õ…
            if (IsValidTarget(hit.gameObject))
            {
                ApplyDamage(hit.gameObject);
            }
        }
    }

    // œ«·… ÃœÌœ… ·· Õﬁﬁ „‰ ‘—Êÿ «· «€ Ê«·√”„«¡
    private bool IsValidTarget(GameObject target)
    {
        // ≈–« ﬂ«‰ „‰ «· «€«  «·√Œ—Ï (€Ì— Enemy)° ›ÂÊ Âœ› ’«·Õ „»«‘—…
        if (target.CompareTag("EnemyFly") ||
            target.CompareTag("monsterEnemy") ||
            target.CompareTag("monsterFly"))
        {
            return true;
        }

        // √„« ≈–« ﬂ«‰ «· «€ "Enemy"° ÌÃ» √‰ ‰ Õﬁﬁ „‰ «·√”„«¡ «·„Õœœ…
        if (target.CompareTag("Enemy"))
        {
            // «· Õﬁﬁ „‰ «·√”„«¡ œ«Œ· ”ﬂ—Ì»  Enemy
            if (target.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (enemy.warriorName == "ChineseMongolian" || enemy.warriorName == "Battering_Gaul")
                {
                    return true;
                }
            }

            // «· Õﬁﬁ „‰ «·«”„ œ«Œ· ”ﬂ—Ì»  EnemyRNG_Advanced
            if (target.TryGetComponent<EnemyRNG_Advanced>(out EnemyRNG_Advanced enemyRng))
            {
                if (enemyRng.warriorName == "ScalingEgyptian")
                {
                    return true;
                }
            }
        }

        // ≈–« ·„ Ì‰ÿ»ﬁ √Ì ‘—ÿ „‰ «·‘—Êÿ «·”«»ﬁ…° Ì „  Ã«Â· «·Âœ›
        return false;
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