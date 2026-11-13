using UnityEngine;
using TMPro;

public class ResourceWar : MonoBehaviour
{
    [Header("UI References - نصوص الموارد")]
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI ironText;
    public TextMeshProUGUI powerText;

    // لتتبع التغيرات في القيم (لتحسين أداء اللعبة)
    private int lastWoodAmount = -1;
    private int lastStoneAmount = -1;
    private int lastIronAmount = -1;
    private int lastPowerAmount = -1;

    void Update()
    {
        // ✅ جلب الموارد مباشرة من السكربت الأساسي بشكل ثابت (Static)
        // (تأكد أن أسماء الدوال تطابق الدوال الموجودة عندك في ResourceManager)
        int currentWood = ResourceManager.GetWood();
        int currentStone = ResourceManager.GetStone();
        int currentIron = ResourceManager.GetIron();
        int currentPower = ResourceManager.GetPower();

        // 1. تحديث نص الخشب
        if (currentWood != lastWoodAmount)
        {
            lastWoodAmount = currentWood;
            if (woodText != null) woodText.text = currentWood.ToString();
        }

        // 2. تحديث نص الحجر
        if (currentStone != lastStoneAmount)
        {
            lastStoneAmount = currentStone;
            if (stoneText != null) stoneText.text = currentStone.ToString();
        }

        // 3. تحديث نص الحديد
        if (currentIron != lastIronAmount)
        {
            lastIronAmount = currentIron;
            if (ironText != null) ironText.text = currentIron.ToString();
        }

        // 4. تحديث نص الطاقة
        if (currentPower != lastPowerAmount)
        {
            lastPowerAmount = currentPower;
            if (powerText != null) powerText.text = currentPower.ToString();
        }
    }
}