using UnityEngine;
using System.Collections;

public class MonsterHealth : MonoBehaviour
{

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;

    [Header("Healing")]
    public float healPerSecond = 5f;
    public bool isInBattle;

    private string monsterName;
    private int level;
    private Coroutine healCoroutine;

    // لمراقبة التغيير
    private string lastMonsterName;
    private int lastLevel;

    void Start()
    {
        UpdateMonsterInfo();
    }

    void Update()
    {
        // إذا تغير الوحش أو المستوى
        if (monsterName != GetObjectNameAndText.objectName || level != GetObjectNameAndText.savedNumber)
        {
            UpdateMonsterInfo();
        }
    }

    void UpdateMonsterInfo()
    {
        // 1️⃣ تحديث الاسم والمستوى
        monsterName = GetObjectNameAndText.objectName;
        level = GetObjectNameAndText.savedNumber;

        if (monsterName == "CaveEnter" && HeroArcher2.isActiveHero)
            monsterName = "DraginFly";
        else if (monsterName == "CaveEnter" && HeroArcher.isActiveHero)
            monsterName = "Wom-kong";
        else if (monsterName == "CaveEnter" && HeroUnit.isActiveHero)
            monsterName = "BearWolf";
        else if (monsterName == "CaveEnter" && HeroUnit2.isActiveHero)
            monsterName = "Mightosour";

        LoadHealthFromDatabase();

        // إعادة ضبط الصحة كاملة عند تغيير الوحش
        currentHealth = maxHealth;

        // حفظ آخر اسم ومستوى
        lastMonsterName = monsterName;
        lastLevel = level;
    }

    void LoadHealthFromDatabase()
    {
        MonsterData monster = MonsterDatabase.GetMonsterByName(monsterName);
        if (monster == null)
        {
            Debug.LogWarning("Monster not found: " + monsterName);
            return;
        }

        MonsterLevel monsterLevel = monster.GetLevel(level);
        if (monsterLevel == null)
        {
            Debug.LogWarning("Level not found: " + level);
            return;
        }

        maxHealth = monsterLevel.health;
    }

    // 🔴 ضرر
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    // ⚔️ بدء معركة
    public void StartBattle()
    {
        isInBattle = true;
        if (healCoroutine != null)
            StopCoroutine(healCoroutine);
    }

    // 🟢 نهاية المعركة
    public void EndBattle()
    {
        isInBattle = false;
        healCoroutine = StartCoroutine(HealOverTime());
    }

    IEnumerator HealOverTime()
    {
        while (currentHealth < maxHealth)
        {
            currentHealth += healPerSecond * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            yield return null;
        }
    }
}
