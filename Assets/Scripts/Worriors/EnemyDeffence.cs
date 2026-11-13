using System.Collections;
using TMPro;
using UnityEngine;
using Pathfinding;

public class EnemyDeffence : MonoBehaviour
{
    private Transform targetHero;
    private GameObject targetHeroObject;

    [Header("Stats")]
    public float health = 100f;
    public float maxHealth = 0f;
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float attackRate = 1f;
    private float attackTimer = 0f;

    [Header("Targeting")]
    private Transform targetBuilding;
    private Collider2D targetBuildingCollider;
    private Transform originalTargetBuilding;
    private bool isAttackingWall = false;

    [Header("UI")]
    public GameObject healthBarPrefab;
    private HealthBarFollow healthBarUI;

    [Header("Warrior Info")]
    public int level = 1;
    public string warriorName;

    private float originalHealth;
    private float originalDamage;

    private Coroutine tempBoostCoroutine;
    private Coroutine conditionalBoostCoroutine;

    [Header("Movement")]
    public AIPath aiPath;

    private string[] defensiveTags = { "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "boomOP" };

    [Header("Path Checking")]
    private float pathCheckTimer = 0f;
    private float pathCheckInterval = 1f;
    private bool isPathBlocked = false;

    private bool isAttackingWallTriggered = false;

    void Start()
    {
        originalHealth = health;
        originalDamage = attackDamage;

        maxHealth = health;
        ParseLevelFromText();
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

    void Update()
    {
        // تحقق من المسار بشكل دوري
        pathCheckTimer += Time.deltaTime;
        if (pathCheckTimer >= pathCheckInterval)
        {
            CheckPathStatus();
            pathCheckTimer = 0f;
        }

        // قرار الهجوم على الجدار أو الهدف الطبيعي
        if (isPathBlocked)
        {
            if (!isAttackingWallTriggered)
            {
                FindNearestWall();
                isAttackingWallTriggered = true;
                if (aiPath != null) aiPath.enabled = false;
            }
        }
        else
        {
            // إذا المسار مفتوح نرجع للسلوك الطبيعي
            if (isAttackingWall)
            {
                isAttackingWall = false;
                isAttackingWallTriggered = false;
                if (aiPath != null) aiPath.enabled = true;
                FindTargetBuilding();
            }
        }

        // سلوك الهجوم
        if (isAttackingWall)
        {
            if (targetBuilding != null && targetBuilding.gameObject.activeInHierarchy)
            {
                MoveTowardsWall();
                AttackBuilding();
            }
            else
            {
                // الجدار تدمر
                isAttackingWall = false;
                isAttackingWallTriggered = false;
                if (aiPath != null) aiPath.enabled = true;
                FindTargetBuilding();
            }
        }
        else if (targetHero != null)
        {
            AttackHero();
        }
        else
        {
            if (targetBuilding == null) FindTargetBuilding();
            if (targetBuilding != null) AttackBuilding();
        }
    }

    // حركة مباشرة للجدار
    private void MoveTowardsWall()
    {
        if (targetBuilding == null) return;

        Vector3 dir = (targetBuilding.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, targetBuilding.position);

        if (dist > attackRange)
        {
            float step = aiPath != null ? aiPath.maxSpeed * Time.deltaTime : 2f * Time.deltaTime;
            transform.position += dir * step;

            if (Vector3.Distance(transform.position, targetBuilding.position) < attackRange)
                transform.position = targetBuilding.position - dir * attackRange;
        }
    }

    // إيجاد أقرب جدار
    private void FindNearestWall()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("wall");
        float minDist = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject wall in walls)
        {
            float d = Vector3.Distance(transform.position, wall.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = wall;
            }
        }

        if (nearest != null)
        {
            originalTargetBuilding = targetBuilding;
            targetBuilding = nearest.transform;
            targetBuildingCollider = nearest.GetComponent<Collider2D>();
            isAttackingWall = true;
        }
        else
        {
            isPathBlocked = false;
            isAttackingWallTriggered = false;
        }
    }

    // التحقق من حالة المسار
    private void CheckPathStatus()
    {
        if (aiPath != null && targetBuilding != null)
        {
            if (aiPath.hasPath && aiPath.velocity.magnitude < 0.1f &&
                Vector3.Distance(transform.position, targetBuilding.position) > attackRange * 2f)
            {
                isPathBlocked = true;
            }
            else
            {
                isPathBlocked = false;
            }
        }
    }

    // باقي الدوال نفس كودك الأصلي (AttackHero, AttackBuilding, TakeDamage...)
    // فقط تم تحسين المنطق الأساسي
    // ----------------------
    private void ParseLevelFromText()
    {
        level = WarriorLevelManager.Instance.GetLevel(warriorName);
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
                attackDamage = warriorLevel.damage;
                if (aiPath != null) aiPath.maxSpeed = warriorLevel.speed;
            }
        }
    }

    void AttackHero()
    {
        if (targetHero == null || targetHeroObject == null) return;

        float distance = Vector3.Distance(transform.position, targetHero.position);
        if (distance <= attackRange && aiPath.velocity.magnitude < 0.01f)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                MonoBehaviour[] scripts = targetHeroObject.GetComponents<MonoBehaviour>();
                foreach (var script in scripts)
                {
                    var method = script.GetType().GetMethod("TakeDamage");
                    var field = script.GetType().GetField("health");

                    if (method != null && field != null)
                    {
                        method.Invoke(script, new object[] { attackDamage });

                        float currentHealth = (float)field.GetValue(script);
                        if (currentHealth <= 0)
                        {
                            targetHero = null;
                            targetHeroObject = null;
                        }

                        break;
                    }
                }
                attackTimer = 0f;
            }
        }
    }

    void FindTargetBuilding()
    {
        string[] defensiveTags = { "boomOP", "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "wood" };
        string[] otherTags = { "trapfire", "trapN", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "mongel" };

        float shortestDistance = Mathf.Infinity;
        GameObject nearestBuilding = null;

        foreach (string tag in defensiveTags)
        {
            foreach (GameObject building in GameObject.FindGameObjectsWithTag(tag))
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestBuilding = building;
                }
            }
        }

        if (nearestBuilding == null)
        {
            foreach (string tag in otherTags)
            {
                foreach (GameObject building in GameObject.FindGameObjectsWithTag(tag))
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
            isAttackingWall = false;
            isPathBlocked = false;
        }
        else
        {
            targetBuilding = null;
            targetBuildingCollider = null;
        }
    }

    void AttackBuilding()
    {
        if (targetBuilding == null || targetBuildingCollider == null) return;

        // إذا كان الهدف جدارًا، استخدم موقع الجدار مباشرة
        Vector2 closestPoint = isAttackingWall ?
            (Vector2)targetBuilding.position :
            targetBuildingCollider.ClosestPoint(transform.position);

        float distance = Vector2.Distance(transform.position, closestPoint);

        if (distance <= attackRange && aiPath.velocity.magnitude < 0.01f)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                // معالجة الهجوم على الجدار
                if (targetBuilding.CompareTag("wall"))
                {
                    building wallComponent = targetBuilding.GetComponent<building>();
                    if (wallComponent != null)
                    {
                        wallComponent.TakeDamage(attackDamage);
                        showNumberPop showNum = targetBuilding.GetComponent<showNumberPop>();
                        if (showNum != null)
                            showNum.OnHit(attackDamage);
                    }
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
            }
        }
    }

    // باقي الدوال تبقى كما هي
    public void TakeDamage(float damage, bool attackedByHero = false)
    {
        health -= damage;
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealth(health);
            healthBarUI.gameObject.SetActive(health < maxHealth && health > 0);
        }

        if (attackedByHero)
        {
            FindAttackerHero();
        }

        if (health <= 0)
        {
            if (healthBarUI != null)
                Destroy(healthBarUI.gameObject);
            Die();
        }
    }

    private void FindAttackerHero()
    {
        GameObject heroObj = GameObject.FindGameObjectWithTag("monster");
        if (heroObj != null && heroObj.activeInHierarchy)
        {
            targetHeroObject = heroObj;
            targetHero = heroObj.transform;
        }
    }

    void Die()
    {
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
        float prevDamage = attackDamage;

        health *= healthMultiplier;
        attackDamage *= damageMultiplier;

        yield return new WaitForSeconds(duration);

        health = prevHealth;
        attackDamage = prevDamage;
    }

    public void HightWithSpeelCondition(float healthMultiplier, float speedMultiplier, float damageMultiplier, System.Func<bool> stopCondition)
    {
        if (conditionalBoostCoroutine != null)
            StopCoroutine(conditionalBoostCoroutine);

        conditionalBoostCoroutine = StartCoroutine(ConditionalBoost(healthMultiplier, speedMultiplier, damageMultiplier, stopCondition));
    }

    private IEnumerator ConditionalBoost(float healthMultiplier, float speedMultiplier, float damageMultiplier, System.Func<bool> stopCondition)
    {
        float prevHealth = health;
        float prevDamage = attackDamage;

        health *= healthMultiplier;
        attackDamage *= damageMultiplier;

        while (!stopCondition())
        {
            yield return null;
        }

        health = prevHealth;
        attackDamage = prevDamage;
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealth(health);
            healthBarUI.gameObject.SetActive(health < maxHealth && health > 0);
        }
    }


}
