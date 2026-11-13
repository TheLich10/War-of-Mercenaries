using Pathfinding;
using System.Collections;
using UnityEngine;

public class ATTACKERS : MonoBehaviour, IDamageable
{
    private Transform targetEnemy;
    private GameObject targetEnemyObject;

    public float health = 100f;
    private float maxHealth = 0f;
    public float speed = 2f;
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float attackRate = 1f;
    private float attackTimer = 0f;

    public GameObject healthBarPrefab;
    private HealthBarFollow healthBarUI;

    // بيانات إضافية
    public int level = 1;
    public string warriorName;

    private bool isMoving = false;
    private Vector3 spawnPoint; // نقطة البداية (المبنى)

    // 🔹 مرجع للثكنة
    private BarracksWatchPost barracks;

    public GameObject dieAnime;
    [Header("Pools")]
    private UnitPool effectPool; // الـ Pool الخاص بتأثيرات الموت (dieAnime)

    [Header("Ranged Attack Settings")]
    public GameObject arrowPrefab; // ضع هنا مجسم السهم
    public Transform arrowSpawnPoint; // النقطة التي يخرج منها السهم (اختياري)

    private SpriteRenderer spriteRenderer; // لتجنب استدعاء GetComponent في الألوان
    private Animator animator;


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
        InitializeWarriorStats();
        maxHealth = health;
        spawnPoint = transform.position; // حفظ مكان البداية

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

    public void SetBarracks(BarracksWatchPost b) // يتم استدعاؤها عند الإنشاء من DefensiveWatchPost
    {
        barracks = b;
    }

    void Update()
    {
        if (targetEnemy != null)
        {
            MoveTowardsEnemy();
            AttackEnemy();
        }
        else
        {
            FindTargetEnemy();

            // لو ما لقى هدف يرجع لنقطة البداية
            if (targetEnemy == null)
            {
                ReturnToSpawn();
            }
        }
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
                attackDamage = warriorLevel.damage;
            }
        }
    }

    void FindTargetEnemy()
    {
        string[] enemyTags;

        // 🔹 التحقق من اسم المحارب لتحديد الأهداف المسموحة
        if (warriorName == "Destroyer" || warriorName == "ScalingEgyptian")
        {
            // هؤلاء المحاربين يمكنهم استهداف الأعداء الطائرين والأرضيين
            enemyTags = new string[] { "Enemy", "monsterEnemy", "monsterFly", "EnemyFly" };
        }
        else
        {
            // باقي المحاربين يستهدفون الأعداء الأرضيين فقط
            enemyTags = new string[] { "Enemy", "monsterEnemy" };
        }

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (string tag in enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
        }

        if (nearestEnemy != null)
        {
            targetEnemyObject = nearestEnemy;
            targetEnemy = nearestEnemy.transform;
        }
        else
        {
            targetEnemy = null;
            targetEnemyObject = null;
        }
    }

    void MoveTowardsEnemy()
    {
        if (targetEnemy == null) return;

        float distance = Vector3.Distance(transform.position, targetEnemy.position);
        if (distance > attackRange)
        {
            HandleAttackAnimation(targetEnemy.position, false);
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, targetEnemy.position, speed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
        }
    }

    void AttackEnemy()
    {
        if (targetEnemy == null) return;

        float distance = Vector3.Distance(transform.position, targetEnemy.position);
        if (distance <= attackRange)
        {
            HandleAttackAnimation(targetEnemy.position, true);
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                if (targetEnemyObject != null)
                {
                    // التحقق إذا كان المحارب رامي سهام
                    if (warriorName == "Destroyer" || warriorName == "ScalingEgyptian")
                    {
                        ShootArrow(targetEnemy);  // استدعاء دالة رمي السهم
                    }
                    else
                    {
                        // 1. محاولة الحصول على سكريبت Enemy العادي
                        Enemy enemyScript = targetEnemyObject.GetComponent<Enemy>();
                        if (enemyScript != null)
                        {
                            enemyScript.TakeDamage(attackDamage, transform);

                            if (enemyScript.health <= 0)
                            {
                                targetEnemy = null;
                                targetEnemyObject = null;
                            }
                        }
                        else
                        {
                            // 2. إذا لم يكن Enemy عادي، نحاول الحصول على EnemyRNG_Advanced
                            EnemyRNG_Advanced advancedEnemyScript = targetEnemyObject.GetComponent<EnemyRNG_Advanced>();
                            if (advancedEnemyScript != null)
                            {
                                advancedEnemyScript.TakeDamage(attackDamage, transform);

                                // نفترض أن سكريبت EnemyRNG_Advanced يحتوي أيضاً على متغير health
                                if (advancedEnemyScript.health <= 0)
                                {
                                    targetEnemy = null;
                                    targetEnemyObject = null;
                                }
                            }
                            else
                            {
                                // 2. إذا لم يكن Enemy عادي، نحاول الحصول على EnemyRNG_Advanced
                                y3soobEnemy y3soob = targetEnemyObject.GetComponent<y3soobEnemy>();
                                if (y3soob != null)
                                {
                                    y3soob.TakeDamage(attackDamage, transform);

                                    // نفترض أن سكريبت EnemyRNG_Advanced يحتوي أيضاً على متغير health
                                    if (y3soob.health <= 0)
                                    {
                                        targetEnemy = null;
                                        targetEnemyObject = null;
                                    }
                                }
                                else
                                {
                                    flayingEnemy flyin = targetEnemyObject.GetComponent<flayingEnemy>();
                                    if (flyin != null)
                                    {
                                        flyin.TakeDamage(attackDamage, transform);

                                        // نفترض أن سكريبت EnemyRNG_Advanced يحتوي أيضاً على متغير health
                                        if (flyin.health <= 0)
                                        {
                                            targetEnemy = null;
                                            targetEnemyObject = null;
                                        }
                                    }
                                    else
                                    {
                                        HealerWarrior herlar = targetEnemyObject.GetComponent<HealerWarrior>();
                                        if (herlar != null)
                                        {
                                            herlar.TakeDamage(attackDamage, transform);

                                            // نفترض أن سكريبت EnemyRNG_Advanced يحتوي أيضاً على متغير health
                                            if (herlar.health <= 0)
                                            {
                                                targetEnemy = null;
                                                targetEnemyObject = null;
                                            }
                                        }
                                        else
                                        {
                                            sapperEnemy spper = targetEnemyObject.GetComponent<sapperEnemy>();
                                            if(spper != null)
                                            {
                                                spper.TakeDamage(attackDamage, transform);
                                                if (spper.health <= 0)
                                                {
                                                    targetEnemy = null;
                                                    targetEnemyObject = null;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }

                }
                attackTimer = 0f;
                if (warriorName.Equals("Kamikaze", System.StringComparison.OrdinalIgnoreCase))
                {
                    Die();
                    return;
                }
            }
        }
    }
    void ShootArrow(Transform target)
    {
        Vector3 spawnPos = arrowSpawnPoint != null ? arrowSpawnPoint.position : transform.position;

        Vector3 dir = (target.position - spawnPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(arrowPrefab, spawnPos, Quaternion.Euler(0, 0, angle));

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetTarget(target);
            projectileScript.damage = attackDamage;

            // 🔹 السطر الجديد: نمرر هوية الرامي (المحارب) إلى السهم
            projectileScript.shooterTransform = this.transform;
        }
    }
    void ReturnToSpawn()
    {
        HandleAttackAnimation(Vector3.zero, false);
        float distance = Vector3.Distance(transform.position, spawnPoint);
        if (distance > 0.1f)
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, spawnPoint, speed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
            gameObject.SetActive(false);
            Destroy(gameObject);

            if (barracks != null)
            {
                barracks.AddWarrior(warriorName);
            }

            else
            {
                Debug.LogError("❌ لا يوجد Barracks محدد لهذا الجندي");
            }
        }

    }

    // ✅ تم تعديل البراميتر الثاني ليتطابق مع الـ Interface
    public void TakeDamage(float damage, Transform attacker = null)
    {
        if (isMoving) return; // لا يتلقى ضرر وهو يتحرك


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
        // 🔹 إذا مات المحارب → ينحذف من الثكنة فعلياً
        if (barracks != null)
        {
            barracks.RemoveWarrior(warriorName);
        }
        if (effectPool != null)
        {
            GameObject effect = effectPool.Spawn(transform.position);
            if (effect != null) effect.transform.rotation = transform.rotation;
        }
        Destroy(gameObject);
    }

    private void HandleAttackAnimation(Vector3 targetPos, bool isAttacking)
    {
        if (animator == null || spriteRenderer == null) return;



        // +++ أضف هذين السطرين هنا للربط بين السكربتين وإيقاف التذبذب +++
        PlayerAnimationOnly playerAnim = GetComponent<PlayerAnimationOnly>();
        if (playerAnim != null) playerAnim.isAttacking = isAttacking;
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // حساب اتجاه الهدف بالنسبة للعدو
        Vector3 direction = targetPos - transform.position;

        // 1. تحديد الانعكاس (Flip) يمين أو يسار (هذا الكود نريده أن يعمل على جميع الوحوش)
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;  // يسار (نعمل فلب)
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // يمين (بدون فلب)
        }


        // 2. تحديد نوع الأنيميشن (للوحوش العادية فقط)

        // إيقاف الأنيميشن إذا لم يكن في حالة هجوم
        if (!isAttacking)
        {
            animator.SetBool("isATKUP", false);
            animator.SetBool("isATKDOWN", false);
            return;
        }

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
