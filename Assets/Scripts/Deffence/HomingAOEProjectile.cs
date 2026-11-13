using System.Collections.Generic;
using UnityEngine;

public class HomingAOEProjectile : MonoBehaviour
{
    public float damage = 20f;          // «·÷——
    public float speed = 5f;            // ”—⁄… «·„ﬁ–Ê›
    public float explosionRadius = 2f;  // ‰’› ﬁÿ— «·÷—»… «·Ã„«⁄Ì…
    private Transform target;           // «·Âœ› «·√”«”Ì

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // ≈–« „« ›Ì Âœ› ÌŒ ›Ì «·„ﬁ–Ê›
            return;
        }

        // «·Õ—ﬂ… »« Ã«Â «·Âœ›
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // «· Õﬁﬁ „‰ «·Ê’Ê·
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        // ‰÷—» ﬂ· «·√⁄œ«¡ ÷„‰ œ«∆—… (÷—» Ã„«⁄Ì)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D col in colliders)
        {
            GameObject obj = col.gameObject;

            if (!hitEnemies.Contains(obj))
            {
                hitEnemies.Add(obj);

                 if (col.TryGetComponent<flayingEnemy>(out flayingEnemy flyeme))
                {
                    flyeme.TakeDamage(damage);
                }
                else if (target.TryGetComponent<y3soobEnemy>(out y3soobEnemy y3son))
                {
                    y3son.TakeDamage(damage);
                }
                else if (target.TryGetComponent<HealerWarrior>(out HealerWarrior healer))
                {
                    healer.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject); // «Œ ›«¡ «·„ﬁ–Ê› »⁄œ «·÷—»…
    }

    // ·—”„ ‰’› ﬁÿ— «·«‰›Ã«— ›Ì «·„‘Âœ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

}
