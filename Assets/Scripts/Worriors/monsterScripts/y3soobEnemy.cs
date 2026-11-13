using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI; // تأكد من أنك تستخدم NavMesh 2D

public class y3soobEnemy : MonoBehaviour
{
    [Header("Animation")]
    private Animator animator;
    private SpriteRenderer spriteRenderer; // لتجنب استدعاء GetComponent في الألوان


    private Transform targetHero;
    private GameObject targetHeroObject;

    public float health = 100f;
    public float maxHealth = 0f;
    public float speed = 2f;
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float attackRate = 1f;
    private float attackTimer = 0f;

    private Transform targetBuilding;
    private Collider2D targetBuildingCollider;

    public GameObject healthBarPrefab;
    private HealthBarFollow healthBarUI;

    // متغير جديد لإطلاق القذائف
    public GameObject projectilePrefab;
    public Transform firePoint;

    // متغيرات جديدة
    public int level = 1; // المستوى الافتراضي
    public string warriorName; // اسم المحارب

    // حفظ القيم الأصلية
    private float originalHealth;
    private float originalSpeed;
    private float originalDamage;

    private Coroutine tempBoostCoroutine;
    private Coroutine conditionalBoostCoroutine;

    public GameObject dieAnime;
    [Header("Pools")]
    private UnitPool effectPool; // الـ Pool الخاص بتأثيرات الموت (dieAnime)

    [Header("Blend Tree Animation")]
    private MonsterAnimationBlendTree blendTreeAnim;

    void Awake()
    {
        // 1. نبحث عن الأوبجكت في المشهد عن طريق اسمه
        GameObject poolObj = GameObject.Find("EffectPoolManager"); // ⚠️ تأكد أن الاسم مطابق لما في المشهد

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
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        blendTreeAnim = GetComponent<MonsterAnimationBlendTree>();



        originalHealth = health;
        originalSpeed = speed;
        originalDamage = attackDamage;

        maxHealth = health;
        // الحصول على المستوى من النص إذا كان موجوداً
        ParseLevelFromText();

        // تعيين خصائص المحارب بناءً على المستوى
        InitializeWarriorStats();

        maxHealth = health;
        FindTargetBuilding();

        if (healthBarPrefab != null)
        {
            GameObject barGO = Instantiate(healthBarPrefab, GameObject.Find("Canvas").transform);
            healthBarUI = barGO.GetComponent<HealthBarFollow>();
            healthBarUI.target = this.transform;
            healthBarUI.Initialize(maxHealth);
            healthBarUI.UpdateHealth(health);
            barGO.SetActive(false);
        }
    }

    // دالة لتحليل المستوى من النص
    private void ParseLevelFromText()
    {
        level = WarriorLevelManager.Instance.GetLevel(warriorName);
    }

    // دالة لتهيئة خصائص المحارب بناءً على المستوى
    private void InitializeWarriorStats()
    {
        // أسماء الوحوش اللي تريد التحقق منها
        string[] monsterNames = { "DraginFly"};

        bool isMonster = false;
        string matchedMonsterName = null;

        // تحقق إذا الاسم يطابق أو يبدأ بأي من أسماء الوحوش (يمكن تعديل حسب الحالة)
        foreach (string mName in monsterNames)
        {
            if (warriorName.StartsWith(mName, System.StringComparison.OrdinalIgnoreCase))
            {
                isMonster = true;
                matchedMonsterName = mName; // نخزن اسم الوحش اللي تطابق
                break;
            }
        }

        if (isMonster)
        {
            // جلب مستوى الوحش الحقيقي من MonsterLevelManager حسب اسم الوحش المطابق
            int currentLevel = MonsterLevelManager.Instance.GetLevel(matchedMonsterName);
            level = currentLevel;

            // جلب بيانات الوحش
            MonsterData monster = MonsterDatabase.GetMonsterByName(matchedMonsterName);
            if (monster != null)
            {
                MonsterLevel monsterLevel = monster.GetLevel(currentLevel);
                if (monsterLevel != null)
                {
                    health = monsterLevel.health;
                    speed = monsterLevel.speed;
                    attackDamage = monsterLevel.damage;
                }
            }
        }
    }
    public float boostRange = 10f;

    void Update()
    {
        // 1️⃣ أولوية قصوى: إذا تم الهجوم علينا، نتجه للمهاجم (targetHero) ونهاجمه فوراً
        if (targetHero != null)
        {
            if (!targetHeroObject.activeInHierarchy)
            {
                targetHero = null;
                targetHeroObject = null;
            }
            else
            {
                MoveTowardsHero();
                AttackHero();
                return; // نوقف الكود هنا للتركيز على المهاجم
            }
        }

        // 2️⃣ البحث عن أقرب حليف (المحاربين)
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;
        string[] enemyTags = { "Enemy", "EnemyFly" };

        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemyObj in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemyObj.transform.position);
                // التحقق مما إذا كان هذا المحارب هو GentleHealerFull
                HealerWarrior healerScript = enemyObj.GetComponent<HealerWarrior>();
                bool isGentleHealer = (healerScript != null && healerScript.warriorName == "GentleHealerFull");

                // 🛑 نجعله هدفاً للتتبع (closestEnemy) فقط إذا لم يكن GentleHealerFull
                if (!isGentleHealer && distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestEnemy = enemyObj;
                }

                // ✅ حساب قيمة التقوية (Boost) ديناميكياً بناءً على مستوى المحارب
                // إذا كان المستوى 1 فالنتيجة 1.1، وإذا كان 2 فالنتيجة 1.2 وهكذا...
                float dynamicBoostValue = 1.0f + (level * 0.1f);

                // ✅ تطبيق البف على الجميع، بما فيهم GentleHealerFull باستخدام القيمة الديناميكية
                IBuffable buffable = enemyObj.GetComponent<IBuffable>();
                if (buffable != null)
                {
                    if (distance <= boostRange)
                        buffable.ApplyRangeBoost(dynamicBoostValue, dynamicBoostValue, dynamicBoostValue);
                    else
                        buffable.RemoveRangeBoost();
                }
            }
        }

        // 3️⃣ إذا وجدنا حليفاً، نتبعه ونهاجم معه
        // 3️⃣ إذا وجدنا حليفاً، نتبعه ونهاجم معه
        if (closestEnemy != null)
        {
            Enemy enemyScript = closestEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                // الحصول على هدف المحارب
                Transform enemyTargetBuilding = enemyScript.GetType()
                    .GetField("targetBuilding", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.GetValue(enemyScript) as Transform;

                // 1. إذا كان المحارب يمتلك هدفاً، نتجه مباشرة نحو الهدف ونهاجمه معه
                if (enemyTargetBuilding != null)
                {
                    float distanceToBuilding = Vector3.Distance(transform.position, enemyTargetBuilding.position);

                    if (distanceToBuilding <= attackRange)
                    {
                        animator.SetBool("Attack", true);
                        attackTimer += Time.deltaTime;

                        if (attackTimer >= attackRate)
                        {
                            Shoot(enemyTargetBuilding);

                            // خصم الدمج
                            if (enemyTargetBuilding.CompareTag("spy"))
                            {
                                DefensiveWatchPost watchPost = enemyTargetBuilding.GetComponent<DefensiveWatchPost>();
                                if (watchPost != null) watchPost.TakeDamage(attackDamage);
                                showNumberPop showNum = enemyTargetBuilding.GetComponent<showNumberPop>();
                                if (showNum != null) showNum.OnHit(attackDamage);
                            }
                            else if (System.Array.Exists(defensiveTags, tag => enemyTargetBuilding.CompareTag(tag)))
                            {
                                DefensiveBuilding defBuilding = enemyTargetBuilding.GetComponent<DefensiveBuilding>();
                                if (defBuilding != null) defBuilding.TakeDamage(attackDamage);
                                showNumberPop showNum = enemyTargetBuilding.GetComponent<showNumberPop>();
                                if (showNum != null) showNum.OnHit(attackDamage);
                            }
                            else
                            {
                                building b = enemyTargetBuilding.GetComponent<building>();
                                if (b != null) b.TakeDamage(attackDamage);
                                TownHall hall = enemyTargetBuilding.GetComponent<TownHall>();
                                if (hall != null) hall.OnHit(attackDamage);
                                showNumberPop showNum = enemyTargetBuilding.GetComponent<showNumberPop>();
                                if (showNum != null) showNum.OnHit(attackDamage);
                            }

                            attackTimer = 0f;
                            AudioManager.Instance.PlaySFX("flyATK");
                        }
                    }
                    else
                    {
                        // التحرك نحو المبنى المستهدف مباشرة (بدون الالتفات لمسافة المحارب)
                        animator.SetBool("Attack", false);
                        transform.position = Vector3.MoveTowards(transform.position, enemyTargetBuilding.position, speed * Time.deltaTime);
                    }
                    return; // نوقف الكود هنا
                }
                else
                {
                    // 2. إذا كان المحارب موجوداً لكن ليس لديه هدف بعد، نتبعه باستمرار وبدون شرط مسافة
                    animator.SetBool("Attack", false);
                    transform.position = Vector3.MoveTowards(transform.position, closestEnemy.transform.position, speed * Time.deltaTime);
                    return;
                }
            }
        }

        // 4️⃣ إذا لم يكن هناك أي حليف في المشهد، نتصرف بشكل فردي ونبحث عن المباني
        FindTargetBuilding();
        if (targetBuilding != null)
        {
            MoveTowardsBuilding();
            AttackBuilding();
        }

    }

    void LateUpdate()
    {
        // هذه الدالة تعمل دائماً بعد انتهاء Update تماماً
        if (blendTreeAnim != null)
        {
            // إرسال حالة الهجوم الفعلية إلى سكربت الأنيميشن
            blendTreeAnim.isAttacking = animator.GetBool("Attack");
        }
    }


    void MoveTowardsHero()
    {
        float distance = Vector3.Distance(transform.position, targetHero.position);
        if (distance > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetHero.position, speed * Time.deltaTime);
        }

    }

    void AttackHero()
    {
        float distance = Vector3.Distance(transform.position, targetHero.position);
        if (distance <= attackRange)
        {
            animator.SetBool("Attack", true);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                if (projectilePrefab != null && firePoint != null)
                {
                    Shoot(targetHero);
                }
                attackTimer = 0f;
                AudioManager.Instance.PlaySFX("flyATK");

            }
        }
        else
        {
            animator.SetBool("Attack", false);

        }
    }

    void FindTargetBuilding()
    {
        string[] defensiveTags = { "spyClan", "spy","boomOP", "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "trapfire", "trapN", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "mongel" };

        string[] otherTags = { "spyClan", "spy", "boomOP", "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "trapfire", "trapN", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "mongel" };

        float shortestDistance = Mathf.Infinity;
        GameObject nearestBuilding = null;

        // أولاً: البحث عن أقرب مبنى دفاعي
        foreach (string tag in defensiveTags)
        {
            GameObject[] buildings = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject building in buildings)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestBuilding = building;
                }
            }
        }

        // إذا لم نجد مبنى دفاعي، نبحث في المباني الأخرى
        if (nearestBuilding == null)
        {
            foreach (string tag in otherTags)
            {
                GameObject[] buildings = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject building in buildings)
                {
                    float distance = Vector3.Distance(transform.position, building.transform.position);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestBuilding = building;
                    }
                }
            }
        }

        if (nearestBuilding != null)
        {
            targetBuilding = nearestBuilding.transform;
            targetBuildingCollider = nearestBuilding.GetComponent<Collider2D>();
        }
        else
        {
            targetBuilding = null;
            targetBuildingCollider = null;
        }
    }

    void MoveTowardsBuilding()
    {
        if (targetBuilding != null && targetBuildingCollider != null)
        {
            Vector2 closestPoint = targetBuildingCollider.ClosestPoint(transform.position);
            float distance = Vector2.Distance(transform.position, closestPoint);

            if (distance > attackRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, closestPoint, speed * Time.deltaTime);
            }
        }
    }

    private string[] defensiveTags = { "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "boomOP" };

    void AttackBuilding()
    {
        if (targetBuilding != null && targetBuildingCollider != null)
        {
            Vector2 closestPoint = targetBuildingCollider.ClosestPoint(transform.position);
            float distance = Vector2.Distance(transform.position, closestPoint);

            if (distance <= attackRange)
            {
                animator.SetBool("Attack", true);

                attackTimer += Time.deltaTime;
                if (attackTimer >= attackRate)
                {

                    // **رمي القذيفة**
                    Shoot(targetBuilding);

                    // التعامل مع الضرر حسب نوع المبنى
                    if (targetBuilding.CompareTag("spy"))
                    {
                        DefensiveWatchPost watchPost = targetBuilding.GetComponent<DefensiveWatchPost>();
                        if (watchPost != null) watchPost.TakeDamage(attackDamage);
                        showNumberPop showNum = targetBuilding.GetComponent<showNumberPop>();
                        if (showNum != null)
                            showNum.OnHit(attackDamage);
                    }
                    else if (System.Array.Exists(defensiveTags, tag => targetBuilding.CompareTag(tag)))
                    {
                        DefensiveBuilding defBuilding = targetBuilding.GetComponent<DefensiveBuilding>();
                        if (defBuilding != null)
                        {
                            defBuilding.TakeDamage(attackDamage);

                            showNumberPop showNum = targetBuilding.GetComponent<showNumberPop>();
                            if (showNum != null)
                                showNum.OnHit(attackDamage);
                        }
                    }
                    else
                    {
                        building b = targetBuilding.GetComponent<building>();
                        if (b != null)
                        {
                            b.TakeDamage(attackDamage);
                            showNumberPop showNum = targetBuilding.GetComponent<showNumberPop>();
                            if (showNum != null)
                                showNum.OnHit(attackDamage);
                        }

                        TownHall hall = targetBuilding.GetComponent<TownHall>();
                        if (hall != null)
                            hall.OnHit(attackDamage);
                    }

                    attackTimer = 0f;
                    AudioManager.Instance.PlaySFX("flyATK");

                }

            }
            else
            {
                animator.SetBool("Attack", false);

            }
        }
    }


    void Shoot(Transform target)
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.SetTarget(target);
                projectileScript.damage = attackDamage;

            }
        }
    }

    public void TakeDamage(float damage, Transform attacker = null)
    {
        health -= damage;

        // إظهار رقم الضرر
        showNumberPop showNum = GetComponent<showNumberPop>();
        if (showNum != null) showNum.OnHit(damage);

        // تحديث شريط الصحة
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealth(health);
            healthBarUI.gameObject.SetActive(health < maxHealth && health > 0);
        }

        // 🔥 إذا تعرضنا للهجوم، نجعل المهاجم هو الهدف الأساسي فوراً
        if (attacker != null)
        {
            targetHero = attacker;
            targetHeroObject = attacker.gameObject;
        }

        if (health <= 0)
        {
            if (healthBarUI != null)
                Destroy(healthBarUI.gameObject);

            Die();
        }
    }
    void Die()
    {
        // 1. استدعاء تأثير الموت من الـ Pool بدلاً من Instantiate
        // الكود الذي كتبناه سابقاً
        if (effectPool != null)
        {
            GameObject effect = effectPool.Spawn(transform.position);
            if (effect != null) effect.transform.rotation = transform.rotation;
        }

        AudioManager.Instance.PlaySFX("dieMonster");
        Destroy(transform.parent.gameObject);
    }

    public void HightWithSpeel(float healthMultiplier, float speedMultiplier, float damageMultiplier, float duration)
    {
        if (tempBoostCoroutine != null)
            StopCoroutine(tempBoostCoroutine);

        tempBoostCoroutine = StartCoroutine(TemporaryBoost(healthMultiplier, speedMultiplier, damageMultiplier, duration));
    }

    private IEnumerator TemporaryBoost(float healthMultiplier, float speedMultiplier, float damageMultiplier, float duration)
    {
        float prevHealth = health;
        float prevSpeed = speed;
        float prevDamage = attackDamage;

        health *= healthMultiplier;
        speed *= speedMultiplier;
        attackDamage *= damageMultiplier;

        yield return new WaitForSeconds(duration);

        health = prevHealth;
        speed = prevSpeed;
        attackDamage = prevDamage;
    }

    public void HightWithSpeelCondition(float healthMultiplier, float speedMultiplier, float damageMultiplier, System.Func<bool> stopCondition)
    {
        if (conditionalBoostCoroutine != null)
            StopCoroutine(conditionalBoostCoroutine);

        conditionalBoostCoroutine = StartCoroutine(ConditionalBoost(healthMultiplier, speedMultiplier, damageMultiplier, stopCondition));
    }

    private IEnumerator ConditionalBoost(float healthMultiplier, float speedMultiplier, float damageMultiplier, System.Func<bool> stopCondition)
    {
        float prevHealth = health;
        float prevSpeed = speed;
        float prevDamage = attackDamage;

        health *= healthMultiplier;
        speed *= speedMultiplier;
        attackDamage *= damageMultiplier;

        while (!stopCondition())
        {
            yield return null;
        }

        health = prevHealth;
        speed = prevSpeed;
        attackDamage = prevDamage;
    }

    public void StartReturnColor(float time)
    {
        StartCoroutine(ReturnWhiteAfterTime(time));
    }

    IEnumerator ReturnWhiteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.white;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        int segments = 60; // عدد الخطوط في الدائرة
        float angleStep = 360f / segments;
        Vector3 prevPoint = Vector3.zero;
        Quaternion tilt = Quaternion.Euler(45f, 0f, 0f); // الميلان على محور X
        Vector3 center = transform.position;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 point = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * 10;

            // تدوير النقطة لإمالة الدائرة
            point = tilt * point;

            if (i > 0)
                Gizmos.DrawLine(center + prevPoint, center + point);

            prevPoint = point;
        }

        // 🔍 فحص وجود Enemy داخل الدائرة
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) <= 10)
            {
                // 📌 هنا العدو داخل مدى الـ Gizmos
                // مبدئياً فقط إظهار خط للمساعدة البصرية
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, enemy.transform.position);
                Debug.Log("the enemy in the range");
                //Enemy.instan.ApplyBoostOnce(0.2f); // زيادة 20%


            }
            else
            {
                Debug.Log("the enemy out of  the range");
                //Enemy.instan.ApplyBoostOnce(0f); // زيادة 20%


            }

        }
    }

    // ✅ دالة لإضافة صحة (Healing)
    public void Heal(float amount)
    {
        health += amount;

        HellTextPop showNum = GetComponent<HellTextPop>();
        if (showNum != null) showNum.OnHit(amount);

        health = Mathf.Min(health, maxHealth);

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealth(health);
            healthBarUI.gameObject.SetActive(health < maxHealth && health > 0);
        }
    }

    public void RunAway()
    {
        StartCoroutine(RunAwayCoroutine());
    }

    private IEnumerator RunAwayCoroutine()
    {
        // تحديد اتجاه الهروب (يمين أو يسار)
        float screenX = Camera.main.WorldToScreenPoint(transform.position).x;
        float screenWidth = Screen.width;

        Vector3 escapeDirection;

        if (screenX < screenWidth / 2f)
        {
            // اهرب لليسار
            escapeDirection = Vector3.left;
        }
        else
        {
            // اهرب لليمين
            escapeDirection = Vector3.right;
        }

        // تعطيل الأهداف
        targetHero = null;
        targetHeroObject = null;
        targetBuilding = null;
        targetBuildingCollider = null;


        float escapeSpeed = 3f;
        float escapeTime = 10f;
        float timer = 0f;

        // الهروب المستمر
        while (timer < escapeTime)
        {
            transform.position += escapeDirection * escapeSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

  
}
