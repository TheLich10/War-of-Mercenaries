using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Text.RegularExpressions;

public class UpgradeMonsterButton : MonoBehaviour
{
    public static UpgradeMonsterButton instain;

    [Header("الربط من الـ Inspector")]
    public TMP_Text monsterNameText;    // اسم الوحش الظاهر
    public TMP_Text waitTimeText;       // وقت الانتظار إن وجد
    public Image[] upgradeSteps;        // 20 صورة تمثل الأسهم
    public Button upgradeButton;        // زر التطوير

    public Color inactiveColor = new Color(0.4f, 0.4f, 0.4f); // رمادي
    public Color activeColor = Color.white; // أبيض

    private bool isWaiting = false;  // لتجنب الضغط المتكرر خلال الانتظار
    private float currentTimer; // متغير جديد للتحكم بالوقت من الخارج

    public GameObject panalClose;
    public GameObject BTNSeedUp;
    public GameObject GoldBtnHide;

    public TMP_Text requrie1;
    public TMP_Text requrie2;
    public TMP_Text GoldReqSpeed;

    private string lastCheckedMonsterName = "";

    private void Awake()
    {
        instain = this;
    }

    private void Update()
    {
        // 🟢 هذا الكود سيعمل فقط لحظة تغير اسم الوحش، مما يضمن أن البيانات دقيقة 100%
        if (monsterNameText != null && monsterNameText.text != lastCheckedMonsterName)
        {
            lastCheckedMonsterName = monsterNameText.text;
            UpdateUpgradeStepsVisual(lastCheckedMonsterName);
            CheckTimerState(lastCheckedMonsterName); // 🟢 فحص وتشغيل العداد هنا بدلاً من OnEnable
        }
    }

    // 🟢 دالة جديدة تتكفل بفحص وتشغيل العداد التنازلي للوحش المطلوب
    public void CheckTimerState(string currentMonster)
    {
        StopAllCoroutines(); // إيقاف أي عداد قديم لمنع التداخل

        if (MonsterLevelManager.Instance.monsterIsWaiting.ContainsKey(currentMonster) &&
            MonsterLevelManager.Instance.monsterIsWaiting[currentMonster])
        {
            long endTime = MonsterLevelManager.Instance.monsterEndTimeTicks[currentMonster];
            long currentTicks = System.DateTime.Now.Ticks;

            if (endTime > currentTicks)
            {
                float remainingSeconds = (float)((endTime - currentTicks) / (double)System.TimeSpan.TicksPerSecond);

                isWaiting = true;
                upgradeButton.interactable = false;
                BTNSeedUp.SetActive(true);
                GoldBtnHide.SetActive(false);

                StartCoroutine(WaitThenIncreaseClicks(remainingSeconds, currentMonster));
            }
            else
            {
                isWaiting = true;
                StartCoroutine(WaitThenIncreaseClicks(0f, currentMonster));
            }
        }
        else
        {
            upgradeButton.interactable = true;
            waitTimeText.text = "";
            isWaiting = false;
            BTNSeedUp.SetActive(false);
            GoldBtnHide.SetActive(true);
        }
    }

    private void OnEnable()
    {
        // 🟢 بمجرد فتح النافذة، نجعل الاسم فارغاً، لتضطر دالة Update لتحديث العداد والصور فوراً
        lastCheckedMonsterName = "";
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void OnUpgradeButtonClicked()
    {
        if (isWaiting) return;

        string monsterName = monsterNameText.text;
        int currentLevel = MonsterLevelManager.Instance.GetLevel(monsterName);

        if (currentLevel >= 6)
        {
            Debug.Log($"🟡 الوحش {monsterName} وصل إلى الحد الأقصى. لن يتم سحب أي جنود.");
            return;
        }

        int requiredUnits1 = 0;
        int requiredUnits2 = 0;

        if (requrie1 != null) int.TryParse(requrie1.text, out requiredUnits1);
        if (requrie2 != null) int.TryParse(requrie2.text, out requiredUnits2);

        if (monsterName == "Mightosour")
        {
            for (int i = 0; i < requiredUnits1; i++) BTNSBarracks.Instance.RemoveUnitMons("bedoiun");
            for (int i = 0; i < requiredUnits2; i++) BTNSBarracks.Instance.RemoveUnitMons("postion");
        }
        else if (monsterName == "BearWolf")
        {
            for (int i = 0; i < requiredUnits1; i++) BTNSBarracks.Instance.RemoveUnitMons("Savge");
            for (int i = 0; i < requiredUnits2; i++) BTNSBarracks.Instance.RemoveUnitMons("Mongolian");
        }
        else if (monsterName == "Wom-kong")
        {
            for (int i = 0; i < requiredUnits1; i++) BTNSBarracks.Instance.RemoveUnitMons("Kamikaze");
            for (int i = 0; i < requiredUnits2; i++) BTNSBarracks.Instance.RemoveUnitMons("Destroyer");
        }
        else if (monsterName == "DraginFly")
        {
            for (int i = 0; i < requiredUnits1; i++) BTNSBarracks.Instance.RemoveUnitMons("knight");
            for (int i = 0; i < requiredUnits2; i++) BTNSBarracks.Instance.RemoveUnitMons("sneaky");
        }

        isWaiting = true;
        upgradeButton.interactable = false;
        BTNSeedUp.SetActive(true);
        GoldBtnHide.SetActive(false);

        float durationInSeconds = 86400f; // 24 ساعة
        long durationTicks = (long)(durationInSeconds * System.TimeSpan.TicksPerSecond);
        long endTimeTicks = System.DateTime.Now.Ticks + durationTicks;

        MonsterLevelManager.Instance.monsterIsWaiting[monsterName] = true;
        MonsterLevelManager.Instance.monsterEndTimeTicks[monsterName] = endTimeTicks;

        StartCoroutine(WaitThenIncreaseClicks(durationInSeconds, monsterName));
    }

    public void OnInstantUpgradeButtonClicked()
    {
        string monsterName = monsterNameText.text;
        int currentLevel = MonsterLevelManager.Instance.GetLevel(monsterName);

        if (currentLevel >= 6) return;

        int requiredClicks = currentLevel + 1;
        int currentClicks = MonsterLevelManager.Instance.monsterClicks.ContainsKey(monsterName) ? MonsterLevelManager.Instance.monsterClicks[monsterName] : 0;

        currentClicks++;

        if (currentClicks >= requiredClicks)
        {
            MonsterLevelManager.Instance.IncreaseLevel(monsterName);
            MonsterLevelManager.Instance.monsterClicks[monsterName] = 0;
            if (panalClose != null) panalClose.SetActive(false);
        }
        else
        {
            MonsterLevelManager.Instance.monsterClicks[monsterName] = currentClicks;
        }

        UpdateUpgradeStepsVisual(monsterName);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(200);
    }

    private IEnumerator WaitThenIncreaseClicks(float waitTime, string monsterName)
    {
        currentTimer = waitTime;
        while (currentTimer > 0)
        {
            int hours = Mathf.FloorToInt(currentTimer / 3600);
            int minutes = Mathf.FloorToInt((currentTimer % 3600) / 60);
            int seconds = Mathf.FloorToInt(currentTimer % 60);

            waitTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

            yield return new WaitForSeconds(1f);
            currentTimer -= 1f;
        }
        waitTimeText.text = "";

        MonsterLevelManager.Instance.monsterIsWaiting[monsterName] = false;
        MonsterLevelManager.Instance.monsterEndTimeTicks[monsterName] = 0;

        int currentLevel = MonsterLevelManager.Instance.GetLevel(monsterName);
        if (currentLevel < 6)
        {
            int requiredClicks = currentLevel + 1;
            int currentClicks = MonsterLevelManager.Instance.monsterClicks.ContainsKey(monsterName)
                                ? MonsterLevelManager.Instance.monsterClicks[monsterName] : 0;

            currentClicks++;

            if (currentClicks >= requiredClicks)
            {
                MonsterLevelManager.Instance.IncreaseLevel(monsterName);
                MonsterLevelManager.Instance.monsterClicks[monsterName] = 0;
                if (panalClose != null) panalClose.SetActive(false);
            }
            else
            {
                MonsterLevelManager.Instance.monsterClicks[monsterName] = currentClicks;
            }
        }

        UpdateUpgradeStepsVisual(monsterName);

        isWaiting = false;
        upgradeButton.interactable = true;
        BTNSeedUp.SetActive(false);
        GoldBtnHide.SetActive(true);
    }

    public void UpdateUpgradeStepsVisual(string monsterName)
    {
        int currentLevel = MonsterLevelManager.Instance.GetLevel(monsterName);
        int currentClicks = MonsterLevelManager.Instance.monsterClicks.ContainsKey(monsterName)
                            ? MonsterLevelManager.Instance.monsterClicks[monsterName] : 0;

        int totalSteps = 0;
        int startingLevel = 1;

        for (int level = startingLevel; level < currentLevel; level++)
        {
            totalSteps += level + 1;
        }

        totalSteps += currentClicks;

        for (int i = 0; i < upgradeSteps.Length; i++)
        {
            if (i < totalSteps)
                upgradeSteps[i].color = activeColor;
            else
                upgradeSteps[i].color = inactiveColor;
        }
    }

    public void ResetCurrentMonsterProgress()
    {
        string[] targetMonsters = { "Wom-kong", "BearWolf", "DraginFly", "Mightosour" };

        foreach (string monster in targetMonsters)
        {
            MonsterLevelManager.Instance.monsterClicks[monster] = 0;
        }

        if (monsterNameText != null && !string.IsNullOrEmpty(monsterNameText.text))
        {
            UpdateUpgradeStepsVisual(monsterNameText.text);
        }
    }

    public void SpeedUpWaitTime()
    {
        if (isWaiting && currentTimer > 0)
        {
            string rawText = GoldReqSpeed.text;
            string numberString = Regex.Match(rawText, @"\d+").Value;

            if (int.TryParse(numberString, out int goldToRemove))
            {
                AudioManager.Instance.PlaySFX("Gold_buy");
                GoldCollector.RemoveGold(goldToRemove);
            }

            currentTimer = 0.1f;
            MonsterLevelManager.Instance.monsterEndTimeTicks[monsterNameText.text] = System.DateTime.Now.Ticks;
        }
    }

    // 🟢 دالة لتحديث الواجهة يدوياً عند تحميل اللعبة والنافذة مفتوحة
    public void ForceRefreshUI()
    {
        lastCheckedMonsterName = "";
    }
}