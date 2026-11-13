using UnityEngine;

public class buildPowerStroge : MonoBehaviour
{
    public event System.Action<int, int> OnPowerStorageChanged;

    public string buildingName = "STATUE OF MIGHT"; // اسم المبنى الخاص بالطاقة
    public int level = 1; // المستوى الحالي للمبنى

    [Header("الإنتاج")]
    public int maxPower = 5; // الحد الأقصى للتخزين
    public int currentPower = 0; // الطاقة المخزنة حاليًا
    private float powerPerHour = 720f; // إنتاج الطاقة في الساعة
    private float powerGenerationTime = 5f; // الوقت بين كل طاقة
    [HideInInspector] public float timer = 0f;
    [Header("أيقونة الموارد")]
    public GameObject resourceIconPrefab; // اسحب Prefab أيقونة الطاقة من الـInspector
    private GameObject resourceIconInstance;
    private Vector3 iconStartPos;
    private bool iconActive = false;

    void Awake()
    {
        InitializeBuildingStats();
    }

    void InitializeBuildingStats()
    {
        BuildingData data = BuildingDatabase.GetBuildingByName(buildingName);
        if (data != null)
        {
            BuildLevelUI levelData = data.GetLevel(level);
            if (levelData != null)
            {
                powerPerHour = levelData.PerHoures;
                maxPower = levelData.CityStroge;
                powerGenerationTime = 3600f / powerPerHour;

                //Debug.Log($"[PowerBlade] Lvl: {level} | Produces: {powerPerHour}/hr | Max: {maxPower} | One unit every {powerGenerationTime} sec");
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

    private void Update()
    {
        // الانتاج
        if (currentPower < maxPower)
        {
            timer += Time.deltaTime;
            if (timer >= powerGenerationTime)
            {
                currentPower++;
                timer = 0f;
                OnPowerStorageChanged?.Invoke(currentPower, maxPower);
            }
        }

        // تحقق من النسبة > 10%
        float percent = (float)currentPower / maxPower;
        if (percent >= 0.1f && !iconActive)
        {
            ShowResourceIcon();
        }
        else if (percent < 0.1f && iconActive)
        {
            HideResourceIcon();
        }

        // تحريك الايقونة اذا موجودة
        if (iconActive && resourceIconInstance != null)
        {
            float floatY = Mathf.Sin(Time.time * 2f) * 0.2f; // حركة لأعلى وأسفل
            resourceIconInstance.transform.position = iconStartPos + new Vector3(0, floatY, 0);
        }
    }

    private void ShowResourceIcon()
    {
        if (resourceIconPrefab != null && resourceIconInstance == null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 1.6f, 0); // فوق المبنى
            resourceIconInstance = Instantiate(resourceIconPrefab, spawnPos, Quaternion.identity, transform);
            iconStartPos = resourceIconInstance.transform.position;
            iconActive = true;
        }
    }

    private void HideResourceIcon()
    {
        if (resourceIconInstance != null)
        {
            Destroy(resourceIconInstance);
            resourceIconInstance = null;
            iconActive = false;
        }
    }

    public int CollectGold(int availableCapacity)
    {
        // إذا لم يكن هناك مساحة، لا يتم جمع أي شيء
        if (availableCapacity <= 0) return 0;

        // حساب الكمية الممكن جمعها (أيهما أقل: الموجود في المبنى أم المساحة المتاحة)
        int amountToCollect = Mathf.Min(currentPower, availableCapacity);

        // خصم الكمية التي تم جمعها فقط من المبنى
        currentPower -= amountToCollect;
        OnPowerStorageChanged?.Invoke(currentPower, maxPower);

        // إخفاء الأيقونة فقط إذا نزل المخزون داخل المبنى تحت 10%
        if ((float)currentPower / maxPower < 0.1f)
        {
            HideResourceIcon();
        }

        // إرجاع الكمية المجمعة فعلياً
        return amountToCollect;
    }

    public int GetCurrentPower()
    {
        return currentPower;
    }

    public float GetMaxPower()
    {
        return maxPower;
    }

    public float GetTimeToFull()
    {
        // إذا كان المبنى ممتلئاً بالفعل، الوقت المتبقي هو صفر
        if (currentPower >= maxPower) return 0f;

        // حساب عدد قطع الخشب المتبقية للامتلاء
        int remainingWood = maxPower - currentPower;

        // الحسبة: (القطع المتبقية × الوقت اللازم للقطعة الواحدة) ونطرح منها الوقت المنقضي في العداد الحالي
        float timeToFull = (remainingWood * powerGenerationTime) - timer;

        // Mathf.Max تضمن عدم إرجاع أي قيمة سالبة تحت أي ظرف
        return Mathf.Max(0f, timeToFull);
    }
    // 🟢 دالة تحميل البيانات وحساب الموارد أثناء غياب اللاعب (للطاقة)
    public void LoadAndCatchUp(int savedPower, float savedTimer, long lastSaveTimeTicks)
    {
        currentPower = savedPower;
        timer = savedTimer;

        if (lastSaveTimeTicks > 0)
        {
            long ticksPassed = System.DateTime.Now.Ticks - lastSaveTimeTicks;
            float secondsPassed = (float)(ticksPassed / System.TimeSpan.TicksPerSecond);

            if (currentPower < maxPower)
            {
                timer += secondsPassed;
                int generatedPower = Mathf.FloorToInt(timer / powerGenerationTime);

                if (generatedPower > 0)
                {
                    currentPower += generatedPower;
                    timer = timer % powerGenerationTime;
                }

                if (currentPower > maxPower)
                {
                    currentPower = maxPower;
                    timer = 0f;
                }
            }
        }

        OnPowerStorageChanged?.Invoke(currentPower, maxPower);
        float percent = (float)currentPower / maxPower;
        if (percent >= 0.1f && !iconActive)
        {
            ShowResourceIcon();
        }
    }
}
