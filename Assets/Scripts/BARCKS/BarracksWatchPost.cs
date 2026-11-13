using System.Collections.Generic;
using UnityEngine;

public class BarracksWatchPost : MonoBehaviour
{
    //public static BarracksWatchPost instnce;

    public event System.Action<float, float> OnWatchChanged; // (currentCapacity, maxCapacity)


    public float maxCapacity = 380; // الحد الأقصى للمحاربين
    public float currentCapacity = 0; // العدد الحالي للمحاربين

    //******************************************//
    public  int bedoiun = 0;
    public  string bdoyName = "";

    public int ScalingEgyptian = 0;
    public string archerName = "";

    public int Destroyer = 0;
    public string DestroyerName = "";


    public int Gaul = 0;
    public string GaulName = "";


    public int Kamikaze = 0;
    public string KamikazeName = "";

    public int knight = 0;
    public string knightName = "";

    public int Mongolian = 0;
    public string MongolianName = "";

    public int ottman = 0;
    public string ottmanName = "";

    public int persian = 0;
    public string persianName = "";

    public int postion = 0;
    public string postionName = "";

    public int sapper = 0;
    public string sapperName = "";

    public int Savge = 0;
    public string SavgeName = "";

    public int sneaky = 0;
    public string sneakyName = "";

    //******************************************//
    public string buildingName; // اسم المبنى
    public int level = 1;       // مستوى المبنى

    // قاموس لتخزين نوع المحارب وعددهم في هذه الثكنة فقط
    private Dictionary<string, int> warriorStorage = new Dictionary<string, int>();

    //private void Awake()
    //{
    //    instnce = this;
    //}

    void Start()
    {
        InitializeBuildingStats();
        SyncWarriorsFromInspector();
    }

    void InitializeBuildingStats()
    {
        DefenceBuildData data = DefenceBuilds.GetBuildingByName(buildingName);
        if (data != null)
        {
            DefenceBuildLevelUI levelData = data.GetLevel(level);
            if (levelData != null)
            {
                maxCapacity = levelData.barrackSpace;
                //Debug.Log($"{buildingName} (L{level}) initialized: Max Space={maxCapacity}"); ديبك
            }
        }
    }

    // ---> الدالة الجديدة <---
    private void SyncWarriorsFromInspector()
    {
        currentCapacity = 0;
        warriorStorage.Clear();

        // دالة محلية صغيرة لتسهيل إضافة كل نوع موجود مسبقاً
        void RegisterInitial(string wName, int count)
        {
            if (count > 0)
            {
                warriorStorage[wName] = count;
                currentCapacity += GetWarriorSpace(wName) * count;
            }
        }

        // تسجيل كل المتغيرات العامة التي قمت بوضعها في الـ Inspector
        RegisterInitial("bedoiun", bedoiun);
        RegisterInitial("ScalingEgyptian", ScalingEgyptian);
        RegisterInitial("Destroyer", Destroyer);
        RegisterInitial("Battering_Gaul", Gaul);
        RegisterInitial("Kamikaze", Kamikaze);
        RegisterInitial("KnightOnHorse", knight);
        RegisterInitial("ChineseMongolian", Mongolian);
        RegisterInitial("Ottoman", ottman);
        RegisterInitial("persian", persian);
        RegisterInitial("Poisonous", postion);
        RegisterInitial("PersianSapper", sapper);
        RegisterInitial("EgyptianSavage", Savge);
        RegisterInitial("sneaky", sneaky);

        // تحديث الواجهة والـ UI بالقيم الجديدة بعد التشغيل
        OnWatchChanged?.Invoke(currentCapacity, maxCapacity);
    }

    public bool CanAddWarrior(string warriorName)
    {
        return (currentCapacity + GetWarriorSpace(warriorName) <= maxCapacity);
    }

    public bool AddWarrior(string warriorName)
    {
        int spaceRequired = GetWarriorSpace(warriorName);

        if (currentCapacity + spaceRequired > maxCapacity)
        {
            Debug.Log($"لا توجد مساحة كافية لإضافة {warriorName} في {buildingName}");
            return false;
        }

        // إضافة المحارب في قاموس هذه الثكنة
        if (warriorStorage.ContainsKey(warriorName))
            warriorStorage[warriorName]++;
        else
            warriorStorage[warriorName] = 1;

        currentCapacity += spaceRequired;

        // تحديث المتغيرات العامة لكل نوع
        if (warriorName == "bedoiun") { bdoyName = warriorName; bedoiun++; }
        else if (warriorName == "ScalingEgyptian") { archerName = warriorName; ScalingEgyptian++; }
        else if (warriorName == "Destroyer") { DestroyerName = warriorName; Destroyer++; }
        else if (warriorName == "Gaul") { GaulName = warriorName; Gaul++; }
        else if (warriorName == "Kamikaze") { KamikazeName = warriorName; Kamikaze++; }
        else if (warriorName == "knight") { knightName = warriorName; knight++; }
        else if (warriorName == "Mongolian") { MongolianName = warriorName; Mongolian++; }
        else if (warriorName == "ottman") { ottmanName = warriorName; ottman++; }
        else if (warriorName == "persian") { persianName = warriorName; persian++; }
        else if (warriorName == "postion") { postionName = warriorName; postion++; }
        else if (warriorName == "sapper") { sapperName = warriorName; sapper++; }
        else if (warriorName == "Savge") { SavgeName = warriorName; Savge++; }
        else if (warriorName == "sneaky") { sneakyName = warriorName; sneaky++; }

        // استدعاء الحدث لإعلام الـ UI
        OnWatchChanged?.Invoke(currentCapacity, maxCapacity);
        //Debug.Log($"تمت إضافة محارب: {warriorName} | العدد الحالي في {buildingName}: {warriorStorage[warriorName]}"); ديبك
        return true;
    }

    public void RemoveWarrior(string warriorName)
    {
        if (warriorStorage.ContainsKey(warriorName) && warriorStorage[warriorName] > 0)
        {
            warriorStorage[warriorName]--;
            currentCapacity -= GetWarriorSpace(warriorName);

            if (warriorStorage[warriorName] == 0)
                warriorStorage.Remove(warriorName);

            // تحديث المتغيرات العامة لكل نوع
            if (warriorName == "bedoiun") { bedoiun--; if (bedoiun < 0) bedoiun = 0; }
            else if (warriorName == "ScalingEgyptian") { ScalingEgyptian--; if (ScalingEgyptian < 0) ScalingEgyptian = 0; }
            else if (warriorName == "Destroyer") { Destroyer--; if (Destroyer < 0) Destroyer = 0; }
            else if (warriorName == "Gaul") { Gaul--; if (Gaul < 0) Gaul = 0; }
            else if (warriorName == "Kamikaze") { Kamikaze--; if (Kamikaze < 0) Kamikaze = 0; }
            else if (warriorName == "knight") { knight--; if (knight < 0) knight = 0; }
            else if (warriorName == "Mongolian") { Mongolian--; if (Mongolian < 0) Mongolian = 0; }
            else if (warriorName == "ottman") { ottman--; if (ottman < 0) ottman = 0; }
            else if (warriorName == "persian") { persian--; if (persian < 0) persian = 0; }
            else if (warriorName == "postion") { postion--; if (postion < 0) postion = 0; }
            else if (warriorName == "sapper") { sapper--; if (sapper < 0) sapper = 0; }
            else if (warriorName == "Savge") { Savge--; if (Savge < 0) Savge = 0; }
            else if (warriorName == "sneaky") { sneaky--; if (sneaky < 0) sneaky = 0; }

            // استدعاء الحدث لإعلام الـ UI
            OnWatchChanged?.Invoke(currentCapacity, maxCapacity);
            //Debug.Log($"تمت إزالة محارب: {warriorName} | العدد المتبقي في {buildingName}: {(warriorStorage.ContainsKey(warriorName) ? warriorStorage[warriorName] : 0)}");  ديبك
        }
        else
        {
            Debug.Log($"لا يوجد محارب من هذا النوع في {buildingName}");
        }
    }

    private int GetWarriorSpace(string warriorName)
    {
        return warriorName switch
        {
            "bedoiun" => 10,
            "ScalingEgyptian" => 70,
            "Destroyer" => 20,
            "Gaul" => 160,
            "Kamikaze" => 60,
            "knight" => 15,
            "Mongolian" => 40,
            "ottman" => 10,
            "persian" => 20,
            "postion" => 30,
            "sapper" => 70,
            "Savge" => 20,
            "sneaky" => 30,
            _ => 0
        };
    }

    public void GetBarracksInfo()
    {
        Debug.Log($"محاربون في {buildingName}:");
        foreach (var warrior in warriorStorage)
        {
            Debug.Log($"{warrior.Key} | العدد: {warrior.Value}");
        }
    }

    public Dictionary<string, int> GetWarriorDictionary()
    {
        return new Dictionary<string, int>(warriorStorage);
    }

    public void ClearAllWarriors()
    {
        // تفريغ القاموس
        warriorStorage.Clear();

        // تصفير السعة
        currentCapacity = 0;

        // تصفير العدّادات المحلية
        bedoiun = 0; bdoyName = "";
        ScalingEgyptian = 0; archerName = "";
        Destroyer = 0; DestroyerName = "";
        Gaul = 0; GaulName = "";
        Kamikaze = 0; KamikazeName = "";
        knight = 0; knightName = "";
        Mongolian = 0; MongolianName = "";
        ottman = 0; ottmanName = "";
        persian = 0; persianName = "";
        postion = 0; postionName = "";
        sapper = 0; sapperName = "";
        Savge = 0; SavgeName = "";
        sneaky = 0; sneakyName = "";

        // تحديث الـ UI
        OnWatchChanged?.Invoke(currentCapacity, maxCapacity);

        Debug.Log($"🧹 تم تفريغ الجنود من {buildingName}");
    }

}
