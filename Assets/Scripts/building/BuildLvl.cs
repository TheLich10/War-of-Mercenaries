using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildLvl : MonoBehaviour
{
    public GameObject upgradeBarPrefab;
    public List<BuildingLevel> levels;
    private int currentLevel = 0;
    private GameObject currentModel;
    private WorkerUI workerUI;
    public float upgradeDuration = 5f;
    private Coroutine currentUpgradeCoroutine;
    private UpgradeBarFollow activeBar;

    // ⏱ أيقونة التسريع
    [Header("⏱ أيقونة التسريع")]
    public GameObject speedUpIconPrefab;
    private GameObject activeSpeedUpIcon;
    private Vector3 speedUpIconStartPos; // موضع البداية للأيقونة
    private bool speedUpIconActive = false;

    //////////////////////////////// SAVING /////////////////////////////////

    public bool isUpgrading = false;
    public long upgradeEndTimeTicks = 0;
    public float upgradeTotalDuration = 0f;

    //////////////////////////////// SAVING /////////////////////////////////

    public void SetUpgradeToOneSecond()
    {
        if (currentUpgradeCoroutine != null)
        {
            StopCoroutine(currentUpgradeCoroutine);
            currentUpgradeCoroutine = null;

            if (activeBar != null)
            {
                activeBar.ForceFinishIn(0.1f);
            }

            currentUpgradeCoroutine = StartCoroutine(UpgradeAfterDelay(0.1f));
            //Debug.Log("⏱ تم تسريع الترقية الحالية لتصبح ثانية واحدة فقط!"); ديبك
        }
        else
        {
            Debug.Log("⏱ لا توجد ترقية حالية لتسريعها.");
        }
    }

    private void Awake()
    {
        BuildingRegistry.RegisterBuilding(this);

        if (workerUI == null)
            workerUI = WorkerUI.Instance;
    }

    private void OnDestroy()
    {
        BuildingRegistry.UnregisterBuilding(this);
    }

    private void Start()
    {
        if (workerUI == null)
            workerUI = WorkerUI.Instance;
        UpgradeToLevel(currentLevel);
    }

    private void Update()
    {
        // 🚀 تحريك الأيقونة إذا موجودة
        if (speedUpIconActive && activeSpeedUpIcon != null)
        {
            float floatY = Mathf.Sin(Time.time * 2f) * 0.2f; // حركة لأعلى وأسفل
            activeSpeedUpIcon.transform.position = speedUpIconStartPos + new Vector3(0, floatY, 0);
        }
    }

    public void UpgradeBuilding()
    {
        if (currentLevel < levels.Count - 1)
        {
            if (workerUI != null && workerUI.TryUseWorker())
            {
                float finalDuration = upgradeDuration; // الوقت الافتراضي للحماية
                string timeText = ""; // سنحفظ فيه النص القادم من الواجهة
                AudioManager.Instance.PlaySFX("build1");
                // 1️⃣ سحب الموارد وجلب نص الوقت من اللوحة الأولى
                if (takeResFromUp.whatPanlShouldTakeFrom == 1)
                {
                    takeResFromUp.instain.RemovingRes();
                    if (takeResFromUp.instain.upgradeTimerText != null)
                        timeText = takeResFromUp.instain.upgradeTimerText.text;
                }
                // 2️⃣ سحب الموارد وجلب نص الوقت من اللوحة الثانية
                else if (takeResFromUp.whatPanlShouldTakeFrom == 2)
                {
                    takeResFromUp2.instain.RemovingRes();
                    if (takeResFromUp2.instain.upgradeTimerText != null)
                        timeText = takeResFromUp2.instain.upgradeTimerText.text;
                }
                // 3️⃣ سحب الموارد وجلب نص الوقت من اللوحة الثالثة
                else if (takeResFromUp.whatPanlShouldTakeFrom == 3)
                {
                    takeResFromUp3.instain.RemovingRes();
                    if (takeResFromUp3.instain.upgradeTimerText != null)
                        timeText = takeResFromUp3.instain.upgradeTimerText.text;
                }

                // ✅ تحويل النص إلى ثواني وتطبيق الوقت الجديد
                if (!string.IsNullOrEmpty(timeText))
                {
                    finalDuration = ConvertTextToSeconds(timeText);
                }

                //////////////////////////////// SAVING /////////////////////////////////

                // 🟢 تسجيل وقت الانتهاء في المستقبل
                isUpgrading = true;
                upgradeTotalDuration = finalDuration;
                upgradeEndTimeTicks = System.DateTime.Now.AddSeconds(finalDuration).Ticks;

                //////////////////////////////// SAVING /////////////////////////////////


                currentUpgradeCoroutine = StartCoroutine(UpgradeAfterDelay(finalDuration));
                //if(takeResFromUp.whatPanlShouldTakeFrom == 1)
                //{
                //    AudioManager.Instance.PlaySFX("build1");
                //    takeResFromUp.instain.RemovingRes();
                //}

                //if (takeResFromUp.whatPanlShouldTakeFrom == 2)
                //{
                //    AudioManager.Instance.PlaySFX("build1");
                //    takeResFromUp2.instain.RemovingRes();
                //}

                //if (takeResFromUp.whatPanlShouldTakeFrom == 3)
                //{
                //    AudioManager.Instance.PlaySFX("build1");
                //    takeResFromUp3.instain.RemovingRes();
                //}


            }
            else
            {
                Debug.LogWarning("❌ لا يوجد عمال متاحون للترقية.");
            }
        }
        else
        {
            Debug.Log("📛 تم الوصول إلى الحد الأقصى من الترقية.");
        }
    }

    // 🟢 أضفنا public وأضفنا المتغير الثاني
    public IEnumerator UpgradeAfterDelay(float remainingTime = -1f, float totalTime = -1f)
    {
        float useRemaining = (remainingTime > 0) ? remainingTime : upgradeDuration;
        float useTotal = (totalTime > 0) ? totalTime : upgradeDuration;

        float remainingTimeForLoop = useRemaining; // للمتغير داخل while loop


        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (var col in colliders)
        {
            if (col.gameObject.name != "upgrade (1)")
                col.enabled = false;
        }

        GameObject canvasObject = GameObject.FindWithTag("MainCanvas");
        if (canvasObject != null)
        {
            GameObject bar = Instantiate(upgradeBarPrefab, canvasObject.transform);
            bar.transform.SetAsFirstSibling();

            UpgradeBarFollow barScript = bar.GetComponent<UpgradeBarFollow>();
            activeBar = barScript;
            // 🟢 تشغيل البار بالوقت الكلي والمتبقي
            barScript.ResumeUpgradeBar(transform, useTotal, useRemaining);
        }

        building buildingScript = GetComponent<building>();
        buildingScript.ShowUpgradeIcon();

        // ✅ إيقاف إنتاج الخشب إذا كان المبنى LUMBER BLADE
        string tagToCheck = GetTagFromBuildingOrChildren(gameObject);
        if (tagToCheck == "wood") // ← "wood" هو نفس التاغ لمبنى LUMBER BLADE حسب دالتك
        {
            buildWoodStroge woodScript = GetComponentInChildren<buildWoodStroge>();
            if (woodScript != null)
            {
                woodScript.enabled = false;
            }
        }
        if (tagToCheck == "power") // ← "wood" هو نفس التاغ لمبنى LUMBER BLADE حسب دالتك
        {
            buildPowerStroge woodScript = GetComponentInChildren<buildPowerStroge>();
            if (woodScript != null)
            {
                woodScript.enabled = false;
            }
        }
        if (tagToCheck == "iron") // ← "wood" هو نفس التاغ لمبنى LUMBER BLADE حسب دالتك
        {
            buildIronStorge woodScript = GetComponentInChildren<buildIronStorge>();
            if (woodScript != null)
            {
                woodScript.enabled = false;
            }
        }
        if (tagToCheck == "stone") // ← "wood" هو نفس التاغ لمبنى LUMBER BLADE حسب دالتك
        {
            buildStoneStroge woodScript = GetComponentInChildren<buildStoneStroge>();
            if (woodScript != null)
            {
                woodScript.enabled = false;
            }
        }
        if (tagToCheck == "CenterCity") // ← "wood" هو نفس التاغ لمبنى LUMBER BLADE حسب دالتك
        {
            BuildingGoldStorage woodScript = GetComponentInChildren<BuildingGoldStorage>();
            if (woodScript != null)
            {
                woodScript.enabled = false;
            }
        }

        // ⏳ متابعة الوقت المتبقي
        while (remainingTimeForLoop > 0)
        {
            remainingTimeForLoop -= Time.deltaTime;

            if (remainingTime <= 300f && activeSpeedUpIcon == null && speedUpIconPrefab != null)
            {
                activeSpeedUpIcon = Instantiate(speedUpIconPrefab, transform);
                activeSpeedUpIcon.transform.localPosition = new Vector3(0, 2, 0);

                // حفظ موضع البداية للحركة
                speedUpIconStartPos = activeSpeedUpIcon.transform.position;
                speedUpIconActive = true;

                //Debug.Log("⚡ ظهر زر التسريع لأن الوقت أقل من 5 دقائق!"); ديبك
            }

            yield return null;
        }

        if (buildingScript != null)
            buildingScript.HideUpgradeIcon();

        currentLevel++;
        UpgradeToLevel(currentLevel);

        if (workerUI != null)
            workerUI.ReturnWorker();

        // ✅ إعادة تفعيل الإنتاج بعد الترقية
        if (tagToCheck == "wood")
        {
            buildWoodStroge woodScript = GetComponentInChildren<buildWoodStroge>();
            if (woodScript != null)
            {
                woodScript.enabled = true;
            }
        }
        if (tagToCheck == "CenterCity")
        {
            BuildingGoldStorage woodScript = GetComponentInChildren<BuildingGoldStorage>();
            if (woodScript != null)
            {
                woodScript.enabled = true;
            }
        }
        if (tagToCheck == "iron")
        {
            buildIronStorge woodScript = GetComponentInChildren<buildIronStorge>();
            if (woodScript != null)
            {
                woodScript.enabled = true;
            }
        }
        if (tagToCheck == "stone")
        {
            buildStoneStroge woodScript = GetComponentInChildren<buildStoneStroge>();
            if (woodScript != null)
            {
                woodScript.enabled = true;
            }
        }
        if (tagToCheck == "power")
        {
            buildPowerStroge woodScript = GetComponentInChildren<buildPowerStroge>();
            if (woodScript != null)
            {
                woodScript.enabled = true;
            }
        }

        if (tagToCheck == "CenterCity" && BuildingLimitSystem.instaine != null)
        {
            BuildingLimitSystem.instaine.UpgradeCityHall();
            //Debug.Log("🏛 Center City upgraded → تحديث حدود المباني"); ديبك
        }

        Sprite buildingSprite = GetSpriteFromDeepChild(currentModel, "anyb_0");
        if (buildingSprite != null)
        {
            string NameBuild = GetBuildingNameByTag(tagToCheck);
            if (NameBuild == "BURNING MIRRORS")
            {
                Sprite buildingSprite2 = Resources.Load<Sprite>("buildingIcons/mirorr");
                PopupManager.instan.ShowPopup($"{NameBuild} upgraded to level {currentLevel + 1}!", buildingSprite2);

            }
            else if (NameBuild == "SKY TOWER")
            {
                Sprite buildingSprite2 = Resources.Load<Sprite>("buildingIcons/flay");
                PopupManager.instan.ShowPopup($"{NameBuild} upgraded to level {currentLevel + 1}!", buildingSprite2);

            }
            else PopupManager.instan.ShowPopup($"{NameBuild} upgraded to level {currentLevel + 1}!", buildingSprite);
        }
        else
        {
            Debug.LogWarning("لم يتم العثور على صورة الابن anyb_0 للترقية");
        }

        // 🗑 إزالة الأيقونة عند انتهاء الترقية
        if (activeSpeedUpIcon != null)
        {
            Destroy(activeSpeedUpIcon);
            activeSpeedUpIcon = null;
            speedUpIconActive = false;
        }

        // 🟢 إيقاف حالة التطوير هنا بعد انتهاء الوقت والترقية فعلياً
        isUpgrading = false;

        currentUpgradeCoroutine = null;
    }


    // باقي الكود كما هو ...
    private void UpgradeToLevel(int level)
    {
        float savedCurrentCapacity = 0; // 1️⃣ لحفظ السعة الحالية قبل الحذف

        // 🔁 إذا كان النموذج القديم يحتوي على Barracks، ألغِ تسجيله
        if (currentModel != null)
        {
            Barracks oldBarracks = currentModel.GetComponentInChildren<Barracks>();
            if (oldBarracks != null)
            {
                // 2️⃣ حفظ currentCapacity قبل الحذف
                savedCurrentCapacity = oldBarracks.currentCapacity;

                BarracksManager.Instance.UnregisterBarracks(oldBarracks);
            }

            Destroy(currentModel); // حذف النموذج القديم
        }

        // تعيين النموذج الجديد وخصائصه
        BuildingLevel newLevel = levels[level];
        currentModel = Instantiate(newLevel.model, transform.position, Quaternion.identity, transform);

        // ✅ إذا كان النموذج الجديد يحتوي على Barracks، سجله
        Barracks newBarracks = currentModel.GetComponentInChildren<Barracks>();
        if (newBarracks != null)
        {
            BarracksManager.Instance.RegisterBarracks(newBarracks);

            // 3️⃣ استرجاع السعة المحفوظة
            newBarracks.currentCapacity = savedCurrentCapacity;
        }

        //Debug.Log($"✅ upgrade done {newLevel.level}!"); ديبك
    }

    string GetTagFromBuildingOrChildren(GameObject building)
    {
        // أولاً الأب
        if (!string.IsNullOrEmpty(building.tag) && building.tag != "Untagged")
        {
            return building.tag;
        }

        // ثم بحث عميق في الأبناء
        foreach (Transform child in building.GetComponentsInChildren<Transform>(true))
        {
            if (!string.IsNullOrEmpty(child.tag) && child.tag != "Untagged")
            {
                return child.tag;
            }
        }

        return "Untagged";
    }

    Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }

    Sprite GetSpriteFromDeepChild(GameObject building, string childName = "anyb_0")
    {
        Transform childTransform = FindDeepChild(building.transform, childName);
        if (childTransform != null)
        {
            Image img = childTransform.GetComponent<Image>();
            if (img != null && img.sprite != null)
            {
                return img.sprite;
            }

            SpriteRenderer sr = childTransform.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                return sr.sprite;
            }
        }
        return null;
    }

    private string GetBuildingNameByTag(string tagToCheck)
    {
        switch (tagToCheck)
        {
            case "wood": return "LUMBER BLADE";
            case "boomOP": return "BEAST CANNON";
            case "stone": return "STONE GRINDER";
            case "iron": return "IRON SMELTER";
            case "power": return "STATUE OF MIGHT";
            case "CenterCity": return "CENTER CITY";
            case "Resoures": return "STOCK PILE";
            case "archer": return "ARCHER'S TOWER";
            case "archerOP": return "GALTING ARROW TOWER";
            case "maker": return "HIRING QUARTERS";
            case "flameT": return "FLAMER TOWER";
            case "flayTower": return "SKY TOWER";
            case "Mirror": return "BURNING MIRRORS";
            case "spy": return "WATCH POST";
            case "spyClan": return "FREEND WATCH POST";
            case "boomTower": return "BOMBARD TOWER";
            case "Barracks": return "MERCENARY BARRACKS";
            case "BrackClan": return "CLAN BARRACKS";
            case "clan": return "CLAN";
            case "exeBuild": return "EXECUTIONAL CUILLOTINE";
            case "plan": return "CITY PLANNER";
            case "post": return "PIGEON POST";
            case "fourMaker": return "CENTRAL HIRING";
            case "hadad": return "BLACKSMITH";
            case "horn": return "TUSK HORN";
            case "NewWorr": return "RECRUITMENT CHAMBER";
            case "training": return "TRAINGING CHAMBER";
            case "Cave": return "MONSTER CAVE";
            case "CaveBrack": return "MONSTER Barrack";
            case "mongel": return "GATAPULT";
            default: return "Building";
        }
    }

    // ✅ دالة جديدة لمعرفة المستوى الحالي من السكريبتات الأخرى
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    // ✅ جعل دالة جلب التاغ عامة (public) لكي نستخدمها للبحث
    public string GetBuildingTag()
    {
        return GetTagFromBuildingOrChildren(gameObject);
    }

    // 🚀 دالة الترقية الفورية (بدون وقت، بدون عمال، بدون إيقاف الإنتاج)
    public void InstantUpgrade()
    {
        if (currentLevel < levels.Count - 1)
        {
            currentLevel++;
            UpgradeToLevel(currentLevel);

            string tagToCheck = GetBuildingTag();

            // تحديث قاعة المدينة إذا كان المبنى هو المركز
            if (tagToCheck == "CenterCity" && BuildingLimitSystem.instaine != null)
            {
                BuildingLimitSystem.instaine.UpgradeCityHall();
                Debug.Log("🏛 Center City upgraded instantly → تحديث حدود المباني");
            }

            // إظهار نافذة الترقية (Popup)
            Sprite buildingSprite = GetSpriteFromDeepChild(currentModel, "anyb_0");
            string NameBuild = GetBuildingNameByTag(tagToCheck);

            if (buildingSprite != null)
            {
                if (NameBuild == "BURNING MIRRORS")
                {
                    Sprite buildingSprite2 = Resources.Load<Sprite>("buildingIcons/mirorr");
                    PopupManager.instan.ShowPopup($"{NameBuild} upgraded instantly to level {currentLevel + 1}!", buildingSprite2);
                }
                else if (NameBuild == "SKY TOWER")
                {
                    Sprite buildingSprite2 = Resources.Load<Sprite>("buildingIcons/flay");
                    PopupManager.instan.ShowPopup($"{NameBuild} upgraded instantly to level {currentLevel + 1}!", buildingSprite2);
                }
                else
                {
                    PopupManager.instan.ShowPopup($"{NameBuild} upgraded instantly to level {currentLevel + 1}!", buildingSprite);
                }
            }

            Debug.Log($"⚡ تم ترقية {NameBuild} فورياً إلى المستوى {currentLevel}!");
        }
        else
        {
            Debug.Log("📛 هذا المبنى وصل للحد الأقصى بالفعل.");
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
    // 🟢 دالة مخصصة لتحميل المستوى المحفوظ مباشرة
    public void SetLevelFromSave(int savedLevel)
    {
        currentLevel = savedLevel;

        // ملاحظة: دالة Start() الموجودة لديك في السكربت 
        // ستعمل تلقائياً وتقرأ currentLevel الجديد وتقوم بإنشاء النموذج المناسب
    }
    // 🟢 دالة جديدة لاستئناف الترقية من نظام التحميل وتسجيل الكوروتين
    public void ResumeUpgrade(float remainingTime, float totalTime)
    {
        // إيقاف أي كوروتين سابق احتياطياً
        if (currentUpgradeCoroutine != null)
        {
            StopCoroutine(currentUpgradeCoroutine);
        }

        // تشغيل الترقية وحفظها في المتغير لكي يتعرف عليها زر التسريع
        currentUpgradeCoroutine = StartCoroutine(UpgradeAfterDelay(remainingTime, totalTime));
    }
}
