using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildPanelChecker : MonoBehaviour
{
    [System.Serializable]
    public class BuildOption
    {
        public Button buildButton;

        [Header("الموارد المطلوبة لهذا البناء")]
        public TMP_Text requiredWoodText;
        public TMP_Text requiredStoneText;
        public TMP_Text requiredPowerText;
        public TMP_Text requiredIronText;
    }

    [Header("قائمة جميع أزرار البناء والمتطلبات الخاصة بها")]
    public List<BuildOption> buildOptions = new List<BuildOption>();

    [Header("نصوص الموارد الحالية (من ResourceManager)")]
    public TMP_Text woodStorageText;
    public TMP_Text stoneStorageText;
    public TMP_Text powerStorageText;
    public TMP_Text ironStorageText;

    void Update()
    {
        foreach (var option in buildOptions)
        {
            CheckResources(option);
        }
    }

    void CheckResources(BuildOption option)
    {
        Color okColor = Color.white;
        Color failColor = Color.red;
        Color fadedColor = new Color(1f, 1f, 1f, 0.5f);

        // 1. محاولة استخراج الأرقام بأمان باستخدام int.TryParse
        bool isWoodValid = int.TryParse(option.requiredWoodText.text, out int requiredWood);
        bool isStoneValid = int.TryParse(option.requiredStoneText.text, out int requiredStone);
        bool isPowerValid = int.TryParse(option.requiredPowerText.text, out int requiredPower);
        bool isIronValid = int.TryParse(option.requiredIronText.text, out int requiredIron);

        // إذا كان أي نص يحتوي على "-" أو نص غير رقمي (مما يعني أن المبنى وصل للحد الأقصى)
        if (!isWoodValid || !isStoneValid || !isPowerValid || !isIronValid)
        {
            option.buildButton.interactable = false; // تعطيل زر البناء/الترقية
            return; // الخروج من الدالة لمنع أي حسابات إضافية
        }

        // 2. استخراج الموارد الحالية بأمان أيضاً (لتجنب أي أخطاء مشابهة مستقبلاً)
        int.TryParse(woodStorageText.text, out int currentWood);
        int.TryParse(stoneStorageText.text, out int currentStone);
        int.TryParse(powerStorageText.text, out int currentPower);
        int.TryParse(ironStorageText.text, out int currentIron);

        bool canBuild = true;

        // خشب
        if (requiredWood == 0)
        {
            option.requiredWoodText.color = fadedColor;
        }
        else if (currentWood < requiredWood)
        {
            option.requiredWoodText.color = failColor;
            canBuild = false;
        }
        else
        {
            option.requiredWoodText.color = okColor;
        }

        // حجر
        if (requiredStone == 0)
        {
            option.requiredStoneText.color = fadedColor;
        }
        else if (currentStone < requiredStone)
        {
            option.requiredStoneText.color = failColor;
            canBuild = false;
        }
        else
        {
            option.requiredStoneText.color = okColor;
        }

        // قوة
        if (requiredPower == 0)
        {
            option.requiredPowerText.color = fadedColor;
        }
        else if (currentPower < requiredPower)
        {
            option.requiredPowerText.color = failColor;
            canBuild = false;
        }
        else
        {
            option.requiredPowerText.color = okColor;
        }

        // حديد
        if (requiredIron == 0)
        {
            option.requiredIronText.color = fadedColor;
        }
        else if (currentIron < requiredIron)
        {
            option.requiredIronText.color = failColor;
            canBuild = false;
        }
        else
        {
            option.requiredIronText.color = okColor;
        }

        // تفعيل أو تعطيل زر البناء لهذا الخيار بناءً على الموارد
        option.buildButton.interactable = canBuild;
    }
}