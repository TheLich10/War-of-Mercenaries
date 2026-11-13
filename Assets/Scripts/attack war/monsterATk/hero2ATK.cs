using Unity.VisualScripting;
using UnityEngine;

public class hero2ATK : MonoBehaviour, IDamageable
{
    private bool isAttacking = false;
    private MonsterAnimationBlendTree blendTreeAnim;

    private Animator animator;
    public float moveSpeed = 2f;
    public float stopDistance = 1f;
    public float attackRate = 1f;
    public float attackDamage = 20f;
    public float health = 150f;

    // ✅ تم إزالة كلمة static لتجنب مشاكل الانتقال بين المشاهد
    public static bool isActiveHero = true;

    // ✅ متغير لمنع تشغيل دالة الإظهار باستمرار في الـ Update
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
    public string monsterName = "Mightosour";

    public GameObject dieAnime;
    [Header("Pools")]
    private UnitPool effectPool; // الـ Pool الخاص بتأثيرات الموت (dieAnime)

    // 🌟 المتغير الجديد للتحكم بوقت الأنيميشن
    [Header("Animation Settings")]
    public float attackAnimationDuration = 0.8f; // 👈 وقت 0.8 ممتاز لرامي السهام ليأخذ وقته في سحب القوس

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

        // 🛑 الدرع الواقي: إذا كان البطل في منتصف ضربة الهجوم، نوقف أي كود آخر لكي يكتمل الأنيميشن بسلاسة
        if (isAttacking) return;

        if (targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.position);

            if (distance > stopDistance)
            {
                // 👈 البطل يتحرك: نلغي الهجوم ونخبر الـ BlendTree لكي يكمل المشي
                if (blendTreeAnim != null) blendTreeAnim.isAttacking = false;
                if (animator != null) animator.SetBool("Attack", false);

                transform.position = Vector3.MoveTowards(transform.position, targetEnemy.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackRate)
                {
                    StartAttack(); // 👈 استدعاء الدالة الجديدة المنظمة للهجوم
                }
            }
        }
        else if (returningToOrigin)
        {
            float distance = Vector3.Distance(transform.position, originalPosition);

            if (distance > 0.1f)
            {
                // 👈 البطل يعود: نلغي الهجوم ونخبر الـ BlendTree ليعود للمشي
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

        Attack();

        // ننتظر حتى ينتهي الأنيميشن (حسب الوقت المحدد 0.8) ثم نعيده للحالة الطبيعية
        Invoke(nameof(ResetAttackAnimation), attackAnimationDuration);
    }

    // 👈 هذه الدالة سيقوم الـ Invoke بتشغيلها لإعادة الأنيميشن إلى false بعد رمي السهم
    private void ResetAttackAnimation()
    {
        isAttacking = false;

        if (blendTreeAnim != null) blendTreeAnim.isAttacking = false;
        if (animator != null) animator.SetBool("Attack", false);
    }

    public void SetTarget(Transform target)
    {
        // ✅ تم إزالة شرط المنع لضمان استلام الهدف دائماً
        targetEnemy = target;
        returningToOrigin = false;
    }

    public void ClearTarget()
    {
        targetEnemy = null;
        returningToOrigin = true;
    }

    void Attack()
    {
        if (targetEnemy != null)
        {
            Enemy enemy = targetEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage, transform);
                Debug.Log("Hero Attacked Enemy");
            }
            else
            {
                EnemyRNG_Advanced enemyRang = targetEnemy.GetComponent<EnemyRNG_Advanced>();
                if (enemyRang != null)
                {
                    enemyRang.TakeDamage(attackDamage, transform);
                }
                else
                {
                    sapperEnemy enemySapper = targetEnemy.GetComponent<sapperEnemy>();
                    if (enemySapper != null)
                    {
                        enemySapper.TakeDamage(attackDamage, transform);
                    }
                }
            }
        }
    }

    public void TakeDamage(float damage, Transform attacker = null)
    {
        health -= damage;

        showNumberPop showNum = GetComponent<showNumberPop>();
        if (showNum != null) showNum.OnHit(damage);

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