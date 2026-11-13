using UnityEngine;

public class HeroRangeControllerATK : MonoBehaviour
{
    public float attackRange = 14f;
    public MonoBehaviour[] heroes;
    public string[] enemyTags = { "Enemy", "monsterEnemy", "monsterFly", "EnemyFly" };

    void Update()
    {
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (string tag in enemyTags)
        {
            // شرط الوحوش الطائرة
            if ((tag == "monsterFly" || tag == "EnemyFly") && !CanAttackFlyingEnemies())
                continue;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject enemy in enemies)
            {
                // 🔥 تعديل هام: حساب المسافة بتجاهل محور Y (مهم لألعاب الـ 2D والـ Isometric)
                Vector3 heroPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 enemyPos = new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z);

                float distance = Vector3.Distance(heroPos, enemyPos);

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

    bool CanAttackFlyingEnemies()
    {
        // افترضت أن isActiveHero هي متغيرات Static بناءً على كودك
        if (heroArcherATK.isActiveHero || heroArcher2ATK.isActiveHero)
        {
            return true;
        }
        return false;
    }

    void SendTargetToActiveHero(Transform target)
    {
        bool targetSent = false;

        // 1. تأكد أن المصفوفة ليست فارغة
        if (heroes == null || heroes.Length == 0)
        {
            //Debug.LogError("مصفوفة الأبطال فارغة! يجب تعيين الأبطال بعد تحميل المشهد.");
            return;
        }

        foreach (var h in heroes)
        {
            // 2. إذا كان البطل null، فهذا يعني أنه تدمر عند تغيير المشهد
            if (h == null)
            {
                //Debug.LogWarning("تحذير: تم العثور على بطل مفقود (null)! يبدو أنه تدمر عند تغيير المشهد ولم يتم تحديث المصفوفة.");
                continue;
            }

            if (!h.gameObject.activeSelf) continue;

            heroATK unit = h.GetComponent<heroATK>();
            heroArcherATK archer = h.GetComponent<heroArcherATK>();
            hero2ATK unitt2 = h.GetComponent<hero2ATK>();
            heroArcher2ATK archer2 = h.GetComponent<heroArcher2ATK>();

            if (unit != null) { unit.SetTarget(target); targetSent = true; }
            else if (archer != null) { archer.SetTarget(target); targetSent = true; }
            else if (unitt2 != null) { unitt2.SetTarget(target); targetSent = true; }
            else if (archer2 != null) { archer2.SetTarget(target); targetSent = true; }

            if (targetSent)
            {
                break;
            }
        }

        // 3. إذا انتهت الحلقة ولم يتم إرسال الهدف لبطل
        if (!targetSent)
        {
            Debug.LogWarning("تم العثور على عدو، ولكن لم يتم العثور على أي بطل مفعل (Active) وقادر على الهجوم!");
        }
    }
    void ClearActiveHeroTarget()
    {
        foreach (var h in heroes)
        {
            if (h == null || !h.gameObject.activeSelf) continue;

            if (h.GetComponent<heroATK>() != null) h.GetComponent<heroATK>().ClearTarget();
            else if (h.GetComponent<heroArcherATK>() != null) h.GetComponent<heroArcherATK>().ClearTarget();
            else if (h.GetComponent<hero2ATK>() != null) h.GetComponent<hero2ATK>().ClearTarget();
            else if (h.GetComponent<heroArcher2ATK>() != null) h.GetComponent<heroArcher2ATK>().ClearTarget();

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