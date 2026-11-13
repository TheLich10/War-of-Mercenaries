using Pathfinding;
using UnityEngine;
using System.Collections;
using TMPro;

public class Enemy : MonoBehaviour, IBuffable
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
    public float explosionRadius = 1.5f;

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

    // 🔥 1. تعريف المتغير هنا لتخزينه مرة واحدة
    private AIDestinationSetter destSetter;
    private SpriteRenderer spriteRenderer; // لتجنب استدعاء GetComponent في الألوان

    private string[] defensiveTags = { "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "boomOP" };

    public GameObject dieAnime;
    [Header("Pools")]
    private UnitPool effectPool; // الـ Pool الخاص بتأثيرات الموت (dieAnime)

    void Awake()
    {
        // 1. نحدد اسم الـ Pool بناءً على اسم الجندي (إذا كان Kamikaze نختار الـ Pool الخاص به، وإلا نختار الافتراضي)
        string targetPoolName = warriorName.Equals("Kamikaze", System.StringComparison.OrdinalIgnoreCase) ? "EffectPoolKamaiz" : "EffectPoolManager";

        // 2. نبحث عن الأوبجكت في المشهد عن طريق الاسم الذي حددناه
        GameObject poolObj = GameObject.Find(targetPoolName);

        // 3. إذا وجدناه، نقوم بسحب سكريبت UnitPool منه ونربطه
        if (poolObj != null)
        {
            effectPool = poolObj.GetComponent<UnitPool>();
        }
        else
        {
            Debug.LogWarning($"⚠️ لم يتم العثور على أوبجكت باسم {targetPoolName} في المشهد!");
        }
    }
    void Start()
    {
        animator = GetComponent<Animator>();

        // 🔥 2. تخزين المكونات عند البداية فقط
        destSetter = GetComponent<AIDestinationSetter>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        ParseLevelFromText();
        InitializeWarriorStats();

        originalHealth = health;
        originalDamage = attackDamage;
        originalSpeed = Speed;
        maxHealth = health;

        //FindTargetBuilding();

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

                // نبحث عن الكوليدر حتى لو كان داخل الأبناء (Children) لتجنب وقوف الجندي بدون سبب
                targetBuildingCollider = targetBuilding.GetComponentInChildren<Collider2D>();
            }
        }

        // 2. تنظيم الهجوم
        if (targetHero != null)
        {
            AttackHero();
        }
        else
        {
            AttackBuilding();
        }

    }

    // ... (دوال ParseLevelFromText و InitializeWarriorStats تبقى كما هي بدون تغيير) ...
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
    }

    void AttackHero()
    {
        if (targetHero == null || targetHeroObject == null)
        {
            if (aiPath != null) aiPath.isStopped = false; // أكمل طريقك إذا مات البطل
            // === الإضافة الجديدة هنا لإيقاف الهجوم ===
            HandleAttackAnimation(Vector3.zero, false);
            if (warriorName == "BearWolf" || warriorName == "Mightosour")
            {
                animator.SetBool("Attack", false);
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, targetHero.position);

        if (distance <= attackRange)
        {
            aiPath.isStopped = true; // 🔥 أوقف الحركة تماماً أثناء الهجوم

            HandleAttackAnimation(targetHero.position, true);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                attackTimer = 0f;
                PlayAttackSound();
                // 🔥 التحقق من الكاميكازي أولاً
                if (warriorName.Equals("Kamikaze", System.StringComparison.OrdinalIgnoreCase))
                {
                    Explode();
                    return;
                }

                // +++ التعديل الجديد يبدأ هنا +++
                if (warriorName == "BearWolf" || warriorName == "Mightosour")
                {
                    animator.SetBool("Attack", true);
                }
                // +++ التعديل الجديد ينتهي هنا +++

                IDamageable damageableTarget = targetHeroObject.GetComponent<IDamageable>();
                if (damageableTarget != null)
                {
                    damageableTarget.TakeDamage(attackDamage, this.transform);
                }

            }
        }
        else
        {
            aiPath.isStopped = false; // 🔥 اسمح له بالحركة للاقتراب من البطل
            HandleAttackAnimation(Vector3.zero, false);
            if (warriorName == "BearWolf" || warriorName == "Mightosour")
            {
                animator.SetBool("Attack", false);
            }
        }
    }


    void AttackBuilding()
    {
        // ❌ أزلنا شرط (targetBuildingCollider == null) من هنا لكي لا يعطل الهجوم
        if (targetBuilding == null)
        {
            if (aiPath != null) aiPath.isStopped = false;
            // === الإضافة الجديدة هنا لإيقاف الهجوم ===
            HandleAttackAnimation(Vector3.zero, false);
            if (warriorName == "BearWolf" || warriorName == "Mightosour")
            {
                animator.SetBool("Attack", false);
            }
            return;
        }

        // 🔥 حساب المسافة: إذا كان هناك كوليدر نستخدمه، وإلا نحسب المسافة لمركز المبنى
        float distance;
        if (targetBuildingCollider != null)
        {
            distance = Vector2.Distance(transform.position, targetBuildingCollider.ClosestPoint(transform.position));
        }
        else
        {
            distance = Vector2.Distance(transform.position, targetBuilding.position);
        }

        // 🔥 التعديل الأهم: نتحقق من المسافة، أو نسأل الذكاء الاصطناعي إذا كان متأكداً أنه وصل للهدف
        bool isCloseEnough = distance <= attackRange || (aiPath != null && aiPath.reachedDestination && distance <= attackRange + 1.5f);

        if (isCloseEnough)
        {
            if (aiPath != null) aiPath.isStopped = true; // أوقف الحركة تماماً أثناء الهجوم

            // 🔹 تحديد موقع الهدف بدقة (سواء الكوليدر أو مركز المبنى) لتشغيل الأنيميشن في الاتجاه الصحيح
            Vector3 exactTargetPos = targetBuildingCollider != null ? (Vector3)targetBuildingCollider.ClosestPoint(transform.position) : targetBuilding.position;
            HandleAttackAnimation(exactTargetPos, true);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                attackTimer = 0f; // تصفير العداد بعد الضربة
                PlayAttackSound();


                // 🔥 التحقق من الكاميكازي أولاً
                if (warriorName.Equals("Kamikaze", System.StringComparison.OrdinalIgnoreCase))
                {
                    Explode();
                    return;
                }

                // +++ التعديل الجديد يبدأ هنا +++
                if ( warriorName == "BearWolf" || warriorName == "Mightosour")
                {
                    animator.SetBool("Attack", true);
                }
                // +++ التعديل الجديد ينتهي هنا +++

                // إلحاق الضرر بالمبنى
                showNumberPop showNum = targetBuilding.GetComponent<showNumberPop>();

                if (targetBuilding.CompareTag("wall"))
                {
                    building wallComponent = targetBuilding.GetComponent<building>();
                    if (wallComponent != null) wallComponent.TakeDamage(attackDamage);
                    if (showNum != null) showNum.OnHit(attackDamage);
                }
                else if (targetBuilding.CompareTag("spy"))
                {
                    DefensiveWatchPost watchPost = targetBuilding.GetComponent<DefensiveWatchPost>();
                    if (watchPost != null) watchPost.TakeDamage(attackDamage);
                    if (showNum != null) showNum.OnHit(attackDamage);
                }
                else
                {
                    // فحص المباني الدفاعية الأخرى والمباني العادية
                    if (System.Array.Exists(defensiveTags, tag => targetBuilding.CompareTag(tag)))
                    {
                        DefensiveBuilding defBuilding = targetBuilding.GetComponent<DefensiveBuilding>();
                        if (defBuilding != null) defBuilding.TakeDamage(attackDamage);
                        if (showNum != null) showNum.OnHit(attackDamage);
                    }
                    else
                    {
                        building b = targetBuilding.GetComponent<building>();
                        if (b != null) b.TakeDamage(attackDamage);

                        TownHall hall = targetBuilding.GetComponent<TownHall>();
                        if (hall != null) hall.OnHit(attackDamage);

                        if (showNum != null) showNum.OnHit(attackDamage);
                    }
                }

            }
        }
        else
        {
            if (aiPath != null) aiPath.isStopped = false; // استمر بالحركة للوصول للمبنى
                                                          // 🔹 أضف هذا السطر لإيقاف الأنيميشن أثناء الحركة
            HandleAttackAnimation(Vector3.zero, false);
            if (warriorName == "BearWolf" || warriorName == "Mightosour")
            {
                animator.SetBool("Attack", false);
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
        if (attacker != null)
        {
            targetHero = attacker;
            targetHeroObject = attacker.gameObject;

            targetBuilding = null;
            targetBuildingCollider = null;

            if (destSetter != null)
            {
                destSetter.target = targetHero;
                //destSetter.enabled = false; // 🔥 نوقف سكريبت البحث عن مباني مؤقتاً لكي لا يغير هدف البطل
            }
        }

        if (health <= 0)
        {
            if (healthBarUI != null)
                Destroy(healthBarUI.gameObject);
            Die();
        }
    }
    private void PlayAttackSound()
    {
        // التحقق من اسم المحارب لتشغيل الصوت المناسب
        if (warriorName.Equals("Kamikaze", System.StringComparison.OrdinalIgnoreCase))
        {
            AudioManager.Instance.PlaySFX("kamakizATK"); // استبدل بـ اسم ملف الصوت الخاص بك
        }
        else if (warriorName.Equals("Ottoman", System.StringComparison.OrdinalIgnoreCase))
        {
            AudioManager.Instance.PlaySFX("otmATK");
        }
        else if (warriorName.Equals("persian", System.StringComparison.OrdinalIgnoreCase))
        {
            AudioManager.Instance.PlaySFX("personATK");
        }
        else if (warriorName.Equals("BearWolf", System.StringComparison.OrdinalIgnoreCase))
        {
            AudioManager.Instance.PlaySFX("flyATK");
        }
        else if (warriorName.Equals("Battering_Gaul", System.StringComparison.OrdinalIgnoreCase))
        {
            AudioManager.Instance.PlaySFX("flyATK");
        }
        else if (warriorName.Equals("Mightosour", System.StringComparison.OrdinalIgnoreCase))
        {
            AudioManager.Instance.PlaySFX("flyATK");
        }
        else
        {
            // الصوت الافتراضي لباقي المحاربين
            AudioManager.Instance.PlaySFX("bedATK");
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

        if (warriorName == "Battering_Gaul")
        {
            AudioManager.Instance.PlaySFX("dieGual");

        }
        else if (warriorName == "BearWolf" || warriorName == "Mightosour")
        {
            AudioManager.Instance.PlaySFX("dieMonster");
        }
        if (warriorName == "Sneaky")
        {
            AudioManager.Instance.PlaySFX("GirlDie");

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


    public void HightWithSpeel(float healthMultiplier, float speedMultiplier, float damageMultiplier, float duration)
    {
        if (tempBoostCoroutine != null)
            StopCoroutine(tempBoostCoroutine);

        tempBoostCoroutine = StartCoroutine(TemporaryBoost(healthMultiplier, speedMultiplier, damageMultiplier, duration));
    }

    private IEnumerator TemporaryBoost(float healthMultiplier, float speedMultiplier, float damageMultiplier, float duration)
    {
        float prevHealth = health;
        float prevSpeed = 0f;
        if (aiPath != null)
        {
            prevSpeed = aiPath.maxSpeed;

        }
        float prevDamage = attackDamage;

        health *= healthMultiplier;
        if (aiPath != null)
        {
            aiPath.maxSpeed *= speedMultiplier;

        }
        attackDamage *= damageMultiplier;
        Debug.Log("is boosting");
        yield return new WaitForSeconds(duration);

        health = prevHealth;
        Speed = prevSpeed;
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

        if (aiPath != null)
            rangePrevSpeed = aiPath.maxSpeed;

        rangePrevDamage = attackDamage;

        // تطبيق الصقل
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
            sr.color = new Color(0.6f, 0.8f, 1f, 1f);
    }

    public void whiteClor()
    {

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.white;
    }

    // ... تأكد من نسخ باقي دوالك السابقة (Heal, Boosts, RunAway) هنا ...
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

        // تعطيل AIPath
        if (aiPath != null)
            aiPath.enabled = false;

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
        if ( warriorName == "BearWolf" || warriorName == "Mightosour")
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

    private void Explode()
    {
        // 1. إنشاء دائرة وهمية وجلب كل العناصر التي بداخلها
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in colliders)
        {
            // تجنب إلحاق الضرر بنفسه
            if (hit.gameObject == this.gameObject) continue;

            // تجاهل الجنود الخاصين بك بناءً على التاغ
            if (hit.CompareTag("Enemy") || hit.CompareTag("EnemyFly") || hit.CompareTag("monsterFly") || hit.CompareTag("CaveBrack"))
            {
                continue;
            }

            // 2. التحقق من الأبطال (Heroes)
            IDamageable damageableTarget = hit.GetComponentInParent<IDamageable>();
            if (damageableTarget != null)
            {
                damageableTarget.TakeDamage(attackDamage, this.transform);
            }



            // 3. التحقق من المباني بترتيب الأولويات (من الأكثر تخصيصاً إلى الأكثر عمومية)
            DefensiveBuilding defBuilding = hit.GetComponentInParent<DefensiveBuilding>();
            DefensiveWatchPost watchPost = hit.GetComponentInParent<DefensiveWatchPost>();
            TownHall hall = hit.GetComponentInParent<TownHall>();
            building normalBuilding = hit.GetComponentInParent<building>();

            // هنا نستخدم else if لضمان تنفيذ كتلة برمجية واحدة فقط للمبنى الواحد
            if (defBuilding != null)
            {
                // إذا كان مبنى دفاعي، ألحق الضرر به وتجاهل الباقي
                defBuilding.TakeDamage(attackDamage);
            }
            else if (watchPost != null)
            {
                // إذا كان برج مراقبة دفاعي، ألحق الضرر به وتجاهل الباقي
                watchPost.TakeDamage(attackDamage);
            }
            else if (hall != null)
            {
                // إذا كان قاعة المدينة
                hall.OnHit(attackDamage);
            }
            else if (normalBuilding != null)
            {
                // لن يصل الكود هنا إلا إذا كان المبنى عادي (لا يحتوي على السكربتات الدفاعية أعلاه)
                normalBuilding.TakeDamage(attackDamage);
            }

            // 4. إظهار رقم الضرر المنبثق
            showNumberPop showNum = hit.GetComponentInParent<showNumberPop>();
            if (showNum != null) showNum.OnHit(attackDamage);
        }

        // 5. تدمير الكاميكازي بعد الانفجار
        Die();
    }
}
