using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WarProgressBar : MonoBehaviour
{
    public static WarProgressBar instance;

    public Image warBarImage;

    public GameObject starEffect;
    public GameObject image30;
    public GameObject image50;
    public GameObject image90;

    private List<building> allBuildings = new List<building>();
    private List<DefensiveBuilding> allDefensiveBuildings = new List<DefensiveBuilding>();

    private float totalMaxHealth = 0f;

    private bool reached30 = false;
    private bool reached50 = false;
    private bool reached90 = false;

    public static int WarPoints = 0;

    public TMP_Text pointsText;
    public TMP_Text currentPointText;

    private int MaxPoint = 30;
    private int currentPointOffset = -10;

    public static int ThepintWin = 0;

    private bool hasFinishedWar = false;
    public string sceneName = "SampleScene";

    private void Awake()
    {
        instance = this;
    }

    // ✅ تحويل دالة Start إلى IEnumerator لتأخير التنفيذ قليلاً
    private IEnumerator Start()
    {
        SaveManager.Instance.LoadAudioAndFPSOnly();
        WarPoints = 0;
        ThepintWin = 0;

        if (GridAttckC.current != null)
            GridAttckC.current.RestALLanim();

        // ⏱️ الانتظار لجزء من الثانية لضمان تشغيل جميع المباني لدوال Start الخاصة بها أولاً
        yield return new WaitForSeconds(0.1f);

        // ✅ البحث عن كل التاغات التي تحتوي على سكربت building
        string[] buildingTags = { "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "spy", "spyClan", "Barracks", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "mongel" };

        foreach (string tag in buildingTags)
        {
            GameObject[] buildingsWithTag = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject go in buildingsWithTag)
            {
                building b = go.GetComponent<building>();
                if (b != null)
                {
                    allBuildings.Add(b);
                    totalMaxHealth += b.maxHealth;

                    // ✅ إذا كان المبنى هو CenterCity نطبق الشروط الخاصة بالمستوى
                    if (tag == "CenterCity")
                    {
                        int level = b.level;
                        if (level >= 1 && level <= 5)
                        {
                            MaxPoint = 30;
                            currentPointOffset = -10;
                        }
                        else if (level == 6 || level == 7)
                        {
                            MaxPoint = 35;
                            currentPointOffset = -13;
                        }
                        else if (level >= 8 && level <= 10)
                        {
                            MaxPoint = 40;
                            currentPointOffset = -17;
                        }
                    }
                }
            }
        }

        // ✅ البحث عن كل التاغات التي تحتوي على سكربت DefensiveBuilding
        string[] defensiveTags = { "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower", "boomOP" };
        foreach (string tag in defensiveTags)
        {
            GameObject[] defensiveWithTag = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject go in defensiveWithTag)
            {
                DefensiveBuilding db = go.GetComponent<DefensiveBuilding>();
                if (db != null)
                {
                    allDefensiveBuildings.Add(db);
                    totalMaxHealth += db.maxHealth;
                }
            }
        }

        image30?.SetActive(false);
        image50?.SetActive(false);
        image90?.SetActive(false);
        starEffect?.SetActive(false);
    }

    private void Update()
    {
        float currentHealth = 0f;

        foreach (building b in allBuildings)
        {
            if (b != null)
                currentHealth += b.health;
        }

        // ✅ حساب الصحة الحالية للمباني الدفاعية
        foreach (DefensiveBuilding db in allDefensiveBuildings)
        {
            if (db != null)
                currentHealth += db.health;
        }

        float damageDone = totalMaxHealth - currentHealth;

        if (warBarImage != null && totalMaxHealth > 0)
        {
            float fillAmount = damageDone / totalMaxHealth;
            warBarImage.fillAmount = fillAmount;

            // هذه المسافة بين بداية currentPoint والقيمة القصوى
            int range = MaxPoint - currentPointOffset;

            // نحدد قيمة currentPoint بناءً على عدد النقاط (WarPoints)
            int currentPoint = currentPointOffset; // افتراضي البداية
            ThepintWin = currentPoint;

            if (WarPoints == 1)
            {
                currentPoint = currentPointOffset + Mathf.RoundToInt(range * 1f / 3f);
                ThepintWin = currentPoint;
            }
            else if (WarPoints == 2)
            {
                currentPoint = currentPointOffset + Mathf.RoundToInt(range * 2f / 3f);
                ThepintWin = currentPoint;
            }
            else if (WarPoints >= 3)
            {
                currentPoint = MaxPoint; // الوصول للحد الأقصى
                ThepintWin = currentPoint;
            }

            // تحديث النصوص
            if (pointsText != null)
            {
                pointsText.text = MaxPoint.ToString();
            }

            if (currentPointText != null)
            {
                currentPointText.text = currentPoint.ToString();
            }

            // تحديث حالة النقاط حسب النسب فقط إذا لم تتحقق من قبل 
            if (fillAmount >= 0.2f && !reached30)
            {
                reached30 = true;
                image30?.SetActive(true);
                StartCoroutine(ShowStar());
                WarPoints = 1;
            }

            if (fillAmount >= 0.55f && !reached50)
            {
                reached50 = true;
                image50?.SetActive(true);
                StartCoroutine(ShowStar());
                WarPoints = 2;
            }

            if (fillAmount >= 0.85f && !reached90)
            {
                reached90 = true;
                image90?.SetActive(true);
                StartCoroutine(ShowStar());
                WarPoints = 3;
            }

            // الشروط لإنهاء المعركة عند اكتمال البار أو تدمير كل المباني
            if ((fillAmount >= 0.99f || currentHealth <= 0) && !hasFinishedWar)
            {
                hasFinishedWar = true; // منع تكرار التحميل
                AudioManager.Instance.PlaySFX("happy");
                StartCoroutine(LoadEndScene());
            }
        }
    }

    private IEnumerator LoadEndScene()
    {
        // الانتظار لمدة ثانيتين
        yield return new WaitForSeconds(2f);

        // تنفيذ كود تحميل المشهد الخاص بك
        if (!string.IsNullOrEmpty(sceneName))
        {
            EndLoader.isWar = true;
            if (WarPoints == 3)
            {
                EndLoader.WinOrLost = true;
                EndLoader.point = 3;
            }
            spyButton.spyButtonState = false;
            SaveManager.Instance.SaveResourcesAndBarracksOnly();
            GlobalSceneManager.LoadSceneViaLoading(sceneName);
        }
        else
        {
            Debug.LogError("⚠️ لم يتم تحديد اسم المشهد! تأكد من إدخال الاسم في الـ Inspector.");
        }
    }

    private IEnumerator ShowStar()
    {
        if (starEffect != null)
        {
            starEffect.SetActive(true);
            AudioManager.Instance.PlaySFX("win_point");
            yield return new WaitForSeconds(2f);
            starEffect.SetActive(false);
        }
    }
}