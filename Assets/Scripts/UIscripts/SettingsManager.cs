using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // إنشاء نسخة واحدة (Singleton)
    public static SettingsManager Instance;

    // 🟢 متغير لحفظ رقم الإطارات الحالي لكي نسحبه في الحفظ
    [HideInInspector] public int currentFPS = 60;

    private void Awake()
    {
        // التأكد من عدم وجود أكثر من مدير إعدادات واحد
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // الحفاظ عليه بين المشاهد
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // تعيين القيمة الحالية كقيمة افتراضية عند بدء اللعبة 
        // (سيتم تغييرها فوراً إذا كان هناك ملف حفظ)
        SetTargetFPS(currentFPS);
    }

    // دالة لربطها بزر الـ 30 إطار
    public void SetFPS30()
    {
        SetTargetFPS(30);
    }

    // دالة لربطها بزر الـ 60 إطار
    public void SetFPS60()
    {
        SetTargetFPS(60);
    }

    // الدالة الرئيسية لتغيير الـ FPS
    public void SetTargetFPS(int fps)
    {
        currentFPS = fps; // 🟢 تحديث المتغير الداخلي
        Application.targetFrameRate = fps;
    }

    // 🟢 دالة التحميل التي سيتصل بها SaveManager
    public void LoadData(int savedFPS)
    {
        // نستدعي الدالة الرئيسية لتعيين الإطارات للرقم المحفوظ
        SetTargetFPS(savedFPS);

    }
}