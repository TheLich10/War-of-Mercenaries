using UnityEngine;

public class buildWoodStroge : MonoBehaviour
{
    public string buildingName = "LUMBER BLADE"; // اسم المبنى
    public int level = 1; // مستوى المبنى الحالي

    public event System.Action<int, int> OnWoodChanged;

    [Header("الإنتاج")]
    public int maxwood = 5; // السعة القصوى
    public int currentwood = 0; // الكمية الحالية
    private float woodPerHour = 720f;
    private float woodGenerationTime = 5f;
    [HideInInspector] public float timer = 0f;

    [Header("ايقونة الموارد")]
    public GameObject resourceIconPrefab; // اسحب Prefab أيقونة الخشب من الـInspector
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
                woodPerHour = levelData.PerHoures;
                maxwood = levelData.CityStroge;

                // حساب الوقت اللازم لإنتاج خشبة واحدة
                woodGenerationTime = 3600f / woodPerHour;

                //Debug.Log($"[LumberBlade] Lvl: {level} | Produces: {woodPerHour}/hr | Max: {maxwood} | One log every {woodGenerationTime} sec"); ديبك
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
        if (currentwood < maxwood)
        {
            timer += Time.deltaTime;
            if (timer >= woodGenerationTime)
            {
                currentwood++;
                timer = 0f;
                OnWoodChanged?.Invoke(currentwood, maxwood);
            }
        }

        // تحقق من النسبة > 10%
        float percent = (float)currentwood / maxwood;
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
            float floatY = Mathf.Sin(Time.time * 2f) * 0.2f; // يتحرك لأعلى وأسفل
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

    // ✅ التعديل هنا: الدالة الآن تستقبل المساحة المتاحة في المخزن كمتغير
    public int CollectGold(int availableCapacity)
    {
        if (availableCapacity <= 0) return 0; // إذا لم يكن هناك مساحة، لا تجمع شيء

        // حساب الكمية الممكن جمعها (أيهما أقل: الموجود في المبنى أو المساحة المتاحة في المخزن)
        int amountToCollect = Mathf.Min(currentwood, availableCapacity);

        // خصم الكمية التي تم جمعها من المبنى فقط
        currentwood -= amountToCollect;
        OnWoodChanged?.Invoke(currentwood, maxwood);

        // إخفاء الأيقونة إذا نزل المخزون في المبنى تحت 10%
        if ((float)currentwood / maxwood < 0.1f)
        {
            HideResourceIcon();
        }

        // إرجاع الكمية التي تم جمعها فعلياً ليتم إضافتها في ResourceManager
        return amountToCollect;
    }

    public int GetCurrentWood()
    {
        return currentwood;
    }

    public float GetMaxWood()
    {
        return maxwood;
    }

    public float GetTimeToFull()
    {
        // إذا كان المبنى ممتلئاً بالفعل، الوقت المتبقي هو صفر
        if (currentwood >= maxwood) return 0f;

        // حساب عدد قطع الخشب المتبقية للامتلاء
        int remainingWood = maxwood - currentwood;

        // الحسبة: (القطع المتبقية × الوقت اللازم للقطعة الواحدة) ونطرح منها الوقت المنقضي في العداد الحالي
        float timeToFull = (remainingWood * woodGenerationTime) - timer;

        // Mathf.Max تضمن عدم إرجاع أي قيمة سالبة تحت أي ظرف
        return Mathf.Max(0f, timeToFull);
    }

    // 🟢 دالة تحميل البيانات وحساب الموارد أثناء غياب اللاعب (للخشب)
    public void LoadAndCatchUp(int savedWood, float savedTimer, long lastSaveTimeTicks)
    {
        currentwood = savedWood;
        timer = savedTimer;

        if (lastSaveTimeTicks > 0)
        {
            long ticksPassed = System.DateTime.Now.Ticks - lastSaveTimeTicks;
            float secondsPassed = (float)(ticksPassed / System.TimeSpan.TicksPerSecond);

            if (currentwood < maxwood)
            {
                timer += secondsPassed;
                int generatedWood = Mathf.FloorToInt(timer / woodGenerationTime);

                if (generatedWood > 0)
                {
                    currentwood += generatedWood;
                    timer = timer % woodGenerationTime;
                }

                if (currentwood > maxwood)
                {
                    currentwood = maxwood;
                    timer = 0f;
                }
            }
        }

        OnWoodChanged?.Invoke(currentwood, maxwood);
        float percent = (float)currentwood / maxwood;
        if (percent >= 0.1f && !iconActive)
        {
            ShowResourceIcon();
        }
    }
}