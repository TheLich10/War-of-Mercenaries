using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class DefensiveBuilding : MonoBehaviour
{
    [Header("fire effict")]
    public GameObject lowHealthEffect;

    [Header("destroy build")]
    public GameObject destroyedPrefab; // اسحب هنا الـ Prefab الخاص بالأنقاض


    [Header("البيانات")]
    public string buildingName; // اسم المبنى
    public int level = 1;

    [Header("الخصائص")]
    public float health = 200f;
    public float maxHealth = 0f;

    public float attackRange = 5f;
    public float attackRate = 1f;
    public float damage = 50f;

    [Header("الإطلاق")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float attackTimer = 0f;
    private GameObject currentTarget; // يحفظ العدو الذي يتم ضربه حالياً

    //private bool isAttacking = false; // لتشغيل الكوروتين لمرة واحدة

    public GameObject healthBarPrefab;
    private HealthBarFollow healthBarUI;

    public float damagePercent = 0.01f; // 1% بشكل افتراضي

    private Dictionary<GameObject, float> beastDamageCache = new Dictionary<GameObject, float>();


    public int sorlevl = 0;
    public GameObject sorlvl1; // اسحب الصورة من Inspector
    public GameObject sorlvl2; // اسحب الصورة من Inspector
    public GameObject sorlvl3; // اسحب الصورة من Inspector
    public GameObject sorlvl4; // اسحب الصورة من Inspector


    void Start()
    {
        if (transform.parent != null)
        {
            building parent = transform.parent.GetComponent<building>();
            if (parent != null)
                sorlevl = parent.sorlevl;
        }
        if (lowHealthEffect != null)
        {
            lowHealthEffect.SetActive(false);
        }

        UpdateSorLevelVisual();
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

        // إذا كان المبنى GALTING ARROW TOWER نشغل الكوروتين الخاص به
        if (buildingName == "GALTING ARROW TOWER")
        {
            StartCoroutine(GaltingAttackPattern());
        }

        // أضفنا FLAMER TOWER ليقوم بتشغيل نفس نمط الضرب المتتالي
        if (buildingName == "SKY TOWER")
        {
            StartCoroutine(SkyAttackPattern());
        }

        // أضفنا FLAMER TOWER ليقوم بتشغيل نفس نمط الضرب المتتالي
        if ( buildingName == "FLAMER TOWER")
        {
            StartCoroutine(FlameAttackPattern());
        }
    }

    void Update()
    {
        // بالنسبة لباقي المباني الهجوم العادي (تم استثناء FLAMER TOWER ليستخدم الكوروتين)
        if (buildingName != "GALTING ARROW TOWER" && buildingName != "SKY TOWER" && buildingName != "FLAMER TOWER")
        {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackRate)
                {
                    AttackEnemy();
                    attackTimer = 0f;
                }
            }
    }

    IEnumerator GaltingAttackPattern()
    {
        while (true)
        {
            bool hasPlayedSound = false; // متغير للتأكد من تشغيل الصوت مرة واحدة

            // يطلق 10 مرات
            for (int i = 0; i < 10; i++)
            {
                AttackEnemy();

                // إذا وجد هدفاً ولم يتم تشغيل الصوت بعد في هذه الدفعة، شغله
                if (currentTarget != null && !hasPlayedSound)
                {
                    AudioManager.Instance.PlaySFX("archerOP_ATK");
                    hasPlayedSound = true;
                }

                yield return new WaitForSeconds(attackRate); // الوقت بين الطلقات
            }

            // استراحة ثانية كاملة
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator SkyAttackPattern()
    {
        while (true)
        {
            bool hasPlayedSound = false;

            // يطلق 4 مرات
            for (int i = 0; i < 4; i++)
            {
                AttackEnemy();

                if (currentTarget != null && !hasPlayedSound)
                {
                    AudioManager.Instance.PlaySFX("skyBuildATK");
                    hasPlayedSound = true;
                }

                yield return new WaitForSeconds(attackRate); // الوقت بين الطلقات
            }

            // استراحة ثانية كاملة
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator FlameAttackPattern()
    {
        while (true)
        {
            bool hasPlayedSound = false;

            // يطلق 3 مرات
            for (int i = 0; i < 4; i++)
            {
                AttackEnemy();

                if (currentTarget != null && !hasPlayedSound)
                {
                    AudioManager.Instance.PlaySFX("flameBuildATk");
                    hasPlayedSound = true;
                }

                yield return new WaitForSeconds(attackRate); // الوقت بين الطلقات
            }

            // استراحة ثانية كاملة
            yield return new WaitForSeconds(3f);
        }
    }

    void InitializeBuildingStats()
    {
        DefenceBuildData data = DefenceBuilds.GetBuildingByName(buildingName);
        if (data != null)
        {
            DefenceBuildLevelUI levelData = data.GetLevel(level);
            if (levelData != null)
            {
                if (buildingName == "BEAST CANNON")
                {
                    damagePercent = 0.01f + 0.005f * (level - 1); // مثال: كل مستوى يزيد 0.5%
                }

                health = levelData.health;

                if (sorlevl == 1)
                {
                    health = health + health * 0.20f;
                }
                else if (sorlevl == 2)
                {
                    health = health + health * 0.30f;
                }
                else if (sorlevl == 3)
                {
                    health = health + health * 0.40f;
                }
                else if (sorlevl == 4)
                {
                    health = health + health * 0.50f;
                }

                maxHealth = health;

                attackRange = levelData.range;
                attackRate = levelData.shotsPerCharge;
                damage = levelData.damage;

                //Debug.Log($"{buildingName} (L{level}) initialized: HP={health}, DMG={damage}, RNG={attackRange}, Rate={attackRate}"); ديبك
            }
            else
            {
                Debug.LogWarning($"No level {level} found for building {buildingName}");
            }
        }
        else
        {
            Debug.LogWarning($"No building found with name {buildingName}");
        }
    }

    void AttackEnemy()
    {
        // 1. التحقق مما إذا كان الهدف الحالي موجوداً، حياً، وما زال ضمن النطاق
        if (currentTarget != null && currentTarget.activeInHierarchy)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distanceToTarget > attackRange)
            {
                currentTarget = null; // الهدف خرج من النطاق، قم بتفريغ المتغير للبحث عن غيره
            }
        }
        else
        {
            currentTarget = null; // الهدف مات أو اختفى
        }

        GameObject nearestEnemy = currentTarget; // نعتمد الهدف الحالي كهدف أساسي

        // 2. إذا لم يكن لدينا هدف (أو مات/ابتعد)، نبحث عن هدف جديد
        if (nearestEnemy == null)
        {
            string[] targetTags = { "Enemy", "EnemyFly", "monsterFly", "monsterEnemy" };
            float shortestDistance = Mathf.Infinity;

            foreach (string tag in targetTags)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject enemy in enemies)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (distance < shortestDistance && distance <= attackRange)
                    {
                        bool canAttack = false;

                        // ====== (شروط الهجوم الخاصة بك تبقى هنا كما هي بالضبط دون تغيير) ======
                        // (مثل شروط SKY TOWER و BEAST CANNON وما إلى ذلك)
                        if (buildingName == "SKY TOWER")
                        {
                            if (tag == "EnemyFly" || tag == "monsterFly") canAttack = true;
                        }
                        else
                        {
                            if (buildingName == "BEAST CANNON")
                            {
                                if (tag == "monsterEnemy" || tag == "monsterFly") canAttack = true;
                            }
                            else
                            {
                                if (tag == "Enemy" || tag == "monsterEnemy") canAttack = true;
                                else if (tag == "EnemyFly" || tag == "monsterFly")
                                {
                                    if (buildingName == "GALTING ARROW TOWER" || buildingName == "ARCHER'S TOWER")
                                        canAttack = true;
                                }
                            }
                        }
                        // =========================================================================

                        if (canAttack)
                        {
                            sapperEnemy enemyScript = enemy.GetComponent<sapperEnemy>();
                            if (enemyScript != null)
                            {
                                if (!enemyScript.isMoving)
                                {
                                    shortestDistance = distance;
                                    nearestEnemy = enemy;
                                }
                            }
                            else
                            {
                                shortestDistance = distance;
                                nearestEnemy = enemy;
                            }
                        }
                    }
                }
            }

            // 3. قفل الهدف الجديد: إذا وجدنا عدواً، نحفظه في المتغير لكي نستمر بضربه
            if (nearestEnemy != null)
            {
                currentTarget = nearestEnemy;
            }
        }

        if (nearestEnemy != null)
        {
            if (buildingName == "BEAST CANNON")
            {
                if (nearestEnemy.GetComponent<Enemy>() != null)
                    ApplyPercentageDamage(nearestEnemy);
                if (nearestEnemy.GetComponent<EnemyRNG_Advanced>() != null)
                    ApplyPercentageDamageRange(nearestEnemy);
                else if (nearestEnemy.GetComponent<y3soobEnemy>() != null)
                    ApplyPercentageDamageY3soob(nearestEnemy);

                int randomNum = UnityEngine.Random.Range(0, 2);

                if (randomNum == 0)
                {
                    AudioManager.Instance.PlaySFX("boomberATK");
                }
                else
                {
                    AudioManager.Instance.PlaySFX("boomberATK2");
                }
            }

            else if (buildingName == "BURNING MIRRORS")
            {
                ShootLine(nearestEnemy.transform);
                AudioManager.Instance.PlaySFX("mirrorATK");
            }
            else if (buildingName == "SKY TOWER")
            {
                ShootWoods(nearestEnemy.transform);
            }
            else if (buildingName == "FLAMER TOWER")
            {
                ShootLine(nearestEnemy.transform);
            }
            else if (buildingName == "BOMBARD TOWER")
            {
                ShootBoomb(nearestEnemy.transform);
                int randomNum = UnityEngine.Random.Range(0, 2);

                if (randomNum == 0)
                {
                    AudioManager.Instance.PlaySFX("boomberATK");
                }
                else
                {
                    AudioManager.Instance.PlaySFX("boomberATK2");
                }
            }
            else if (buildingName == "ARCHER'S TOWER")
            {
                Shoot(nearestEnemy.transform);
                AudioManager.Instance.PlaySFX("archerATK");
            }
            else if (buildingName == "GALTING ARROW TOWER")
            {
                Shoot(nearestEnemy.transform);
            }
            else
            {
                Shoot(nearestEnemy.transform);
            }
        }
    }



    void ApplyPercentageDamage(GameObject enemy)
{
    Enemy enemyScript = enemy.GetComponent<Enemy>(); // سكريبت العدو
    if (enemyScript != null)
    {
        float dmg;

        // إذا لم يتم حساب الضرر مسبقًا لهذا العدو
        if (!beastDamageCache.ContainsKey(enemy))
        {
                dmg = Mathf.Round(enemyScript.maxHealth * damagePercent);
                beastDamageCache[enemy] = dmg; // خزنه
        }
        else
        {
            dmg = beastDamageCache[enemy]; // استخدم الضرر المحسوب مسبقًا
        }
            ShootOboomb(enemyScript.transform, dmg);
            //enemyScript.TakeDamage(dmg);

            Debug.Log($"BEAST CANNON hit {enemy.name} for {dmg} damage.");
    }
}
    void ApplyPercentageDamageRange(GameObject enemy)
    {
        EnemyRNG_Advanced enemyScript = enemy.GetComponent<EnemyRNG_Advanced>(); // سكريبت العدو
        if (enemyScript != null)
        {
            float dmg;

            // إذا لم يتم حساب الضرر مسبقًا لهذا العدو
            if (!beastDamageCache.ContainsKey(enemy))
            {
                dmg = Mathf.Round(enemyScript.maxHealth * damagePercent);
                beastDamageCache[enemy] = dmg; // خزنه
            }
            else
            {
                dmg = beastDamageCache[enemy]; // استخدم الضرر المحسوب مسبقًا
            }
            ShootOboomb(enemyScript.transform, dmg);
            //enemyScript.TakeDamage(dmg);

            Debug.Log($"BEAST CANNON hit {enemy.name} for {dmg} damage.");
        }
    }

    void ApplyPercentageDamageY3soob(GameObject enemy)
    {
        y3soobEnemy enemyScript = enemy.GetComponent<y3soobEnemy>(); // سكريبت العدو
        if (enemyScript != null)
        {
            float dmg;

            // إذا لم يتم حساب الضرر مسبقًا لهذا العدو
            if (!beastDamageCache.ContainsKey(enemy))
            {
                dmg = Mathf.Round(enemyScript.maxHealth * damagePercent);
                beastDamageCache[enemy] = dmg; // خزنه
            }
            else
            {
                dmg = beastDamageCache[enemy]; // استخدم الضرر المحسوب مسبقًا
            }

            ShootOboomb(enemyScript.transform, dmg);
            //enemyScript.TakeDamage(dmg);

            Debug.Log($"BEAST CANNON hit {enemy.name} for {dmg} damage (y3soob).");
        }
    }




    void Shoot(Transform target, float customDamage = 0f)
    {
        Vector3 dir = (target.position - firePoint.position).normalized;

        // حساب زاوية الدوران بناءً على اتجاه الهدف
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // نصنع السهم مع تدويره
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0, 0, angle));

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetTarget(target);
        projectileScript.damage = customDamage > 0f ? customDamage : damage;
    }

    void ShootOboomb(Transform target, float customDamage = 0f)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, projectilePrefab.transform.rotation);

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.SetTarget(target);
        projectileScript.damage = customDamage > 0f ? customDamage : damage;
    }

    void ShootBoomb(Transform target)
    {

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, projectilePrefab.transform.rotation);
        ProjectileAoE projectileScript = projectile.GetComponent<ProjectileAoE>();
        projectileScript.SetTarget(target);
        projectileScript.damage = damage;
    }

    void ShootLine(Transform target)
    {
        Vector2 dir = (target.position - firePoint.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0, 0, angle));

        mirrorLine projectileScript = projectile.GetComponent<mirrorLine>();
        projectileScript.damage = damage;
        projectileScript.DestroyAfterDelay(2.5f);
    }

    void ShootWoods(Transform target)
    {
        Vector2 dir = (target.position - firePoint.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0, 0, angle));

        HomingAOEProjectile projectileScript = projectile.GetComponent<HomingAOEProjectile>();
        projectileScript.SetTarget(target);
        projectileScript.damage = damage;

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
            // 1. حذف شريط الصحة
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

            // 3. تدمير المبنى الحالي بالكامل
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

    public void IncreaseSorLevel(int amount = 1)
    {
        sorlevl += amount;

        // نتأكد أنه ما يطلع عن الحدود (0 - 4)
        sorlevl = Mathf.Clamp(sorlevl, 0, 4);

        // تحديث الصور فورًا
        UpdateSorLevelVisual();

        // إذا كان الأب يحتوي على أبناء، نزامنهم مع الأب
        DefensiveBuilding[] children = GetComponentsInChildren<DefensiveBuilding>();
        foreach (DefensiveBuilding child in children)
        {
            if (child != this)
            {
                child.sorlevl = sorlevl;
                child.UpdateSorLevelVisual(); // تحديث الصور للأبناء مباشرة
            }
        }

        Debug.Log($"🔼 Sor Level الجديد للمبنى {buildingName}: {sorlevl}");
    }


    private void UpdateSorLevelVisual()
    {
        // نغلق كل المستويات إذا كانت موجودة
        if (sorlvl1 != null) sorlvl1.SetActive(false);
        if (sorlvl2 != null) sorlvl2.SetActive(false);
        if (sorlvl3 != null) sorlvl3.SetActive(false);
        if (sorlvl4 != null) sorlvl4.SetActive(false);

        // بعدين نشغل المستوى المناسب حسب القيمة إذا كان موجود
        switch (sorlevl)
        {
            case 1:
                if (sorlvl1 != null) sorlvl1.SetActive(true);
                break;
            case 2:
                if (sorlvl2 != null) sorlvl2.SetActive(true);
                break;
            case 3:
                if (sorlvl3 != null) sorlvl3.SetActive(true);
                break;
            case 4:
                if (sorlvl4 != null) sorlvl4.SetActive(true);
                break;
        }
    }

    // 🟢 دالة جديدة لاستقبال مستوى السور من ملف الحفظ وتحديث الصور فوراً
    // 🟢 دالة جديدة لاستقبال مستوى السور من ملف الحفظ وتحديث الصور والطاقة
    public void LoadSorLevel(int savedLevel)
    {
        sorlevl = savedLevel;
        UpdateSorLevelVisual();

        // تحديث السكربت الأب لمنع التعارض
        if (transform.parent != null)
        {
            building parent = transform.parent.GetComponent<building>();
            if (parent != null)
            {
                parent.sorlevl = savedLevel;
            }
        }

        // 🟢 تحديث طاقة المبنىناءً على مستوى السور الجديد لكي لا يعود للصفر
        InitializeBuildingStats();
        health = maxHealth;

        if (healthBarUI != null)
        {
            healthBarUI.Initialize(maxHealth);
            healthBarUI.UpdateHealth(health);
        }

        Debug.Log($"✅ تم تحميل مستوى السور للمبنى {buildingName} بنجاح، المستوى: {savedLevel}");
    }

}
