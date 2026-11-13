using Pathfinding;
using System.Collections;
using UnityEngine;

public class HealerWarrior : MonoBehaviour, IBuffable
{
    [Header("Animation")]
    private Animator animator;

    private SpriteRenderer spriteRenderer; // لتجنب استدعاء GetComponent في الألوان

    public int level = 1;            // المستوى الافتراضي
    public string warriorName;       // اسم المحارب

    public float health;             // صحة المحارب
    public float maxHealth = 0f;

    public float speed;              // سرعة المحارب
    public float healAmount = 5f;    // مقدار الشفاء كل مرة (نجيبه من الدمج في البيانات)
    public float healRate = 1f;      // كم ثانية بين كل عملية شفاء
    public float healRange = 5f;     // مدى البحث عن المحاربين
    public float followDistance = 2f;// المسافة اللي يوقف عندها بجانب المحارب

    private float healTimer = 0f;

    public GameObject healthBarPrefab;
    private HealthBarFollow healthBarUI;

    public GameObject projectilePrefab;
    public Transform firePoint;

    string[] allyTags = { "Enemy", "EnemyFly", "monsterFly", "monsterEnemy" };

    public GameObject dieAnime;
    [Header("Pools")]
    private UnitPool effectPool; // الـ Pool الخاص بتأثيرات الموت (dieAnime)

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

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        maxHealth = health;

        // تهيئة بيانات المحارب عند بداية التشغيل
        InitializeWarriorStats();
        maxHealth = health;

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

    void Update()
    {
        healTimer += Time.deltaTime;

        // ✅ الحصول على أقرب جندي مصاب (للحركة نحوه فقط)
        Transform closestAlly = FindClosestAlly();

        if (closestAlly != null)
        {
            float distance = Vector3.Distance(transform.position, closestAlly.position);

            // التحرك باتجاه الجندي الأقرب المصاب
            if (distance > followDistance)
            {
                transform.position = Vector3.MoveTowards(
                 transform.position,
                 closestAlly.position,
                 Time.deltaTime * speed
                );

                HandleAttackAnimation(Vector3.zero, false);
            }
            else
            {
                HandleAttackAnimation(closestAlly.position, true);
            }
        }
        else
        {
            HandleAttackAnimation(Vector3.zero, false);
        }

        // ✅ معالجة جميع الحلفاء اللي داخل مدى healRange
        // داخل دالة Update
        if (healTimer >= healRate)
        {
            // نحفظ نتيجة العلاج في متغير
            bool didHeal = HealAlliesInRange();

            healTimer = 0f;

            // نشغل الصوت فقط إذا نجحت عملية علاج واحدة على الأقل
            if (didHeal)
            {
                AudioManager.Instance.PlaySFX("hlrATK");
            }
        }
    }

    // دالة جديدة: ترجع أقرب جندي ناقص دم
    // دالة ترجع أقرب جندي ناقص دم
    // دالة ترجع أقرب جندي ناقص دم فقط
    // دالة ترجع أقرب جندي ناقص دم، وإذا لم يوجد ترجع أقرب جندي سليم
    Transform FindClosestAlly()
    {
        Transform closestWounded = null;
        float shortestWoundedDist = Mathf.Infinity;

        Transform closestHealthy = null;
        float shortestHealthyDist = Mathf.Infinity;

        foreach (string tag in allyTags)
        {
            GameObject[] allies = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject ally in allies)
            {
                // ✅ تجاهل الشافي نفسه، والأب الخاص به (لتجنب أن يعتبر نفسه أقرب جندي سليم ويقف في مكانه)
                if (ally.transform == transform || (transform.parent != null && ally.transform == transform.parent))
                    continue;

                // ✅ التحقق من أن الأوبجكت هو جندي فعلي وليس مجرد أوبجكت فارغ يحمل التاج
                if (!IsValidAlly(ally))
                    continue;

                float distance = Vector3.Distance(transform.position, ally.transform.position);

                // التحقق مما إذا كان الجندي يحتاج لعلاج
                if (NeedsHealing(ally))
                {
                    if (distance < shortestWoundedDist)
                    {
                        shortestWoundedDist = distance;
                        closestWounded = ally.transform;
                    }
                }
                else // إذا كان الجندي لا يحتاج لعلاج (دمه ممتلئ)
                {
                    if (distance < shortestHealthyDist)
                    {
                        shortestHealthyDist = distance;
                        closestHealthy = ally.transform;
                    }
                }
            }
        }

        // ✅ الأولوية: إرجاع أقرب جندي مصاب، وإذا لم يوجد نرجع أقرب جندي سليم ليلحقه
        if (closestWounded != null)
        {
            return closestWounded;
        }
        else
        {
            return closestHealthy;
        }
    }

    // ✅ دالة مساعدة للتأكد من أن الأوبجكت الذي يحمل التاج هو جندي صالح للتتبع والشفاء
    private bool IsValidAlly(GameObject ally)
    {
        return ally.GetComponent<Enemy>() != null ||
               ally.GetComponent<sapperEnemy>() != null ||
               ally.GetComponent<y3soobEnemy>() != null ||
               ally.GetComponent<flayingEnemy>() != null ||
               ally.GetComponent<EnemyRNG_Advanced>() != null;
    }




    private void InitializeWarriorStats()
    {
        WarriorTring warrior = WarriorTringDatabase.GetWarriorByName(warriorName);
        if (warrior != null)
        {
            WarriorLevel warriorLevel = warrior.GetLevel(level);
            if (warriorLevel != null)
            {
                health = warriorLevel.health;
                speed = warriorLevel.speed;
                healAmount = warriorLevel.damage;   // مقدار الهيل = قيمة الدمج
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
        AudioManager.Instance.PlaySFX("GirlDie");
        Destroy(transform.parent.gameObject);
    }

    bool HealAlliesInRange()
    {
        float epsilon = 0.1f;
        bool hasHealedSomeone = false; // متغير لمعرفة هل تم علاج أحد

        foreach (string tag in allyTags)
        {
            GameObject[] allies = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject ally in allies)
            {
                float distance = Vector3.Distance(transform.position, ally.transform.position);

                if (distance <= healRange)
                {
                    if (TryHeal(ally, epsilon))
                    {
                        Shoot(ally.transform);
                        Debug.Log($"{warriorName} healed {ally.name}");
                        hasHealedSomeone = true; // تم العلاج بنجاح
                    }
                }
            }
        }

        return hasHealedSomeone; // إرجاع النتيجة
    }

    // دالة مساعدة لمعرفة ما إذا كانت الشخصية تحتاج لعلاج (دمها ناقص)
    private bool NeedsHealing(GameObject ally)
    {
        float epsilon = 0.1f;

        Enemy e = ally.GetComponent<Enemy>();
        if (e != null) return e.health < e.maxHealth - epsilon;

        sapperEnemy s = ally.GetComponent<sapperEnemy>();
        if (s != null) return s.health < s.maxHealth - epsilon;

        y3soobEnemy y = ally.GetComponent<y3soobEnemy>();
        if (y != null) return y.health < y.maxHealth - epsilon;

        flayingEnemy f = ally.GetComponent<flayingEnemy>();
        if (f != null) return f.health < f.maxHealth - epsilon;

        EnemyRNG_Advanced rng = ally.GetComponent<EnemyRNG_Advanced>();
        if (rng != null) return rng.health < rng.maxHealth - epsilon;

        return false;
    }

    // دالة مساعدة تقوم بتطبيق العلاج على المكون المناسب
    private bool TryHeal(GameObject ally, float epsilon)
    {
        Enemy e = ally.GetComponent<Enemy>();
        if (e != null && e.health < e.maxHealth - epsilon) { e.Heal(healAmount); return true; }

        sapperEnemy s = ally.GetComponent<sapperEnemy>();
        if (s != null && s.health < s.maxHealth - epsilon) { s.Heal(healAmount); return true; }

        y3soobEnemy y = ally.GetComponent<y3soobEnemy>();
        if (y != null && y.health < y.maxHealth - epsilon) { y.Heal(healAmount); return true; }

        flayingEnemy f = ally.GetComponent<flayingEnemy>();
        if (f != null && f.health < f.maxHealth - epsilon) { f.Heal(healAmount); return true; }

        EnemyRNG_Advanced rng = ally.GetComponent<EnemyRNG_Advanced>();
        if (rng != null && rng.health < rng.maxHealth - epsilon) { rng.Heal(healAmount); return true; }

        return false; // لم يتم العلاج
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
            }
        }
    }
    private Coroutine tempBoostCoroutine;

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
        float prevDamage = healAmount;

        health *= healthMultiplier;
        speed *= speedMultiplier;
        healAmount *= damageMultiplier;

        yield return new WaitForSeconds(duration);

        health = prevHealth;
        speed = prevSpeed;
        healAmount = prevDamage;
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

    private bool isRangeBoostActive = false;

    private float rangePrevHealth;
    private float rangePrevSpeed;
    private float rangePrevDamage;

    public void ApplyRangeBoost(float healthMultiplier, float speedMultiplier, float damageMultiplier)
    {
        if (isRangeBoostActive) return;

        isRangeBoostActive = true;

        rangePrevHealth = health;

        rangePrevSpeed = speed;

        rangePrevDamage = healAmount;

        // تطبيق الصقل
        health *= healthMultiplier;

        speed *= speedMultiplier;

        healAmount *= damageMultiplier;
        blueColor();
        Debug.Log("Range boost applied");
    }

    public void RemoveRangeBoost()
    {
        if (!isRangeBoostActive) return;

        isRangeBoostActive = false;

        health = rangePrevHealth;

        speed = rangePrevSpeed;

        healAmount = rangePrevDamage;

        StartReturnColor(0f);
        whiteClor();
        Debug.Log("Range boost removed");
    }

    public void blueColor()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(0.6f, 0.8f, 1f, 1f); ;
    }

    public void whiteClor()
    {

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.white;
    }

    private void HandleAttackAnimation(Vector3 targetPos, bool isAttacking)
    {
        if (animator == null || spriteRenderer == null) return;

        // +++ أضف هذين السطرين هنا للربط بين السكربتين وإيقاف التذبذب +++
        PlayerAnimationOnly playerAnim = GetComponent<PlayerAnimationOnly>();
        if (playerAnim != null) playerAnim.isAttacking = isAttacking;
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // إيقاف الأنيميشن إذا لم يكن في حالة هجوم
        if (!isAttacking)
        {
            animator.SetBool("isATKUP", false);
            animator.SetBool("isATKDOWN", false);
            return;
        }

        // حساب اتجاه الهدف بالنسبة للعدو
        Vector3 direction = targetPos - transform.position;

        // 1. تحديد الانعكاس (Flip) يمين أو يسار
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;  // يسار (نعمل فلب)
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // يمين (بدون فلب)
        }

        // 2. تحديد نوع الأنيميشن بناءً على القوانين التي ذكرتها
        if (direction.y > 0)
        {
            // أعلى، أو أعلى يمين، أو أعلى يسار
            animator.SetBool("isATKUP", true);
            animator.SetBool("isATKDOWN", false);
        }
        else if (direction.y < 0)
        {
            // أسفل، أو أسفل يمين، أو أسفل يسار
            animator.SetBool("isATKUP", false);
            animator.SetBool("isATKDOWN", true);
        }
        else
        {
            // في حالة التوازي التام (يمين أو يسار بالضبط)
            if (direction.x >= 0)
            {
                animator.SetBool("isATKUP", true);
                animator.SetBool("isATKDOWN", false);
            }
            else
            {
                animator.SetBool("isATKUP", false);
                animator.SetBool("isATKDOWN", true);
            }
        }
    }
}
