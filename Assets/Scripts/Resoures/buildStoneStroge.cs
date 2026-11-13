using UnityEngine;

public class buildStoneStroge : MonoBehaviour
{
    public event System.Action<int, int> OnStoneChanged;

    public string buildingName = "STONE GRINDER"; // اسم المبنى الخاص بالحجر
    public int level = 1; // مستوى المبنى الحالي

    [Header("الإنتاج")]
    public int maxStone = 5; // السعة القصوى
    public int currentStone = 0; // الكمية الحالية
    private float stonePerHour = 720f; // كمية الإنتاج بالساعة
    private float stoneGenerationTime = 5f; // كل كم ثانية يتم إنتاج حجر واحد
    [HideInInspector] public float timer = 0f; // تم التعديل هنا
    [Header("أيقونة الموارد")]
    public GameObject resourceIconPrefab; // اسحب Prefab أيقونة الحجر من الـInspector
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
                stonePerHour = levelData.PerHoures;
                maxStone = levelData.CityStroge;
                stoneGenerationTime = 3600f / stonePerHour;

                //Debug.Log($"[StoneBlade] Lvl: {level} | Produces: {stonePerHour}/hr | Max: {maxStone} | One stone every {stoneGenerationTime} sec");
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
        if (currentStone < maxStone)
        {
            timer += Time.deltaTime;
            if (timer >= stoneGenerationTime)
            {
                currentStone++;
                timer = 0f;
                OnStoneChanged?.Invoke(currentStone, maxStone);
            }
        }

        // تحقق من النسبة > 10%
        float percent = (float)currentStone / maxStone;
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
        int amountToCollect = Mathf.Min(currentStone, availableCapacity);

        // خصم الكمية التي تم جمعها فقط من المبنى
        currentStone -= amountToCollect;
        OnStoneChanged?.Invoke(currentStone, maxStone);

        // إخفاء الأيقونة فقط إذا نزل المخزون داخل المبنى تحت 10%
        if ((float)currentStone / maxStone < 0.1f)
        {
            HideResourceIcon();
        }

        // إرجاع الكمية المجمعة فعلياً
        return amountToCollect;
    }

    public int GetCurrentStone()
    {
        return currentStone;
    }

    public float GetMaxStone()
    {
        return maxStone;
    }

    public float GetTimeToFull()
    {
        // إذا كان المبنى ممتلئاً بالفعل، الوقت المتبقي هو صفر
        if (currentStone >= maxStone) return 0f;

        // حساب عدد قطع الخشب المتبقية للامتلاء
        int remainingWood = maxStone - currentStone;

        // الحسبة: (القطع المتبقية × الوقت اللازم للقطعة الواحدة) ونطرح منها الوقت المنقضي في العداد الحالي
        float timeToFull = (remainingWood * stoneGenerationTime) - timer;

        // Mathf.Max تضمن عدم إرجاع أي قيمة سالبة تحت أي ظرف
        return Mathf.Max(0f, timeToFull);
    }
    // 🟢 دالة تحميل البيانات وحساب الموارد أثناء غياب اللاعب (الـ Offline Progress)
    public void LoadAndCatchUp(int savedStone, float savedTimer, long lastSaveTimeTicks)
    {
        currentStone = savedStone;
        timer = savedTimer;

        // إذا كان هناك وقت محفوظ مسبقاً (اللاعب لم يبدأ من الصفر)
        if (lastSaveTimeTicks > 0)
        {
            // حساب الثواني التي مرت منذ إغلاق اللعبة
            long ticksPassed = System.DateTime.Now.Ticks - lastSaveTimeTicks;
            float secondsPassed = (float)(ticksPassed / System.TimeSpan.TicksPerSecond);

            if (currentStone < maxStone)
            {
                // إضافة الوقت المنقضي إلى العداد الحالي
                timer += secondsPassed;

                // حساب عدد الأحجار التي تم إنتاجها خلال هذا الوقت
                int generatedStones = Mathf.FloorToInt(timer / stoneGenerationTime);

                if (generatedStones > 0)
                {
                    currentStone += generatedStones;
                    // الاحتفاظ بالكسور من الوقت المتبقي لضمان دقة الإنتاج
                    timer = timer % stoneGenerationTime;
                }

                // التأكد من أن الكمية لا تتجاوز الحد الأقصى للمخزن
                if (currentStone > maxStone)
                {
                    currentStone = maxStone;
                    timer = 0f;
                }
            }
        }

        // تحديث الواجهة والأيقونة فوراً بعد الحساب
        OnStoneChanged?.Invoke(currentStone, maxStone);
        float percent = (float)currentStone / maxStone;
        if (percent >= 0.1f && !iconActive)
        {
            ShowResourceIcon();
        }
    }
}
