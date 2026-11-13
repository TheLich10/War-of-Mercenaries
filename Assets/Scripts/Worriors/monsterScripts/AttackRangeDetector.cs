using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HeroRangeController : MonoBehaviour
{
    public float attackRange = 14f;
    public MonoBehaviour[] heroes; // مجموعة الأبطال سواء HeroUnit أو HeroArcher

    public string[] enemyTags = { "Enemy", "monsterEnemy"};

    void Update()
    {

        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance <= attackRange && distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }


        if (nearestEnemy != null)
        {
            SendTargetToActiveHero(nearestEnemy.transform);
        }
        else
        {
            ClearActiveHeroTarget();
        }
    }


    void SendTargetToActiveHero(Transform target)
    {
        foreach (var h in heroes)
        {
            GameObject heroObj = h.gameObject;

            if (!heroObj.activeSelf) continue;

            if (h is HeroUnit unit)
            {
                unit.SetTarget(target);
            }
            else if (h is HeroArcher archer)
            {
                archer.SetTarget(target);
            }
            else if (h is HeroUnit2 unitt2)
            {
                unitt2.SetTarget(target);
            }
            else if (h is HeroArcher2 archer2)
            {
                archer2.SetTarget(target);
            }

            // نرسل الهدف لأول بطل مفعّل فقط
            break;
        }
    }

    void ClearActiveHeroTarget()
    {
        foreach (var h in heroes)
        {
            GameObject heroObj = h.gameObject;

            if (!heroObj.activeSelf) continue;

            if (h is HeroUnit unit)
            {
                unit.ClearTarget();
            }
            else if (h is HeroArcher archer)
            {
                archer.ClearTarget();
            }
            else if (h is HeroUnit2 unitt2)
            {
                unitt2.ClearTarget();
            }
            else if (h is HeroArcher2 archer2)
            {
                archer2.ClearTarget();
            }
            break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        int segments = 40;
        float angleStep = 360f / segments;
        Quaternion tilt = Quaternion.Euler(45, 0, 0);

        Vector3 center = transform.position;
        Vector3 prevPoint = center + tilt * new Vector3(attackRange, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + tilt * new Vector3(Mathf.Cos(angle) * attackRange, Mathf.Sin(angle) * attackRange, 0);

            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
