using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterHealthUI : MonoBehaviour
{
    [Header("Monster Health Reference")]
    public MonsterHealth monsterHealth;

    [Header("UI Elements")]
    public TMP_Text healthText; // «·‰’ ⁄·Ï ‘ﬂ· 100/100
    public Image healthBar;     // ’Ê—… „‰ ‰Ê⁄ Filled

    [Header("Settings")]
    public string monsterTag = "CaveBrack"; // ÷⁄ Tag ·ﬂ· ÊÕ‘ ›Ì Prefab

    void Start()
    {
        // Õ«Ê· —»ÿ «·ÊÕ‘  ·ﬁ«∆Ì« ⁄‰œ «·»œ«Ì…
        if (monsterHealth == null)
        {
            FindAndAssignMonster();
        }
    }

    void Update()
    {
        // ≈–« ·„ Ìﬂ‰ Â‰«ﬂ ÊÕ‘ „— »ÿ° Õ«Ê· «·⁄ÀÊ— ⁄·ÌÂ
        if (monsterHealth == null)
        {
            FindAndAssignMonster();
            return;
        }

        //  ÕœÌÀ «·‰’
        healthText.text = Mathf.Ceil(monsterHealth.currentHealth) + "/" + Mathf.Ceil(monsterHealth.maxHealth);

        //  ÕœÌÀ «·»«—
        if (healthBar != null && monsterHealth.maxHealth > 0)
        {
            healthBar.fillAmount = monsterHealth.currentHealth / monsterHealth.maxHealth;
        }
    }

    private void FindAndAssignMonster()
    {
        // «·»ÕÀ ⁄‰ «·ÊÕ‘ ›Ì «·„‘Âœ Õ”» Tag
        GameObject monsterObj = GameObject.FindGameObjectWithTag(monsterTag);
        if (monsterObj != null)
        {
            MonsterHealth mh = monsterObj.GetComponent<MonsterHealth>();
            if (mh != null)
            {
                monsterHealth = mh;
            }
            else
            {
                Debug.LogWarning("MonsterHealth component not found on object with Tag: " + monsterTag);
            }
        }
    }
}
