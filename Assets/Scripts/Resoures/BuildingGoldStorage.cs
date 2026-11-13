using UnityEngine;

public class BuildingGoldStorage : MonoBehaviour
{
    public event System.Action<int, int> OnGoldChanged; // (current, max)

    [Header("الإنتاج")]
    public int maxGold = 5; // الحد الأقصى لتخزين الذهب
    public int currentGold = 0; // الذهب المخزن حاليًا
    public float goldGenerationTime = 5f; // كل كم ثانية يضيف ذهب
    [HideInInspector] public float timer = 0f;

    [Header("أيقونة الموارد")]
    public GameObject resourceIconPrefab; // اسحب Prefab أيقونة الذهب من الـInspector
    private GameObject resourceIconInstance;
    private Vector3 iconStartPos;
    private bool iconActive = false;

    private void Update()
    {
        // توليد الذهب بمرور الوقت
        if (currentGold < maxGold)
        {
            timer += Time.deltaTime;
            if (timer >= goldGenerationTime)
            {
                currentGold++; // إضافة ذهب واحد
                timer = 0f; // إعادة تعيين المؤقت
                OnGoldChanged?.Invoke(currentGold, maxGold);
            }
        }

        // تحقق من النسبة > 10%
        float percent = (float)currentGold / maxGold;
        if (percent >= 0.5f && !iconActive)
        {
            ShowResourceIcon();
        }
        else if (percent < 0.5f && iconActive)
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
            Vector3 spawnPos = transform.position + new Vector3(0, 2f, 0); // فوق المبنى
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

    public int CollectGold()
    {
        int collectedGold = currentGold; // تخزين الذهب المخزن لجمعه
        currentGold = 0; // إعادة العداد إلى الصفر
        OnGoldChanged?.Invoke(currentGold, maxGold);

        // اخفاء الايقونة بعد الجمع
        HideResourceIcon();

        return collectedGold;
    }

    public int GetCurrentGold()
    {
        return currentGold;
    }

    public float GetTimeToFull()
    {
        // إذا كان المبنى ممتلئاً بالفعل، الوقت المتبقي هو صفر
        if (currentGold >= maxGold) return 0f;

        // حساب عدد قطع الخشب المتبقية للامتلاء
        int remainingWood = maxGold - currentGold;

        // الحسبة: (القطع المتبقية × الوقت اللازم للقطعة الواحدة) ونطرح منها الوقت المنقضي في العداد الحالي
        float timeToFull = (remainingWood * goldGenerationTime) - timer;

        // Mathf.Max تضمن عدم إرجاع أي قيمة سالبة تحت أي ظرف
        return Mathf.Max(0f, timeToFull);
    }
    // 🟢 دالة تحميل البيانات وحساب الموارد أثناء غياب اللاعب (للذهب)
    public void LoadAndCatchUp(int savedGold, float savedTimer, long lastSaveTimeTicks)
    {
        currentGold = savedGold;
        timer = savedTimer;

        if (lastSaveTimeTicks > 0)
        {
            long ticksPassed = System.DateTime.Now.Ticks - lastSaveTimeTicks;
            float secondsPassed = (float)(ticksPassed / System.TimeSpan.TicksPerSecond);

            if (currentGold < maxGold)
            {
                timer += secondsPassed;
                int generatedGold = Mathf.FloorToInt(timer / goldGenerationTime);

                if (generatedGold > 0)
                {
                    currentGold += generatedGold;
                    timer = timer % goldGenerationTime;
                }

                if (currentGold > maxGold)
                {
                    currentGold = maxGold;
                    timer = 0f;
                }
            }
        }

        OnGoldChanged?.Invoke(currentGold, maxGold);
        float percent = (float)currentGold / maxGold;
        if (percent >= 0.5f && !iconActive) // نسبة 50% للذهب كما حددتها
        {
            ShowResourceIcon();
        }
    }
}
