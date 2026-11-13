using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WarPointsManager : MonoBehaviour
{
    public static WarPointsManager Instance;

    public TMP_Text warPointsText;
    public Image levelImage;
    public GameObject congratulationsPanel;

    public Sprite level1Sprite;
    public Sprite level2Sprite;
    public Sprite level3Sprite;
    public Sprite level4Sprite;
    public Sprite level5Sprite;
    public Sprite level6Sprite;

    // تم إزالة private لتتمكن باقي السكربتات من قراءتها بسهولة
    public static int warPoints = 0;
    public static int lastLevelReached = 0;

    void Awake()
    {
        Instance = this;
        // تم إزالة PlayerPrefs من هنا.. التحميل سيتم عبر SaveManager
    }

    // 🟢 دالة جديدة لاستقبال البيانات من ملف الحفظ (JSON)
    public void LoadData(int savedPoints, int savedLevel)
    {
        warPoints = savedPoints;
        lastLevelReached = savedLevel;
        UpdateUI();
    }

    // ✅ تحديث النقاط في واجهة المستخدم وتغيير الصورة
    private void UpdateUI()
    {
        if (warPointsText != null) warPointsText.text = warPoints.ToString();
        UpdateLevelSprite();
    }

    // ✅ دالة تغيير الصورة بناءً على النقاط
    private void UpdateLevelSprite()
    {
        if (warPoints >= 3200 && lastLevelReached < 6)
            SetLevel(level6Sprite, 6);
        else if (warPoints >= 2600 && lastLevelReached < 5)
            SetLevel(level5Sprite, 5);
        else if (warPoints >= 2000 && lastLevelReached < 4)
            SetLevel(level4Sprite, 4);
        else if (warPoints >= 1400 && lastLevelReached < 3)
            SetLevel(level3Sprite, 3);
        else if (warPoints >= 800 && lastLevelReached < 2)
            SetLevel(level2Sprite, 2);
        else if (warPoints >= 400 && lastLevelReached < 1)
            SetLevel(level1Sprite, 1);
    }

    // ✅ دالة تعيين المستوى وتحديث الصورة
    private void SetLevel(Sprite newSprite, int level)
    {
        if (levelImage != null && newSprite != null)
        {
            levelImage.sprite = newSprite;
        }

        lastLevelReached = level;
        // تم إزالة PlayerPrefs

        // إظهار بنل التهنئة
        if (congratulationsPanel != null)
        {
            congratulationsPanel.SetActive(true);
        }
    }

    // ✅ دالة لإضافة النقاط
    public static void AddWarPoints(int amount)
    {
        warPoints += amount;
        // تم إزالة PlayerPrefs

        if (Instance != null)
        {
            Instance.UpdateUI();
        }
    }

    // ✅ دالة لإنقاص النقاط
    public static void RemoveWarPoints(int amount)
    {
        warPoints -= amount;
        if (warPoints < 0) warPoints = 0;

        // تم إزالة PlayerPrefs

        if (Instance != null)
        {
            Instance.UpdateUI();
        }
    }

    // ✅ دالة للحصول على عدد النقاط الحالي
    public static int GetWarPoints()
    {
        return warPoints;
    }

    // 🟢 دالة إضافية للحصول على المستوى (مفيدة للحفظ)
    public static int GetLastLevelReached()
    {
        return lastLevelReached;
    }

    public static void ResetWarPoints()
    {
        warPoints = 0;
        lastLevelReached = 0; // من الأفضل تصفير المستوى أيضاً عند التصفير
        // تم إزالة PlayerPrefs

        if (Instance != null)
        {
            Instance.UpdateUI();
        }
    }
}