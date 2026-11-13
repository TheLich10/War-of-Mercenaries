using System.Collections.Generic;
using UnityEngine;

public class ProjectileAoE : MonoBehaviour
{
    public float speed = 10f; // سرعة القذيفة
    public float damage = 20f; // ضرر القذيفة
    public float explosionRadius = 3f; // نطاق الانفجار (الضرر الجماعي)

    private Transform target;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    public GameObject dieAnime;
    [Header("Pools")]
    private UnitPool effectPool; // الـ Pool الخاص بتأثيرات الموت (dieAnime)

    void Awake()
    {
        // 1. نبحث عن الأوبجكت في المشهد عن طريق اسمه
        GameObject poolObj = GameObject.Find("boombpool"); // ⚠️ تأكد أن الاسم مطابق لما في المشهد

        // 2. إذا وجدناه، نقوم بسحب سكريبت UnitPool منه ونربطه
        if (poolObj != null)
        {
            effectPool = poolObj.GetComponent<UnitPool>();
        }
        else
        {
            Debug.LogWarning("⚠️ لم يتم العثور على أوبجكت باسم EffectPoolManager في المشهد!");
        }
    }

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

        // التحرك نحو الهدف
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // التحقق من الوصول للهدف
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        // نضرب كل الأعداء ضمن دائرة (ضرب جماعي)
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D col in colliders)
        {
            GameObject obj = col.gameObject;

            if (!hitEnemies.Contains(obj))
            {
                hitEnemies.Add(obj);

                if (col.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.TakeDamage(damage);
                }
                else if (col.TryGetComponent<EnemyRNG_Advanced>(out EnemyRNG_Advanced enemyRNG))
                {
                    enemyRNG.TakeDamage(damage);
                }
                else if (col.TryGetComponent<sapperEnemy>(out sapperEnemy spaerEnemy))
                {
                    spaerEnemy.TakeDamage(damage);
                }
            }
        }

        if (effectPool != null)
        {
            GameObject effect = effectPool.Spawn(transform.position);
            if (effect != null) effect.transform.rotation = transform.rotation;
        }

        Destroy(gameObject); // اختفاء المقذوف بعد الضربة
    }

    // هذه الدالة تساعدك على رؤية نطاق الانفجار (الضرر الجماعي) داخل محرر يونيتي لتسهيل ضبط المساحة
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}