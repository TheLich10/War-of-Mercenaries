using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f; // سرعة القذيفة
    public float damage = 20f; // ضرر القذيفة
    private Transform target;

    [HideInInspector] public Transform shooterTransform;

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

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
            // 🟢 الإضافة الثانية: تمرير "shooterTransform" بجانب الـ damage
            if (target.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage, shooterTransform);
            }
            else if (target.TryGetComponent<EnemyRNG_Advanced>(out EnemyRNG_Advanced enemyRNG))
            {
                enemyRNG.TakeDamage(damage, shooterTransform);
            }
            else if (target.TryGetComponent<sapperEnemy>(out sapperEnemy spaerEnemy))
            {
                spaerEnemy.TakeDamage(damage, shooterTransform);
            }
            else if (target.TryGetComponent<flayingEnemy>(out flayingEnemy flyeme))
            {
                flyeme.TakeDamage(damage, shooterTransform);
            }
            else if (target.TryGetComponent<HealerWarrior>(out HealerWarrior healer))
            {
                healer.TakeDamage(damage, shooterTransform);
            }
            else if (target.TryGetComponent<y3soobEnemy>(out y3soobEnemy y3son))
            {
                y3son.TakeDamage(damage, shooterTransform);
            }
            else if (target.TryGetComponent<ATTACKERS>(out ATTACKERS Attacker))
            {
                Attacker.TakeDamage(damage, shooterTransform);
            }
            else if (target.TryGetComponent<heroArcher2ATK>(out heroArcher2ATK arch2))
            {
                arch2.TakeDamage(damage, shooterTransform);
            }
            else if (target.TryGetComponent<heroArcherATK>(out heroArcherATK arch1))
            {
                arch1.TakeDamage(damage, shooterTransform);
            }
            else if (target.TryGetComponent<heroATK>(out heroATK hero1))
            {
                hero1.TakeDamage(damage, shooterTransform);
            }
            else if (target.TryGetComponent<hero2ATK>(out hero2ATK hero2))
            {   
                hero2.TakeDamage(damage, shooterTransform);
            }




        Destroy(gameObject);
        }

    }
