using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUI : MonoBehaviour
{
    public TMP_Text workerText;      // لعرض عدد العمّال (المتاح/المملوك)
    public TMP_Text costText;        // لعرض تكلفة شراء العامل الجديد
    public Button buyButton;         // زر الشراء

    // مصفوفة لربط الأوبجكتات الخاصة بالعمال من الـ Inspector
    // العنصر [0] = الأوبجكت الخاص بالعامل الثاني
    // العنصر [1] = الأوبجكت الخاص بالعامل الثالث
    // العنصر [2] = الأوبجكت الخاص بالعامل الرابع
    // العنصر [3] = الأوبجكت الخاص بالعامل الخامس
    public GameObject[] workerObjects;

    private int totalOwnedWorkers = 1;   // عدد العمّال الذين تملكهم حالياً
    private int availableWorkers = 1;    // عدد العمّال المتاحين حالياً

    public static WorkerUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// محاولة استخدام عامل. تنجح فقط إذا كان هناك عمال متاحون.
    /// </summary>
    public bool TryUseWorker()
    {
        if (availableWorkers > 0)
        {
            availableWorkers--;
            UpdateUI();
            return true;
        }
        else
        {
            Debug.LogWarning("❌ لا يوجد عمال متاحون حالياً!");
            return false;
        }
    }

    /// <summary>
    /// استرجاع عامل بعد انتهاء عمله.
    /// </summary>
    public void ReturnWorker()
    {
        if (availableWorkers < totalOwnedWorkers)
        {
            availableWorkers++;
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("⚠️ جميع العمال متاحون بالفعل!");
        }
    }

    /// <summary>
    /// شراء عامل جديد.
    /// </summary>
    public void BuyWorker()
    {
        int maxLimit = 5; // الحد الأقصى لعدد العمّال
        if (totalOwnedWorkers < maxLimit)
        {
            totalOwnedWorkers++;
            availableWorkers++; // العامل الجديد يكون متاح

            // تفعيل الأوبجكت المرتبط بالعامل الجديد بناءً على رقمه
            // تم استخدام (totalOwnedWorkers - 2) لأن المصفوفة تبدأ من الصفر، والعامل الجديد يبدأ من الرقم 2
            int objectIndex = totalOwnedWorkers - 2;
            if (workerObjects != null && objectIndex >= 0 && objectIndex < workerObjects.Length)
            {
                if (workerObjects[objectIndex] != null)
                {
                    workerObjects[objectIndex].SetActive(true);
                }
            }

            UpdateUI();
        }
        else
        {
            Debug.Log("🚫 وصلت إلى الحد الأقصى للعمال.");
        }
    }

    /// <summary>
    /// التحقق من توفر عامل.
    /// </summary>
    public bool HasAvailableWorker()
    {
        return availableWorkers > 0;
    }

    /// <summary>
    /// تحديث واجهة المستخدم.
    /// </summary>
    private void UpdateUI()
    {
        // تحديث نص العمال
        workerText.text = $"{availableWorkers}/{totalOwnedWorkers}";

        // حساب تكلفة العامل القادم
        int cost = GetNextWorkerCost();

        // إذا لم تصل للحد الأقصى → أظهر التكلفة
        if (totalOwnedWorkers < 5)
        {
            costText.text = $"Buy  {cost}";
            buyButton.gameObject.SetActive(true);
        }
        else
        {
            // إخفاء الزر إذا وصلت للحد
            buyButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// حساب تكلفة العامل القادم حسب عدد العمّال الحاليين.
    /// </summary>
    private int GetNextWorkerCost()
    {
        switch (totalOwnedWorkers)
        {
            case 1: return 50;
            case 2: return 500;
            case 3: return 1000;
            case 4: return 2000; // تقدر تعدل الرقم لو مختلف عندك
            default: return 0;
        }
    }

    public int GetAvailableWorkers() => availableWorkers;
    public int GetOwnedWorkers() => totalOwnedWorkers;

    // 🟢 دالة 1: يتم استدعاؤها عند فتح اللعبة لإرجاع عدد العمال الكلي وتفعيل المجسمات
    public void LoadOwnedWorkers(int totalSaved)
    {
        totalOwnedWorkers = totalSaved;
        availableWorkers = totalSaved; // نجعل الجميع متاحين مبدئياً

        // تفعيل أو تعطيل الأوبجكتات (المجسمات) الخاصة بالعمال حسب العدد المحفوظ
        if (workerObjects != null)
        {
            for (int i = 0; i < workerObjects.Length; i++)
            {
                if (totalOwnedWorkers >= i + 2)
                    workerObjects[i].SetActive(true);
                else
                    workerObjects[i].SetActive(false);
            }
        }

        UpdateUI();
    }

    // 🟢 دالة 2: يتم استدعاؤها من نظام الحفظ لسحب عامل إذا كان المبنى لا يزال قيد الإنشاء
    public void OccupyWorkerDuringLoad()
    {
        if (availableWorkers > 0)
        {
            availableWorkers--;
            UpdateUI();
        }
    }
}