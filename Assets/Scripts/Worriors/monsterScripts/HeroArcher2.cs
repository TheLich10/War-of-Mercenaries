using UnityEngine;

public class HeroArcher2 : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float stopDistance = 1f;
    public float attackRate = 1f;
    public float attackDamage = 20f;
    public float health = 150f;

    public GameObject projectilePrefab;
    public Transform firePoint;

    public static bool isActiveHero = true;

    private Transform targetEnemy;
    private float attackTimer = 0f;

    // ✅ تم استبدال الموقع الثابت بمرجع لمبنى الثكنة
    private Transform homeBarracks;
    private bool returningToOrigin = false;

    private Transform caveTarget = null;

    public float detectionRange = 5f;

    // إضافات شريط الصحة
    public GameObject healthBarPrefab;
    private HealthBarFollow healthBarUI;
    private float maxHealth;

    // ✅ متغير جديد للتحكم في ارتفاع نقطة العودة للثكنة
    public float barracksHeightOffset = 1.2f;

    private bool wasActiveHero;
    void Start()
    {
        wasActiveHero = isActiveHero;
        FindHomeBarracks();
        InitializeWarriorStats();

        if (healthBarPrefab != null)
        {
            GameObject barGO = Instantiate(healthBarPrefab, GameObject.Find("Canvas").transform);
            healthBarUI = barGO.GetComponent<HealthBarFollow>();
            healthBarUI.target = this.transform;
            healthBarUI.Initialize(health);
            healthBarUI.UpdateHealth(health);
            barGO.SetActive(false);
        }

        if (!isActiveHero)
        {
            FindCave();
        }
        else
        {
            returningToOrigin = true; // إضافة ضرورية
        }
    }

    private void InitializeWarriorStats()
    {
        // جلب مستوى الوحش الحقيقي من MonsterLevelManager حسب اسم الوحش المطابق
        int currentLevel = MonsterLevelManager.Instance.GetLevel("DraginFly");

        // جلب بيانات الوحش
        MonsterData monster = MonsterDatabase.GetMonsterByName("DraginFly");
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

    void Update()
    {
        if (isActiveHero && !wasActiveHero)
        {
            returningToOrigin = true;
            if (homeBarracks == null) FindHomeBarracks();
        }
        wasActiveHero = isActiveHero;

        if (isActiveHero == true)
        {
            ShowObject();
        }

        if (!isActiveHero)
        {
            ReturnToCave();
            return;
        }

        if (targetEnemy != null)
        {
            // تحرك وهاجم العدو
            float distance = Vector3.Distance(transform.position, targetEnemy.position);

            if (distance > stopDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetEnemy.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackRate)
                {
                    Attack();
                    attackTimer = 0f;
                }
            }
        }
        else if (returningToOrigin)
        {
            if (homeBarracks != null)
            {
                // ✅ تحديد الموقع الهدف مع إضافة الارتفاع الجديد (Offset)
                Vector3 targetPosition = homeBarracks.position + new Vector3(0, barracksHeightOffset, 0);

                // حساب المسافة بناءً على الموقع الجديد
                float distance = Vector3.Distance(transform.position, targetPosition);

                if (distance > 0.1f)
                {
                    // التحرك نحو الموقع الجديد (المرتفع قليلاً)
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                }
                else
                {
                    returningToOrigin = false;
                }
            }
            else
            {
                // في حال لم يتم العثور على المبنى
                returningToOrigin = false;
            }
        }
    }

    // ✅ دالة جديدة للبحث عن ثكنة الوحوش
    void FindHomeBarracks()
    {
        GameObject barracks = GameObject.FindGameObjectWithTag("CaveBrack");
        if (barracks != null)
        {
            homeBarracks = barracks.transform;
        }
        else
        {
            Debug.LogWarning("لم يتم العثور على مبنى يحمل التاغ CaveBrack!");
        }
    }

    void ReturnToCave()
    {
        if (caveTarget == null)
        {
            FindCave();
            if (caveTarget == null) return;
        }

        float distance = Vector3.Distance(transform.position, caveTarget.position);

        if (distance > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, caveTarget.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            HideObject();
        }
    }

    void FindCave()
    {
        GameObject cave = GameObject.FindGameObjectWithTag("Cave");
        if (cave != null)
        {
            caveTarget = cave.transform;
        }
    }

    public void SetTarget(Transform target)
    {
        if (!isActiveHero) return;

        targetEnemy = target;
        returningToOrigin = false;
    }

    public void ClearTarget()
    {
        if (!isActiveHero) return;

        targetEnemy = null;
        returningToOrigin = true;

        // تحديث مكان المبنى في حال تم إنشاء مبنى جديد أو تغييره أثناء القتال
        if (homeBarracks == null)
        {
            FindHomeBarracks();
        }
    }

    void Attack()
    {
        if (targetEnemy != null && projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile2 projectileScript = projectile.GetComponent<Projectile2>();
            projectileScript.SetTarget(targetEnemy, attackDamage);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Hero Archer Died");
        gameObject.SetActive(false);
    }

    void HideObject()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
            rend.enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }
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