using UnityEngine;
using TMPro;
using System.Collections;

public class RedCoinCollector : MonoBehaviour
{
    public TextMeshProUGUI redCoinText; // مرجع إلى النص في UI لعرض العملة
    public TextMeshProUGUI redCoinText2; // مرجع إلى النص في UI لعرض العملة

    private static int redCoinAmount = 0; // كمية العملة الحالية

    // جعلناه public لنتمكن من الوصول إليه في SaveManager
    public static RedCoinCollector instance;

    void Awake()
    {
        instance = this; // حفظ مرجع هذا الكائن
    }

    // 🟢 دالة جديدة لاستقبال البيانات من ملف الحفظ (JSON)
    public void LoadData(int savedAmount)
    {
        redCoinAmount = savedAmount;
        UpdateRedCoinUI();
    }

    void UpdateRedCoinUI()
    {
        if (redCoinText != null)
            redCoinText.text = redCoinAmount.ToString();

        if (redCoinText2 != null && redCoinText != null)
            redCoinText2.text = redCoinText.text;
    }

    // ✅ دالة عامة لإضافة عملة
    public static void AddRedCoin(int amount)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.AddRedCoinGradually(amount));
        }
        else
        {
            redCoinAmount += amount;
        }
    }

    private IEnumerator AddRedCoinGradually(int amountToAdd)
    {
        int added = 0;
        int step = 1; // يبدأ من 1 ويتضاعف

        while (added < amountToAdd)
        {
            int remaining = amountToAdd - added;
            int increment = Mathf.Min(step, remaining);

            redCoinAmount += increment;
            added += increment;

            UpdateRedCoinUI(); // نكتفي بتحديث الواجهة (تم مسح كود الحفظ المكرر)

            step *= 2; // يتضاعف
            yield return new WaitForSeconds(0.05f); // كل دفعة بينهما وقت قصير
        }
    }

    // ✅ دالة عامة لنقصان عملة تدريجيًا
    public static void RemoveRedCoin(int amount)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.RemoveRedCoinGradually(amount));
        }
        else
        {
            redCoinAmount -= amount;
            if (redCoinAmount < 0) redCoinAmount = 0;
        }
    }

    private IEnumerator RemoveRedCoinGradually(int amountToRemove)
    {
        int removed = 0;
        int step = 1; // يبدأ من 1 ويتضاعف
        while (removed < amountToRemove)
        {
            int remaining = amountToRemove - removed;
            int decrement = Mathf.Min(step, remaining);

            redCoinAmount -= decrement;
            if (redCoinAmount < 0) redCoinAmount = 0;

            removed += decrement;

            UpdateRedCoinUI();

            step *= 2; // يتضاعف
            yield return new WaitForSeconds(0.05f); // كل دفعة بينها وقت قصير
        }
    }

    // ✅ دالة للحصول على القيمة الحالية
    public static int GetRedCoinAmount()
    {
        return redCoinAmount;
    }
}