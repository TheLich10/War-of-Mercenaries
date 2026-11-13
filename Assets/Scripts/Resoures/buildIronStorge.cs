using UnityEngine;

public class buildIronStorge : MonoBehaviour
{
    public event System.Action<int, int> OnIronChanged;

    public string buildingName = "IRON SMELTER"; // اسم مبنى الحديد
    public int level = 1; // المستوى الحالي للمبنى

    [Header("الإنتاج")]
    public int maxIron = 5; // الحد الأقصى للتخزين
    public int currentIron = 0; // الكمية الحالية
    private float ironPerHour = 720f; // كمية الحديد التي تُنتج في الساعة
    private float ironGenerationTime = 5f; // كم ثانية لإنتاج واحدة
    [HideInInspector] public float timer = 0f;

    [Header("أيقونة الموارد")]
    public GameObject resourceIconPrefab; // اسحب Prefab أيقونة الحديد من الـInspector
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
                ironPerHour = levelData.PerHoures;
                maxIron = levelData.CityStroge;
                ironGenerationTime = 3600f / ironPerHour;

                //Debug.Log($"[IronBlade] Lvl: {level} | Produces: {ironPerHour}/hr | Max: {maxIron} | One iron every {ironGenerationTime} sec");
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
        if (currentIron < maxIron)
        {
            timer += Time.deltaTime;
            if (timer >= ironGenerationTime)
            {
                currentIron++;
                timer = 0f;
                OnIronChanged?.Invoke(currentIron, maxIron);
            }
        }

        // تحقق من النسبة > 10%
        float percent = (float)currentIron / maxIron;
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
        int amountToCollect = Mathf.Min(currentIron, availableCapacity);

        // خصم الكمية التي تم جمعها فقط من المبنى
        currentIron -= amountToCollect;
        OnIronChanged?.Invoke(currentIron, maxIron);

        // إخفاء الأيقونة فقط إذا نزل المخزون داخل المبنى تحت 10%
        if ((float)currentIron / maxIron < 0.1f)
        {
            HideResourceIcon();
        }

        // إرجاع الكمية المجمعة فعلياً
        return amountToCollect;
    }

    public int GetCurrentIron()
    {
        return currentIron;
    }

    public float GetMaxIron()
    {
        return maxIron;
    }

    public float GetTimeToFull()
    {
        // إذا كان المبنى ممتلئاً بالفعل، الوقت المتبقي هو صفر
        if (currentIron >= maxIron) return 0f;

        // حساب عدد قطع الخشب المتبقية للامتلاء
        int remainingWood = maxIron - currentIron;

        // الحسبة: (القطع المتبقية × الوقت اللازم للقطعة الواحدة) ونطرح منها الوقت المنقضي في العداد الحالي
        float timeToFull = (remainingWood * ironGenerationTime) - timer;

        // Mathf.Max تضمن عدم إرجاع أي قيمة سالبة تحت أي ظرف
        return Mathf.Max(0f, timeToFull);
    }
    // 🟢 دالة تحميل البيانات وحساب الموارد أثناء غياب اللاعب (للحديد)
    public void LoadAndCatchUp(int savedIron, float savedTimer, long lastSaveTimeTicks)
    {
        currentIron = savedIron;
        timer = savedTimer;

        if (lastSaveTimeTicks > 0)
        {
            long ticksPassed = System.DateTime.Now.Ticks - lastSaveTimeTicks;
            float secondsPassed = (float)(ticksPassed / System.TimeSpan.TicksPerSecond);

            if (currentIron < maxIron)
            {
                timer += secondsPassed;
                int generatedIron = Mathf.FloorToInt(timer / ironGenerationTime);

                if (generatedIron > 0)
                {
                    currentIron += generatedIron;
                    timer = timer % ironGenerationTime;
                }

                if (currentIron > maxIron)
                {
                    currentIron = maxIron;
                    timer = 0f;
                }
            }
        }

        OnIronChanged?.Invoke(currentIron, maxIron);
        float percent = (float)currentIron / maxIron;
        if (percent >= 0.1f && !iconActive)
        {
            ShowResourceIcon();
        }
    }
}
