using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.UI;

public class UpgradeWarriorButton : MonoBehaviour
{
    public static UpgradeWarriorButton Instance;

    public TMP_Text RequrePower;
    public TMP_Text GoldFinsh;

    public TrainingBuilding currentBuilding;

    private System.Collections.Generic.Dictionary<TrainingBuilding, UpgradeBarFollow> activeBars = new System.Collections.Generic.Dictionary<TrainingBuilding, UpgradeBarFollow>();
    private System.Collections.Generic.Dictionary<TrainingBuilding, bool> busyBuildings = new System.Collections.Generic.Dictionary<TrainingBuilding, bool>();
    private System.Collections.Generic.Dictionary<TrainingBuilding, string> upgradingWarriors = new System.Collections.Generic.Dictionary<TrainingBuilding, string>();
    private System.Collections.Generic.Dictionary<TrainingBuilding, Coroutine> activeCoroutines = new System.Collections.Generic.Dictionary<TrainingBuilding, Coroutine>();

    // 🟢 قواميس جديدة لحفظ الوقت الكلي ووقت الانتهاء في المستقبل لكل مبنى
    private System.Collections.Generic.Dictionary<TrainingBuilding, long> upgradeEndTimes = new System.Collections.Generic.Dictionary<TrainingBuilding, long>();
    private System.Collections.Generic.Dictionary<TrainingBuilding, float> upgradeTotalDurations = new System.Collections.Generic.Dictionary<TrainingBuilding, float>();

    [System.Serializable]
    public struct WarriorButton
    {
        public string warriorName;
        public Button button;
    }

    [Header("أزرار الشخصيات")]
    public WarriorButton[] warriorButtonsList;

    void Awake()
    {
        Instance = this;
    }

    [Header("الربط من الـ Inspector")]
    public TMP_Text warriorNameText;
    public TMP_Text waitTimeText;
    public GameObject panelToClose;
    public GameObject upgradeBarPrefab;

    public void OnUpgradeButtonClicked()
    {
        if (currentBuilding != null && busyBuildings.ContainsKey(currentBuilding) && busyBuildings[currentBuilding])
        {
            //Debug.LogWarning("هذا المبنى يقوم بتطوير محارب بالفعل!");
            PopupText.instan.ShowPopup("Warrior under development");
            return;
        }

        string fullName = warriorNameText.text;
        string cleanName = RemoveLevelInfo(fullName);
        float waitTime = ConvertTextToSeconds(waitTimeText.text);
        int RequirePower = int.Parse(RequrePower.text);

        Debug.Log("the name is : " + cleanName);

        busyBuildings[currentBuilding] = true;
        upgradingWarriors[currentBuilding] = cleanName;

        // 🟢 حفظ وقت الانتهاء المستقبلي والمدة الكلية
        upgradeEndTimes[currentBuilding] = System.DateTime.Now.AddSeconds(waitTime).Ticks;
        upgradeTotalDurations[currentBuilding] = waitTime;

        ResourceManager.RemovePower(RequirePower);
        SetWarriorButtonState(cleanName, false);

        ShowUpgradeBar(waitTime, cleanName, currentBuilding);

        if (panelToClose != null)
            panelToClose.SetActive(false);
    }

    private string RemoveLevelInfo(string fullName)
    {
        return Regex.Replace(fullName, @"\s*\(level\s[1-6]\)", "", RegexOptions.IgnoreCase).Trim();
    }

    private void ShowUpgradeBar(float duration, string warriorName, TrainingBuilding building)
    {
        Coroutine c = StartCoroutine(ShowUpgradeBarCoroutine(duration, warriorName, building));
        activeCoroutines[building] = c;
    }

    private IEnumerator ShowUpgradeBarCoroutine(float duration, string warriorName, TrainingBuilding building)
    {
        GameObject canvasObject = GameObject.FindWithTag("MainCanvas");
        if (canvasObject == null || upgradeBarPrefab == null)
            yield break;

        if (building == null) yield break;

        GameObject bar = Instantiate(upgradeBarPrefab, canvasObject.transform);
        bar.transform.SetAsFirstSibling();

        UpgradeBarFollow newBar = bar.GetComponent<UpgradeBarFollow>();
        if (newBar != null)
        {
            activeBars[building] = newBar;
            newBar.StartUpgradeBar(building.transform, duration);
        }

        yield return new WaitForSeconds(duration);
        FinishUpgrade(warriorName, building);
    }

    private void FinishUpgrade(string warriorName, TrainingBuilding building)
    {
        WarriorLevelManager.Instance.IncreaseLevel(warriorName, building);
        SetWarriorButtonState(warriorName, true);

        if (busyBuildings.ContainsKey(building)) busyBuildings[building] = false;
        if (activeCoroutines.ContainsKey(building)) activeCoroutines.Remove(building);
        if (upgradingWarriors.ContainsKey(building)) upgradingWarriors.Remove(building);
        if (activeBars.ContainsKey(building)) activeBars.Remove(building);

        // 🟢 تنظيف قواميس الوقت
        if (upgradeEndTimes.ContainsKey(building)) upgradeEndTimes.Remove(building);
        if (upgradeTotalDurations.ContainsKey(building)) upgradeTotalDurations.Remove(building);
    }

    // 🟢 قمنا بإضافة المعامل TrainingBuilding building لكي يعرف الزر من يسرع
    public void SpeedUpUpgrade(TrainingBuilding targetBuilding)
    {
        // 1. التحقق من أن هذا المبنى مشغول بالفعل
        if (targetBuilding == null || !busyBuildings.ContainsKey(targetBuilding) || !busyBuildings[targetBuilding])
        {
            Debug.LogWarning("هذا المبنى غير مشغول حالياً!");
            return;
        }

        string cleanName = upgradingWarriors.ContainsKey(targetBuilding) ? upgradingWarriors[targetBuilding] : "";
        if (string.IsNullOrEmpty(cleanName)) return;

        // 2. تسريع الترقية في الـ WarriorLevelManager
        WarriorLevelManager.Instance.SpeedUpCurrentUpgrade(cleanName, targetBuilding);

        // 3. تسريع البار الخاص بهذا المبنى تحديداً
        if (activeBars.ContainsKey(targetBuilding) && activeBars[targetBuilding] != null)
        {
            activeBars[targetBuilding].ForceFinishIn(0.1f); // وقت قصير جداً لإنهاء البار
        }

        // 4. إيقاف الكوروتين الخاص بهذا المبنى فقط
        if (activeCoroutines.ContainsKey(targetBuilding) && activeCoroutines[targetBuilding] != null)
        {
            StopCoroutine(activeCoroutines[targetBuilding]);
        }

        // 5. إنهاء الترقية وتحديث المستوى
        FinishUpgrade(cleanName, targetBuilding);

        // 6. خصم الذهب
        string rawText = GoldFinsh.text;
        string numberString = Regex.Match(rawText, @"\d+").Value;
        if (int.TryParse(numberString, out int goldToRemove))
        {
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(goldToRemove);
        }
    }
    private void SetWarriorButtonState(string wName, bool state)
    {
        foreach (var wb in warriorButtonsList)
        {
            if (wb.warriorName.Equals(wName, System.StringComparison.OrdinalIgnoreCase))
            {
                if (wb.button != null) wb.button.gameObject.SetActive(state);
                break;
            }
        }
    }

    float ConvertTextToSeconds(string timeString)
    {
        timeString = timeString.Replace("\u200B", "").Replace(" ", "").Trim().ToUpper();
        if (string.IsNullOrEmpty(timeString)) return 10f;

        char timeUnit = timeString[timeString.Length - 1];
        string numberString = timeString.Substring(0, timeString.Length - 1);

        if (float.TryParse(numberString, out float timeValue))
        {
            switch (timeUnit)
            {
                case 'S': return timeValue;
                case 'M': return timeValue * 60f;
                case 'H': return timeValue * 3600f;
                case 'D': return timeValue * 86400f;
                default:
                    if (float.TryParse(timeString, out float rawValue)) return rawValue;
                    return 300f;
            }
        }
        return 300f;
    }

    // ==========================================
    // 🟢 دوال جديدة خاصة بنظام الحفظ والتحميل 🟢
    // ==========================================

    // استئناف التطوير بعد فتح اللعبة
    public void ResumeWarriorUpgrade(TrainingBuilding building, string warriorName, long endTimeTicks, float totalDuration)
    {
        System.TimeSpan diff = new System.DateTime(endTimeTicks) - System.DateTime.Now;
        float remainingSeconds = (float)diff.TotalSeconds;

        if (remainingSeconds > 0)
        {
            busyBuildings[building] = true;
            upgradingWarriors[building] = warriorName;
            upgradeEndTimes[building] = endTimeTicks;
            upgradeTotalDurations[building] = totalDuration;

            SetWarriorButtonState(warriorName, false);

            Coroutine c = StartCoroutine(ResumeUpgradeBarCoroutine(remainingSeconds, totalDuration, warriorName, building));
            activeCoroutines[building] = c;
        }
        else
        {
            // الوقت انتهى أثناء النوم! نرقي فوراً
            FinishUpgrade(warriorName, building);
        }
    }

    // كوروتين لإظهار البار واستئناف الوقت من النسبة الصحيحة
    private IEnumerator ResumeUpgradeBarCoroutine(float remainingTime, float totalTime, string warriorName, TrainingBuilding building)
    {
        GameObject canvasObject = GameObject.FindWithTag("MainCanvas");
        if (canvasObject == null || upgradeBarPrefab == null || building == null) yield break;

        GameObject bar = Instantiate(upgradeBarPrefab, canvasObject.transform);
        bar.transform.SetAsFirstSibling();

        UpgradeBarFollow newBar = bar.GetComponent<UpgradeBarFollow>();
        if (newBar != null)
        {
            activeBars[building] = newBar;
            newBar.ResumeUpgradeBar(building.transform, totalTime, remainingTime);
        }

        yield return new WaitForSeconds(remainingTime);
        FinishUpgrade(warriorName, building);
    }

    // دوال مساعدة لمدير الحفظ لقراءة البيانات
    public bool IsBuildingBusy(TrainingBuilding building) => busyBuildings.ContainsKey(building) && busyBuildings[building];
    public string GetUpgradingWarrior(TrainingBuilding building) => upgradingWarriors.ContainsKey(building) ? upgradingWarriors[building] : "";
    public long GetEndTime(TrainingBuilding building) => upgradeEndTimes.ContainsKey(building) ? upgradeEndTimes[building] : 0;
    public float GetTotalDuration(TrainingBuilding building) => upgradeTotalDurations.ContainsKey(building) ? upgradeTotalDurations[building] : 0f;

    // 🟢 كوروتين جديد للانتظار حتى يكتمل ظهور المبنى عند التحميل
    public IEnumerator WaitAndResumeWarrior(GameObject rootBuilding, string warriorName, long endTimeTicks, float totalDuration)
    {
        TrainingBuilding tBuilding = null;

        // ننتظر حتى يتم إنشاء المجسم (Model) ويظهر سكربت TrainingBuilding
        while (tBuilding == null)
        {
            if (rootBuilding == null) yield break; // حماية إذا تم تدمير المبنى فجأة

            // البحث عن السكربت حتى لو كان في كائن مخفي مؤقتاً
            tBuilding = rootBuilding.GetComponentInChildren<TrainingBuilding>(true);

            yield return new WaitForSeconds(0.2f); // ننتظر أجزاء من الثانية ونجرب مجدداً
        }

        // بمجرد أن نجده، نرسل له البيانات لاستئناف التطوير والبار
        ResumeWarriorUpgrade(tBuilding, warriorName, endTimeTicks, totalDuration);
    }
}