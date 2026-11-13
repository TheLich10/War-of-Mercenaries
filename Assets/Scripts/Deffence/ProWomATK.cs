using System.Collections.Generic;
using UnityEngine;

public class ProWomATK : MonoBehaviour
{
    public float speed = 10f; // ”—⁄… «·ﬁ–Ì›…
    public float damage = 20f; // ÷—— «·ﬁ–Ì›…
    public float explosionRadius = 3f; // ‰ÿ«ﬁ «·«‰›Ã«— («·÷—— «·Ã„«⁄Ì)

    private Transform target;
    [HideInInspector] public Transform shooterTransform;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();


    // √÷›‰« „ €Ì— realDamage ·«” ﬁ»«· «·œ„Ã «·ÕﬁÌﬁÌ
    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
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

                if (col.TryGetComponent<Enemy>(out Enemy enemy1))
                {
                    enemy1.TakeDamage(damage, shooterTransform);
                }
                else if (col.TryGetComponent<EnemyRNG_Advanced>(out EnemyRNG_Advanced enemyRang))
                {
                    enemyRang.TakeDamage(damage, shooterTransform);
                }
                else if (col.TryGetComponent<HealerWarrior>(out HealerWarrior enemyHelaer))
                {
                    enemyHelaer.TakeDamage(damage, shooterTransform);
                }
                else if (col.TryGetComponent<flayingEnemy>(out flayingEnemy emeyFly))
                {
                    emeyFly.TakeDamage(damage, shooterTransform);
                }
                else if (col.TryGetComponent<sapperEnemy>(out sapperEnemy saperEn))
                {
                    saperEn.TakeDamage(damage, shooterTransform);
                }
                else if (col.TryGetComponent<y3soobEnemy>(out y3soobEnemy y3sob))
                {
                    y3sob.TakeDamage(damage, shooterTransform);
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
