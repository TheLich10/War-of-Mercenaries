using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class openNewWorior : MonoBehaviour
{
    public static openNewWorior instain;

    public static bool OtomanIsOpen = false;
    public GameObject OtmanIcon;
    public Image OtmanBarFill;

    public static bool KnightIsOpen = false;
    public GameObject KinghtIcon;
    public Image KnightBarFill;

    public static bool PersonIsOpen = false;
    public GameObject PersonIcon;
    public Image PersonBarFill;

    public static bool KamkaziIsOpen = false;
    public GameObject LamakziIcon;
    public Image KamkaziBarFill;

    public static bool SavgeIsOpen = false;
    public GameObject SavgeIcon;
    public Image SavgeBarFill;

    public static bool DestroyIsOpen = false;
    public GameObject DestoryIcon;
    public Image DestoryBarFill;

    public static bool PositionIsOpen = false;
    public GameObject PostionIcon;
    public Image PostionBarFill;

    public static bool SneakyIsOpen = false;
    public GameObject SneakyIcon;
    public Image SneakyBarFill;

    public static bool MongaleIsOpen = false;
    public GameObject MongalIcon;
    public Image MonglaBarFill;

    public static bool EgypetIsOpen = false;
    public GameObject EgypetIcon;
    public Image BarFill;

    public static bool HealerIsOpen = false;
    public GameObject HealerIcon;
    public Image HealerBarFill;

    public static bool SapperIsOpen = false;
    public GameObject SaperIcon;
    public Image SapperBarFill;

    public static bool FlayingIsOpen = false;
    public GameObject FlayIcon;
    public Image FlayBarFill;

    public static bool GaulIsOpen = false;
    public GameObject GaulIcon;
    public Image GaulBarFill;

    [Header("زر الفتح العام")]
    public Button OpenButton;

    [Header("زر الإنهاء الآن")]
    public Button FinishButton;
    public TMP_Text textButons;

    [Header("النصوص")]
    public TMP_Text powerStorageText;
    public TMP_Text GolldText;

    [Header("Upgrade Bar Prefab")]
    public GameObject upgradeBarPrefab;

    public int[] goldValues = { 24, 69, 100, 180, 255, 340, 460, 550, 669, 755, 898, 1000, 1200, 1500 };
    [Header("أوقات فتح المحاربين")]
    public float[] unlockTimes = { 900f, 3600f, 18000f, 43200f, 72000f, 86400f, 129600f, 172800f, 216000f, 259200f, 302400f, 345600f, 388800f, 432000f };

    [Header("تكلفة المحاربين")]
    public TMP_Text requiredPowerOtoman;
    public TMP_Text requiredPowerKnight;
    public TMP_Text requiredPowerPerson;
    public TMP_Text requiredPowerKamkazi;
    public TMP_Text requiredPowerSavge;
    public TMP_Text requiredPowerDestroy;
    public TMP_Text requiredPowerPosition;
    public TMP_Text requiredPowerSneaky;
    public TMP_Text requiredPowerMongal;
    public TMP_Text requiredPowerEgypet;
    public TMP_Text requiredPowerHealer;
    public TMP_Text requiredPowerSapper;
    public TMP_Text requiredPowerFlaying;
    public TMP_Text requiredPowerGaul;

    public TMP_Text Timer;
    public static int townHallLevel = 1;
    private building townHallBuilding;

    public int currentIndex = 0;

    private bool forceSpeedUp = false;
    private GameObject currentOverheadBar;
    public TMP_Text GoldTimer;

    [Header("نافذة المحارب الجديد (Panel)")]
    public GameObject newWarriorPanel;
    public Image newWarriorImage;
    public TMP_Text newWarriorNameText;

    [Header("بيانات المحاربين للبانل")]
    public string[] warriorNames = { "JANISSARY", "Knight", "Person", "Chinese Kamikaze", "Egyptian Savage", "Destroyer Knight", "Poisonous Aztec", "Sneaky Roman", "Chinese Mongolian", "Scaling Egyptian", "Gentle Healer", "Persian Sapper", "flying ottoman", "Battering Gaul" };

    public string[] warriorImagePaths = {
        "Warriors/Otoman", "Warriors/Knight", "Warriors/Person", "Warriors/Kamkazi",
        "Warriors/Savge", "Warriors/Destroy", "Warriors/Position", "Warriors/Sneaky",
        "Warriors/Mongale", "Warriors/Egypet", "Warriors/Healer", "Warriors/Sapper",
        "Warriors/Flaying", "Warriors/Gaul"
    };

    [Header("متغيرات الحفظ")]
    public bool isUnlocking = false;
    public long unlockEndTimeTicks = 0;
    public float unlockTotalDuration = 0f;

    // 🟢 الحماية 1: متغير لضمان عدم تشغيل أكثر من عداد في نفس الوقت
    private Coroutine activeWarriorRoutine;

    private void Awake()
    {
        instain = this;
    }

    void Start()
    {
        GameObject townHallObj = GameObject.FindGameObjectWithTag("NewWorr");
        GolldText.text = "RECRUIT   " + goldValues[currentIndex].ToString();

        if (townHallObj != null)
        {
            townHallBuilding = townHallObj.GetComponent<building>();
            townHallLevel = townHallBuilding.level;
        }

        Timer.gameObject.SetActive(false);

        // 🟢 منع تشغيل العداد إذا كان يعمل بالفعل
        OpenButton.onClick.AddListener(() => {
            if (!isUnlocking)
            {
                if (activeWarriorRoutine != null) StopCoroutine(activeWarriorRoutine);
                activeWarriorRoutine = StartCoroutine(OpenNextWarriorRoutine());
            }
        });
    }

    void Update()
    {
        TMP_Text requiredText = GetCurrentRequiredText();
        if (requiredText != null) CheckResources(requiredText);

        if (currentIndex >= GetMaxWarriorsAllowed())
            OpenButton.interactable = false;

        if (currentIndex >= 14)
        {
            FinishButton.gameObject.SetActive(false);
            textButons.text = "MAX LEVEL";
        }
    }

    public void FinishOpenWarrior()
    {
        if (currentIndex >= GetMaxWarriorsAllowed()) return;

        Image currentBar = GetCurrentBarFill();
        GameObject currentIcon = GetCurrentIcon();

        if (currentIcon != null) Destroy(currentIcon);
        if (currentBar != null) Destroy(currentBar.gameObject);

        switch (currentIndex)
        {
            case 0: OtomanIsOpen = true; break;
            case 1: KnightIsOpen = true; break;
            case 2: PersonIsOpen = true; break;
            case 3: KamkaziIsOpen = true; break;
            case 4: SavgeIsOpen = true; break;
            case 5: DestroyIsOpen = true; break;
            case 6: PositionIsOpen = true; break;
            case 7: SneakyIsOpen = true; break;
            case 8: MongaleIsOpen = true; break;
            case 9: EgypetIsOpen = true; break;
            case 10: HealerIsOpen = true; break;
            case 11: SapperIsOpen = true; break;
            case 12: FlayingIsOpen = true; break;
            case 13: GaulIsOpen = true; break;
        }

        Debug.Log("⚡ تم فتح المحارب فورًا رقم: " + currentIndex);
        isUnlocking = false;
        ShowNewWarriorPanel(currentIndex);
        currentIndex++;

        if (GolldText != null && currentIndex < goldValues.Length)
            GolldText.text = "RECRUIT   " + goldValues[currentIndex].ToString();
        else if (GolldText != null)
        {
            GolldText.text = "MAX LEVEL";
            FinishButton.gameObject.SetActive(false);
        }
    }

    private bool buildingNotFoundLogged = false;

    int GetMaxWarriorsAllowed()
    {
        GameObject townHallObj = GameObject.FindGameObjectWithTag("NewWorr");
        if (townHallObj != null)
        {
            townHallBuilding = townHallObj.GetComponent<building>();
            townHallLevel = townHallBuilding.level;
        }
        else
        {
            if (!buildingNotFoundLogged)
            {
                buildingNotFoundLogged = true;
            }
            return 0;
        }

        switch (townHallLevel)
        {
            case 1: return 3;
            case 2: return 7;
            case 3: return 11;
            case 4: return 14;
            default: return 0;
        }
    }

    void CheckResources(TMP_Text option)
    {
        if (option == null || powerStorageText == null) return;

        Color okColor = Color.white;
        Color failColor = Color.red;

        int requiredPower = 0;
        int currentPower = 0;

        int.TryParse(option.text, out requiredPower);
        int.TryParse(powerStorageText.text, out currentPower);

        if (currentPower < requiredPower)
        {
            option.color = failColor;
            OpenButton.interactable = false;
        }
        else
        {
            option.color = okColor;
            OpenButton.interactable = true;
        }
    }

    TMP_Text GetCurrentRequiredText()
    {
        switch (currentIndex)
        {
            case 0: return requiredPowerOtoman;
            case 1: return requiredPowerKnight;
            case 2: return requiredPowerPerson;
            case 3: return requiredPowerKamkazi;
            case 4: return requiredPowerSavge;
            case 5: return requiredPowerDestroy;
            case 6: return requiredPowerPosition;
            case 7: return requiredPowerSneaky;
            case 8: return requiredPowerMongal;
            case 9: return requiredPowerEgypet;
            case 10: return requiredPowerHealer;
            case 11: return requiredPowerSapper;
            case 12: return requiredPowerFlaying;
            case 13: return requiredPowerGaul;
            default: return null;
        }
    }

    IEnumerator OpenNextWarriorRoutine(float loadedRemainingTime = -1f, float loadedTotalTime = -1f)
    {
        // 🟢 التعديل السحري: إذا كان هذا استئنافاً من الحفظ، نتجاوز الفحص لأن اللاعب دفع الذهب مسبقاً!
        if (loadedRemainingTime <= 0)
        {
            if (currentIndex >= GetMaxWarriorsAllowed())
            {
                Debug.Log("❌ وصلت للحد الأقصى حسب مستوى المبنى");
                yield break;
            }
        }
        OpenButton.interactable = false;
        FinishButton.gameObject.SetActive(false);
        textButons.text = "RECRUIT";
        OpenButton.gameObject.SetActive(false);

        float currentDuration = (loadedTotalTime > 0) ? loadedTotalTime : unlockTimes[currentIndex];
        float remainingSeconds = (loadedRemainingTime > 0) ? loadedRemainingTime : currentDuration;

        isUnlocking = true;
        unlockTotalDuration = currentDuration;

        if (loadedRemainingTime <= 0)
            unlockEndTimeTicks = System.DateTime.Now.AddSeconds(currentDuration).Ticks;
        else
            unlockEndTimeTicks = System.DateTime.Now.AddSeconds(loadedRemainingTime).Ticks;

        StartCoroutine(ShowUpgradeBarOverBuilding(remainingSeconds, currentDuration));

        Image currentBar = GetCurrentBarFill();
        GameObject currentIcon = GetCurrentIcon();
        if (currentIcon != null) Destroy(currentIcon);

        if (currentBar != null)
        {
            forceSpeedUp = false;

            while (remainingSeconds > 0)
            {
                if (forceSpeedUp)
                {
                    remainingSeconds = 0.1f;
                    forceSpeedUp = false;

                    if (currentOverheadBar != null) Destroy(currentOverheadBar);

                    string rawText = GoldTimer.text;
                    string numberString = Regex.Match(rawText, @"\d+").Value;
                    if (int.TryParse(numberString, out int goldToRemove))
                    {
                        AudioManager.Instance.PlaySFX("Gold_buy");
                        GoldCollector.RemoveGold(goldToRemove);
                    }
                }

                Timer.gameObject.SetActive(true);
                remainingSeconds -= Time.deltaTime;

                float elapsed = currentDuration - remainingSeconds;
                currentBar.fillAmount = 1f - Mathf.Clamp01(elapsed / currentDuration);

                int hours = (int)(remainingSeconds / 3600);
                int minutes = (int)((remainingSeconds % 3600) / 60);
                int seconds = (int)(remainingSeconds % 60);
                Timer.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

                yield return null;
            }

            Timer.text = "00:00:00";
            Timer.gameObject.SetActive(false);
            Destroy(currentBar.gameObject);
        }

        switch (currentIndex)
        {
            case 0: OtomanIsOpen = true; break;
            case 1: KnightIsOpen = true; break;
            case 2: PersonIsOpen = true; break;
            case 3: KamkaziIsOpen = true; break;
            case 4: SavgeIsOpen = true; break;
            case 5: DestroyIsOpen = true; break;
            case 6: PositionIsOpen = true; break;
            case 7: SneakyIsOpen = true; break;
            case 8: MongaleIsOpen = true; break;
            case 9: EgypetIsOpen = true; break;
            case 10: HealerIsOpen = true; break;
            case 11: SapperIsOpen = true; break;
            case 12: FlayingIsOpen = true; break;
            case 13: GaulIsOpen = true; break;
        }

        isUnlocking = false;
        activeWarriorRoutine = null;

        if (GolldText != null && currentIndex < goldValues.Length)
            GolldText.text = "RECRUIT  " + goldValues[currentIndex].ToString();

        Debug.Log("تم فتح المحارب رقم: " + currentIndex);
        ShowNewWarriorPanel(currentIndex);
        currentIndex++;

        OpenButton.interactable = true;
        FinishButton.gameObject.SetActive(true);
        textButons.text = "";
        OpenButton.gameObject.SetActive(true);
    }

    Image GetCurrentBarFill()
    {
        switch (currentIndex)
        {
            case 0: return OtmanBarFill;
            case 1: return KnightBarFill;
            case 2: return PersonBarFill;
            case 3: return KamkaziBarFill;
            case 4: return SavgeBarFill;
            case 5: return DestoryBarFill;
            case 6: return PostionBarFill;
            case 7: return SneakyBarFill;
            case 8: return MonglaBarFill;
            case 9: return BarFill;
            case 10: return HealerBarFill;
            case 11: return SapperBarFill;
            case 12: return FlayBarFill;
            case 13: return GaulBarFill;
            default: return null;
        }
    }

    GameObject GetCurrentIcon()
    {
        switch (currentIndex)
        {
            case 0: return OtmanIcon;
            case 1: return KinghtIcon;
            case 2: return PersonIcon;
            case 3: return LamakziIcon;
            case 4: return SavgeIcon;
            case 5: return DestoryIcon;
            case 6: return PostionIcon;
            case 7: return SneakyIcon;
            case 8: return MongalIcon;
            case 9: return EgypetIcon;
            case 10: return HealerIcon;
            case 11: return SaperIcon;
            case 12: return FlayIcon;
            case 13: return GaulIcon;
            default: return null;
        }
    }

    private IEnumerator ShowUpgradeBarOverBuilding(float remainingTime, float totalTime)
    {
        if (upgradeBarPrefab == null) yield break;

        GameObject buildingObj = null;

        while (buildingObj == null)
        {
            buildingObj = GameObject.FindGameObjectWithTag("NewWorr");
            yield return new WaitForSeconds(0.5f);
        }

        GameObject canvasObj = GameObject.FindWithTag("MainCanvas");
        if (canvasObj == null) yield break;

        GameObject barObj = Instantiate(upgradeBarPrefab, canvasObj.transform);
        currentOverheadBar = barObj;
        barObj.transform.SetAsFirstSibling();

        UpgradeBarFollow barScript = barObj.GetComponent<UpgradeBarFollow>();
        if (barScript != null)
        {
            barScript.ResumeUpgradeBar(buildingObj.transform, totalTime, remainingTime);
        }

        yield return new WaitForSeconds(remainingTime);

        if (barObj != null) Destroy(barObj);
    }

    public void SpeedUpTimer()
    {
        forceSpeedUp = true;
    }

    private void ShowNewWarriorPanel(int index)
    {
        if (newWarriorPanel != null)
        {
            AudioManager.Instance.PlaySFX("openNew");
            newWarriorPanel.SetActive(true);

            if (newWarriorNameText != null && index < warriorNames.Length)
                newWarriorNameText.text = warriorNames[index];

            if (newWarriorImage != null && index < warriorImagePaths.Length)
            {
                Sprite loadedSprite = Resources.Load<Sprite>(warriorImagePaths[index]);
                if (loadedSprite != null)
                {
                    newWarriorImage.sprite = loadedSprite;
                    newWarriorImage.preserveAspect = true;
                }
            }
        }
    }

    public void LoadWarriorData(int savedIndex, bool savedIsUnlocking, long savedEndTime, float savedTotalDuration)
    {
        // 🟢 طباعة في الكونسول لنعرف ماذا وصل من ملف الحفظ بالضبط!
        //Debug.Log($"🔄 [LoadWarrior] Index: {savedIndex} | isUnlocking: {savedIsUnlocking} | EndTime: {savedEndTime}"); ديبك

        currentIndex = savedIndex;

        // إيقاف أي عداد قديم إذا كان يعمل قبل التحميل
        if (activeWarriorRoutine != null) StopCoroutine(activeWarriorRoutine);

        HideUnlockedWarriorsUI();

        OtomanIsOpen = currentIndex > 0;
        KnightIsOpen = currentIndex > 1;
        PersonIsOpen = currentIndex > 2;
        KamkaziIsOpen = currentIndex > 3;
        SavgeIsOpen = currentIndex > 4;
        DestroyIsOpen = currentIndex > 5;
        PositionIsOpen = currentIndex > 6;
        SneakyIsOpen = currentIndex > 7;
        MongaleIsOpen = currentIndex > 8;
        EgypetIsOpen = currentIndex > 9;
        HealerIsOpen = currentIndex > 10;
        SapperIsOpen = currentIndex > 11;
        FlayingIsOpen = currentIndex > 12;
        GaulIsOpen = currentIndex > 13;

        if (GolldText != null && currentIndex < goldValues.Length)
            GolldText.text = "RECRUIT   " + goldValues[currentIndex].ToString();

        if (savedIsUnlocking)
        {
            System.TimeSpan diff = new System.DateTime(savedEndTime) - System.DateTime.Now;
            float remaining = (float)diff.TotalSeconds;

            Debug.Log($"⏳ [LoadWarrior] الوقت المتبقي: {remaining} ثانية");

            if (remaining > 0)
            {
                activeWarriorRoutine = StartCoroutine(OpenNextWarriorRoutine(remaining, savedTotalDuration));
            }
            else
            {
                FinishOpenWarrior();
            }
        }
        else
        {
            // 🟢 إعادة ترتيب الواجهة إذا لم يكن هناك فتح جاري لضمان عدم اختفاء الأزرار
            isUnlocking = false;
            Timer.gameObject.SetActive(false);
            OpenButton.gameObject.SetActive(true);
            textButons.text = "RECRUIT";
        }
    }

    private void HideUnlockedWarriorsUI()
    {
        int actualIndex = currentIndex;
        for (int i = 0; i < actualIndex; i++)
        {
            currentIndex = i;
            GameObject icon = GetCurrentIcon();
            Image bar = GetCurrentBarFill();

            if (icon != null) Destroy(icon);
            if (bar != null) Destroy(bar.gameObject);
        }
        currentIndex = actualIndex;
    }
}