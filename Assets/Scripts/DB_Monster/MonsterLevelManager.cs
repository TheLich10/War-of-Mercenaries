using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MonsterLevelManager : MonoBehaviour
{
    public static MonsterLevelManager Instance;

    private Dictionary<string, int> monsterLevels = new Dictionary<string, int>();

    public Dictionary<string, int> monsterClicks = new Dictionary<string, int>();
    public Dictionary<string, bool> monsterIsWaiting = new Dictionary<string, bool>();
    public Dictionary<string, long> monsterEndTimeTicks = new Dictionary<string, long>();

    [Header("نصوص الوحوش لعرض المستويات")]
    public TMP_Text Wom_kong;
    public TMP_Text Mightosour;
    public TMP_Text BearWolf;
    public TMP_Text DraginFly;

    [Header("نص وقت الانتظار قبل زيادة المستوى")]
    public TMP_Text upgradeDelayText; // (مثلاً "1" للثانية الواحدة)

    private Dictionary<string, TMP_Text> monsterTexts = new Dictionary<string, TMP_Text>();
    private List<string> monsterNames = new List<string>
    {
        "Wom-kong", "Mightosour", "BearWolf", "DraginFly"
    };


    void Awake()
    {
        Instance = this;

        // تم نقل تحميل البيانات إلى دالة LoadMonstersData
        monsterTexts["Wom-kong"] = Wom_kong;
        monsterTexts["DraginFly"] = DraginFly;
        monsterTexts["BearWolf"] = BearWolf;
        monsterTexts["Mightosour"] = Mightosour;
    }

    public void IncreaseLevel(string monsterName)
    {
        StartCoroutine(UpgradeWithDelay(monsterName));
    }

    private IEnumerator UpgradeWithDelay(string monsterName)
    {
        float delay = 0f; // القيمة الافتراضية
        if (upgradeDelayText != null)
        {
            float.TryParse(upgradeDelayText.text, out delay);
        }

        yield return new WaitForSeconds(delay);

        if (!monsterLevels.ContainsKey(monsterName)) yield break;

        int currentLevel = monsterLevels[monsterName];
        if (currentLevel < 6)
        {
            currentLevel++;
            monsterLevels[monsterName] = currentLevel;
            PlayerPrefs.SetInt("MonsterLevel_" + monsterName, currentLevel);
            PlayerPrefs.Save();
            UpdateUI(monsterName);

            // ✅ السطر المضاف لعرض اسم الوحش والمستوى الجديد في الـ Console
            Debug.Log($"upgrade monster  : {monsterName} to level : {currentLevel}");
        }
    }


    public int GetLevel(string monsterName)
    {
        return monsterLevels.ContainsKey(monsterName) ? monsterLevels[monsterName] : 1;
    }

    private void UpdateUI(string monsterName)
    {
        if (monsterTexts.ContainsKey(monsterName))
        {
            int level = monsterLevels[monsterName];
            monsterTexts[monsterName].text = level.ToString();

        }
    }

    private void UpdateAllUI()
    {
        foreach (string name in monsterNames)
        {
            UpdateUI(name);
        }
    }

    public void ResetAllLevels()
    {
        foreach (string name in monsterNames)
        {
            monsterLevels[name] = 1;
            PlayerPrefs.SetInt("MonsterLevel_" + name, 1);
        }
        PlayerPrefs.Save();
        UpdateAllUI();
    }

    public void LoadMonstersData(List<MonsterSaveData> savedData)
    {
        foreach (var data in savedData)
        {
            monsterLevels[data.monsterName] = data.level;
            monsterClicks[data.monsterName] = data.currentClicks;
            monsterIsWaiting[data.monsterName] = data.isWaiting;
            monsterEndTimeTicks[data.monsterName] = data.upgradeEndTimeTicks;
        }
        UpdateAllUI();

        // 🟢 الأسطر الجديدة: إجبار واجهة التطوير على تحديث الصور فوراً بعد تحميل البيانات
        if (UpgradeMonsterButton.instain != null && UpgradeMonsterButton.instain.gameObject.activeInHierarchy)
        {
            string currentName = UpgradeMonsterButton.instain.monsterNameText.text;
            if (!string.IsNullOrEmpty(currentName))
            {
                UpgradeMonsterButton.instain.UpdateUpgradeStepsVisual(currentName);
            }
        }
        // داخل دالة LoadMonstersData في نهايتها:
        if (UpgradeMonsterButton.instain != null && UpgradeMonsterButton.instain.gameObject.activeInHierarchy)
        {
            UpgradeMonsterButton.instain.ForceRefreshUI(); // 🟢 إجبار التحديث
        }
    }

    public List<MonsterSaveData> GetMonstersDataForSave()
    {
        List<MonsterSaveData> list = new List<MonsterSaveData>();
        foreach (string name in monsterNames)
        {
            list.Add(new MonsterSaveData
            {
                monsterName = name,
                level = GetLevel(name),
                currentClicks = monsterClicks.ContainsKey(name) ? monsterClicks[name] : 0,
                isWaiting = monsterIsWaiting.ContainsKey(name) ? monsterIsWaiting[name] : false,
                upgradeEndTimeTicks = monsterEndTimeTicks.ContainsKey(name) ? monsterEndTimeTicks[name] : 0
            });
        }
        return list;
    }
}
