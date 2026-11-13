using System.Collections.Generic;
using UnityEngine;

public class ProjectWOM : MonoBehaviour
{
    public float speed = 10f; // ”—⁄… «·ﬁ–Ì›…
    public float damage = 20f; // ÷—— «·ﬁ–Ì›…
    public float explosionRadius = 3f; // ‰ÿ«ﬁ «·«‰›Ã«— («·÷—— «·Ã„«⁄Ì)

    private Transform target;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();


    // √÷›‰« „ €Ì— realDamage ·«” ﬁ»«· «·œ„Ã «·ÕﬁÌﬁÌ
    public void SetTarget(Transform enemyTarget, float realDamage)
    {
        target = enemyTarget;
        damage = realDamage; // «” »œ«· «·œ„Ã «·«› —«÷Ì (20) »«·œ„Ã «·ÕﬁÌﬁÌ «·ﬁ«œ„ „‰ «·⁄œÊ
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // «· Õ—ﬂ ‰ÕÊ «·Âœ›
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // «· Õﬁﬁ „‰ «·Ê’Ê· ··Âœ›
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

                if (col.TryGetComponent<DefensiveBuilding>(out DefensiveBuilding deffen1))
                {
                    deffen1.TakeDamage(damage);
                }
                else if (col.TryGetComponent<building>(out building building1))
                {
                    building1.TakeDamage(damage);
                }
                else if (col.TryGetComponent<DefensiveWatchPost>(out DefensiveWatchPost spy1))
                {
                    spy1.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject); // «Œ ›«¡ «·„ﬁ–Ê› »⁄œ «·÷—»…
    }

    // Â–Â «·œ«·…  ”«⁄œﬂ ⁄·Ï —ƒÌ… ‰ÿ«ﬁ «·«‰›Ã«— («·÷—— «·Ã„«⁄Ì) œ«Œ· „Õ—— ÌÊ‰Ì Ì · ”ÂÌ· ÷»ÿ «·„”«Õ…
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
