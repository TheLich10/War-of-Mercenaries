using UnityEngine;
using UnityEngine.UI;

public class SwitchFPSbtn : MonoBehaviour
{
    // 🟢 إضافة نسخة لتسهيل الوصول إليها من SaveManager
    public static SwitchFPSbtn Instance;

    [Header("صور الأزرار (Image Components)")]
    public Image button1Image; // (مثلاً: زر الـ 30 إطار)
    public Image button2Image; // (مثلاً: زر الـ 60 إطار)

    [Header("الصور المراد التبديل بينها (Sprites)")]
    public Sprite defaultSprite; // الصورة الافتراضية للزر غير المفعل
    public Sprite activeSprite;  // الصورة للزر المفعل

    private void Awake()
    {
        Instance = this;
    }

    // 🟢 تتفعل تلقائياً كلما فتحت نافذة الإعدادات
    private void OnEnable()
    {
        RefreshUI();
    }

    // 🟢 دالة مخصصة لتحديث شكل الأزرار بناءً على رقم الـ FPS المحفوظ
    public void RefreshUI()
    {
        if (SettingsManager.Instance != null)
        {
            // نفترض أن الزر الأول يمثل 30 والزر الثاني يمثل 60
            if (SettingsManager.Instance.currentFPS == 30)
            {
                OnButton1Clicked();
            }
            else if (SettingsManager.Instance.currentFPS == 60)
            {
                OnButton2Clicked();
            }
        }
    }

    // اربط هذه الدالة بحدث OnClick الخاص بالزر الأول
    public void OnButton1Clicked()
    {
        if (button2Image != null) button2Image.sprite = defaultSprite;
        if (button1Image != null) button1Image.sprite = activeSprite;
    }

    // اربط هذه الدالة بحدث OnClick الخاص بالزر الثاني
    public void OnButton2Clicked()
    {
        if (button1Image != null) button1Image.sprite = defaultSprite;
        if (button2Image != null) button2Image.sprite = activeSprite;
    }
}