using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class WarriorLevelManager : MonoBehaviour
{
    public static WarriorLevelManager Instance;

    private Dictionary<string, int> warriorLevels = new Dictionary<string, int>();

    private Dictionary<string, Coroutine> activeUpgrades = new Dictionary<string, Coroutine>();


    [Header("نصوص المحاربين لعرض المستويات")]
    public TMP_Text bedoiunText;
    public TMP_Text GaulText;
    public TMP_Text flyingOttomanText;
    public TMP_Text DestroyerText;
    public TMP_Text ScalingEgyptianText;
    public TMP_Text HelaerText;
    public TMP_Text KamikazeText;
    public TMP_Text knightText;
    public TMP_Text MongolianText;
    public TMP_Text ottmanText;
    public TMP_Text persianText;
    public TMP_Text postionText;
    public TMP_Text sapperText;
    public TMP_Text SavgeText;
    public TMP_Text sneakyText;

    [Header("نص وقت الانتظار قبل زيادة المستوى")]
    public TMP_Text upgradeDelayText; // ✅ هذا النص يحتوي على الوقت (مثلاً: "2")

    private Dictionary<string, TMP_Text> warriorTexts = new Dictionary<string, TMP_Text>();
    private List<string> warriorNames = new List<string>
    {
        "bedoiun", "Battering_Gaul", "flyingottoman", "Destroyer", "ScalingEgyptian",
        "GentleHealerFull", "Kamikaze", "KnightOnHorse", "ChineseMongolian", "Ottoman",
        "persian", "Poisonous", "PersianSapper", "EgyptianSavage", "Sneaky"
    };

    //public bool isUpgrading = false;
    private HashSet<string> warriorsBeingUpgraded = new HashSet<string>();


    void Awake()
    {
        Instance = this;

        foreach (string name in warriorNames)
        {
            int level = 1;
            warriorLevels[name] = level;
        }

        warriorTexts["bedoiun"] = bedoiunText;
        warriorTexts["Battering_Gaul"] = GaulText;
        warriorTexts["flyingottoman"] = flyingOttomanText;
        warriorTexts["Destroyer"] = DestroyerText;
        warriorTexts["ScalingEgyptian"] = ScalingEgyptianText;
        warriorTexts["GentleHealerFull"] = HelaerText;
        warriorTexts["Kamikaze"] = KamikazeText;
        warriorTexts["KnightOnHorse"] = knightText;
        warriorTexts["ChineseMongolian"] = MongolianText;
        warriorTexts["Ottoman"] = ottmanText;
        warriorTexts["persian"] = persianText;
        warriorTexts["Poisonous"] = postionText;
        warriorTexts["PersianSapper"] = sapperText;
        warriorTexts["EgyptianSavage"] = SavgeText;
        warriorTexts["Sneaky"] = sneakyText;

        UpdateAllUI();
    }

    // 🟢 التعديل الجوهري: دالة الترقية أصبحت فورية ولا تنتظر
    public void IncreaseLevel(string warriorName, TrainingBuilding building)
    {
        Debug.Log($"🚀 ترقية فورية للمحارب: {warriorName}");

        if (warriorLevels.ContainsKey(warriorName))
        {
            int currentLevel = warriorLevels[warriorName];
            if (currentLevel < 6)
            {
                currentLevel++;
                warriorLevels[warriorName] = currentLevel;
                UpdateUI(warriorName);
                Debug.Log($"✅ تم رفع مستوى {warriorName} إلى: {currentLevel}");
            }
        }
    }




    private IEnumerator UpgradeWithDelay(string warriorName, TrainingBuilding building, float forcedDelay)
    {
        warriorsBeingUpgraded.Add(warriorName);
        building.isBusy = true;

        float delay = forcedDelay > 0 ? forcedDelay : 0f;
        if (forcedDelay < 0 && upgradeDelayText != null)
            float.TryParse(upgradeDelayText.text, out delay);

        yield return new WaitForSeconds(delay);

        if (warriorLevels.ContainsKey(warriorName))
        {
            int currentLevel = warriorLevels[warriorName];
            if (currentLevel < 6)
            {
                currentLevel++;
                warriorLevels[warriorName] = currentLevel;
                UpdateUI(warriorName);
            }
        }

        warriorsBeingUpgraded.Remove(warriorName);
        activeUpgrades.Remove(warriorName);
        building.isBusy = false;
    }




    public int GetLevel(string warriorName)
    {
        return warriorLevels.ContainsKey(warriorName) ? warriorLevels[warriorName] : 1;
    }

    private void UpdateUI(string warriorName)
    {
        if (warriorTexts.ContainsKey(warriorName))
        {
            int level = warriorLevels[warriorName];
            warriorTexts[warriorName].text = level.ToString();
        }
    }

    private void UpdateAllUI()
    {
        foreach (string name in warriorNames)
        {
            UpdateUI(name);
        }
    }

    public void ResetAllLevels()
    {
        foreach (string name in warriorNames)
        {
            warriorLevels[name] = 1;
        }
        UpdateAllUI();
    }

    public void SpeedUpCurrentUpgrade(string warriorName, TrainingBuilding building)
    {
        if (!activeUpgrades.ContainsKey(warriorName))
            return;

        StopCoroutine(activeUpgrades[warriorName]);

        Coroutine c = StartCoroutine(
            UpgradeWithDelay(warriorName, building, 0.5f)
        );

        activeUpgrades[warriorName] = c;
    }

    public List<WarriorLevelData> GetWarriorLevelsForSave()
    {
        List<WarriorLevelData> data = new List<WarriorLevelData>();
        foreach (var entry in warriorLevels)
        {
            data.Add(new WarriorLevelData { name = entry.Key, level = entry.Value });
        }
        return data;
    }

    // 🟢 دالة لاستقبال البيانات من مدير الحفظ
    public void LoadWarriorLevels(List<WarriorLevelData> data)
    {
        warriorLevels.Clear();
        foreach (var item in data)
        {
            warriorLevels[item.name] = item.level;
        }
        UpdateAllUI(); // تحديث الواجهة فوراً
    }

}
