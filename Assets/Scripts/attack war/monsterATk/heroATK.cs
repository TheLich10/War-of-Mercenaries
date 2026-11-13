using Unity.VisualScripting;
using UnityEngine;

public class heroATK : MonoBehaviour, IDamageable
{
    private bool isAttacking = false;
    private MonsterAnimationBlendTree blendTreeAnim; // 👈 المتغير الجديد
    private Animator animator;
    public float moveSpeed = 2f;
    public float stopDistance = 1f;
    public float attackRate = 1f;
    public float attackDamage = 20f;
    public float health = 150f;

    // 💡 تم إزالة static لأن كل بطل يجب أن يكون له حالته المستقلة
    // إذا كان هناك سكربت آخر يعتمد على static هنا أخبرني، ولكن منطقياً يجب ألا تكون static
    public static bool isActiveHero = true;

    private bool hasBeenShown = false; // ✅ متغير جديد لمنع التقطيع (Lag)

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
    public int manualLevel = 1;
    public string monsterName = "BearWolf";

    public GameObject dieAnime;
    [Header("Pools")]
    private UnitPool effectPool;

    void Awake()
    {
        GameObject poolObj = GameObject.Find("EffectPoolManager");
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
        // 💡 لا تجعل البطل false هنا إلا إذا كنت متأكداً أن هناك سكربت آخر سيقوم بتفعيله بعد التحميل
        // إذا كان البطل يظهر في المشهد من ملف حفظ (Load)، يجب أن يكون true لكي يهجم فوراً
        animator = GetComponent<Animator>(); // 👈 إضافة هذا السطر
        blendTreeAnim = GetComponent<MonsterAnimationBlendTree>(); // 👈 إضافة هذا السطر
        originalPosition = transform.position;
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

    private void InitializeWarriorStats()
    {
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
        }
    }

    // 💡 أضف هذا المتغير في الأعلى مع باقي المتغيرات
    [Header("Animation Settings")]
    public float attackAnimationDuration = 0.6f; // 👈 اجعل هذا الرقم مطابقاً لطول أنيميشن الهجوم في الـ Animator

    void Update()
    {
        if (isActiveHero && !hasBeenShown)
        {
            ShowObject();
            hasBeenShown = true;
        }

        if (!isActiveHero) return;

        // 🛑 إذا كان البطل في منتصف ضربة الهجوم، نوقف أي كود آخر لكي يكتمل الأنيميشن ولا يقطعه الـ Idle أو المشي
        if (isAttacking) return;

        if (targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.position);

            if (distance > stopDistance)
            {
                // البطل يتحرك -> نتأكد أن الهجوم متوقف
                if (blendTreeAnim != null) blendTreeAnim.isAttacking = false;
                if (animator != null) animator.SetBool("Attack", false);

                transform.position = Vector3.MoveTowards(transform.position, targetEnemy.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                // البطل وصل للهدف وتوقف
                attackTimer += Time.deltaTime;

                if (attackTimer >= attackRate)
                {
                    StartAttack(); // 👈 استدعاء دالة بدء الهجوم
                }
            }
        }
        else if (returningToOrigin)
        {
            float distance = Vector3.Distance(transform.position, originalPosition);

            if (distance > 0.1f)
            {
                // البطل يعود
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

    // ✅ دالة جديدة لتنظيم عملية الهجوم
    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = 0f;

        if (blendTreeAnim != null) blendTreeAnim.isAttacking = true;
        if (animator != null) animator.SetBool("Attack", true);

        Attack(); // تنفيذ خصم الدم

        // ننتظر حتى ينتهي الأنيميشن (حسب الوقت المحدد) ثم نعيده للحالة الطبيعية
        Invoke(nameof(ResetAttackAnimation), attackAnimationDuration);
    }

    private void ResetAttackAnimation()
    {
        isAttacking = false;

        if (blendTreeAnim != null) blendTreeAnim.isAttacking = false;
        if (animator != null) animator.SetBool("Attack", false);
    }
    public void SetTarget(Transform target)
    {
        // ✅ إذا وصل الهدف إلى هنا والـ Controller أرسله، اقبله فوراً (تم إلغاء شرط الرفض)
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
            Die();
        }
    }

    void Die()
    {
        if (healthBarUI != null) Destroy(healthBarUI.gameObject);

        if (effectPool != null)
        {
            GameObject effect = effectPool.Spawn(transform.position);
            if (effect != null) effect.transform.rotation = transform.rotation;
        }
        Destroy(gameObject);
    }

    void ShowObject()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends) rend.enabled = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        // ⚠️ ملاحظة: تفعيل جميع السكربتات بهذه الطريقة قد يسبب مشاكل متداخلة، يفضل تحديد السكربت المطلوب تفعيله
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) script.enabled = true;
    }
}