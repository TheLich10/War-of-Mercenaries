using Pathfinding;
using UnityEngine;

public class HeroUnit : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float stopDistance = 1f;
    public float attackRate = 1f;
    public float attackDamage = 20f;
    public float health = 150f;

    public static bool isActiveHero = true;

    private Transform targetEnemy;
    private float attackTimer = 0f;

    private Transform homeBarracks;
    private bool returningToOrigin = false;

    private Transform caveTarget = null;

    public float detectionRange = 5f;

    // إضافات شريط الصحة
    public GameObject healthBarPrefab;
    private HealthBarFollow healthBarUI;
    private float maxHealth;

    // ✅ متغير جديد لتتبع حالة التفعيل السابقة
    private bool wasActiveHero;

    // ✅ متغير جديد للتحكم في ارتفاع نقطة العودة للثكنة
    public float barracksHeightOffset = 1.2f;
    void Start()
    {
        wasActiveHero = isActiveHero; // تسجيل الحالة الأولية

        FindHomeBarracks();
        InitializeWarriorStats();

        maxHealth = health;

        // إنشاء شريط الصحة
        if (healthBarPrefab != null)
        {
            GameObject barGO = Instantiate(healthBarPrefab, GameObject.Find("Canvas").transform);
            healthBarUI = barGO.GetComponent<HealthBarFollow>();
            healthBarUI.target = this.transform;
            healthBarUI.Initialize(maxHealth);
            healthBarUI.UpdateHealth(health);
            barGO.SetActive(false);
        }

        if (!isActiveHero)
        {
            FindCave();
        }
        else
        {
            // ✅ إذا بدأ اللعبة وهو مفعل، دعه يتجه للثكنة فوراً
            returningToOrigin = true;
        }
    }

    private void InitializeWarriorStats()
    {
        int currentLevel = MonsterLevelManager.Instance.GetLevel("BearWolf");

        MonsterData monster = MonsterDatabase.GetMonsterByName("BearWolf");
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
        // ✅ التحقق مما إذا تم تفعيل الوحش للتو (تغيرت الحالة من false إلى true)
        if (isActiveHero && !wasActiveHero)
        {
            returningToOrigin = true;
            if (homeBarracks == null)
            {
                FindHomeBarracks();
            }
        }
        wasActiveHero = isActiveHero; // تحديث الحالة الحالية

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

        if (homeBarracks == null)
        {
            FindHomeBarracks();
        }
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
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (healthBarUI != null)
        {
            if (!healthBarUI.gameObject.activeSelf && health > 0)
                healthBarUI.gameObject.SetActive(true);

            healthBarUI.UpdateHealth(health);
        }

        if (health <= 0)
        {
            if (healthBarUI != null)
            {
                Destroy(healthBarUI.gameObject);
                healthBarUI = null;
            }

            Die();
        }
    }

    void Die()
    {
        Debug.Log("Hero Died");
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