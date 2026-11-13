using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class YesNobTns : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;

    public TextMeshProUGUI timeTextTMP;
    private float savedBuildTime = 0f; // المتغير الذي سيحفظ الوقت

    public static int result = 0;

    private void OnEnable()
    {
        // نتأكد أن النص موجود لتجنب أي أخطاء
        if (timeTextTMP != null)
        {
            savedBuildTime = ConvertTextToSeconds(timeTextTMP.text);
        }
    }
    private void Start()
    {
        yesButton.onClick.AddListener(PlaceBuildingNow);
        noButton.onClick.AddListener(CancelBuilding);
    }

    void PlaceBuildingNow()
    {
        result = 1;

        if (cheakBuildBuyGold.isGoldBuild == true)
        {
            GridBuildingSystem.current.TryPlaceBuilding(0.1f); // وقت البناء بالذهب
            //Debug.Log("is Gold build its True ");
        }
        else
        {
            // نستخدم القيمة التي تم حفظها عند ظهور اللوحة
            GridBuildingSystem.current.TryPlaceBuilding(savedBuildTime);
            //Debug.Log("is Not Gold build its False ");
            //Debug.Log("the time is want is "+savedBuildTime);
        }

        cheakBuildBuyGold.isGoldBuild = false;
        result = 0;

        // تصفير القيمة بعد إتمام البناء كما طلبت
        savedBuildTime = 0f;
    }

    // --- الدالة الجديدة لتحويل النص إلى ثوانٍ ---
    float ConvertTextToSeconds(string timeString)
    {
        // تنظيف النص من المسافات العادية، ومن الحرف المخفي الخاص بـ TextMeshPro
        timeString = timeString.Replace("\u200B", "").Replace(" ", "").Trim().ToUpper();

        if (string.IsNullOrEmpty(timeString)) return 10f; // قيمة افتراضية لحماية الكود

        // جلب آخر حرف لمعرفة الوحدة الزمنية (S, M, H, D)
        char timeUnit = timeString[timeString.Length - 1];

        // فصل الرقم عن الحرف
        string numberString = timeString.Substring(0, timeString.Length - 1);

        // التأكد من أن الجزء الأول هو رقم صحيح
        if (float.TryParse(numberString, out float timeValue))
        {
            switch (timeUnit)
            {
                case 'S': return timeValue;               // ثواني
                case 'M': return timeValue * 60f;         // دقائق
                case 'H': return timeValue * 3600f;       // ساعات
                case 'D': return timeValue * 86400f;      // أيام
                default:
                    // في حال كان النص رقماً فقط بدون حروف، نتعامل معه كثواني
                    if (float.TryParse(timeString, out float rawValue)) return rawValue;
                    return 300f;
            }
        }
        return 300f; // في حال فشل التحويل لأي سبب، نعيد 10 ثواني كإجراء آمن
    }

    void CancelBuilding()
    {
        result = 2;
            GridBuildingSystem.current.CancelBuilding();
        
        result = 0;
    }
}
