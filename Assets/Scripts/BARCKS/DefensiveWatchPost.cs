using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class DefensiveWatchPost : MonoBehaviour
{
    [Header("fire Effect")]
    public GameObject lowHealthEffect;

    [Header("destroy build")]
    public GameObject destroyedPrefab; // اسحب هنا الـ Prefab الخاص بالأنقاض

    [Header("البيانات")]
    public string buildingName;
    public int level = 1;

    [Header("الخصائص")]
    public float health = 200f;
    public float maxHealth = 0f;
    public float attackRange = 5f;

    public GameObject healthBarPrefab;
    private HealthBarFollow healthBarUI;

    [Header("الثكنة")]
    private BarracksWatchPost linkedBarracks;

    [Header("الجنود (13 نوع)")]
    [Tooltip("ضع هنا الـ UnitPool لكل محارب حسب اسمه بدلاً من الـ Prefab")]
    private readonly string[] warriorNames = {
    "bedoiun", "ScalingEgyptian", "Destroyer", "EgyptianSavage",
    "Battering_Gaul", "sneaky", "Kamikaze", "KnightOnHorse",
    "ChineseMongolian", "Ottoman", "persian", "Poisonous", "PersianSapper"
};

    // ماب يربط الاسم بالـ Pool
    private Dictionary<string, UnitPool> warriorPoolMap = new Dictionary<string, UnitPool>();

    private bool isDeploying = false;

    void Awake()
    {
        // يربط نفسه مباشرة مع BarracksWatchPost الموجود على نفس الأوبجكت
        linkedBarracks = GetComponent<BarracksWatchPost>();

        if (linkedBarracks == null)
        {
            //Debug.LogWarning("⚠️ لم يتم العثور على BarracksWatchPost في نفس الكائن!");
        }
        LinkPoolsDynamically();
    }


    void Start()
    {
        InitializeBuildingStats();
        health = maxHealth;

        if (healthBarPrefab != null)
        {
            GameObject barGO = Instantiate(healthBarPrefab, GameObject.Find("Canvas").transform);
            healthBarUI = barGO.GetComponent<HealthBarFollow>();

            healthBarUI.target = this.transform;
            healthBarUI.Initialize(maxHealth);
            healthBarUI.UpdateHealth(health);
            barGO.SetActive(false);
        }

        if (lowHealthEffect != null)
        {
            lowHealthEffect.SetActive(false);
        }

    }
    void LinkPoolsDynamically()
    {
        foreach (string wName in warriorNames)
        {
            // نفترض أن اسم الأوبجكت في المشهد هو اسم الجندي + كلمة Pool (مثال: bedoiunPool)
            string expectedObjectName = wName + "Pool";
            GameObject poolObj = GameObject.Find(expectedObjectName);

            if (poolObj != null)
            {
                UnitPool poolScript = poolObj.GetComponent<UnitPool>();
                if (poolScript != null && !warriorPoolMap.ContainsKey(wName))
                {
                    warriorPoolMap.Add(wName, poolScript);
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ لم يتم العثور على أوبجكت باسم {expectedObjectName} في المشهد!");
            }
        }
    }

    void Update()
    {
        DetectEnemies();
    }

    void InitializeBuildingStats()
    {
        DefenceBuildData data = DefenceBuilds.GetBuildingByName(buildingName);
        if (data != null)
        {
            DefenceBuildLevelUI levelData = data.GetLevel(level);
            if (levelData != null)
            {
                health = levelData.health;
                maxHealth = health;
                attackRange = levelData.range;

                //Debug.Log($"{buildingName} (L{level}) initialized: HP={health}, RNG={attackRange}");  ديبك
            }
        }
    }

    void DetectEnemies()
    {
        if (linkedBarracks == null || isDeploying) return;

        string[] targetTags = { "Enemy", "monsterEnemy", "monsterFly", "EnemyFly" };

        foreach (string tag in targetTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= attackRange)
                {
                    // 🔹 إضافة الشرط هنا لمعرفة هل العدو طائر أم لا
                    bool isFlyingEnemy = (tag == "EnemyFly" || tag == "monsterFly");

                    // 🔹 تمرير المتغير للدالة
                    DeployWarriors(isFlyingEnemy);
                    return;
                }
            }
        }
    }

    // 🔹 إضافة المتغير isFlyingEnemy هنا
    void DeployWarriors(bool isFlyingEnemy)
    {
        if (linkedBarracks == null) return;

        isDeploying = true;
        // 🔹 تمرير المتغير للـ Coroutine
        StartCoroutine(DeployWarriorsCoroutine(isFlyingEnemy));
    }

    // 🔹 استقبال المتغير هنا
    private System.Collections.IEnumerator DeployWarriorsCoroutine(bool isFlyingEnemy)
    {
        Dictionary<string, int> warriors = linkedBarracks.GetWarriorDictionary();

        foreach (var entry in warriors)
        {
            string warriorType = entry.Key;

            // 🔹 الإضافة الجديدة: التحقق من نوع العدو والجنود المسموح لهم
            if (isFlyingEnemy)
            {
                if (warriorType != "Destroyer" && warriorType != "ScalingEgyptian")
                {
                    continue; // إذا كان العدو طائر والجندي ليس من هذين النوعين، تخطاه ولا تخرجه
                }
            }

            int count = entry.Value;

            for (int i = 0; i < count; i++)
            {
                // ... (باقي الكود الخاص بك داخل الـ for يبقى كما هو تماماً دون أي تغيير) ...
                UnitPool poolToUse = null;

                if (warriorPoolMap.ContainsKey(warriorType))
                {
                    poolToUse = warriorPoolMap[warriorType];
                }
                else
                {
                    Debug.LogWarning($"لا يوجد Pool للنوع {warriorType}!");
                    continue;
                }

                GameObject warriorGO = poolToUse.Spawn(transform.position);
                warriorGO.name = warriorType;

                ATTACKERS attackerScript = warriorGO.GetComponent<ATTACKERS>();
                if (attackerScript != null)
                {
                    attackerScript.SetBarracks(linkedBarracks);
                }
                else
                {
                    Debug.LogWarning($"⚠️ الـ Prefab المسمى {warriorType} لا يحتوي على سكربت ATTACKERS!");
                }

                linkedBarracks.RemoveWarrior(warriorType);

                yield return new WaitForSeconds(0.5f);
            }
        }

        isDeploying = false;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealth(health);
            healthBarUI.gameObject.SetActive(health < maxHealth && health > 0);
        }

        // الشرط الجديد: إذا كانت الصحة أقل من أو تساوي 30%
        if (lowHealthEffect != null)
        {
            if (health <= maxHealth * 0.30f && health > 0)
            {
                // نتحقق إذا كان التأثير غير مفعل لتشغيل الصوت لمرة واحدة فقط عند وصول الصحة لـ 30%
                if (!lowHealthEffect.activeSelf)
                {
                    AudioManager.Instance.PlaySFX("30HPbuild");
                    AudioManager.Instance.PlaySFX("30HPFire");

                    lowHealthEffect.SetActive(true); // إظهار الأوبجكت
                }
            }
            else if (health > maxHealth * 0.30f)
            {
                lowHealthEffect.SetActive(false); // إخفاء الأوبجكت إذا زادت الصحة
            }
        }

        if (health <= 0)
        {
            if (healthBarUI != null)
                Destroy(healthBarUI.gameObject);

            Bounds buildingBounds = GetComponent<Collider2D>().bounds;

            // 2. إظهار المبنى المهدم (الأنقاض) في نفس المكان والزاوية
            if (destroyedPrefab != null)
            {
                Instantiate(destroyedPrefab, transform.position, transform.rotation);
            }

            int randomNum = UnityEngine.Random.Range(0, 2);

            if (randomNum == 0)
            {
                AudioManager.Instance.PlaySFX("destroyBuild");
            }
            else
            {
                AudioManager.Instance.PlaySFX("boomberATK2");
            }

            building buildingScript = GetComponent<building>();
            if (buildingScript != null && GirdSystem2.current != null)
            {
                // نمرر الـ area الخاصة بسكربت building إلى GirdSystem2
                GirdSystem2.current.ClearBuildingArea(buildingScript.area);
            }
            Destroy(gameObject);
            // تحديث المنطقة المحيطة بالمبنى فقط
            GraphUpdateObject guo = new GraphUpdateObject(buildingBounds);
            AstarPath.active.UpdateGraphs(guo);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        int segments = 60;
        float angleStep = 360f / segments;
        Vector3 prevPoint = Vector3.zero;
        Quaternion tilt = Quaternion.Euler(45f, 0f, 0f);
        Vector3 center = transform.position;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 point = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * attackRange;
            point = tilt * point;

            if (i > 0)
                Gizmos.DrawLine(center + prevPoint, center + point);

            prevPoint = point;
        }
    }
}
