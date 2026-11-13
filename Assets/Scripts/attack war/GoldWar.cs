using UnityEngine;
using TMPro;

public class GoldWar : MonoBehaviour
{
    public TextMeshProUGUI goldText; // مرجع إلى نص الذهب في UI

    private int lastGoldAmount = -1; // لتتبع التغيرات في قيمة الذهب

    void Update()
    {
        // ✅ التعديل هنا: جلبنا الذهب مباشرة من السكربت الأساسي بدلاً من PlayerPrefs
        int currentGold = GoldCollector.GetGold();

        // نقوم بتحديث النص فقط إذا تغيرت قيمة الذهب (لتحسين أداء اللعبة)
        if (currentGold != lastGoldAmount)
        {
            lastGoldAmount = currentGold;

            if (goldText != null)
            {
                goldText.text = currentGold.ToString();
            }
        }
    }
}