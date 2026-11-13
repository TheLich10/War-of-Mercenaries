using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoldPriceFromTime : MonoBehaviour
{
    [Header("نص الوقت")]
    public TMP_Text timeText;

    [Header("زر الذهب")]
    public Button goldButton;
    public TMP_Text goldPriceText;

    private int goldPrice;

    private void Update()
    {
        UpdateGoldPrice();
    }

    void UpdateGoldPrice()
    {
        int totalMinutes = 0;
        bool isValidTime = false;

        // تقسيم النص بناءً على النقطتين ":"
        string[] timeParts = timeText.text.Split(':');

        // إذا كانت الصيغة: ساعات:دقائق:ثواني (مثلاً 48:00:00)
        if (timeParts.Length == 3)
        {
            if (int.TryParse(timeParts[0], out int hours) &&
                int.TryParse(timeParts[1], out int minutes))
            {
                totalMinutes = (hours * 60) + minutes;
                isValidTime = true;
            }
        }
        // إذا كانت الصيغة: أيام:ساعات:دقائق:ثواني (مثلاً 02:00:00:00)
        else if (timeParts.Length == 4)
        {
            if (int.TryParse(timeParts[0], out int days) &&
                int.TryParse(timeParts[1], out int hours) &&
                int.TryParse(timeParts[2], out int minutes))
            {
                totalMinutes = (days * 24 * 60) + (hours * 60) + minutes;
                isValidTime = true;
            }
        }

        // إذا نجحت عملية استخراج الدقائق
        if (isValidTime)
        {
            if (totalMinutes < 5)
            {
                goldPrice = 0;
            }
            else
            {
                // (ملاحظة: تعليقك السابق يقول 3 دقائق ولكن الحسبة كانت تقسم على 5)
                goldPrice = totalMinutes / 8;
            }
        }
        else
        {
            // في حال كان النص فارغاً أو بصيغة غير معروفة
            goldPrice = 0;
        }

        // تحديث النص داخل البوتن
        goldPriceText.text = "FINISH NOW! " + goldPrice.ToString();
    }

    public int GetGoldPrice()
    {
        return goldPrice;
    }
}