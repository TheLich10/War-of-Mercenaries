using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AI; // تأكد من أنك تستخدم NavMesh 2D

public class flayingEnemy : MonoBehaviour, IBuffable
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
        WarriorTring warrior = WarriorTringDatabase.GetWarriorByName(warriorName);
        if (warrior != null)
        {
            WarriorLevel warriorLevel = warrior.GetLevel(level);
            if (warriorLevel != null)
            {
                health = warriorLevel.health;
                speed = warriorLevel.speed;
                attackDamage = warriorLevel.damage;
            }
        }
    }

    void Update()
    {
        if (targetHero != null)
        {
            MoveTowardsHero();
            AttackHero();
        }
        else
        {
            FindTargetBuilding();
            if (targetBuilding != null)
            {
                MoveTowardsBuilding();
                AttackBuilding();
            }
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
            // 🔹 تشغيل أنيميشن الهجوم وتوجيهه نحو البطل
            HandleAttackAnimation(targetHero.position, true);

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
            HandleAttackAnimation(Vector3.zero, false);
        }
    }

    void FindTargetBuilding()
    {
        string[] defensiveTags = { "spyClan", "spy", "boomOP", "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "trapfire", "trapN", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "mongel" };

        string[] otherTags = { "spyClan", "spy","boomOP", "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "trapfire", "trapN", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "mongel" };

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
                HandleAttackAnimation(closestPoint, true);

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
                HandleAttackAnimation(Vector3.zero, false);
            }
        }
        else
        {
            HandleAttackAnimation(Vector3.zero, false);
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
        int randomNum = UnityEngine.Random.Range(0, 2);

        if (randomNum == 0)
        {
            AudioManager.Instance.PlaySFX("die1");
        }
        else
        {
            AudioManager.Instance.PlaySFX("die2");
        }

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

        rangePrevDamage = attackDamage;

        // تطبيق الصقل
        health *= healthMultiplier;

        speed *= speedMultiplier;

        attackDamage *= damageMultiplier;
        blueColor();
        Debug.Log("Range boost applied");
    }

    public void RemoveRangeBoost()
    {
        if (!isRangeBoostActive) return;

        isRangeBoostActive = false;

        health = rangePrevHealth;

            speed = rangePrevSpeed;

        attackDamage = rangePrevDamage;

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
            spriteRenderer.flipX = true;  // يسار (نعمل فلب)
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
