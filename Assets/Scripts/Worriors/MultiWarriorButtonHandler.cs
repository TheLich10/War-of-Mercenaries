using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WarriorData
{
    public string warriorName;
    public Button recruitButton;

    [Header("Training Display (5 Buttons)")]
    public Button[] trainingDisplayButtons;

    public GameObject objectToSpawn;
    [HideInInspector] public Sprite[] originalTargetSprites;
    [HideInInspector] public bool[] isSlotBusy;

    [Header("Training Timer Texts (5 Texts)")]
    public TMP_Text[] trainingTimerTexts;

    public UnitPool warriorPool; // <--- أضف هذا السطر لربط الـ Pool الخاص بهذا الجندي

    public TMP_Text trainingDurationText; // اسحب هنا النص الذي يحتوي على وقت تدريب المحارب الأصلي (مثل 10S)


}

public class MultiWarriorButtonHandler : MonoBehaviour
{
    private float lastRecruitTime = -10f;
    public float recruitCooldown = 0.5f;

    public TMP_Text GoldCount; // اسحب هنا النص الذي يحتوي على وقت تدريب المحارب الأصلي (مثل 10S)


    [Header("Iron Cost Texts (15)")]
    public TMP_Text[] warriorIronTexts = new TMP_Text[15];

    [Header("Iron Storage")]
    public TMP_Text ironStorage;

    [Header("Settings")]
    public WarriorData[] warriors;
    public Transform spawnLocation;

    [Header("Storage UI")]
    public Button[] storageButtons;
    public TMP_Text[] readyCountTexts;
    public TMP_Text[] timerTexts;
    public GameObject[] storageIndicators;
    public int maxStackSize = 20;

    private Dictionary<string, WarriorData> warriorDict = new Dictionary<string, WarriorData>();

    private string[] storageWarriorNames;
    private int[] storageCounts;
    private int[] readyCounts;
    private Sprite[] originalStorageSprites;

    private Transform[] spawnLocations;
    private int lastSpawnIndex = -1;
    private bool hasFoundFourMaker = false; // يحفظ حالة ما إذا تم العثور على المبنى

    // --- التعديلات الجديدة الخاصة بالذهب ---
    private int currentGoldTotal = 0;
    private Dictionary<string, int> activeWarriorCounts = new Dictionary<string, int>();

    [Header("Save System")]
    private bool isLoading = false;
    // مصفوفات بحجم 5 لتتبع الوقت واسم الجندي في كل خانة من الخانات الخمس
    private int[] currentTrainingTimeRemaining = new int[5];
    private string[] currentlyTrainingWarriorNames = new string[5];

    private void RecalculateTotalGold()
    {
        int totalGold = 0;

        foreach (var kvp in activeWarriorCounts)
        {
            string wName = kvp.Key;
            int count = kvp.Value;

            if (count <= 0) continue;

            // حساب المجموعات: (العدد - 1) تقسيم 10 + 1 
            // هذا يعطي: 1-10 = 1 | 11-20 = 2 | 21-30 = 3 وهكذا
            int groupsOfTen = ((count - 1) / 10) + 1;

            switch (wName)
            {
                // جنود يحسبون بنظام المجموعات (كل 10)
                case "bedoiun":
                    totalGold += groupsOfTen * 1;
                    break;
                case "ottman":
                    totalGold += groupsOfTen * 2;
                    break;

                // جنود يحسبون بالعدد الفردي (كل 1 جندي له سعره)
                case "knight":
                case "persian":
                    totalGold += count * 1; break;
                case "Kamikaze":
                case "Savge":
                    totalGold += count * 3; break;
                case "Destroyer":
                case "postion":
                    totalGold += count * 4; break;
                case "ScalingEgyptian":
                    totalGold += count * 5; break;
                case "sneaky":
                    totalGold += count * 6; break;
                case "Mongolian":
                    totalGold += count * 7; break;
                case "Helaer":
                case "sapper":
                case "flyingOttoman":
                    totalGold += count * 10; break;
                case "Gaul":
                    totalGold += count * 30; break;
            }
        }

        currentGoldTotal = totalGold;
        if (GoldCount != null)
            GoldCount.text = "FINISH NOW! " + currentGoldTotal.ToString();
    }
    // --------------------------------------

    void Awake()
    {
        int storageSize = storageButtons.Length;

        storageWarriorNames = new string[storageSize];
        storageCounts = new int[storageSize];
        readyCounts = new int[storageSize];
        originalStorageSprites = new Sprite[storageSize];

        for (int i = 0; i < warriors.Length; i++)
        {
            warriorDict[warriors[i].warriorName] = warriors[i];

            int slotsCount = warriors[i].trainingDisplayButtons.Length;
            warriors[i].originalTargetSprites = new Sprite[slotsCount];
            warriors[i].isSlotBusy = new bool[slotsCount];

            for (int j = 0; j < slotsCount; j++)
            {
                if (warriors[i].trainingDisplayButtons[j] != null)
                {
                    warriors[i].originalTargetSprites[j] = warriors[i].trainingDisplayButtons[j].image.sprite;
                }

                if (warriors[i].trainingTimerTexts != null && warriors[i].trainingTimerTexts.Length > j && warriors[i].trainingTimerTexts[j] != null)
                {
                    warriors[i].trainingTimerTexts[j].text = "";
                }
            }

            int index = i;
            warriors[index].recruitButton.onClick.AddListener(() => RecruitWarrior(warriors[index]));
        }

        for (int i = 0; i < storageButtons.Length; i++)
        {
            originalStorageSprites[i] = storageButtons[i].image.sprite;
            storageCounts[i] = 0;
            readyCounts[i] = 0;
            storageWarriorNames[i] = "";

            if (readyCountTexts != null && readyCountTexts.Length > i && readyCountTexts[i] != null)
                readyCountTexts[i].text = "";

            if (timerTexts != null && timerTexts.Length > i && timerTexts[i] != null)
                timerTexts[i].text = "";

            if (storageIndicators != null && storageIndicators.Length > i && storageIndicators[i] != null)
                storageIndicators[i].SetActive(false);

            UpdateStorageUI(i);

            int storageIndex = i;
            storageButtons[storageIndex].onClick.AddListener(() => CancelWarrior(storageIndex));
        }
    }

    void Update()
    {
        if (isLoading) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("maker") || hit.collider.CompareTag("fourMaker"))
                {
                    CheckAndUpdateBuildings(hit.collider.gameObject);
                    UpdateIronColors();
                }
            }
        }

        ProcessReadyWarriors();
    }

    void ProcessReadyWarriors()
    {
        if (Barracks.instnce == null) return;

        for (int i = 0; i < storageButtons.Length; i++)
        {
            if (readyCounts[i] > 0)
            {
                string currentWarriorName = storageWarriorNames[i];

                // التحقق من المساحة المخصصة لهذا الجندي قبل خصمه
                if (Barracks.instnce.CanAddSpecificWarrior(currentWarriorName))
                {
                    readyCounts[i]--;
                    SpawnWarrior(warriorDict[currentWarriorName]);

                    if (storageCounts[i] <= 0 && readyCounts[i] <= 0)
                    {
                        ShiftQueue();
                    }
                    else
                    {
                        UpdateStorageUI(i);
                    }
                }

                break; // نوقف الحلقة هنا لكي ينتظر دوره إذا لم تتوفر مساحة
            }
        }
    }

    void SpawnWarrior(WarriorData warrior) // 🟢 لاحظ التغيير هنا إلى WarriorData بدلاً من WarriorType
    {
        // 1. إضافة الجندي منطقياً للثكنة أولاً (هذا هو الأهم حتى لا يضيع العدد أبداً)
        if (Barracks.instnce != null)
        {
            Barracks.instnce.AddWarrior(warrior.warriorName);
        }

        // 2. تحديد مكان النزول (إذا كانت المصفوفة فارغة نستخدم المكان الافتراضي)
        Vector3 spawnPos = Vector3.zero;
        bool hasLocation = false;

        if (spawnLocations != null && spawnLocations.Length > 0 && spawnLocations[0] != null)
        {
            lastSpawnIndex = (lastSpawnIndex + 1) % spawnLocations.Length;
            spawnPos = spawnLocations[lastSpawnIndex].position;
            hasLocation = true;
        }
        else if (this.spawnLocation != null) // استخدام المكان الافتراضي (تأكد من سحبه في الإنسبكتور)
        {
            spawnPos = this.spawnLocation.position;
            hasLocation = true;
        }

        // 3. النزول البصري في الخريطة من الـ Pool
        if (hasLocation)
        {
            if (warrior.warriorPool != null)
            {
                warrior.warriorPool.Spawn(spawnPos);
            }
            else
            {
                Debug.LogWarning("⚠️ لم يتم تعيين UnitPool للجندي: " + warrior.warriorName);
            }
        }
        else
        {
            // في حال لم تحدد مكان نزول افتراضي، الجندي سيبقى في الثكنة بأمان ولن يضيع
            Debug.LogWarning("⚠️ لم يتم العثور على مكان نزول للجندي! تمت إضافته للثكنة كعدد فقط.");
        }
    }

    void CheckAndUpdateBuildings(GameObject clickedBuilding)
    {
        spawnLocations = new Transform[1];
        spawnLocations[0] = clickedBuilding.transform;

        // إذا تم النقر على مبنى fourMaker، نحفظ الحالة لتبقى الأزرار مفعلة دائماً
        // يتحقق النظام في كل مرة عما إذا كان هناك أي مبنى بهذا التاج موجود فعلياً في المشهد
        hasFoundFourMaker = (GameObject.FindGameObjectWithTag("fourMaker") != null);

        // إذا تم العثور على fourMaker (الآن أو سابقاً)، نُفعّل جميع الأزرار ونتجاهل المستويات
        if (hasFoundFourMaker)
        {
            for (int i = 0; i < storageButtons.Length && i < 4; i++)
                storageButtons[i].interactable = true;
        }
        else if (clickedBuilding.CompareTag("maker"))
        {
            // سيعمل هذا الكود فقط في حال لم يتم النقر على fourMaker مسبقاً
            building makerBuilding = clickedBuilding.GetComponent<building>();
            if (makerBuilding != null)
            {
                int lvl = makerBuilding.level;
                for (int i = 0; i < storageButtons.Length && i < 4; i++)
                {
                    if (i == 0 || i == 1) storageButtons[i].interactable = true;
                    else if (i == 2) storageButtons[i].interactable = (lvl >= 2);
                    else if (i == 3) storageButtons[i].interactable = (lvl >= 3);
                }
            }
        }
    }

    public void CancelWarrior(int idx)
    {
        string warriorName = storageWarriorNames[idx];
        bool hasCanceled = false;

        if (storageCounts[idx] > 0)
        {
            storageCounts[idx]--;
            hasCanceled = true;
        }
        else if (readyCounts[idx] > 0)
        {
            readyCounts[idx]--;
            hasCanceled = true;
        }

        if (hasCanceled && !string.IsNullOrEmpty(warriorName))
        {
            // --- خصم الجندي من العداد وإعادة حساب الذهب ---
            if (activeWarriorCounts.ContainsKey(warriorName) && activeWarriorCounts[warriorName] > 0)
            {
                activeWarriorCounts[warriorName]--;
            }
            RecalculateTotalGold();
            // ----------------------------------

            int warriorIndex = System.Array.FindIndex(warriors, w => w.warriorName == warriorName);
            if (warriorIndex >= 0 && warriorIndex < warriorIronTexts.Length)
            {
                int cost = int.Parse(warriorIronTexts[warriorIndex].text);
                ResourceManager.AddIron(cost);
                UpdateIronColors();
            }

            if (storageCounts[idx] <= 0 && readyCounts[idx] <= 0)
            {
                ShiftQueue();
            }
            else
            {
                UpdateStorageUI(idx);
            }
        }
    }

    void ShiftQueue()
    {
        for (int i = 0; i < storageButtons.Length - 1; i++)
        {
            if (storageCounts[i] == 0 && readyCounts[i] == 0)
            {
                for (int j = i + 1; j < storageButtons.Length; j++)
                {
                    if (storageCounts[j] > 0 || readyCounts[j] > 0)
                    {
                        storageCounts[i] = storageCounts[j];
                        readyCounts[i] = readyCounts[j];
                        storageWarriorNames[i] = storageWarriorNames[j];

                        storageCounts[j] = 0;
                        readyCounts[j] = 0;
                        storageWarriorNames[j] = "";
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < storageButtons.Length; i++)
        {
            UpdateStorageUI(i);
        }
    }

    private Color normalIronColor = Color.white;

    public void RecruitWarrior(WarriorData warrior)
    {
        if (Time.time - lastRecruitTime < recruitCooldown) return;
        lastRecruitTime = Time.time;

        int warriorIndex = System.Array.FindIndex(warriors, w => w == warrior);

        if (warriorIndex >= 0 && warriorIndex < warriorIronTexts.Length)
        {
            int currentIron = int.Parse(ironStorage.text);
            TMP_Text costText = warriorIronTexts[warriorIndex];
            int cost = int.Parse(costText.text);

            if (currentIron < cost)
            {
                costText.color = Color.red;
                return;
            }
            else costText.color = normalIronColor;

            int idx = FindStorageIndex(warrior.warriorName);
            if (idx == -1 || (idx < storageButtons.Length && !storageButtons[idx].interactable))
            {
                PopupText.instan.ShowPopup("Storage Maxed");
                return;
            }

            ResourceManager.RemoveIron(cost);
            UpdateIronColors();
        }

        // --- إضافة الذهب عند نجاح التجنيد ---
        if (!activeWarriorCounts.ContainsKey(warrior.warriorName))
        {
            activeWarriorCounts[warrior.warriorName] = 0;
        }
        activeWarriorCounts[warrior.warriorName]++;
        RecalculateTotalGold();

        AddToStorage(warrior);
        StartNextFromStorage();
    }

    void AddToStorage(WarriorData warrior)
    {
        int idx = FindStorageIndex(warrior.warriorName);
        if (idx == -1) return;
        if (idx < storageButtons.Length && !storageButtons[idx].interactable) return;

        storageWarriorNames[idx] = warrior.warriorName;
        storageCounts[idx]++;
        UpdateStorageUI(idx);
    }

    int FindStorageIndex(string name)
    {
        for (int i = 0; i < storageButtons.Length; i++)
        {
            int total = storageCounts[i] + readyCounts[i];
            if ((storageCounts[i] > 0 || readyCounts[i] > 0) && storageWarriorNames[i] == name && total < maxStackSize && storageButtons[i].interactable)
                return i;
        }
        for (int i = 0; i < storageButtons.Length; i++)
        {
            if (storageCounts[i] == 0 && readyCounts[i] == 0 && storageButtons[i].interactable) return i;
        }
        return -1;
    }

    // 🟢 استبدل الكوروتين القديم بهذا
    IEnumerator TrainingProcess(WarriorData warrior, int slotIndex, int startingTime = -1)
    {
        warrior.isSlotBusy[slotIndex] = true;
        currentlyTrainingWarriorNames[slotIndex] = warrior.warriorName; // حفظ اسم الجندي المتدرب

        warrior.trainingDisplayButtons[slotIndex].image.sprite = warrior.recruitButton.image.sprite;
        TMP_Text txt = warrior.trainingDisplayButtons[slotIndex].GetComponentInChildren<TMP_Text>();

        int targetIdxForTimer = -1;
        for (int i = 0; i < storageButtons.Length; i++)
        {
            int total = storageCounts[i] + readyCounts[i];
            if (storageWarriorNames[i] == warrior.warriorName && (storageCounts[i] > 0 || readyCounts[i] > 0) && total < maxStackSize)
            {
                targetIdxForTimer = i;
                break;
            }
        }

        if (targetIdxForTimer == -1)
        {
            for (int i = 0; i < storageButtons.Length; i++)
            {
                if (storageCounts[i] == 0 && readyCounts[i] == 0 && storageButtons[i].interactable)
                {
                    targetIdxForTimer = i;
                    break;
                }
            }
        }

        // 🟢 تحديد الوقت المتبقي لهذه الخانة تحديداً
        if (startingTime == -1)
            currentTrainingTimeRemaining[slotIndex] = (int)ConvertTextToSeconds(warrior.trainingDurationText.text);
        else
            currentTrainingTimeRemaining[slotIndex] = startingTime;

        // 🟢 العداد الزمني يعتمد على المصفوفة
        while (currentTrainingTimeRemaining[slotIndex] > 0)
        {
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(currentTrainingTimeRemaining[slotIndex]);
            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            if (warrior.trainingTimerTexts != null && warrior.trainingTimerTexts.Length > slotIndex && warrior.trainingTimerTexts[slotIndex] != null)
            {
                warrior.trainingTimerTexts[slotIndex].text = formattedTime;
            }

            yield return new WaitForSeconds(1);
            currentTrainingTimeRemaining[slotIndex]--;
        }

        if (targetIdxForTimer != -1 && timerTexts != null && timerTexts.Length > targetIdxForTimer && timerTexts[targetIdxForTimer] != null)
        {
            timerTexts[targetIdxForTimer].text = "";
        }
        if (warrior.trainingTimerTexts != null && warrior.trainingTimerTexts.Length > slotIndex && warrior.trainingTimerTexts[slotIndex] != null)
        {
            warrior.trainingTimerTexts[slotIndex].text = "";
        }

        currentlyTrainingWarriorNames[slotIndex] = ""; // تفريغ الاسم بعد الانتهاء

        // النزول وتكملة الكود كما هو في سكربتك
        if (Barracks.instnce != null && Barracks.instnce.CanAddSpecificWarrior(warrior.warriorName))
        {
            if (txt != null) txt.text = "";
            warrior.trainingDisplayButtons[slotIndex].image.sprite = warrior.originalTargetSprites[slotIndex];

            SpawnWarrior(warrior);

            warrior.isSlotBusy[slotIndex] = false;
            StartNextFromStorage();
            yield break;
        }

        int targetIdx = -1;
        while (true)
        {
            for (int i = 0; i < storageButtons.Length; i++)
            {
                int total = storageCounts[i] + readyCounts[i];
                if (storageWarriorNames[i] == warrior.warriorName && (storageCounts[i] > 0 || readyCounts[i] > 0) && total < maxStackSize)
                {
                    targetIdx = i;
                    break;
                }
            }

            if (targetIdx == -1)
            {
                for (int i = 0; i < storageButtons.Length; i++)
                {
                    if (storageCounts[i] == 0 && readyCounts[i] == 0 && storageButtons[i].interactable)
                    {
                        targetIdx = i;
                        storageWarriorNames[i] = warrior.warriorName;
                        break;
                    }
                }
            }

            if (targetIdx != -1) break;

            if (txt != null) txt.text = "Wait";
            yield return new WaitForSeconds(0.5f);
        }

        if (txt != null) txt.text = "";
        warrior.trainingDisplayButtons[slotIndex].image.sprite = warrior.originalTargetSprites[slotIndex];

        readyCounts[targetIdx]++;
        UpdateStorageUI(targetIdx);

        warrior.isSlotBusy[slotIndex] = false;
        StartNextFromStorage();
    }

    void StartNextFromStorage()
    {
        ShiftQueue();

        for (int i = 0; i < storageButtons.Length; i++)
        {
            if (storageCounts[i] > 0)
            {
                string nextWarriorName = storageWarriorNames[i];
                if (warriorDict.ContainsKey(nextWarriorName))
                {
                    WarriorData w = warriorDict[nextWarriorName];

                    int availableSlotIndex = -1;
                    for (int j = 0; j < w.trainingDisplayButtons.Length; j++)
                    {
                        bool isGloballyBusy = false;

                        // التحقق من هذه الخانة في جميع أنواع المحاربين للتأكد من أنها ليست محجوزة لأي نوع آخر
                        foreach (var checkWarrior in warriors)
                        {
                            if (checkWarrior.isSlotBusy[j])
                            {
                                isGloballyBusy = true;
                                break;
                            }
                        }

                        // إذا لم تكن محجوزة لأي محارب، نخصصها للمحارب الحالي
                        if (!isGloballyBusy)
                        {
                            availableSlotIndex = j;
                            break;
                        }
                    }

                    if (availableSlotIndex != -1)
                    {
                        storageCounts[i]--;
                        UpdateStorageUI(i);

                        StartCoroutine(TrainingProcess(w, availableSlotIndex));

                        StartNextFromStorage();
                        return;
                    }
                }
            }
        }
    }

    void UpdateStorageUI(int idx)
    {
        if (storageCounts[idx] <= 0 && readyCounts[idx] <= 0)
        {
            storageCounts[idx] = 0;
            readyCounts[idx] = 0;
            storageWarriorNames[idx] = "";
            storageButtons[idx].image.sprite = originalStorageSprites[idx];
            storageButtons[idx].GetComponentInChildren<TMP_Text>().text = "";

            if (readyCountTexts != null && readyCountTexts.Length > idx && readyCountTexts[idx] != null)
                readyCountTexts[idx].text = "";

            if (storageIndicators != null && storageIndicators.Length > idx && storageIndicators[idx] != null)
                storageIndicators[idx].SetActive(false);
        }
        else
        {
            WarriorData w = warriorDict[storageWarriorNames[idx]];
            storageButtons[idx].image.sprite = w.recruitButton.image.sprite;

            storageButtons[idx].GetComponentInChildren<TMP_Text>().text = storageCounts[idx] > 0 ? storageCounts[idx].ToString() : "";

            if (readyCountTexts != null && readyCountTexts.Length > idx && readyCountTexts[idx] != null)
            {
                readyCountTexts[idx].text = readyCounts[idx] > 0 ? readyCounts[idx].ToString() : "";
            }

            if (storageIndicators != null && storageIndicators.Length > idx && storageIndicators[idx] != null)
                storageIndicators[idx].SetActive(true);
        }
    }

    void UpdateIronColors()
    {
        int currentIron = 0;
        if (ironStorage != null && !string.IsNullOrEmpty(ironStorage.text))
        {
            int.TryParse(ironStorage.text, out currentIron);
        }

        for (int i = 0; i < warriors.Length && i < warriorIronTexts.Length; i++)
        {
            TMP_Text costText = warriorIronTexts[i];
            if (costText == null) continue;

            int cost = 0;
            int.TryParse(costText.text, out cost);

            if (currentIron < cost) costText.color = Color.red;
            else costText.color = normalIronColor;
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
                    return 10f;
            }
        }
        return 10f; // في حال فشل التحويل لأي سبب، نعيد 10 ثواني كإجراء آمن
    }

    // ==========================================
    // دالة التجنيد الفوري المصححة
    // ==========================================
    public void InstantRecruitAllQueued()
    {
        // --- الإضافة الجديدة: خصم الذهب وتصفير العداد ---
        if (currentGoldTotal > 0)
        {
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(currentGoldTotal); // سحب مبلغ الذهب الكلي

            activeWarriorCounts.Clear(); // تصفير قائمة الجنود لأنهم انتهوا من التدريب
            RecalculateTotalGold();      // تحديث النص ليصبح 0
        }
        // ----------------------------------------------

        // 1. إيقاف جميع عمليات التدريب (الكوروتين) لتجنب تداخل الأوقات
        StopAllCoroutines();

        // 2. أولاً: معالجة وتجنيد الجنود الذين هم قيد التدريب حالياً (في خانات العرض)
        for (int i = 0; i < warriors.Length; i++)
        {
            WarriorData w = warriors[i];
            if (w.isSlotBusy == null) continue;

            for (int j = 0; j < w.isSlotBusy.Length; j++)
            {
                if (w.isSlotBusy[j])
                {
                    // هذا الجندي كان يتدرب وقت النقر على الزر
                    if (Barracks.instnce != null && Barracks.instnce.CanAddSpecificWarrior(w.warriorName))
                    {
                        // الثكنة فيها مساحة، استدعه فوراً
                        SpawnWarrior(w);
                    }
                    else
                    {
                        // الثكنة ممتلئة، نعيده كجندي "جاهز" في المخزن لكي لا يختفي
                        int targetIdx = -1;

                        // البحث عن خانة تحتوي على نفس النوع
                        for (int k = 0; k < storageButtons.Length; k++)
                        {
                            int total = storageCounts[k] + readyCounts[k];
                            if (storageWarriorNames[k] == w.warriorName && (storageCounts[k] > 0 || readyCounts[k] > 0) && total < maxStackSize)
                            {
                                targetIdx = k;
                                break;
                            }
                        }
                        // إذا لم يجد، نبحث عن أول خانة فارغة
                        if (targetIdx == -1)
                        {
                            for (int k = 0; k < storageButtons.Length; k++)
                            {
                                if (storageCounts[k] == 0 && readyCounts[k] == 0 && storageButtons[k].interactable)
                                {
                                    targetIdx = k;
                                    storageWarriorNames[k] = w.warriorName;
                                    break;
                                }
                            }
                        }
                        // إضافة الجندي لعداد الجاهزين
                        if (targetIdx != -1)
                        {
                            readyCounts[targetIdx]++;
                        }
                    }

                    // تنظيف الخانة وإعادتها لحالتها الطبيعية (إرجاع الصورة وإخفاء النص)
                    w.isSlotBusy[j] = false;

                    if (w.trainingDisplayButtons != null && w.trainingDisplayButtons.Length > j && w.trainingDisplayButtons[j] != null)
                    {
                        if (w.originalTargetSprites != null && w.originalTargetSprites.Length > j)
                            w.trainingDisplayButtons[j].image.sprite = w.originalTargetSprites[j];

                        TMP_Text btnTxt = w.trainingDisplayButtons[j].GetComponentInChildren<TMP_Text>();
                        if (btnTxt != null) btnTxt.text = "";
                    }

                    if (w.trainingTimerTexts != null && w.trainingTimerTexts.Length > j && w.trainingTimerTexts[j] != null)
                    {
                        w.trainingTimerTexts[j].text = "";
                    }
                }
            }
        }

        // مسح نصوص التايمر المعروضة فوق أزرار المخزن (إن وجدت)
        if (timerTexts != null)
        {
            for (int i = 0; i < timerTexts.Length; i++)
            {
                if (timerTexts[i] != null) timerTexts[i].text = "";
            }
        }

        // 3. ثانياً: معالجة الجنود المنتظرين في الطابور (الذين لم يبدأ تدريبهم بعد)
        for (int i = 0; i < storageButtons.Length; i++)
        {
            while (storageCounts[i] > 0)
            {
                string warriorName = storageWarriorNames[i];

                // فحص هل الثكنة تتسع لهذا الجندي
                if (Barracks.instnce != null && Barracks.instnce.CanAddSpecificWarrior(warriorName))
                {
                    storageCounts[i]--; // خصم الجندي من المخزن
                    SpawnWarrior(warriorDict[warriorName]); // استدعاء الجندي فوراً
                }
                else
                {
                    // الثكنة ممتلئة لهذا النوع، نتوقف ونتركه في المخزن
                    break;
                }
            }
            // تحديث واجهة الخانة بعد العملية
            UpdateStorageUI(i);
        }

        // 4. ترتيب المخزن واستئناف التدريب العادي لمن تبقى (إن لم تتسع لهم الثكنة)
        ShiftQueue();
        StartNextFromStorage();
    }
    // ==========================================
    // دوال نظام الحفظ والمحاكاة المتعددة (النسخة الآمنة)
    // ==========================================
    // ==========================================
    // دوال نظام الحفظ والمحاكاة المتعددة (مطابقة للسكربت الناجح 100%)
    // ==========================================
    public MultiTrainingSaveData GetSaveData()
    {
        MultiTrainingSaveData data = new MultiTrainingSaveData();

        // حفظ القوائم الحالية كما هي
        data.storageWarriorNames = this.storageWarriorNames;
        data.storageCounts = this.storageCounts;
        data.readyCounts = this.readyCounts;

        int slotsCount = warriors.Length > 0 ? warriors[0].trainingDisplayButtons.Length : 5;
        data.slotIsBusy = new bool[slotsCount];
        data.slotWarriorNames = new string[slotsCount];
        data.slotRemainingTimes = new int[slotsCount];

        for (int i = 0; i < slotsCount; i++)
        {
            bool isBusy = false;
            foreach (var w in warriors) { if (w.isSlotBusy[i]) { isBusy = true; break; } }

            data.slotIsBusy[i] = isBusy;
            data.slotWarriorNames[i] = currentlyTrainingWarriorNames[i];
            data.slotRemainingTimes[i] = currentTrainingTimeRemaining[i];
        }
        return data;
    }

    public void LoadAndCatchUp(MultiTrainingSaveData data, long lastGlobalSaveTicks)
    {
        // إيقاف التدريب وقفل التحديثات العشوائية
        StopAllCoroutines();
        isLoading = true;

        // 🟢 1. استعادة البيانات وتحديث الواجهة فوراً (هذا ما كان ينقصنا وهو سبب نجاح السكربت الأول)
        if (data.storageWarriorNames != null) this.storageWarriorNames = (string[])data.storageWarriorNames.Clone();
        if (data.storageCounts != null) this.storageCounts = (int[])data.storageCounts.Clone();
        if (data.readyCounts != null) this.readyCounts = (int[])data.readyCounts.Clone();

        // تحديث الواجهة فوراً ليظهر المحاربون المخزنون دون تأخير
        for (int i = 0; i < storageButtons.Length; i++)
        {
            UpdateStorageUI(i);
        }

        // 2. تنظيف مؤقت لخانات التدريب لضمان عدم وجود بيانات معلقة
        for (int i = 0; i < warriors.Length; i++)
        {
            if (warriors[i].isSlotBusy != null)
            {
                for (int j = 0; j < warriors[i].isSlotBusy.Length; j++)
                {
                    warriors[i].isSlotBusy[j] = false;
                }
            }
        }

        // 3. تأجيل تشغيل تعويض الوقت للجنود الذين يتدربون حالياً حتى تستقر الخريطة
        StartCoroutine(ApplyOfflineCatchUpMulti(data, lastGlobalSaveTicks));
    }

    private IEnumerator ApplyOfflineCatchUpMulti(MultiTrainingSaveData data, long lastGlobalSaveTicks)
    {
        // تأخير بسيط لاستقرار الثكنات كما فعلنا بنجاح في السكربت الأول
        yield return new WaitForSeconds(0.5f);

        if (data.slotIsBusy == null || data.slotWarriorNames == null)
        {
            isLoading = false;
            yield break;
        }

        int slotsCount = data.slotIsBusy.Length;
        long currentTicks = System.DateTime.Now.Ticks;
        int secondsOffline = 0;

        if (lastGlobalSaveTicks > 0)
        {
            secondsOffline = (int)((currentTicks - lastGlobalSaveTicks) / System.TimeSpan.TicksPerSecond);
        }

        // وضع سقف زمني للتعويض لتجنب الانهيار إذا تركت اللعبة لأشهر (مثلاً شهر = 2592000 ثانية)
        if (secondsOffline > 2592000) secondsOffline = 2592000;

        bool[] slotBusy = (bool[])data.slotIsBusy.Clone();
        string[] slotWarrior = (string[])data.slotWarriorNames.Clone();
        int[] slotTime = (int[])data.slotRemainingTimes.Clone();

        while (secondsOffline > 0)
        {
            bool stateChanged = false;

            // 1. إنقاص الوقت من جميع الخانات الشغالة
            for (int j = 0; j < slotsCount; j++)
            {
                if (slotBusy[j])
                {
                    slotTime[j]--;
                    if (slotTime[j] <= 0) // انتهى التدريب لهذا الجندي
                    {
                        int targetIdx = FindStorageIndex(slotWarrior[j]);
                        if (targetIdx == -1) targetIdx = 0; // احتياطي

                        if (targetIdx < readyCounts.Length)
                        {
                            // التأكد من وضع اسم الجندي إذا كانت الخانة فارغة (نفس حل السكربت الناجح)
                            if (string.IsNullOrEmpty(storageWarriorNames[targetIdx]))
                            {
                                storageWarriorNames[targetIdx] = slotWarrior[j];
                            }
                            readyCounts[targetIdx]++;
                        }

                        slotBusy[j] = false;
                        slotWarrior[j] = "";
                        stateChanged = true;
                    }
                }
            }

            // 2. سحب الجندي التالي من الطابور لبدء تدريبه في المحاكاة
            if (stateChanged)
            {
                for (int j = 0; j < slotsCount; j++)
                {
                    if (!slotBusy[j])
                    {
                        for (int i = 0; i < storageWarriorNames.Length; i++)
                        {
                            if (storageCounts[i] > 0)
                            {
                                string nextW = storageWarriorNames[i];
                                storageCounts[i]--;

                                slotBusy[j] = true;
                                slotWarrior[j] = nextW;
                                if (warriorDict.ContainsKey(nextW))
                                {
                                    slotTime[j] = (int)ConvertTextToSeconds(warriorDict[nextW].trainingDurationText.text);
                                }
                                else
                                {
                                    slotTime[j] = 10;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            secondsOffline--;
        }

        // 3. تحديث الواجهة النهائية لتعكس ما حدث أوفلاين
        for (int i = 0; i < storageButtons.Length; i++)
        {
            UpdateStorageUI(i);
        }

        // 4. تشغيل التدريبات المرئية (الكوروتين) للجنود الذين لم تنتهِ أوقاتهم
        for (int j = 0; j < slotsCount; j++)
        {
            if (slotBusy[j] && !string.IsNullOrEmpty(slotWarrior[j]) && warriorDict.ContainsKey(slotWarrior[j]))
            {
                StartCoroutine(TrainingProcess(warriorDict[slotWarrior[j]], j, slotTime[j]));
            }
        }

        // فك القفل لتعود اللعبة لطبيعتها
        isLoading = false;
    }
}
