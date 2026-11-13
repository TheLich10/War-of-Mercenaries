using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WarriorType
{
    public string warriorName;
    public Button recruitButton;
    public Button trainingDisplayButton;
    public GameObject objectToSpawn;
    [HideInInspector] public Sprite originalTargetSprite;
    public TMP_Text trainingTimerText; // اسحب هنا النص الذي سيكون فوق زر التدريب
    public UnitPool warriorPool; // <--- أضف هذا السطر لربط الـ Pool الخاص بهذا الجندي

    public TMP_Text trainingDurationText; // اسحب هنا النص الذي يحتوي على وقت تدريب المحارب الأصلي (مثل 10S)
}

public class WarriorButtonHandler : MonoBehaviour
{
    public static WarriorButtonHandler instnce;
    public TMP_Text GoldCount; // اسحب هنا النص الذي يحتوي على وقت تدريب المحارب الأصلي (مثل 10S)


    private float lastRecruitTime = -10f;
    public float recruitCooldown = 0.5f; // نصف ثانية

    [Header("Iron Cost Texts (15)")]
    public TMP_Text[] warriorIronTexts = new TMP_Text[15]; // النصوص

    [Header("Iron Storage")]
    public TMP_Text ironStorage; // كمية الحديد الحالية

    [Header("Settings")]
    public WarriorType[] warriors;
    public Transform spawnLocation;

    [Header("Storage UI")]
    public Button[] storageButtons; // أزرار التخزين הـ 4
    public TMP_Text[] readyCountTexts; // النصوص الجديدة لعرض عدد الجاهزين (يجب أن يكون عددها 4 أيضاً)
    public TMP_Text[] timerTexts; // نصوص الوقت (يجب أن يكون عددها 4 أيضاً وتضعها في الإنسبكتور)
    public GameObject[] storageIndicators;
    public int maxStackSize = 20;

    private Dictionary<string, WarriorType> warriorDict = new Dictionary<string, WarriorType>();

    private string[] storageWarriorNames;
    private int[] storageCounts; // عداد المنتظرين للتدريب
    private int[] readyCounts;   // عداد الجاهزين للنزول
    private Sprite[] originalStorageSprites;

    private bool isTraining = false;
    private Transform[] spawnLocations;
    private int lastSpawnIndex = -1;
    private bool hasFoundFourMaker = false; // يحفظ حالة ما إذا تم العثور على المبنى

    // --- التعديلات الجديدة الخاصة بالذهب ---
    private int currentGoldTotal = 0;
    private Dictionary<string, int> activeWarriorCounts = new Dictionary<string, int>();

    [Header("Save System")]
    public string panelID; // 🟢 هام جداً: أعطِ اسماً مختلفاً لكل بنل من الإنسبكتور (مثلاً: Panel1, Panel2...)
    private int currentTrainingTimeRemaining = 0; // لتتبع الوقت المتبقي أثناء التدريب
    private string currentlyTrainingWarriorName = ""; // اسم الجندي الذي يتدرب الآن

    private bool isLoading = false;

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

    private void Awake()
    {
        instnce = this;
        if (GoldCount != null) GoldCount.text = "FINISH NOW! 0";

        int storageSize = storageButtons.Length;

        storageWarriorNames = new string[storageSize];
        storageCounts = new int[storageSize];
        readyCounts = new int[storageSize]; // تهيئة مصفوفة الجاهزين
        originalStorageSprites = new Sprite[storageSize];

        for (int i = 0; i < warriors.Length; i++)
        {
            warriorDict[warriors[i].warriorName] = warriors[i];
            warriors[i].originalTargetSprite = warriors[i].trainingDisplayButton.image.sprite;

            int index = i;
            warriors[index].recruitButton.onClick.AddListener(() => RecruitWarrior(warriors[index]));
            // --- الإضافة الجديدة لتفريغ النص ---
            if (warriors[i].trainingTimerText != null)
                warriors[i].trainingTimerText.text = "";
        }

        for (int i = 0; i < storageButtons.Length; i++)
        {
            originalStorageSprites[i] = storageButtons[i].image.sprite;
            storageCounts[i] = 0;
            readyCounts[i] = 0;
            storageWarriorNames[i] = "";

            if (readyCountTexts != null && readyCountTexts.Length > i && readyCountTexts[i] != null)
                readyCountTexts[i].text = ""; // إخفاء نص الجاهزين في البداية
            // --- الإضافة الجديدة لإخفاء الوقت في البداية ---
            if (timerTexts != null && timerTexts.Length > i && timerTexts[i] != null)
                timerTexts[i].text = "";
            // --- الإضافة الجديدة لإخفاء الأوبجكت في البداية ---
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

                // التعديل هنا: نتحقق هل الثكنة تتسع "لهذا الجندي بالتحديد" قبل خصمه
                if (Barracks.instnce.CanAddSpecificWarrior(currentWarriorName))
                {
                    readyCounts[i]--; // يتم الخصم فقط إذا كانت المساحة تكفي
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

                break; // نوقف الحلقة هنا دائماً لكي ينتظر الجندي دوره إذا لم تتوفر له مساحة
            }
        }
    }

    void SpawnWarrior(WarriorType warrior)
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

    // الدالة الجديدة التي ستقوم بربطها بالزر
    // دالة لاستدعاء الجندي من الـ Pool فقط بدون إضافته للثكنة
    public void SpawnWarriorOnlyByName(string warriorNameToSpawn)
    {
        if (warriorDict.ContainsKey(warriorNameToSpawn))
        {
            WarriorType warrior = warriorDict[warriorNameToSpawn];

            // التحقق من وجود مكان للنزول
            if (spawnLocations != null && spawnLocations.Length > 0)
            {
                lastSpawnIndex = (lastSpawnIndex + 1) % spawnLocations.Length;

                // تم إزالة كود الثكنة (Barracks) من هنا تماماً

                // استدعاء الجندي من الـ Pool
                if (warrior.warriorPool != null)
                {
                    // 1. التحقق مما إذا كان موقع الترسب الحالي قد تم تدميره
                    if (spawnLocations[lastSpawnIndex] == null)
                    {
                        // 2. البحث عن مبنى جديد يحمل التاغ "maker"
                        GameObject newMaker = GameObject.FindWithTag("maker");

                        // 3. التأكد من أننا وجدنا مبنى بالفعل
                        if (newMaker != null)
                        {
                            // تحديث مكان الترسب المفقود بالمبنى الجديد
                            spawnLocations[lastSpawnIndex] = newMaker.transform;
                        }
                        else
                        {
                            // في حال لم يتبقَ أي مبنى يحمل تاغ "maker" في المشهد
                            Debug.LogWarning("لم يتم العثور على أي مبنى بتاغ 'maker'. لا يمكن ترسب المحارب.");
                            return; // نوقف تنفيذ الكود هنا حتى لا يظهر خطأ جديد
                        }
                    }

                    // 4. الآن أصبح من الآمن استدعاء الترسب
                    warrior.warriorPool.Spawn(spawnLocations[lastSpawnIndex].position);
                }
                else
                {
                    Debug.LogWarning("⚠️ لم يتم تعيين UnitPool للجندي: " + warrior.warriorName);
                }
            }
        }
        else
        {
            Debug.LogWarning("⚠️ لم يتم العثور على جندي بهذا الاسم: " + warriorNameToSpawn);
        }
    }

    void CheckAndUpdateBuildings(GameObject clickedBuilding)
    {
        spawnLocations = new Transform[1];
        spawnLocations[0] = clickedBuilding.transform;

        // إذا تم النقر على مبنى fourMaker، نحفظ الحالة لتبقى الأزرار مفعلة دائماً
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

        // الأولوية لإلغاء المنتظرين أولاً، ثم الجاهزين
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
            // الخانة تعتبر فارغة فقط إذا كان العدادان 0
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

    public void RecruitWarrior(WarriorType warrior)
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
        if (!isTraining) StartNextFromStorage();
    }

    void AddToStorage(WarriorType warrior)
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
            // التحقق من أن المجموع الكلي (منتظرين + جاهزين) أقل من الحد الأقصى
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

    // استبدل دالة التدريب القديمة بهذه الدالة بالكامل
    IEnumerator TrainingProcess(WarriorType warrior, int startingTime = -1)
    {
        isTraining = true;
        currentlyTrainingWarriorName = warrior.warriorName; // حفظ اسم الجندي الحالي لنظام الحفظ
        warrior.trainingDisplayButton.image.sprite = warrior.recruitButton.image.sprite;

        TMP_Text txt = warrior.trainingDisplayButton.GetComponentInChildren<TMP_Text>();

        // 1. البحث عن الخانة التي يتم تدريب هذا الجندي لصالحها لعرض الوقت
        int targetIdxForTimer = -1;
        for (int i = 0; i < storageButtons.Length; i++)
        {
            if (storageWarriorNames[i] == warrior.warriorName && (storageCounts[i] > 0 || readyCounts[i] > 0))
            {
                targetIdxForTimer = i;
                break;
            }
        }

        // 2. إذا لم يكن له خانة مسبقاً، نبحث عن أول خانة فارغة
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

        // 🟢 التعديل الجديد: تحديد الوقت (سواء بدأ للتو أو تم تحميله من الحفظ)
        if (startingTime == -1)
        {
            currentTrainingTimeRemaining = (int)ConvertTextToSeconds(warrior.trainingDurationText.text);
        }
        else
        {
            currentTrainingTimeRemaining = startingTime;
        }

        // 🟢 حلقة الوقت المتبقي
        while (currentTrainingTimeRemaining > 0)
        {
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(currentTrainingTimeRemaining);
            string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            bool isBarracksFull = (Barracks.instnce != null && !Barracks.instnce.CanAddSpecificWarrior(warrior.warriorName));

            if (isBarracksFull && targetIdxForTimer != -1 && timerTexts != null && timerTexts.Length > targetIdxForTimer && timerTexts[targetIdxForTimer] != null)
            {
                timerTexts[targetIdxForTimer].text = formattedTime;
                if (warrior.trainingTimerText != null) warrior.trainingTimerText.text = "";
            }
            else
            {
                if (warrior.trainingTimerText != null) warrior.trainingTimerText.text = formattedTime;
                if (targetIdxForTimer != -1 && timerTexts != null && timerTexts.Length > targetIdxForTimer && timerTexts[targetIdxForTimer] != null)
                    timerTexts[targetIdxForTimer].text = "";
            }

            yield return new WaitForSeconds(1);
            currentTrainingTimeRemaining--; // إنقاص الوقت ثانية بثانية
        }

        // مسح كلا النصين بعد انتهاء الوقت
        if (targetIdxForTimer != -1 && timerTexts != null && timerTexts.Length > targetIdxForTimer && timerTexts[targetIdxForTimer] != null)
        {
            timerTexts[targetIdxForTimer].text = "";
        }
        if (warrior.trainingTimerText != null)
        {
            warrior.trainingTimerText.text = "";
        }

        currentlyTrainingWarriorName = ""; // تفريغ الاسم بعد الانتهاء

        // النزول إذا كانت الثكنة تتسع
        if (Barracks.instnce != null && Barracks.instnce.CanAddSpecificWarrior(warrior.warriorName))
        {
            txt.text = "";
            warrior.trainingDisplayButton.image.sprite = warrior.originalTargetSprite;

            SpawnWarrior(warrior);

            isTraining = false;
            StartNextFromStorage();
            yield break;
        }

        // الانتظار إذا كانت الثكنة ممتلئة
        int targetIdx = -1;
        while (true)
        {
            for (int i = 0; i < storageButtons.Length; i++)
            {
                if (storageWarriorNames[i] == warrior.warriorName && (storageCounts[i] > 0 || readyCounts[i] > 0))
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

            txt.text = "Wait";
            yield return new WaitForSeconds(0.5f);
        }

        txt.text = "";
        warrior.trainingDisplayButton.image.sprite = warrior.originalTargetSprite;

        readyCounts[targetIdx]++;
        UpdateStorageUI(targetIdx);

        isTraining = false;
        StartNextFromStorage();
    }

    void StartNextFromStorage()
    {
        ShiftQueue(); // للتأكد من ترتيب الطابور

        for (int i = 0; i < storageButtons.Length; i++)
        {
            if (storageCounts[i] > 0)
            {
                string nextWarriorName = storageWarriorNames[i];
                storageCounts[i]--; // أخذ جندي من قائمة المنتظرين للبدء بتدريبه

                UpdateStorageUI(i);

                if (warriorDict.ContainsKey(nextWarriorName))
                {
                    StartCoroutine(TrainingProcess(warriorDict[nextWarriorName]));
                }
                return;
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
            // 🟢 التعديل الأهم: حماية قوية من الـ Null
            if (originalStorageSprites != null && originalStorageSprites.Length > idx && originalStorageSprites[idx] != null)
            {
                storageButtons[idx].image.sprite = originalStorageSprites[idx];
            }

            storageButtons[idx].GetComponentInChildren<TMP_Text>().text = "";

            if (readyCountTexts != null && readyCountTexts.Length > idx && readyCountTexts[idx] != null)
                readyCountTexts[idx].text = "";

            if (storageIndicators != null && storageIndicators.Length > idx && storageIndicators[idx] != null)
                storageIndicators[idx].SetActive(false);
        }
        else
        {
            WarriorType w = warriorDict[storageWarriorNames[idx]];
            storageButtons[idx].image.sprite = w.recruitButton.image.sprite;

            // تحديث نص الانتظار
            storageButtons[idx].GetComponentInChildren<TMP_Text>().text = storageCounts[idx] > 0 ? storageCounts[idx].ToString() : "";

            // تحديث نص الجاهزين
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
                case 'S': return timeValue;               // ثواني
                case 'M': return timeValue * 60f;         // دقائق
                case 'H': return timeValue * 3600f;       // ساعات
                case 'D': return timeValue * 86400f;      // أيام
                default:
                    // في حال كان النص رقماً فقط بدون حروف، نتعامل معه كثواني
                    if (float.TryParse(timeString, out float rawValue)) return rawValue;
                    return 10f;
            }
        }
        return 10f; // في حال فشل التحويل لأي سبب، نعيد 10 ثواني كإجراء آمن
    }

    // ==========================================
    // دالة التجنيد الفوري (اربطها بالزر الذي تريده)
    // ==========================================
    // ==========================================
    // دالة التجنيد الفوري (النسخة المصححة للسكريبت الأول)
    // ==========================================
    public void InstantRecruitAllQueued()
    {
        // --- الإضافة الجديدة: خصم الذهب وتصفير العداد ---
        if (currentGoldTotal > 0)
        {
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(currentGoldTotal); // سحب مبلغ الذهب الكلي

            activeWarriorCounts.Clear(); // تصفير قائمة الجنود لأنهم انتهوا من التدريب
            RecalculateTotalGold();      // تحديث النص ليصبح 0
        }
        // ----------------------------------------------

        // 1. إيقاف التدريب الحالي (الكوروتين)
        StopAllCoroutines();
        isTraining = false;

        // 2. أولاً: معالجة الجندي الذي يتدرب حالياً لكي لا يختفي
        for (int i = 0; i < warriors.Length; i++)
        {
            WarriorType w = warriors[i];

            // نعرف أن هذا الجندي هو الذي يتدرب حالياً إذا كانت صورته متغيرة عن الأصلية
            if (w.trainingDisplayButton != null && w.originalTargetSprite != null && w.trainingDisplayButton.image.sprite != w.originalTargetSprite)
            {
                // الجندي كان يتدرب وقت النقر على الزر
                if (Barracks.instnce != null && Barracks.instnce.CanAddSpecificWarrior(w.warriorName))
                {
                    // الثكنة تتسع له، ننزله فوراً
                    SpawnWarrior(w);
                }
                else
                {
                    // الثكنة ممتلئة، نعيده كـ "جاهز" في المخزن لينتظر مساحة
                    int targetIdx = -1;

                    // نبحث عن خانة تحتوي على نفس النوع
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
                    // إضافته لعداد الجاهزين
                    if (targetIdx != -1)
                    {
                        readyCounts[targetIdx]++;
                    }
                }

                // تنظيف واجهة هذا الجندي
                w.trainingDisplayButton.image.sprite = w.originalTargetSprite;
                if (w.trainingTimerText != null) w.trainingTimerText.text = "";

                TMP_Text btnTxt = w.trainingDisplayButton.GetComponentInChildren<TMP_Text>();
                if (btnTxt != null) btnTxt.text = "";
            }
            else
            {
                // تنظيف احتياطي لنصوص الوقت لباقي الجنود
                if (w.trainingTimerText != null) w.trainingTimerText.text = "";
            }
        }

        // مسح نصوص الوقت من فوق أزرار التخزين (إن وجدت)
        if (timerTexts != null)
        {
            for (int i = 0; i < timerTexts.Length; i++)
            {
                if (timerTexts[i] != null) timerTexts[i].text = "";
            }
        }

        // 3. ثانياً: المرور على جميع خانات التخزين لتجنيد المنتظرين فوراً
        for (int i = 0; i < storageButtons.Length; i++)
        {
            // طالما يوجد جنود منتظرين في هذه الخانة
            while (storageCounts[i] > 0)
            {
                string warriorName = storageWarriorNames[i];

                // التحقق مما إذا كانت الثكنة تتسع لهذا الجندي تحديداً
                if (Barracks.instnce != null && Barracks.instnce.CanAddSpecificWarrior(warriorName))
                {
                    storageCounts[i]--; // خصم الجندي من قائمة الانتظار
                    SpawnWarrior(warriorDict[warriorName]); // تجنيده وبعثه فوراً
                }
                else
                {
                    // إذا لم تعد الثكنة تتسع، نتوقف عن سحب المزيد (سيبقون كـ مخزون)
                    break;
                }
            }
            // تحديث واجهة الخانة (الرقم المعروض عليها) بعد العملية
            UpdateStorageUI(i);
        }

        // 4. ترتيب الطابور واستئناف نظام التدريب الطبيعي لمن تبقى (إن وجدوا ولم تتسع لهم الثكنة)
        ShiftQueue();
        StartNextFromStorage();
    }

    // ==========================================
    // دوال نظام الحفظ (Save / Load)
    // ==========================================

    public TrainingPanelSaveData GetSaveData()
    {
        TrainingPanelSaveData data = new TrainingPanelSaveData();
        data.panelID = this.panelID;
        data.storageWarriorNames = this.storageWarriorNames;
        data.storageCounts = this.storageCounts;
        data.readyCounts = this.readyCounts;
        data.isTraining = this.isTraining;
        data.currentTrainingWarrior = this.currentlyTrainingWarriorName;
        data.remainingTimeInSeconds = this.currentTrainingTimeRemaining;
        return data;
    }

    public void LoadAndCatchUp(TrainingPanelSaveData data, long lastGlobalSaveTicks)
    {
        // 1. إيقاف أي تدريب قديم قيد التشغيل وقفل النزول مؤقتاً
        StopAllCoroutines();
        isTraining = false;
        currentlyTrainingWarriorName = "";
        isLoading = true;

        // 2. تحميل البيانات (استخدام Clone لضمان استقلالية المصفوفات)
        this.storageWarriorNames = (string[])data.storageWarriorNames.Clone();
        this.storageCounts = (int[])data.storageCounts.Clone();
        this.readyCounts = (int[])data.readyCounts.Clone();

        for (int i = 0; i < storageButtons.Length; i++)
        {
            UpdateStorageUI(i);
        }

        // 3. تأجيل حساب الوقت والنزول حتى نهاية الفريم لضمان مسح المباني القديمة
        StartCoroutine(ApplyOfflineCatchUp(data, lastGlobalSaveTicks));
    }

    private IEnumerator ApplyOfflineCatchUp(TrainingPanelSaveData data, long lastGlobalSaveTicks)
    {
        // 🟢 1. ننتظر نصف ثانية (بدلاً من إطار واحد) 
        // هذا يضمن 100% أن المباني القديمة تدمرت، والجديدة ظهرت، والثكنة حسبت سعتها الصحيحة بدقة
        yield return new WaitForSeconds(0.5f);

        if (data.isTraining && !string.IsNullOrEmpty(data.currentTrainingWarrior))
        {
            long currentTicks = System.DateTime.Now.Ticks;
            int secondsOffline = (int)((currentTicks - lastGlobalSaveTicks) / System.TimeSpan.TicksPerSecond);
            int remainingTime = data.remainingTimeInSeconds;
            string currentWarrior = data.currentTrainingWarrior;

            while (secondsOffline > 0)
            {
                if (secondsOffline >= remainingTime)
                {
                    secondsOffline -= remainingTime;

                    int targetIdx = FindStorageIndex(currentWarrior);
                    if (targetIdx == -1) targetIdx = 0; // الخانة الاحتياطية

                    if (targetIdx < readyCounts.Length)
                    {
                        // 🟢 2. التعديل الأهم: التأكد من تسجيل اسم الجندي في الخانة إذا كانت فارغة!
                        // بدون هذا السطر، كان النظام يضيف جندي "مجهول" يتجاوز سعة الثكنة
                        if (string.IsNullOrEmpty(storageWarriorNames[targetIdx]))
                        {
                            storageWarriorNames[targetIdx] = currentWarrior;
                        }

                        readyCounts[targetIdx]++;
                        UpdateStorageUI(targetIdx);
                    }

                    bool foundNext = false;
                    for (int i = 0; i < storageButtons.Length; i++)
                    {
                        if (storageCounts[i] > 0)
                        {
                            currentWarrior = storageWarriorNames[i];
                            storageCounts[i]--;
                            UpdateStorageUI(i);

                            if (warriorDict.ContainsKey(currentWarrior))
                            {
                                remainingTime = (int)ConvertTextToSeconds(warriorDict[currentWarrior].trainingDurationText.text);
                                foundNext = true;
                            }
                            break;
                        }
                    }

                    if (!foundNext)
                    {
                        isTraining = false;
                        currentlyTrainingWarriorName = "";
                        isLoading = false; // فك القفل
                        yield break;
                    }
                }
                else
                {
                    remainingTime -= secondsOffline;
                    secondsOffline = 0;
                }
            }

            // استكمال التدريب لما تبقى من الوقت
            if (warriorDict.ContainsKey(currentWarrior))
            {
                StartCoroutine(TrainingProcess(warriorDict[currentWarrior], remainingTime));
            }
        }

        // 🟢 فك القفل بعد انتهاء كل العمليات لتتمكن دالة Update من استئناف عملها بشكل طبيعي
        isLoading = false;
    }

}
