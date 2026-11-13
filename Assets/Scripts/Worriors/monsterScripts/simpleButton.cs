using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleButton : MonoBehaviour
{
    public static SimpleButton Instance; // 🟢 للوصول إلى هذا السكربت من مدير الحفظ

    public Button y3ssobBTN;
    public Button bearBTN;
    public Button elefentBTN;
    public Button womkongBTN;

    public GameObject y3ssobImage;
    public GameObject bearImage;
    public GameObject elefentImage;
    public GameObject womkongImage;

    public TMP_Text y3ssobText;
    public TMP_Text bearText;
    public TMP_Text elefentText;
    public TMP_Text womkongText;

    private string y3ssobDefault = "UNLEASH";
    private string bearDefault = "UNLEASH";
    private string elefentDefault = "UNLEASH";
    private string womkongDefault = "UNLEASH";

    private Color normalColor = Color.white;
    private Color selectedColor = Color.green;

    // 🟢 متغير لحفظ اسم البطل المفعل حالياً (افتراضياً None)
    public string currentActiveHero = "None";

    void Awake()
    {
        Instance = this; // 🟢 تهيئة النسخة
    }

    void Start()
    {
        y3ssobBTN.onClick.AddListener(OnButtonClicked);
        bearBTN.onClick.AddListener(OnButtonClicked2);
        elefentBTN.onClick.AddListener(OnButtonClicked3);
        womkongBTN.onClick.AddListener(OnButtonClicked4);
    }

    // 🟢 قمت بتغييرها إلى public لكي نستطيع استخدامها داخل دالة التحميل
    public void ResetButtons()
    {
        ResetButton(y3ssobBTN, y3ssobText, y3ssobDefault);
        ResetButton(bearBTN, bearText, bearDefault);
        ResetButton(elefentBTN, elefentText, elefentDefault);
        ResetButton(womkongBTN, womkongText, womkongDefault);

        y3ssobImage.SetActive(true);
        bearImage.SetActive(true);
        elefentImage.SetActive(true);
        womkongImage.SetActive(true);

        currentActiveHero = "None"; // 🟢 تصفير البطل الحالي
    }

    void ResetButton(Button btn, TMP_Text textField, string originalText)
    {
        btn.GetComponent<Image>().color = normalColor;
        textField.text = originalText;
    }

    void SetButtonUsed(Button btn, TMP_Text textField)
    {
        btn.GetComponent<Image>().color = selectedColor;
        textField.text = "Used";
    }

    void OnButtonClicked()
    {
        if (y3ssobText.text == "Used")
        {
            ResetButtons();
            HeroArcher2.isActiveHero = false;
            return;
        }

        ResetButtons();
        SetButtonUsed(y3ssobBTN, y3ssobText);
        y3ssobImage.SetActive(false);

        HeroArcher2.isActiveHero = true;
        HeroUnit2.isActiveHero = false;
        HeroArcher.isActiveHero = false;
        HeroUnit.isActiveHero = false;

        currentActiveHero = "y3ssob"; // 🟢 تسجيل البطل المختار
        AudioManager.Instance.PlaySFX("y3sb_SPN2");
    }

    void OnButtonClicked2()
    {
        if (bearText.text == "Used")
        {
            ResetButtons();
            HeroUnit.isActiveHero = false;
            return;
        }

        ResetButtons();
        SetButtonUsed(bearBTN, bearText);
        bearImage.SetActive(false);

        HeroUnit.isActiveHero = true;
        HeroArcher.isActiveHero = false;
        HeroUnit2.isActiveHero = false;
        HeroArcher2.isActiveHero = false;

        currentActiveHero = "bear"; // 🟢 تسجيل البطل المختار
        AudioManager.Instance.PlaySFX("monster_SPN");
    }

    void OnButtonClicked3()
    {
        if (elefentText.text == "Used")
        {
            ResetButtons();
            HeroUnit2.isActiveHero = false;
            return;
        }

        ResetButtons();
        SetButtonUsed(elefentBTN, elefentText);
        elefentImage.SetActive(false);

        HeroUnit2.isActiveHero = true;
        HeroUnit.isActiveHero = false;
        HeroArcher.isActiveHero = false;
        HeroArcher2.isActiveHero = false;

        currentActiveHero = "elefent"; // 🟢 تسجيل البطل المختار
        AudioManager.Instance.PlaySFX("monster_SPN");
    }

    void OnButtonClicked4()
    {
        if (womkongText.text == "Used")
        {
            ResetButtons();
            HeroArcher.isActiveHero = false;
            return;
        }

        ResetButtons();
        SetButtonUsed(womkongBTN, womkongText);
        womkongImage.SetActive(false);

        HeroArcher.isActiveHero = true;
        HeroUnit.isActiveHero = false;
        HeroArcher2.isActiveHero = false;
        HeroUnit2.isActiveHero = false;

        currentActiveHero = "womkong"; // 🟢 تسجيل البطل المختار
        AudioManager.Instance.PlaySFX("monster_SPN");
    }

    // 🟢 دالة جديدة: يتم استدعاؤها من SaveManager عند فتح اللعبة
    public void LoadSavedHero(string savedHero)
    {
        ResetButtons(); // تصفير كل شيء أولاً

        // تعطيل كل الأبطال لضمان عدم وجود تداخل
        HeroArcher2.isActiveHero = false;
        HeroUnit.isActiveHero = false;
        HeroUnit2.isActiveHero = false;
        HeroArcher.isActiveHero = false;

        // تفعيل البطل بناءً على الاسم المحفوظ
        if (savedHero == "y3ssob")
        {
            SetButtonUsed(y3ssobBTN, y3ssobText);
            y3ssobImage.SetActive(false);
            HeroArcher2.isActiveHero = true;
            currentActiveHero = "y3ssob";
        }
        else if (savedHero == "bear")
        {
            SetButtonUsed(bearBTN, bearText);
            bearImage.SetActive(false);
            HeroUnit.isActiveHero = true;
            currentActiveHero = "bear";
        }
        else if (savedHero == "elefent")
        {
            SetButtonUsed(elefentBTN, elefentText);
            elefentImage.SetActive(false);
            HeroUnit2.isActiveHero = true;
            currentActiveHero = "elefent";
        }
        else if (savedHero == "womkong")
        {
            SetButtonUsed(womkongBTN, womkongText);
            womkongImage.SetActive(false);
            HeroArcher.isActiveHero = true;
            currentActiveHero = "womkong";
        }
    }
}