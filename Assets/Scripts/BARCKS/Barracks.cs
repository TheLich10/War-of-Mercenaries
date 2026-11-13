using System.Collections.Generic;
using UnityEngine;

public class Barracks : MonoBehaviour
{
    public static Barracks instnce;

    public static List<Barracks> allActiveBarracks = new List<Barracks>();

    // متغيرات جديدة لحل مشكلة التوقيت
    public static float pendingLoadedCapacity = 0;
    public static bool needsCapacityDistribution = false;


    public float maxCapacity = 200; // الحد الأقصى للمحاربين
    public float currentCapacity = 0; // العدد الحالي للمحاربين

    //public event System.Action<float, float> OnCapacityChanged; // (current, max)


    //******************************************//
    public static int bedoiun = 0;
    public static string bdoyName = "";

    public static int ScalingEgyptian = 0;
    public static string archerName = "";

    public static int Destroyer = 0;
    public static string DestroyerName = "";

    public static int flyingOttoman = 0;
    public static string flyingOttomanName = "";

    public static int Gaul = 0;
    public static string GaulName = "";

    public static int Helaer = 0;
    public static string HelaerNmae = "";

    public static int Kamikaze = 0;
    public static string KamikazeName = "";

    public static int knight = 0;
    public static string knightName = "";

    public static int Mongolian = 0;
    public static string MongolianName = "";

    public static int ottman = 0;
    public static string ottmanName = "";

    public static int persian = 0;
    public static string persianName = "";

    public static int postion = 0;
    public static string postionName = "";

    public static int sapper = 0;
    public static string sapperName = "";

    public static int Savge = 0;
    public static string SavgeName = "";

    public static int sneaky = 0;
    public static string sneakyName = "";


    //******************************************//
    public string buildingName; // اسم المبنى الذي سيتم استخدامه للربط
    public int level = 1; // مستوى المبنى


    // قاموس لتخزين نوع المحارب وعددهم
    private static Dictionary<string, int> warriorStorage = new Dictionary<string, int>();

    // أضف هذا السطر مع المتغيرات في الأعلى
    public List<Warrior> activeWarriors = new List<Warrior>();

    [Header("أيقونة امتلاء الثكنات")]
    public GameObject resourceIconPrefab; // اسحب Prefab الأيقونة هنا
    private GameObject resourceIconInstance;
    private Vector3 iconStartPos;
    private bool iconActive = false;

    private void Awake()
    {
        instnce = this;
        // إضافة الثكنة الحالية للقائمة
        if (!allActiveBarracks.Contains(this))
        {
            allActiveBarracks.Add(this);
        }

        // 🟢 التعديل الأول: تسجيل الثكنة في المدير فور إنشائها بدلاً من انتظار Start
        if (BarracksManager.Instance != null)
        {
            BarracksManager.Instance.RegisterBarracks(this);
        }
    }

    void Start()
    {
        // نقوم بتحديث سعة الثكنة وإطلاق الحدث للواجهة
        InitializeBuildingStats();
    }

    private void Update()
    {
        // هذا الكود سيعمل لمرة واحدة فقط لحظة اكتمال بناء الثكنات
        if (needsCapacityDistribution && allActiveBarracks.Count > 0)
        {
            // تنظيف القائمة من أي ثكنة محذوفة
            allActiveBarracks.RemoveAll(b => b == null || b.gameObject == null);

            if (allActiveBarracks.Count > 0)
            {
                // تقسيم السعة على الثكنات بالتساوي
                float capacityPerBarrack = pendingLoadedCapacity / allActiveBarracks.Count;
                foreach (var b in allActiveBarracks)
                {
                    b.currentCapacity = capacityPerBarrack;
                }

                // إطفاء الأمر حتى لا يتكرر
                needsCapacityDistribution = false;
                pendingLoadedCapacity = 0;

                // تحديث الواجهة (UI)
                if (BarracksManager.Instance != null)
                {
                    BarracksManager.Instance.makeEvent();
                    //Debug.Log("✅ تم توزيع السعة بنجاح على الثكنات!"); ديبك
                }
            }
        }
        // 1. التحقق من امتلاء السعة الكلية لجميع الثكنات
        if (BarracksManager.Instance != null)
        {
            // هل السعة الحالية الكلية تساوي أو تتجاوز السعة القصوى الكلية؟
            bool isAllBarracksFull = BarracksManager.Instance.GetTotalCurrentCapacity() >= BarracksManager.Instance.GetTotalMaxCapacity();

            if (isAllBarracksFull && !iconActive)
            {
                ShowResourceIcon();
            }
            else if (!isAllBarracksFull && iconActive)
            {
                HideResourceIcon();
            }
        }

        // 2. تحريك الأيقونة إذا كانت موجودة
        if (iconActive && resourceIconInstance != null)
        {
            float floatY = Mathf.Sin(Time.time * 2f) * 0.2f; // حركة لأعلى وأسفل
            resourceIconInstance.transform.position = iconStartPos + new Vector3(0, floatY, 0);
        }
    }

    // 🟢 دالة إظهار الأيقونة
    private void ShowResourceIcon()
    {
        if (resourceIconPrefab != null && resourceIconInstance == null)
        {
            // تم وضع 2.5f لضمان ظهور الأيقونة فوق الثكنة، يمكنك تعديل الرقم حسب حجم المبنى عندك
            Vector3 spawnPos = transform.position + new Vector3(0, 1.5f, 0);
            resourceIconInstance = Instantiate(resourceIconPrefab, spawnPos, Quaternion.identity, transform);
            iconStartPos = resourceIconInstance.transform.position;
            iconActive = true;
        }
    }

    // 🟢 دالة إخفاء الأيقونة
    private void HideResourceIcon()
    {
        if (resourceIconInstance != null)
        {
            Destroy(resourceIconInstance);
            resourceIconInstance = null;
            iconActive = false;
        }
    }

    private void OnDestroy()
    {
        if (allActiveBarracks.Contains(this))
        {
            allActiveBarracks.Remove(this);
        }
        // 🟢 هذا السطر مهم جداً لمنع الأخطاء في المدير
        if (BarracksManager.Instance != null)
        {
            BarracksManager.Instance.UnregisterBarracks(this);
        }
    }



    // أضف هذه الدالة الجديدة
    void InitializeBuildingStats()
    {
        BuildingData data = BuildingDatabase.GetBuildingByName(buildingName);
        if (data != null)
        {
            BuildLevelUI levelData = data.GetLevel(level);
            if (levelData != null)
            {
                float oldCapacity = maxCapacity; // حفظ السعة القديمة
                maxCapacity = levelData.Barackspace; // تعيين السعة الجديدة

                // إعلام BarracksManager بالتغيير مع تحديد السعة القديمة والجديدة
                BarracksManager.Instance.UpdateBarracksCapacity(this, oldCapacity, maxCapacity);

                //Debug.Log($"{buildingName} (L{level}) initialized: Max Space={maxCapacity}"); ديبك

                // نبلغ أي UI أن القيم تغيرت
                BarracksManager.Instance.makeEvent();
            }
            else
            {
                Debug.LogWarning($"No level {level} found for building {buildingName}");
            }
        }
        else
        {
            Debug.LogWarning($"No building found with name {buildingName}");
        }
    }




    public Dictionary<string, int> GetWarriorDictionary()
    {
        // نعيد نسخة من القاموس لتجنب التعديلات الخارجية
        return new Dictionary<string, int>(warriorStorage);
    }

    public bool CanAddWarrior()
    {
        return currentCapacity < maxCapacity;
    }

    public bool AddWarrior(string warriorName)
    {
        //Debug.Log("the max i want is :"+maxCapacity); ديبك
        int spaceRequired = 0;

        // حساب السعة المطلوبة لكل نوع
        if (warriorName == "bedoiun") spaceRequired = 10;
        else if (warriorName == "ScalingEgyptian") spaceRequired = 70;
        else if (warriorName == "Destroyer") spaceRequired = 20;
        else if (warriorName == "flyingOttoman") spaceRequired = 70;
        else if (warriorName == "Gaul") spaceRequired = 160;
        else if (warriorName == "Helaer") spaceRequired = 200;
        else if (warriorName == "Kamikaze") spaceRequired = 60;
        else if (warriorName == "knight") spaceRequired = 15;
        else if (warriorName == "Mongolian") spaceRequired = 40;
        else if (warriorName == "ottman") spaceRequired = 10;
        else if (warriorName == "persian") spaceRequired = 20;
        else if (warriorName == "postion") spaceRequired = 30;
        else if (warriorName == "sapper") spaceRequired = 70;
        else if (warriorName == "Savge") spaceRequired = 20;
        else if (warriorName == "sneaky") spaceRequired = 30;


        // تحقق من المساحة المتاحة
        if (BarracksManager.Instance.GetTotalCurrentCapacity() + spaceRequired > BarracksManager.Instance.GetTotalMaxCapacity())
        {
            //Debug.Log("its what i want " + currentCapacity + "the mac space : " + maxCapacity);
            Debug.Log("⚠ لا توجد مساحة كافية لإضافة هذا المحارب!");
            return false;
        }


        // إضافة إلى القاموس
        if (warriorStorage.ContainsKey(warriorName))
        {
            warriorStorage[warriorName]++;
        }
        else
        {
            warriorStorage[warriorName] = 1;
        }

        // زيادة السعة الحالية
        currentCapacity += spaceRequired;

        // تحديث المتغيرات العامة (نفس الكود الذي كتبته أنت سابقًا)
        if (warriorName == "bedoiun")
        {
            bdoyName = warriorName;
            bedoiun++;
        }
        else if (warriorName == "ScalingEgyptian")
        {
            archerName = warriorName;
            ScalingEgyptian++;
        }
        else if (warriorName == "Destroyer")
        {
            DestroyerName = warriorName;
            Destroyer++;
        }
        else if (warriorName == "flyingOttoman")
        {
            flyingOttomanName = warriorName;
            flyingOttoman++;
        }
        else if (warriorName == "Gaul")
        {
            GaulName = warriorName;
            Gaul++;
        }
        else if (warriorName == "Helaer")
        {
            HelaerNmae = warriorName;
            Helaer++;
        }
        else if (warriorName == "Kamikaze")
        {
            KamikazeName = warriorName;
            Kamikaze++;
        }
        else if (warriorName == "knight")
        {
            knightName = warriorName;
            knight++;
        }
        else if (warriorName == "Mongolian")
        {
            MongolianName = warriorName;
            Mongolian++;
        }
        else if (warriorName == "ottman")
        {
            ottmanName = warriorName;
            ottman++;
        }
        else if (warriorName == "persian")
        {
            persianName = warriorName;
            persian++;
        }
        else if (warriorName == "postion")
        {
            postionName = warriorName;
            postion++;
        }
        else if (warriorName == "sapper")
        {
            sapperName = warriorName;
            sapper++;
        }
        else if (warriorName == "Savge")
        {
            SavgeName = warriorName;
            Savge++;
        }
        else if (warriorName == "sneaky")
        {
            sneakyName = warriorName;
            sneaky++;
        }

        //Debug.Log("✔ تمت إضافة محارب: " + warriorName + " | العدد الحالي: " + warriorStorage[warriorName]);  ديبك
        BarracksManager.Instance.makeEvent();
        // Random.Range(0, 2) سيقوم بإرجاع إما الرقم 0 أو 1 بشكل عشوائي
        int randomNum = UnityEngine.Random.Range(0, 2);

        if (randomNum == 0)
        {
            AudioManager.Instance.PlaySFX("bed_SPN2");
        }
        else
        {
            AudioManager.Instance.PlaySFX("bed_SPN");
        }

        return true;
    }

    public bool RemoveWarrior(string warriorName, bool sendToMonster = false)
    {
        if (warriorStorage.ContainsKey(warriorName) && warriorStorage[warriorName] > 0)
        {
            warriorStorage[warriorName]--;

            int spaceFreed = 0;

            if (warriorName == "bedoiun") { spaceFreed = 10;
                bedoiun--;
                if (bedoiun < 0) bedoiun = 0;
            }
            else if (warriorName == "ScalingEgyptian") { spaceFreed = 70;
                ScalingEgyptian--;
                if (ScalingEgyptian < 0) ScalingEgyptian = 0;
            }
            else if (warriorName == "Destroyer") { spaceFreed = 20;
                Destroyer--;
                if (Destroyer < 0) Destroyer = 0;
            }
            else if (warriorName == "flyingOttoman") { spaceFreed = 70;
                flyingOttoman--;
                if (flyingOttoman < 0) flyingOttoman = 0;
            }
            else if (warriorName == "Gaul") { spaceFreed = 160;
                Gaul--;
                if (Gaul < 0) Gaul = 0;
            }
            else if (warriorName == "Helaer") { spaceFreed = 200;
                Helaer--;
                if (Helaer < 0) Helaer = 0;
            }
            else if (warriorName == "Kamikaze") { spaceFreed = 60;
                Kamikaze--;
                if (Kamikaze < 0) Kamikaze = 0;
            }
            else if (warriorName == "knight") { spaceFreed = 15;
                knight--;
                if (knight < 0) knight = 0;
            }
            else if (warriorName == "Mongolian") { spaceFreed = 40;
                Mongolian--;
                if (Mongolian < 0) Mongolian = 0;
            }
            else if (warriorName == "ottman") { spaceFreed = 10;
                ottman--;
                if (ottman < 0) ottman = 0;
            }
            else if (warriorName == "persian") { spaceFreed = 20;
                persian--;
                if (persian < 0) persian = 0;
            }
            else if (warriorName == "postion") { spaceFreed = 30;
                postion--;
                if (postion < 0) postion = 0;
            }
            else if (warriorName == "sapper") { spaceFreed = 70;
                sapper--;
                if (sapper < 0) sapper = 0;
            }
            else if (warriorName == "Savge") { spaceFreed = 20;
                Savge--;
                if (Savge < 0) Savge = 0;
            }
            else if (warriorName == "sneaky") { spaceFreed = 30;
                sneaky--;
                if (sneaky < 0) sneaky = 0;
            }

            currentCapacity -= spaceFreed;

            if (warriorStorage[warriorName] == 0)
                warriorStorage.Remove(warriorName);

            BarracksManager.Instance.makeEvent();

            // =========================================================
            // البحث عن الجندي في قائمة الثكنات المسجلة
            bool warriorFoundAndRemoved = false;

            foreach (Barracks barracks in allActiveBarracks)
            {
                if (barracks == null) continue; // حماية إضافية لتجنب الأخطاء

                for (int i = 0; i < barracks.activeWarriors.Count; i++)
                {
                    Warrior w = barracks.activeWarriors[i];

                    // نتحقق أن الجندي موجود، ويحمل نفس الاسم، ولم يسبق له المغادرة
                    if (w != null && w.gameObject.activeInHierarchy && w.warriorName == warriorName && !w.isDismissed)
                    {
                        if (sendToMonster == true)
                        {
                            w.GoToExitAndToMonster();
                        }
                        else
                        {
                            w.GoToExitAndDeactivate();
                        }

                        barracks.activeWarriors.RemoveAt(i); // نحذفه من قائمة الثكنة الصحيحة
                        warriorFoundAndRemoved = true; // نؤكد أننا وجدناه
                        break; // نوقف حلقة البحث داخل هذه الثكنة لأننا حذفنا جندي واحد
                    }
                }

                // إذا وجدنا الجندي في هذه الثكنة، نتوقف عن البحث في باقي الثكنات
                if (warriorFoundAndRemoved)
                {
                    break;
                }
            }
            // =========================================================
            // ---------------------------------------------------------

            return true; // ✅ تم الحذف
        }

        Debug.Log("⚠ لا يوجد محارب من هذا النوع في الثكنة!");
        return false; // ❌ لم يتم الحذف
    }


    public void GetBarracksInfo()
    {
        Debug.Log("📌 محاربون في الثكنة:");
        foreach (var warrior in warriorStorage)
        {
            Debug.Log("🛡 " + warrior.Key + " | العدد: " + warrior.Value);

        }
    }

    public bool CanAddSpecificWarrior(string warriorName)
    {
        int spaceRequired = 0;

        // حساب السعة المطلوبة لكل نوع بناءً على الاسم
        if (warriorName == "bedoiun") spaceRequired = 10;
        else if (warriorName == "ScalingEgyptian") spaceRequired = 70;
        else if (warriorName == "Destroyer") spaceRequired = 20;
        else if (warriorName == "flyingOttoman") spaceRequired = 70;
        else if (warriorName == "Gaul") spaceRequired = 160;
        else if (warriorName == "Helaer") spaceRequired = 200;
        else if (warriorName == "Kamikaze") spaceRequired = 60;
        else if (warriorName == "knight") spaceRequired = 15;
        else if (warriorName == "Mongolian") spaceRequired = 40;
        else if (warriorName == "ottman") spaceRequired = 10;
        else if (warriorName == "persian") spaceRequired = 20;
        else if (warriorName == "postion") spaceRequired = 30;
        else if (warriorName == "sapper") spaceRequired = 70;
        else if (warriorName == "Savge") spaceRequired = 20;
        else if (warriorName == "sneaky") spaceRequired = 30;

        // التحقق مما إذا كانت المساحة الحالية + مساحة الجندي المطلوب تتجاوز الحد الأقصى
        if (BarracksManager.Instance.GetTotalCurrentCapacity() + spaceRequired > BarracksManager.Instance.GetTotalMaxCapacity())
        {
            return false; // لا توجد مساحة كافية لهذا الجندي
        }

        return true; // توجد مساحة كافية
    }

    // 🟢 دالة لتجهيز البيانات للحفظ
    // 🟢 دالة الحفظ (أصبحت Static)
    public static List<WarriorCountData> GetWarriorsForSaveGlobal()
    {
        List<WarriorCountData> list = new List<WarriorCountData>();
        foreach (var w in warriorStorage)
        {
            list.Add(new WarriorCountData { warriorName = w.Key, count = w.Value });
        }
        return list;
    }

    // 🟢 دالة التحميل (أصبحت Static)
    public static void LoadGlobalWarriors(List<WarriorCountData> savedData)
    {
        warriorStorage.Clear();
        bedoiun = ScalingEgyptian = Destroyer = flyingOttoman = Gaul = 0;
        Helaer = Kamikaze = knight = Mongolian = ottman = persian = 0;
        postion = sapper = Savge = sneaky = 0;

        float totalNeededCapacity = 0;

        foreach (var data in savedData)
        {
            warriorStorage[data.warriorName] = data.count;
            int spaceRequired = 0;
            string wName = data.warriorName;

            if (wName == "bedoiun") { spaceRequired = 10; bdoyName = wName; bedoiun = data.count; }
            else if (wName == "ScalingEgyptian") { spaceRequired = 70; archerName = wName; ScalingEgyptian = data.count; }
            else if (wName == "Destroyer") { spaceRequired = 20; DestroyerName = wName; Destroyer = data.count; }
            else if (wName == "flyingOttoman") { spaceRequired = 70; flyingOttomanName = wName; flyingOttoman = data.count; }
            else if (wName == "Gaul") { spaceRequired = 160; GaulName = wName; Gaul = data.count; }
            else if (wName == "Helaer") { spaceRequired = 200; HelaerNmae = wName; Helaer = data.count; }
            else if (wName == "Kamikaze") { spaceRequired = 60; KamikazeName = wName; Kamikaze = data.count; }
            else if (wName == "knight") { spaceRequired = 15; knightName = wName; knight = data.count; }
            else if (wName == "Mongolian") { spaceRequired = 40; MongolianName = wName; Mongolian = data.count; }
            else if (wName == "ottman") { spaceRequired = 10; ottmanName = wName; ottman = data.count; }
            else if (wName == "persian") { spaceRequired = 20; persianName = wName; persian = data.count; }
            else if (wName == "postion") { spaceRequired = 30; postionName = wName; postion = data.count; }
            else if (wName == "sapper") { spaceRequired = 70; sapperName = wName; sapper = data.count; }
            else if (wName == "Savge") { spaceRequired = 20; SavgeName = wName; Savge = data.count; }
            else if (wName == "sneaky") { spaceRequired = 30; sneakyName = wName; sneaky = data.count; }

            totalNeededCapacity += (spaceRequired * data.count);
        }

        // 🟢 بدلاً من التوزيع الفاشل هنا، نحفظ القيمة ونأمر الثكنات بتوزيعها متى ما أصبحت جاهزة
        pendingLoadedCapacity = totalNeededCapacity;
        needsCapacityDistribution = true;
    }
}