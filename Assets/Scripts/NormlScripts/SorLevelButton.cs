using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro; // مهم للقاموس


public class SorLevelButton : MonoBehaviour
{
    public static SorLevelButton instain;
    public Button levelUpButton; // اسحب الزر من الـ Inspector
    public GameObject panal1;

    [Header("Upgrade Bar")]
    public GameObject upgradeBarPrefab; // شريط الترقية
    public float upgradeDuration = 30f; // مدة الانتظار والترقية

    private UpgradeBarFollow activeBar;


    private Dictionary<GameObject, float> remainingTimes = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, bool> isUpgradingDict = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, UpgradeBarFollow> activeBars = new Dictionary<GameObject, UpgradeBarFollow>();

    [Header("⏱ أيقونة التسريع")]
    public GameObject speedUpIconPrefab;

    private Dictionary<GameObject, GameObject> activeSpeedUpIcons = new Dictionary<GameObject, GameObject>();

    [Header("BTNS for sor Resourec")]
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;


    private void Awake()
    {
        instain = this;
    }

    private void Start()
    {
        button1.onClick.AddListener(() => OnAnyButtonClicked(button1));
        button2.onClick.AddListener(() => OnAnyButtonClicked(button2));
        button3.onClick.AddListener(() => OnAnyButtonClicked(button3));
        button4.onClick.AddListener(() => OnAnyButtonClicked(button4));

        if (levelUpButton != null)
            levelUpButton.onClick.AddListener(OnLevelUpClick);
    }

    int BTN = 0;
    void OnAnyButtonClicked(Button clickedButton)
    {
        if (clickedButton == button1)
        {
            BTN = 1;            // شرط الزر الأول
        }
        else if (clickedButton == button2)
        {
            BTN = 2;            // شرط الزر الثاني
        }
        else if (clickedButton == button3)
        {
            BTN = 3;            // شرط الزر الثالث
        }
        else if (clickedButton == button4)
        {
            BTN = 4;            // شرط الزر الرابع
        }
    }

    private void Update()
    {
        foreach (var kvp in remainingTimes)
        {
            GameObject buildingObj = kvp.Key;
            float timeLeft = kvp.Value;

            if (timeLeft <= 300f && !activeSpeedUpIcons.ContainsKey(buildingObj) && speedUpIconPrefab != null)
            {
                GameObject icon = Instantiate(speedUpIconPrefab, buildingObj.transform);
                icon.transform.localPosition = new Vector3(0, 2, 0);
                activeSpeedUpIcons[buildingObj] = icon;
            }
        }

        // تحريك كل الأيقونات
        foreach (var kvp in activeSpeedUpIcons)
        {
            if (kvp.Value != null)
            {
                float floatY = Mathf.Sin(Time.time * 2f) * 0.2f;
                kvp.Value.transform.position = kvp.Key.transform.position + new Vector3(0, 1f + floatY, 0);
            }
        }

    }




    public void OnLevelUpClick()
    {
        bool paidWithGold = towBTNforSor.buyGoldSorDone;


        if (BuildingSelector.targetBuilding == null)
        {
            Debug.LogWarning("❌ لا يوجد مبنى محدد للترقية");
            return;
        }

        // الحصول على مستوى مركز المدينة
        GameObject townHallObj = GameObject.FindGameObjectWithTag("CenterCity");
        int townHallLevel = 0;
        if (townHallObj != null)
        {
            building townHallBuilding = townHallObj.GetComponent<building>();
            if (townHallBuilding != null)
                townHallLevel = townHallBuilding.level;
        }
        else
        {
            Debug.LogError("⚠️ لم يتم العثور على مبنى بوسم (CenterCity)!");
            return;
        }

        // جلب السكربت المناسب: building أو DefensiveBuilding
        MonoBehaviour parentBuilding = BuildingSelector.targetBuilding.GetComponent<building>() as MonoBehaviour
                                     ?? BuildingSelector.targetBuilding.GetComponent<DefensiveBuilding>() as MonoBehaviour;

        if (parentBuilding == null)
        {
            Debug.LogWarning("❌ المبنى المحدد لا يحتوي على سكربت 'building' أو 'DefensiveBuilding'");
            return;
        }

        // الحصول على sorlevl بشكل عام
        int parentSorLevel = 0;
        if (parentBuilding is building b)
            parentSorLevel = b.sorlevl;
        else if (parentBuilding is DefensiveBuilding d)
            parentSorLevel = d.sorlevl;

        // التحقق من شروط الترقية وفق مستوى مركز المدينة
        int maxLevelAllowed = 0;
        switch (townHallLevel)
        {
            case 5: maxLevelAllowed = 1; break;
            case 6: maxLevelAllowed = 2; break;
            case 7: maxLevelAllowed = 3; break;
            case 8: maxLevelAllowed = 4; break;
            case 9: maxLevelAllowed = 4; break;
            case 10: maxLevelAllowed = 4; break;
            default: maxLevelAllowed = 0; break;
        }

        if (townHallLevel < 5 || parentSorLevel >= maxLevelAllowed)
        {
            Debug.LogWarning($"❌ لا يمكنك تطوير هذا المبنى. مستوى مركز المدينة: {townHallLevel}, الحد المسموح به للمستوى الحالي: {maxLevelAllowed}");
            return;
        }

        // التحقق من توفر عامل
        if (WorkerUI.Instance.HasAvailableWorker())
        {
            WorkerUI.Instance.TryUseWorker();
            if (paidWithGold)
            {
                upgradeDuration = 0.1f;
                GoldCollector.RemoveGold(towBTNforSor.goldToRemove); // هنا ضفت الذهب الحذف 
                towBTNforSor.goldToRemove = 0;
                AudioManager.Instance.PlaySFX("Gold_buy");

            }
            else
            {
                // ================= بداية التعديل =================
                string timeText = "";

                if (BTN == 1 && TakeResFromSor.instain.upgradeTimerText != null)
                    timeText = TakeResFromSor.instain.upgradeTimerText.text;
                else if (BTN == 2 && TakeResFromSor2.instain.upgradeTimerText != null)
                    timeText = TakeResFromSor2.instain.upgradeTimerText.text;
                else if (BTN == 3 && TakeResFromSor3.instain.upgradeTimerText != null)
                    timeText = TakeResFromSor3.instain.upgradeTimerText.text;
                else if (BTN == 4 && TakeResFromSor4.instain.upgradeTimerText != null)
                    timeText = TakeResFromSor4.instain.upgradeTimerText.text;

                if (!string.IsNullOrEmpty(timeText))
                    upgradeDuration = ConvertTextToSeconds(timeText);
                else
                    upgradeDuration = 30f; // وقت افتراضي للحماية
                // ================= نهاية التعديل =================
            }
            StartCoroutine(LevelUpWithBar(parentBuilding, BuildingSelector.targetBuilding.transform.Find("Capsule").gameObject));

            if (!paidWithGold)
            {
                if (BTN == 1)
                    TakeResFromSor.instain.RemovingRes();
                else if (BTN == 2)
                    TakeResFromSor2.instain.RemovingRes();
                else if (BTN == 3)
                    TakeResFromSor3.instain.RemovingRes();
                else if (BTN == 4)
                    TakeResFromSor4.instain.RemovingRes();
            }

            towBTNforSor.ResetGoldPurchase();


        }
        else
        {
            Debug.LogWarning("❌ لا يوجد عامل متاح لتطوير المبنى!");

        }
    }


    private IEnumerator LevelUpWithBar(MonoBehaviour parentBuilding, GameObject capsuleObj)
    {
        panal1.SetActive(false);

        // إظهار Capsule
        capsuleObj.SetActive(true);

        // 🔴 تعطيل الكوليدرات الخاصة بالمبنى الأصلي فقط
        Collider2D[] mainColliders = parentBuilding.GetComponentsInChildren<Collider2D>(true);

        // مر على الكوليدرات واحد واحد
        foreach (Collider2D col in mainColliders)
        {
            // ✅ تجاهل أي كوليدر موجود تحت الابن Capsule
            // ✅ فعل الكوليدر إذا كان على Capsule
            if (col.transform.name == "Capsule" || col.transform.IsChildOf(parentBuilding.transform.Find("Capsule")))
            {
                col.enabled = true;
            }
            else
            {
                // ❌ عطل أي كوليدر آخر
                col.enabled = false;
            }
        }

        // 🔴 تعطيل سكربت Gold على المبنى الأصلي
        BuildingGoldStorage goldStorageScript = parentBuilding.GetComponentInChildren<BuildingGoldStorage>(true);
        if (goldStorageScript != null)
            goldStorageScript.enabled = false;

        // إنشاء شريط الترقية

        GameObject canvasObject = GameObject.FindWithTag("MainCanvas");
        if (canvasObject != null && upgradeBarPrefab != null)
        {
            GameObject bar = Instantiate(upgradeBarPrefab, canvasObject.transform);
            bar.transform.SetAsFirstSibling();
            UpgradeBarFollow barScript = bar.GetComponent<UpgradeBarFollow>();
            if (barScript != null)
            {
                barScript.StartUpgradeBar(capsuleObj.transform, upgradeDuration);
                activeBars[capsuleObj] = barScript;
            }
        }

        remainingTimes[capsuleObj] = upgradeDuration;
        isUpgradingDict[capsuleObj] = true;

        while (remainingTimes[capsuleObj] > 0f)
        {
            remainingTimes[capsuleObj] -= Time.deltaTime;
            yield return null;
        }

        // الترقية انتهت
        if (parentBuilding is building b)
            b.IncreaseSorLevel();
        else if (parentBuilding is DefensiveBuilding d)
            d.IncreaseSorLevel();

        // إعادة العامل
        WorkerUI.Instance.ReturnWorker();

        // ✅ إعادة تفعيل الكوليدرات
        foreach (Collider2D col in mainColliders)
            col.enabled = true;

        // ✅ إعادة تفعيل سكربت Gold
        if (goldStorageScript != null)
            goldStorageScript.enabled = true;

        // حذف شريط الترقية
        if (activeBars.TryGetValue(capsuleObj, out UpgradeBarFollow barToDestroy))
        {
            Destroy(barToDestroy.gameObject);
            activeBars.Remove(capsuleObj);
        }
        if (activeSpeedUpIcons.TryGetValue(capsuleObj, out GameObject icon))
        {
            Destroy(icon);
            activeSpeedUpIcons.Remove(capsuleObj);
        }



        capsuleObj.SetActive(false);
        isUpgradingDict[capsuleObj] = false;
        remainingTimes.Remove(capsuleObj);
    }


    public TMP_Text GOLDtEXT; // اربطها من الـ Inspector
    int ExtractNumberFromText(string text)
    {
        string digitsOnly = "";
        foreach (char c in text)
        {
            if (char.IsDigit(c))
                digitsOnly += c;
        }

        if (string.IsNullOrEmpty(digitsOnly))
            return 0;

        return int.Parse(digitsOnly);
    }
    //public GameObject closeOnClickPanal;
    // دالة لتقصير الوقت المتبقي لأي مبنى إلى ثانية واحدة
    public void SorSpeed()
    {
        int requiredGold = ExtractNumberFromText(GOLDtEXT.text);
        Debug.Log("THE GOLD IS " + requiredGold);

        // التحقق من وجود مبنى محدد في BuildingClickChecker
        BuildLvl selectedBuild = BuildingClickChecker.instaint?.SelectedBuildLvl;
        if (selectedBuild == null)
        {
            Debug.LogWarning("❌ لا يوجد مبنى محدد في BuildingClickChecker!");
            return;
        }

        // الحصول على Capsule الخاص بالمبنى
        Transform capsuleChild = selectedBuild.transform.Find("Capsule");
        if (capsuleChild == null)
        {
            Debug.LogError("❌ لم يتم العثور على Capsule!");
            return;
        }

        GameObject buildingObj = capsuleChild.gameObject;

        // التحقق إن كان المبنى في قاموس الوقت
        if (remainingTimes.ContainsKey(buildingObj))
        {
            GoldCollector.RemoveGold(requiredGold);
            remainingTimes[buildingObj] = 0.1f; // ضبط الوقت على ثانية واحدة
            Debug.Log("⏱️ تم ضبط الوقت المتبقي للترقية إلى 1 ثانية");

            // فقط المبنى المحدد سيؤثر عليه شريط الترقية
            if (activeBars.TryGetValue(buildingObj, out UpgradeBarFollow bar))
            {
                if (bar != null)
                {
                    bar.ForceCompleteUpgrade(); // تأكد أن هذه الدالة لا تدمر أي شريط آخر
                    if (activeSpeedUpIcons.TryGetValue(buildingObj, out GameObject icon))
                    {
                        Destroy(icon);
                        activeSpeedUpIcons.Remove(buildingObj);
                    }                         // بعد اكتمال البار، يمكن إزالة من القاموس
                    activeBars.Remove(buildingObj);



                }
            }
        }
        else
        {
            Debug.LogWarning("⚠️ المبنى الحالي ليس في قائمة الترقية الحالية");
        }
    }

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


}
