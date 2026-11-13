using UnityEngine;

public class heroArcher2ATK : MonoBehaviour, IDamageable
{
    private bool isAttacking = false;
    private MonsterAnimationBlendTree blendTreeAnim;
    private Animator animator;
    public float moveSpeed = 2f;
    public float stopDistance = 1f;
    public float attackRate = 1f;
    public float attackDamage = 20f;
    public float health = 150f;

    public GameObject projectilePrefab;
    public Transform firePoint;

    public static bool isActiveHero = true;
    private bool hasBeenShown = false;
    private Transform targetEnemy;
    private float attackTimer = 0f;

    private Vector3 originalPosition;
    private bool returningToOrigin = false;

    public float detectionRange = 5f;

    // ✅ إضافات شريط الصحة
    public GameObject healthBarPrefab;
    private HealthBarFollow healthBarUI;
    private float maxHealth;


    [Header("Manual Level Control")]
    public int manualLevel = 1; // ⬅️ انت تتحكم به
    public string monsterName = "DraginFly";

    public GameObject dieAnime;
    [Header("Pools")]
    private UnitPool effectPool; // الـ Pool الخاص بتأثيرات الموت (dieAnime)

    // 🌟 المتغير الجديد للتحكم بوقت الأنيميشن
    [Header("Animation Settings")]
    public float attackAnimationDuration = 0.8f; // 👈 وقت الأنيميشن، عدله حسب الحاجة

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
        isActiveHero = true;
        animator = GetComponent<Animator>();
        blendTreeAnim = GetComponent<MonsterAnimationBlendTree>();
        originalPosition = transform.position;

        InitializeWarriorStats();

        maxHealth = health;
        // ✅ إنشاء شريط الصحة
        if (healthBarPrefab != null)
        {
            GameObject barGO = Instantiate(healthBarPrefab, GameObject.Find("Canvas").transform);
            healthBarUI = barGO.GetComponent<HealthBarFollow>();
            healthBarUI.target = this.transform;
            healthBarUI.Initialize(maxHealth);
            healthBarUI.UpdateHealth(health);
            barGO.SetActive(false); // يبدأ مخفيًا ✅
        }
    }

    private void InitializeWarriorStats()
    {
        // ⬅️ نستخدم المستوى اليدوي بدل النظام التلقائي
        int currentLevel = manualLevel;

        MonsterData monster = MonsterDatabase.GetMonsterByName(monsterName);
        if (monster != null)
        {
            MonsterLevel monsterLevel = monster.GetLevel(currentLevel);
            if (monsterLevel != null)
            {
                health = monsterLevel.health;
                moveSpeed = monsterLevel.speed;
                attackDamage = monsterLevel.damage;
            }
            else
            {
                Debug.LogWarning($"Level {currentLevel} not found for {monsterName}");
            }
        }
        else
        {
            Debug.LogWarning($"Monster {monsterName} not found in database");
        }
    }

    void Update()
    {
        // ✅ إصلاح مشكلة الأداء: تشغيل دالة الإظهار مرة واحدة فقط
        if (isActiveHero && !hasBeenShown)
        {
            ShowObject();
            hasBeenShown = true;
        }

        // إيقاف التحديث إذا كان البطل غير مفعل
        if (!isActiveHero) return;

        // 🛑 الدرع الواقي: نوقف أي حركة أو أكواد أخرى أثناء رمي السهم لكي يكتمل الأنيميشن بسلاسة
        if (isAttacking) return;

        if (targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.position);

            if (distance > stopDistance)
            {
                // 👈 إيقاف أنيميشن الهجوم لأن رامي السهام يتحرك نحو العدو
                if (blendTreeAnim != null) blendTreeAnim.isAttacking = false;
                if (animator != null) animator.SetBool("Attack", false);

                transform.position = Vector3.MoveTowards(transform.position, targetEnemy.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackRate)
                {
                    StartAttack(); // 👈 استدعاء الدالة الجديدة لتنظيم الهجوم
                }
            }
        }
        else if (returningToOrigin)
        {
            float distance = Vector3.Distance(transform.position, originalPosition);

            if (distance > 0.1f)
            {
                // 👈 إيقاف أنيميشن الهجوم أثناء العودة
                if (blendTreeAnim != null) blendTreeAnim.isAttacking = false;
                if (animator != null) animator.SetBool("Attack", false);

                transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                returningToOrigin = false;
                transform.position = originalPosition;
            }
        }
    }

    // 🌟 دالة جديدة لتنظيم عملية الهجوم وحماية الأنيميشن
    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = 0f;

        if (blendTreeAnim != null) blendTreeAnim.isAttacking = true;
        if (animator != null) animator.SetBool("Attack", true);

        Attack(); // إطلاق السهم

        // ننتظر حتى ينتهي الأنيميشن (حسب الوقت المحدد) ثم نعيده للحالة الطبيعية
        Invoke(nameof(ResetAttackAnimation), attackAnimationDuration);
    }

    // 👈 الدالة التي يتم استدعاؤها لإعادة الأنيميشن إلى وضع الإيقاف
    private void ResetAttackAnimation()
    {
        isAttacking = false;

        if (blendTreeAnim != null) blendTreeAnim.isAttacking = false;
        if (animator != null) animator.SetBool("Attack", false);
    }

    public void SetTarget(Transform target)
    {
        // ✅ تم إزالة شرط المنع لضمان استلام الهدف حتى لو كان البطل غير مفعل
        targetEnemy = target;
        returningToOrigin = false;
    }

    public void ClearTarget()
    {
        // ✅ تم إزالة شرط المنع هنا أيضاً
        targetEnemy = null;
        returningToOrigin = true;
    }

    void Attack()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            ShootArrow(targetEnemy);
        }
    }

    void ShootArrow(Transform target)
    {
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        Vector3 dir = (target.position - spawnPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.Euler(0, 0, angle));

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetTarget(target);
            projectileScript.damage = attackDamage;

            // 🔹 نمرر هوية الرامي (المحارب) إلى السهم
            if (target.GetComponent<Enemy>() == null)
            {
                // نمرر هوية الرامي (المحارب) إلى السهم فقط إذا لم يكن الهدف Enemy
                projectileScript.shooterTransform = this.transform;
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
        Debug.Log("Hero Died");

        // تدمير شريط الصحة أولاً إن وجد
        if (healthBarUI != null)
            Destroy(healthBarUI.gameObject);

        if (effectPool != null)
        {
            GameObject effect = effectPool.Spawn(transform.position);
            if (effect != null) effect.transform.rotation = transform.rotation;
        }

        // تدمير البطل نهائياً من الذاكرة
        Destroy(gameObject);
    }

    void ShowObject()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
            rend.enabled = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
    }
}