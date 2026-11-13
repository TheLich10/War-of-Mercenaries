using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiMonsterHealthBar : MonoBehaviour
{
    public static UiMonsterHealthBar Instance;

    [Header("UI Elements")]
    public Image monsterHealthBarFill; // ÇáÈÇÑ
    public TMP_Text monsterHealthText; // ÇáäÕ

    private MonsterHealth currentMonster;

    void Awake()
    {
        Instance = this;
    }

    // ÇÓÊÏÚÇÁ ÚäÏ ÇáäÞÑ Úáì ãÈäì ÇáßåÝ áÊÍÏíÏ ÇáæÍÔ
    public void SetCurrentMonster(MonsterHealth monster)
    {
        // Ýß ÇáÇÔÊÑÇß ãä ÇáæÍÔ ÇáÓÇÈÞ
        if (currentMonster != null)
        {
            // áæ ÃÖÝÊ ÃÍÏÇË ãÓÊÞÈáíÉ íãßä Ýß ÇáÇÔÊÑÇß åäÇ
        }

        currentMonster = monster;

        if (currentMonster != null)
        {
            UpdateMonsterUI(currentMonster.currentHealth);
        }
    }

    void Update()
    {
        if (currentMonster == null) return;

        // ÊÍÏíË ÇáäÕ æÇáÈÇÑ ÈÔßá ãÓÊãÑ ÍÓÈ ÇáÕÍÉ ÇáÍÇáíÉ
        UpdateMonsterUI(currentMonster.currentHealth);
    }

    private void UpdateMonsterUI(float currentHealth)
    {
        if (monsterHealthText != null)
            monsterHealthText.text = currentHealth.ToString(); // ÝÞØ ÇáÕÍÉ ÇáÍÇáíÉ

        if (monsterHealthBarFill != null && currentMonster.maxHealth > 0)
            monsterHealthBarFill.fillAmount = currentHealth / currentMonster.maxHealth;
    }
}
