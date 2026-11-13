using System.Collections;
using TMPro;
using UnityEngine;
using Pathfinding;

public class EnemyRNG_Advanced : MonoBehaviour, IBuffable
{
    [Header("Animation")]
    private Animator animator;

    private Transform targetHero;
    private GameObject targetHeroObject;

    [Header("Stats")]
    public float health = 100f;
    public float maxHealth = 0f;
    public float attackDamage = 10f;
    public float Speed = 10f;

    public float attackRange = 1.5f;
    public float attackRate = 1f;
    private float attackTimer = 0f;

    [Header("Targeting")]
    private Transform targetBuilding;
    private Collider2D targetBuildingCollider;

    [Header("UI")]
    public GameObject healthBarPrefab;
    private HealthBarFollow healthBarUI;

    [Header("Warrior Info")]
    public int level = 1;
    public string warriorName;

    private float originalHealth;
    private float originalDamage;
    private float originalSpeed;

    private Coroutine tempBoostCoroutine;
    private Coroutine conditionalBoostCoroutine;

    [Header("Movement")]
    public AIPath aiPath;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab; // قذيفة يتم إطلاقها
    public Transform firePoint;         // نقطة إطلاق القذيفة

    // 🔥 1. تعريف المكونات لتخزينها مرة واحدة
    private AIDestinationSetter destSetter;
    private SpriteRenderer spriteRenderer;
    private showNumberPop myShowNumPop;

    private string[] defensiveTags = { "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "boomOP" };


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
        animator = GetComponent<Animator>();

        // 🔥 2. تخزين المكونات الثقيلة هنا لتخفيف الضغط عن الـ Update
        destSetter = GetComponent<AIDestinationSetter>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myShowNumPop = GetComponent<showNumberPop>();

        ParseLevelFromText();
        InitializeWarriorStats();

        originalHealth = health;
        originalDamage = attackDamage;
        originalSpeed = Speed;
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
        // 🔥 1. تحديث الهدف بشكل دائم ومتزامن مع الذكاء الاصطناعي
        if (destSetter != null && destSetter.target != null)
        {
            if (targetBuilding != destSetter.target)
            {
                targetBuilding = destSetter.target;

                // نبحث عن الكوليدر حتى لو كان داخل الأبناء (Children)
                targetBuildingCollider = targetBuilding.GetComponentInChildren<Collider2D>();
            }
        }

        // 🔥 2. تنظيم الاستدعاء
        if (targetHero != null)
        {
            AttackHero();
        }
        else
        {
            AttackBuilding();
        }
    }

    private void ParseLevelFromText()
    {
        level = WarriorLevelManager.Instance.GetLevel(warriorName);
    }

    private void InitializeWarriorStats()
    {
        string[] monsterNames = { "Wom-kong", "DraginFly", "BearWolf", "Mightosour" };

        bool isMonster = false;
        string matchedMonsterName = null;

        foreach (string mName in monsterNames)
        {
            if (warriorName.StartsWith(mName, System.StringComparison.OrdinalIgnoreCase))
            {
                isMonster = true;
                matchedMonsterName = mName;
                break;
            }
        }

        if (isMonster)
        {
            int currentLevel = MonsterLevelManager.Instance.GetLevel(matchedMonsterName);
            MonsterData monster = MonsterDatabase.GetMonsterByName(matchedMonsterName);
            if (monster != null)
            {
                MonsterLevel monsterLevel = monster.GetLevel(currentLevel);
                if (monsterLevel != null)
                {
                    health = monsterLevel.health;
                    if (aiPath != null) aiPath.maxSpeed = monsterLevel.speed;
                    attackDamage = monsterLevel.damage;
                }
            }
        }
        else
        {
            WarriorTring warrior = WarriorTringDatabase.GetWarriorByName(warriorName);
            if (warrior != null)
            {
                WarriorLevel warriorLevel = warrior.GetLevel(level);
                if (warriorLevel != null)
                {
                    health = warriorLevel.health;
                    attackDamage = warriorLevel.damage;
                    if (aiPath != null) aiPath.maxSpeed = warriorLevel.speed;
                }
            }
        }
        // +++ التعديل الجديد يبدأ هنا +++
        if (warriorName == "Wom-kong")
        {
            switch (MonsterLevelManager.Instance.GetLevel("Wom-kong"))
            {
                case 1: attackRange = 1.5f; break;
                case 2: attackRange = 2.5f; break;
                case 3: attackRange = 3.5f; break;
                case 4: attackRange = 4.5f; break;
                case 5: attackRange = 5.0f; break;
                case 6: attackRange = 5.5f; break;
                default:
                    // في حال كان المستوى أعلى من 6، سيأخذ القيمة الأخيرة أو يمكنك برمجتها لتزيد
                    if (level > 6) attackRange = 5.5f;
                    break;
            }
            if (aiPath != null)
            {
                aiPath.slowdownDistance = attackRange;
                aiPath.endReachedDistance = attackRange;
            }
        }
        // +++ التعديل الجديد ينتهي هنا +++
    }

    void AttackHero()
    {
        if (targetHero == null || targetHeroObject == null)
        {
            if (aiPath != null) aiPath.isStopped = false; // أكمل طريقك إذا مات البطل
            // === الإضافة الجديدة هنا لإيقاف الهجوم ===
            HandleAttackAnimation(Vector3.zero, false);
            if (warriorName == "Wom-kong")
            {
                animator.SetBool("Attack", false);
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, targetHero.position);

        // 🔥 شرط المسافة الذكي
        bool isCloseEnough = distance <= attackRange || (aiPath != null && aiPath.reachedDestination && distance <= attackRange + 1.5f);

        if (isCloseEnough)
        {
            if (aiPath != null) aiPath.isStopped = true; // أوقف الحركة تماماً أثناء الرمي
            HandleAttackAnimation(targetHero.position, true);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                // +++ التعديل الجديد يبدأ هنا +++
                if (warriorName == "Wom-kong")
                {
                    animator.SetBool("Attack", true);
                }
                // +++ التعديل الجديد ينتهي هنا +++

                IDamageable damageableTarget = targetHeroObject.GetComponent<IDamageable>();

                if (damageableTarget != null)
                {
                    Shoot(this.transform);
                    damageableTarget.TakeDamage(attackDamage, this.transform);
                }

                attackTimer = 0f;
                PlayAttackSound();

            }
        }
        else
        {
            if (aiPath != null) aiPath.isStopped = false; // اسمح له بالحركة للاقتراب
            HandleAttackAnimation(Vector3.zero, false);
            if (warriorName == "Wom-kong")
            {
                animator.SetBool("Attack", false);
            }
        }
    }

    void AttackBuilding()
    {
        // ❌ أزلنا شرط (targetBuildingCollider == null) لكي لا يتعطل الهجوم
        if (targetBuilding == null)
        {
            if (aiPath != null) aiPath.isStopped = false;
            // === الإضافة الجديدة هنا لإيقاف الهجوم ===
            HandleAttackAnimation(Vector3.zero, false);
            if (warriorName == "Wom-kong")
            {
                animator.SetBool("Attack", false);
            }
            return;
        }

        // 🔥 حساب المسافة
        float distance;
        if (targetBuildingCollider != null)
        {
            distance = Vector2.Distance(transform.position, targetBuildingCollider.ClosestPoint(transform.position));
        }
        else
        {
            distance = Vector2.Distance(transform.position, targetBuilding.position);
        }

        // 🔥 شرط المسافة الذكي الخاص بالرماة
        bool isCloseEnough = distance <= attackRange || (aiPath != null && aiPath.reachedDestination && distance <= attackRange + 1.5f);

        if (isCloseEnough)
        {
            if (aiPath != null) aiPath.isStopped = true; // أوقف الحركة تماماً أثناء الرماية

            // 🔹 تحديد موقع الهدف بدقة لتشغيل الأنيميشن في الاتجاه الصحيح
            Vector3 exactTargetPos = targetBuildingCollider != null ? (Vector3)targetBuildingCollider.ClosestPoint(transform.position) : targetBuilding.position;
            HandleAttackAnimation(exactTargetPos, true);


            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                // +++ التعديل الجديد يبدأ هنا +++
                if (warriorName == "Wom-kong")
                {
                    animator.SetBool("Attack", true);
                }
                // +++ التعديل الجديد ينتهي هنا +++

                if (projectilePrefab != null && firePoint != null)
                {
                    if (warriorName == "Wom-kong")
                    {
                        ShootWOM(targetBuilding);
                    }
                    else Shoot(targetBuilding);
                }

                showNumberPop targetShowNum = targetBuilding.GetComponent<showNumberPop>();

                if (targetBuilding.CompareTag("spy"))
                {
                    DefensiveWatchPost watchPost = targetBuilding.GetComponent<DefensiveWatchPost>();
                    if (watchPost != null) watchPost.TakeDamage(attackDamage);
                    if (targetShowNum != null) targetShowNum.OnHit(attackDamage);
                }
                else if (System.Array.Exists(defensiveTags, tag => targetBuilding.CompareTag(tag)))
                {
                    DefensiveBuilding defBuilding = targetBuilding.GetComponent<DefensiveBuilding>();
                    if (defBuilding != null) defBuilding.TakeDamage(attackDamage);
                    if (targetShowNum != null) targetShowNum.OnHit(attackDamage);
                }
                else
                {
                    building b = targetBuilding.GetComponent<building>();
                    if (b != null) b.TakeDamage(attackDamage);

                    TownHall hall = targetBuilding.GetComponent<TownHall>();
                    if (hall != null) hall.OnHit(attackDamage);

                    if (targetShowNum != null) targetShowNum.OnHit(attackDamage);
                }

                attackTimer = 0f;
                PlayAttackSound();
            }
        }
        else
        {
            if (aiPath != null) aiPath.isStopped = false; // استمر بالحركة للوصول لنطاق الرمي

            HandleAttackAnimation(Vector3.zero, false);
            if (warriorName == "Wom-kong")
            {
                animator.SetBool("Attack", false);
            }
        }
    }

    void Shoot(Transform target)
    {
        Vector2 dir = (target.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.Euler(0, 0, angle));

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetTarget(target);
        }
    }

    void ShootWOM(Transform target)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        ProjectWOM projectileScript = projectile.GetComponent<ProjectWOM>();
        if (projectileScript != null)
        {
            // هنا قمنا بإرسال المتغير attackDamage كمعطى ثاني للدالة
            projectileScript.SetTarget(target, attackDamage);
        }
    }

    public void TakeDamage(float damage, Transform attacker = null)
    {
        health -= damage;

        // استخدام المكون المخزن مسبقاً
        if (myShowNumPop != null) myShowNumPop.OnHit(damage);

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealth(health);
            healthBarUI.gameObject.SetActive(health < maxHealth && health > 0);
        }

        // 🔥 تعيين المهاجم كهدف جديد وتوجيه مسار الـ AI نحوه
        if (attacker != null)
        {
            targetHero = attacker;
            targetHeroObject = attacker.gameObject;

            // إلغاء استهداف المبنى
            targetBuilding = null;
            targetBuildingCollider = null;

            // توجيه الذكاء الاصطناعي لملاحقة البطل الذي هاجمه
            if (destSetter != null)
                destSetter.target = targetHero;
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

        if (warriorName == "Wom-kong")
        {
            AudioManager.Instance.PlaySFX("dieMonster");

        }
        else
        {
            int randomNum = UnityEngine.Random.Range(0, 2);

            if (randomNum == 0)
            {
                AudioManager.Instance.PlaySFX("die1");
            }
            else
            {
                AudioManager.Instance.PlaySFX("die2");
            }
        }
        Destroy(gameObject);
    }

    private void PlayAttackSound()
    {
        // التحقق من اسم المحارب لتشغيل الصوت المناسب
        if (warriorName.Equals("Wom-kong", System.StringComparison.OrdinalIgnoreCase))
        {
            AudioManager.Instance.PlaySFX("flyATK"); // استبدل بـ اسم ملف الصوت الخاص بك
        }
        else
        {
            // الصوت الافتراضي لباقي المحاربين
            AudioManager.Instance.PlaySFX("DES_SCAL_ATK");
        }
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
        float prevSpeed = aiPath != null ? aiPath.maxSpeed : Speed;
        float prevDamage = attackDamage;

        health *= healthMultiplier;
        if (aiPath != null)
            aiPath.maxSpeed *= speedMultiplier;
        attackDamage *= damageMultiplier;

        yield return new WaitForSeconds(duration);

        health = prevHealth;
        if (aiPath != null)
            aiPath.maxSpeed = prevSpeed;
        attackDamage = prevDamage;
    }

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

    public void StartReturnColor(float time)
    {
        StartCoroutine(ReturnWhiteAfterTime(time));
    }

    IEnumerator ReturnWhiteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // 🔥 استخدام المتغير المخزن
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
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

        if (aiPath != null)
            rangePrevSpeed = aiPath.maxSpeed;

        rangePrevDamage = attackDamage;

        health *= healthMultiplier;

        if (aiPath != null)
            aiPath.maxSpeed *= speedMultiplier;

        attackDamage *= damageMultiplier;
        blueColor();
        Debug.Log("Range boost applied");
    }

    public void RemoveRangeBoost()
    {
        if (!isRangeBoostActive) return;

        isRangeBoostActive = false;

        health = rangePrevHealth;

        if (aiPath != null)
            aiPath.maxSpeed = rangePrevSpeed;

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

    public void RunAway()
    {
        StartCoroutine(RunAwayCoroutine());
    }

    private IEnumerator RunAwayCoroutine()
    {
        float screenX = Camera.main.WorldToScreenPoint(transform.position).x;
        float screenWidth = Screen.width;

        Vector3 escapeDirection;

        if (screenX < screenWidth / 2f)
        {
            escapeDirection = Vector3.left;
        }
        else
        {
            escapeDirection = Vector3.right;
        }

        targetHero = null;
        targetHeroObject = null;
        targetBuilding = null;
        targetBuildingCollider = null;

        if (aiPath != null)
            aiPath.enabled = false;

        float escapeSpeed = 3f;
        float escapeTime = 10f;
        float timer = 0f;

        while (timer < escapeTime)
        {
            transform.position += escapeDirection * escapeSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    private void HandleAttackAnimation(Vector3 targetPos, bool isAttacking)
    {
        if (animator == null || spriteRenderer == null) return;

        // +++ التعديل الجديد: إرسال حالة الهجوم لسكربت الـ Blend Tree لمنع تشغيل الـ Idle +++
        MonsterAnimationBlendTree blendTreeAnim = GetComponent<MonsterAnimationBlendTree>();
        if (blendTreeAnim != null) blendTreeAnim.isAttacking = isAttacking;

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

        // 🔥 التعديل الجديد: إذا كان الوحش لا يحتوي على متغيرات isATKUP و isATKDOWN في الـ Animator، ننهي الدالة هنا
        if (warriorName == "Wom-kong")
        {
            return;
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