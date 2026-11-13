using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI; // ✅ لإستخدام Button

[System.Serializable]
public class BuildingDataLimit
{
    public string buildingName;       // اسم المبنى (مثلاً: WoodCamp, StoneMine ...)
    public TMP_Text countText;        // النص (0/2)
    public GameObject lockPanel;      // البنل (القفل)
    public Button buildingButton;     // ✅ زر البناء الخاص بهذا المبنى

    [Header("الاعدادات")]
    public int currentCount = 0;      // المباني الحالية
    public int[] maxPerLevel;         // الحد الأقصى لكل مستوى مركز المدينة
    public int unlockCityLevel = 1;   // المستوى المطلوب لفتح هذا المبنى (مثلاً 5 أو 10)

    [HideInInspector] public int maxAllowed; // الحد المسموح الحالي
}

public class BuildingLimitSystem : MonoBehaviour
{
    public static BuildingLimitSystem instaine;

    [Header("مستوى مركز المدينة")]
    public int cityHallLevel = 1;

    [Header("قائمة المباني")]
    public List<BuildingDataLimit> buildings = new List<BuildingDataLimit>();

    private void Awake()
    {
        instaine = this;
    }

    void Start()
    {
        UpdateAllBuildings();
    }

    /// <summary>
    /// رفع مستوى مركز المدينة
    /// </summary>
    public void UpgradeCityHall()
    {
        cityHallLevel++;
        UpdateAllBuildings();
    }

    /// <summary>
    /// بناء مبنى جديد حسب الاسم
    /// </summary>
    public void Build(string buildingName)
    {
        BuildingDataLimit building = buildings.Find(b => b.buildingName == buildingName);
        if (building == null) return;

        if (cityHallLevel < building.unlockCityLevel)
        {
            Debug.Log("❌ هذا المبنى مقفل حتى تصل للمستوى " + building.unlockCityLevel);
            return;
        }

        if (building.buildingName == "CENTRAL HIRING")
        {
            BuildingDataLimit hq = buildings.Find(b => b.buildingName == "HIRING QUARTERS");
            if (hq == null || hq.currentCount < 5)
            {
                Debug.Log("❌ هذا المبنى مقفل! تحتاج إلى بناء 5 مباني HIRING QUARTERS أولاً.");
                return;
            }
        }
        // --- شرط مبنى BEAST CAVE ---
        if (building.buildingName == "BEAST CAVE")
        {
            BuildingDataLimit bk = buildings.Find(b => b.buildingName == "BEAST KEEPER");
            if (bk == null || bk.currentCount < 1)
            {
                Debug.Log("❌ هذا المبنى مقفل! تحتاج إلى بناء BEAST KEEPER أولاً.");
                return;
            }
        }
        // ---------------------------

        if (building.currentCount < building.maxAllowed)
        {
            building.currentCount++;

            // ✅ نقوم بتحديث كل المباني لكي يفتح CENTRAL HIRING فوراً إذا اكتمل الشرط
            UpdateAllBuildings();

        }
        else
        {
            //Debug.Log("❌ وصلت للحد الأقصى من " + building.buildingName);
        }
    }

    /// <summary>
    /// تحديث جميع المباني عند رفع مستوى مركز المدينة
    /// </summary>
    private void UpdateAllBuildings()
    {
        foreach (var building in buildings)
        {
            UpdateBuildingLimit(building);
            UpdateBuildingUI(building);
        }
    }

    private void UpdateBuildingLimit(BuildingDataLimit building)
    {
        bool isLocked = cityHallLevel < building.unlockCityLevel;

        // --- إضافة التحقق الخاص بمبنى CENTRAL HIRING ---
        if (!isLocked && building.buildingName == "CENTRAL HIRING")
        {
            BuildingDataLimit hq = buildings.Find(b => b.buildingName == "HIRING QUARTERS");
            if (hq == null || hq.currentCount < 5)
            {
                isLocked = true; // نعتبره مقفولاً لعدم استيفاء شرط المباني
            }
        }
        // ---------------------------------------------
        // --- التحقق الخاص بمبنى BEAST CAVE ---
        if (!isLocked && building.buildingName == "BEAST CAVE")
        {
            BuildingDataLimit bk = buildings.Find(b => b.buildingName == "BEAST KEEPER");
            if (bk == null || bk.currentCount < 1)
            {
                isLocked = true;
            }
        }
        // -------------------------------------

        if (isLocked)
        {
            building.maxAllowed = 0; // مقفل
            return;
        }

        if (building.maxPerLevel != null && building.maxPerLevel.Length > 0)
        {
            int index = Mathf.Clamp(cityHallLevel - 1, 0, building.maxPerLevel.Length - 1);
            building.maxAllowed = building.maxPerLevel[index];
        }
        else
        {
            building.maxAllowed = 1; // افتراضي: مبنى واحد فقط
        }
    }

    private void UpdateBuildingUI(BuildingDataLimit building)
    {
        bool isLocked = cityHallLevel < building.unlockCityLevel;
        string lockMessage = "Center City (Lvl " + building.unlockCityLevel + ")";

        // --- تحديث الواجهة الخاصة بمبنى CENTRAL HIRING ---
        if (!isLocked && building.buildingName == "CENTRAL HIRING")
        {
            BuildingDataLimit hq = buildings.Find(b => b.buildingName == "HIRING QUARTERS");
            if (hq == null || hq.currentCount < 5)
            {
                isLocked = true;
                lockMessage = "Need 5 HIRING QUARTERS"; // رسالة القفل المخصصة
            }
        }
        // ---------------------------------------------
        // --- تحديث الواجهة الخاصة بمبنى BEAST CAVE ---
        if (!isLocked && building.buildingName == "BEAST CAVE")
        {
            BuildingDataLimit bk = buildings.Find(b => b.buildingName == "BEAST KEEPER");
            if (bk == null || bk.currentCount < 1)
            {
                isLocked = true;
                lockMessage = "Need BEAST KEEPER"; // رسالة القفل المخصصة
            }
        }
        // --------------------------------------------

        if (isLocked)
        {
            building.countText.text = lockMessage;
            building.lockPanel.SetActive(true);
            if (building.buildingButton != null)
                building.buildingButton.interactable = false; // ❌ لا يمكن الضغط
            return;
        }

        building.countText.text = building.currentCount + "/" + building.maxAllowed;

        bool reachedMax = building.currentCount >= building.maxAllowed;

        building.lockPanel.SetActive(reachedMax);

        if (building.buildingButton != null)
            building.buildingButton.interactable = !reachedMax; // ✅ تفعيل/تعطيل الزر
    }
    // ==========================================
    // 🟢 دوال الحفظ والتحميل (Save & Load System)
    // ==========================================

    /// <summary>
    /// تجهيز البيانات وإرسالها لمدير الحفظ
    /// </summary>
    public BuildingLimitSaveData GetSaveData()
    {
        BuildingLimitSaveData data = new BuildingLimitSaveData();
        data.cityHallLevel = this.cityHallLevel;

        foreach (var b in buildings)
        {
            data.buildingLimits.Add(new BuildingLimitItemSave
            {
                buildingName = b.buildingName,
                currentCount = b.currentCount
            });
        }
        return data;
    }

    /// <summary>
    /// استقبال البيانات المحفوظة وتطبيقها على اللعبة
    /// </summary>
    public void LoadData(BuildingLimitSaveData data)
    {
        if (data == null) return;

        // تحميل مستوى مركز المدينة
        this.cityHallLevel = data.cityHallLevel;

        // تحميل عدد المباني لكل مبنى
        foreach (var savedBuilding in data.buildingLimits)
        {
            BuildingDataLimit b = buildings.Find(x => x.buildingName == savedBuilding.buildingName);
            if (b != null)
            {
                b.currentCount = savedBuilding.currentCount;
            }
        }

        // تحديث الواجهة والحدود بناءً على البيانات الجديدة
        UpdateAllBuildings();
    }
}
