using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeWorirRecqure : MonoBehaviour
{
    public static TimeWorirRecqure Instance;

    void Awake()
    {
        Instance = this;
    }


    [Header("TimerReq")]
    public TMP_Text bedoiunText;
    public TMP_Text scalingEgyptianText;
    public TMP_Text destroyerText;
    public TMP_Text flyingOttomanText;
    public TMP_Text gaulText;
    public TMP_Text helaerText;
    public TMP_Text kamikazeText;
    public TMP_Text knightText;
    public TMP_Text mongolianText;
    public TMP_Text ottmanText;
    public TMP_Text persianText;
    public TMP_Text postionText;
    public TMP_Text sapperText;
    public TMP_Text savageText;
    public TMP_Text sneakyText;

    [Header("LEVELS")]
    public TMP_Text bedoiunTextlvl;
    public TMP_Text scalingEgyptianTextlvl;
    public TMP_Text destroyerTextlvl;
    public TMP_Text flyingOttomanTextlvl;
    public TMP_Text gaulTextlvl;
    public TMP_Text helaerTextlvl;
    public TMP_Text kamikazeTextlvl;
    public TMP_Text knightTextlvl;
    public TMP_Text mongolianTextlvl;
    public TMP_Text ottmanTextlvl;
    public TMP_Text persianTextlvl;
    public TMP_Text postionTextlvl;
    public TMP_Text sapperTextlvl;
    public TMP_Text savageTextlvl;
    public TMP_Text sneakyTextlvl;

    [Header("Iron Display")]
    public TMP_Text ironText;         // ← يعرض الرقم الكامل مثل 12500
    public TMP_Text ironShortText;   // ← يعرض الرقم المختصر مثل 12.5k
    public TMP_Text MsxStroge;
    public Image ironBarImg;

    float currentAmount;
    float maxCapacity;

    void Start()
    {
        MsxStroge.text = ResourceManager.Instance.GetTotalCapacity().ToString();

        currentAmount = ResourceManager.Instance.iron;
        maxCapacity = ResourceManager.Instance.GetTotalCapacity();
        ironBarImg.fillAmount = Mathf.Clamp01((float)currentAmount / maxCapacity);

        UpdateIronTexts();
        UpdateIronTexts();
        UpdateIronDisplay();
    }
    void Update()
    {
        MsxStroge.text = ResourceManager.Instance.GetTotalCapacity().ToString();

        ironBarImg.fillAmount = Mathf.Clamp01((float)currentAmount / maxCapacity);

        if (ResourceManager.Instance != null && ironText != null)
        {
            ironText.text = ResourceManager.Instance.iron.ToString();
        }
    }
    public void UpdateIronDisplay()
    {
        if (ResourceManager.Instance != null)
        {
            int ironAmount = ResourceManager.Instance.iron;

            // النص العادي
            if (ironText != null)
                ironText.text = ironAmount.ToString();

            // النص المختصر
            if (ironShortText != null)
                ironShortText.text = FormatShortNumber(ironAmount);
        }
    }
    string FormatShortNumber(int number)
    {
        if (number >= 1_000_000)
            return (number / 1_000_000f).ToString("0.#") + "M";
        else if (number >= 1_000)
            return (number / 1_000f).ToString("0.#") + "k";
        else
            return number.ToString();
    }


    void UpdateIronTexts()
    {
        SetIronText("bedoiun", bedoiunTextlvl, bedoiunText);
        SetIronText("ScalingEgyptian", scalingEgyptianTextlvl, scalingEgyptianText);
        SetIronText("Destroyer", destroyerTextlvl, destroyerText);
        SetIronText("flyingottoman", flyingOttomanTextlvl, flyingOttomanText);
        SetIronText("Battering_Gaul", gaulTextlvl, gaulText);
        SetIronText("GentleHealerFull", helaerTextlvl, helaerText);
        SetIronText("Kamikaze", kamikazeTextlvl, kamikazeText);
        SetIronText("KnightOnHorse", knightTextlvl, knightText);
        SetIronText("ChineseMongolian", mongolianTextlvl, mongolianText);
        SetIronText("Ottoman", ottmanTextlvl, ottmanText);
        SetIronText("persian", persianTextlvl, persianText);
        SetIronText("Poisonous", postionTextlvl, postionText);
        SetIronText("PersianSapper", sapperTextlvl, sapperText);
        SetIronText("EgyptianSavage", savageTextlvl, savageText);
        SetIronText("Sneaky", sneakyTextlvl, sneakyText);
    }

    void SetIronText(string warriorName, TMP_Text levelText, TMP_Text TimeText)
    {
        if (int.TryParse(levelText.text, out int level))
        {
            WarriorTring warrior = WarriorTringDatabase.GetWarriorByName(warriorName);
            if (warrior != null)
            {
                WarriorLevel data = warrior.GetLevel(level);
                if (data != null)
                {
                    TimeText.text = data.recruitTime.ToString();
                }
                else
                {
                    TimeText.text = "Not Found";
                }
            }
            else
            {
                TimeText.text = "UnKown Worriro";
            }
        }
        else
        {
            TimeText.text = "خطأ في المستوى";
        }
    }
}
