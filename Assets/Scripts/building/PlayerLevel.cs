using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevel : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;
    public int baseXP = 100;
    public float xpIncreasePercent = 10f;

    private int xpToLevelUp;

    public Image xpBarFill;
    public TextMeshProUGUI levelText;

    // ✅ Panel التهنئة
    public GameObject levelUpPanel;

    // ✅ Text لعرض المستوى داخل البانل
    public TextMeshProUGUI levelUpPanelText;

    public static PlayerLevel instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 🟢 إذا لم يكن هناك قيم (لعبة جديدة)، نضبط القيم الافتراضية
        if (xpToLevelUp == 0)
        {
            xpToLevelUp = baseXP;
        }
        UpdateUI();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= xpToLevelUp)
        {
            currentXP -= xpToLevelUp;
            level++;

            // زيادة النقاط المطلوبة للمستوى القادم
            xpToLevelUp = Mathf.CeilToInt(xpToLevelUp * (1f + xpIncreasePercent / 100f));

            // ✅ عرض Panel التهنئة
            ShowLevelUpPanel();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (xpBarFill != null)
            xpBarFill.fillAmount = (float)currentXP / xpToLevelUp;

        if (levelText != null)
            levelText.text = level.ToString();
    }

    void ShowLevelUpPanel()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
            AudioManager.Instance.PlaySFX("levelUp");

            // ✅ تحديث النص داخل البانل
            if (levelUpPanelText != null)
                levelUpPanelText.text = level.ToString();
        }
    }

    // ✅ يمكن استدعاؤها من زر الإغلاق داخل البانل
    public void HideLevelUpPanel()
    {
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }
    // 🟢 دالة تجهيز البيانات للحفظ
    public PlayerLevelSaveData GetLevelData()
    {
        return new PlayerLevelSaveData
        {
            level = this.level,
            currentXP = this.currentXP,
            baseXP = this.baseXP,
            xpToLevelUp = this.xpToLevelUp
        };
    }

    // 🟢 دالة تحميل البيانات
    public void LoadLevelData(PlayerLevelSaveData data)
    {
        if (data == null || data.level == 0) return; // حماية في حال كان ملف الحفظ فارغاً

        this.level = data.level;
        this.currentXP = data.currentXP;
        this.baseXP = data.baseXP;
        this.xpToLevelUp = data.xpToLevelUp;

        // تحديث الواجهة فوراً
        UpdateUI();
    }
}
