using System.Collections;
using TMPro;
using UnityEngine;
using Pathfinding;

public class sneaky : MonoBehaviour
{
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

    private string[] defensiveTags = { "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "boomOP" };


    void Start()
    {
        ParseLevelFromText();
        InitializeWarriorStats();

        originalHealth = health;
        originalDamage = attackDamage;
        originalSpeed = Speed;
        maxHealth = health;
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
        AIDestinationSetter setter = GetComponent<AIDestinationSetter>();
        if (setter != null && setter.target != null)
        {
            targetBuilding = setter.target;
            targetBuildingCollider = targetBuilding.GetComponent<Collider2D>();
        }

        // تابع الهجوم على الهدف الحالي
        AttackBuilding();

        // يمكنك إضافة سلوك الهجوم على البطل هنا إذا كان موجود
        if (targetHero != null)
        {
            AttackHero();
        }
    }

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
        string[] defensiveTags = { "wood", "stone", "iron", "Resoures", "CenterCity" };
        string[] otherTags = { "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "boomOP", "trapfire", "trapN", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "mongel" };

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

        // حساب المسافة بشكل أدق باستخدام الـ Collider
        float distance = Vector2.Distance(transform.position, targetBuildingCollider.ClosestPoint(transform.position));

        // ✅ إذا الهدف جدار
        if (targetBuilding.CompareTag("wall") && distance <= attackRange && aiPath.velocity.magnitude < 0.1f)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                building wallComponent = targetBuilding.GetComponent<building>();
                if (wallComponent != null)
                {
                    wallComponent.TakeDamage(attackDamage);

                    showNumberPop showNum = targetBuilding.GetComponent<showNumberPop>();
                    if (showNum != null)
                        showNum.OnHit(attackDamage);
                }
                attackTimer = 0f;
            }
            return;
        }

        // ✅ باقي المباني
        if (distance <= attackRange && aiPath.velocity.magnitude < 0.1f)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackRate)
            {
                if (System.Array.Exists(defensiveTags, tag => targetBuilding.CompareTag(tag)))
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
