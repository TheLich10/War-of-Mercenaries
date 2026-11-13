using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ResourceManager : MonoBehaviour
{
    // Singleton
    public static ResourceManager Instance;
    public event System.Action<int, int> OnResourcesChanged;

    public Image woodBarImg;
    public Image stoneBarImg;
    public Image ironBarImg;
    public Image powerBarImg;

    // كائنات تحتوي على النص + الصورة معًا
    public RectTransform woodUIGroup;
    public RectTransform stoneUIGroup;
    public RectTransform ironUIGroup;
    public RectTransform powerUIGroup;

    void Awake()
    {
        Instance = this;
    }

    // 🔹 التعديل الأول: فصل الموارد كقيم ثابتة (Global) لتبقى محفوظة وتتعدل من أي مشهد
    public static int globalIron = 10000;
    public static int globalWood = 10000;
    public static int globalStone = 10000;
    public static int globalPower = 10000;

    // 🔹 التعديل الثاني: استخدام الخصائص لربطها بالقيم الثابتة (لكي لا تتعطل السكربتات الأخرى)
    public int iron { get => globalIron; set => globalIron = value; }
    public int wood { get => globalWood; set => globalWood = value; }
    public int stone { get => globalStone; set => globalStone = value; }
    public int power { get => globalPower; set => globalPower = value; }

    public int cityStorageCapacity = 1000000000;
    public int storagePerBuilding = 100;

    // نصوص TMP
    public TMP_Text ironText;
    public TMP_Text woodText;
    public TMP_Text stoneText;
    public TMP_Text powerText;

    [Header("Short Resource")]
    public TMP_Text shortIronText;
    public TMP_Text shortWoodText;
    public TMP_Text shortStoneText;
    public TMP_Text shortPowerText;

    private Dictionary<RectTransform, Coroutine> activePulses = new Dictionary<RectTransform, Coroutine>();
    private Dictionary<RectTransform, Vector3> originalScales = new Dictionary<RectTransform, Vector3>();

    [Header("الصورة عند الامتلاء")]
    public GameObject redBarIron;
    public GameObject redBarWood;
    public GameObject redBarStone;
    public GameObject redBarPower;

    public TMP_Text maxStroge;
    public TMP_Text maxStroge2;
    public TMP_Text maxStroge3;
    public TMP_Text maxStroge4;

    private List<int> storageBuildings = new List<int>();

    // 🔹 إضافة دوال الجلب لكي يقرأها سكربت ResourceWar في أي مشهد
    public static int GetIron() { return globalIron; }
    public static int GetWood() { return globalWood; }
    public static int GetStone() { return globalStone; }
    public static int GetPower() { return globalPower; }

    public void AddStorageBuilding(int capacity)
    {
        storageBuildings.Add(capacity);
        UpdateStorageValues();
        UpdateUI();
    }

    public void RemoveStorageBuilding(int capacity)
    {
        if (storageBuildings.Contains(capacity))
        {
            storageBuildings.Remove(capacity);
            UpdateStorageValues();
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("لم يتم العثور على مبنى تخزين بهذه السعة: " + capacity);
        }
    }

    public int GetTotalStorageCapacity()
    {
        int total = cityStorageCapacity;
        foreach (int capacity in storageBuildings)
        {
            total += capacity;
        }
        return total;
    }

    public int GetTotalCapacity()
    {
        return GetTotalStorageCapacity();
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("iron"))
        {
            iron = PlayerPrefs.GetInt("iron");
            wood = PlayerPrefs.GetInt("wood");
            stone = PlayerPrefs.GetInt("stone");
            power = PlayerPrefs.GetInt("power");

            PlayerPrefs.DeleteKey("iron");
            PlayerPrefs.DeleteKey("wood");
            PlayerPrefs.DeleteKey("stone");
            PlayerPrefs.DeleteKey("power");
            PlayerPrefs.Save();

            Invoke("SaveGameDelayed", 0.1f);
        }

        //if (EndLoader.isWar == true)
        //{
        //    AddIron(PlayerResources.iron);
        //    PlayerResources.iron = 0;

        //    AddPower(PlayerResources.power);
        //    PlayerResources.power = 0;

        //    AddStone(PlayerResources.stone);
        //    PlayerResources.stone = 0;

        //    AddWood(PlayerResources.wood);
        //    PlayerResources.wood = 0;

        //    PlayerResources.instance.UpdateUI();
        //    EndLoader.isWar = false;
        //}

        UpdateUI();
    }

    void SaveGameDelayed()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();
    }

    public ResourceSaveData GetResourceData()
    {
        return new ResourceSaveData { iron = iron, wood = wood, stone = stone, power = power };
    }

    public void LoadResources(ResourceSaveData data)
    {
        if (data == null) return;
        iron = data.iron;
        wood = data.wood;
        stone = data.stone;
        power = data.power;

        UpdateUI();
        OnResourcesChanged?.Invoke(GetTotalResourcesAmount(), GetTotalResourcesCapacity());
    }

    public void UpdateUI()
    {
        int totalCapacity = GetTotalCapacity();

        UpdateResourceUI(iron, totalCapacity, ironText, shortIronText, ironBarImg, redBarIron);
        UpdateResourceUI(wood, totalCapacity, woodText, shortWoodText, woodBarImg, redBarWood);
        UpdateResourceUI(stone, totalCapacity, stoneText, shortStoneText, stoneBarImg, redBarStone);
        UpdateResourceUI(power, totalCapacity, powerText, shortPowerText, powerBarImg, redBarPower);

        if (maxStroge != null) maxStroge.text = totalCapacity.ToString();
        if (maxStroge2 != null) maxStroge2.text = totalCapacity.ToString();
        if (maxStroge3 != null) maxStroge3.text = totalCapacity.ToString();
        if (maxStroge4 != null) maxStroge4.text = totalCapacity.ToString();
    }

    private void UpdateResourceUI(int currentAmount, int maxCapacity, TMP_Text normalText, TMP_Text shortText, Image bar, GameObject redBar)
    {
        if (normalText != null) normalText.text = currentAmount.ToString();
        if (shortText != null) shortText.text = FormatShortNumber(currentAmount);

        if (redBar != null) redBar.SetActive(currentAmount >= maxCapacity);
        if (bar != null) bar.fillAmount = Mathf.Clamp01((float)currentAmount / maxCapacity);
    }

    // 🔹 التعديل الثالث: تحديث دوال الإضافة لتعمل سواء كنا في نفس المشهد أو مشهد آخر
    public static void AddIron(int amount)
    {
        if (Instance != null)
        {
            int capacityLeft = Instance.GetTotalCapacity() - globalIron;
            int amountToAdd = Mathf.Min(amount, capacityLeft);

            if (amountToAdd > 0)
            {
                Instance.StartCoroutine(Instance.AddResourceGradually(amountToAdd, "iron"));
            }
        }
        else
        {
            // خصم / إضافة مباشرة إذا كنا في مشهد آخر (بدون الحاجة لـ UI)
            globalIron += amount;
        }
    }

    public static void AddWood(int amount)
    {
        if (Instance != null)
        {
            int capacityLeft = Instance.GetTotalCapacity() - globalWood;
            int amountToAdd = Mathf.Min(amount, capacityLeft);

            if (amountToAdd > 0)
            {
                Instance.StartCoroutine(Instance.AddResourceGradually(amountToAdd, "wood"));
            }
        }
        else
        {
            globalWood += amount;
        }
    }

    public static void AddStone(int amount)
    {
        if (Instance != null)
        {
            int capacityLeft = Instance.GetTotalCapacity() - globalStone;
            int amountToAdd = Mathf.Min(amount, capacityLeft);

            if (amountToAdd > 0)
            {
                Instance.StartCoroutine(Instance.AddResourceGradually(amountToAdd, "stone"));
            }
        }
        else
        {
            globalStone += amount;
        }
    }

    public static void AddPower(int amount)
    {
        if (Instance != null)
        {
            int capacityLeft = Instance.GetTotalCapacity() - globalPower;
            int amountToAdd = Mathf.Min(amount, capacityLeft);

            if (amountToAdd > 0)
            {
                Instance.StartCoroutine(Instance.AddResourceGradually(amountToAdd, "power"));
            }
        }
        else
        {
            globalPower += amount;
        }
    }

    private IEnumerator AddResourceGradually(int amountToAdd, string resourceName)
    {
        int added = 0;
        int step = 1;

        if (resourceName == "iron" && ironUIGroup != null) StartCoroutine(PulseObject(ironUIGroup));
        else if (resourceName == "wood" && woodUIGroup != null) StartCoroutine(PulseObject(woodUIGroup));
        else if (resourceName == "stone" && stoneUIGroup != null) StartCoroutine(PulseObject(stoneUIGroup));
        else if (resourceName == "power" && powerUIGroup != null) StartCoroutine(PulseObject(powerUIGroup));

        while (added < amountToAdd)
        {
            int remaining = amountToAdd - added;
            int increment = Mathf.Min(step, remaining);

            if (resourceName == "iron") iron += increment;
            else if (resourceName == "wood") wood += increment;
            else if (resourceName == "stone") stone += increment;
            else if (resourceName == "power") power += increment;

            added += increment;

            UpdateUI();
            OnResourcesChanged?.Invoke(GetTotalResourcesAmount(), GetTotalResourcesCapacity());

            if (TimeWorirRecqure.Instance != null)
                TimeWorirRecqure.Instance.UpdateIronDisplay();

            step *= 2;
            yield return new WaitForSeconds(0.05f);
        }
    }

    // 🔹 التعديل الرابع: تحديث دوال الحذف لتعمل في أي مشهد
    public static void RemoveIron(int amount)
    {
        amount = Mathf.Min(amount, globalIron);
        if (amount > 0)
        {
            if (Instance != null)
                Instance.StartCoroutine(Instance.RemoveResourceGradually(amount, "iron"));
            else
                globalIron -= amount; // خصم مباشر للمشهد الخارجي
        }
    }

    public static void RemoveWood(int amount)
    {
        amount = Mathf.Min(amount, globalWood);
        if (amount > 0)
        {
            if (Instance != null)
                Instance.StartCoroutine(Instance.RemoveResourceGradually(amount, "wood"));
            else
                globalWood -= amount;
        }
    }

    public static void RemoveStone(int amount)
    {
        amount = Mathf.Min(amount, globalStone);
        if (amount > 0)
        {
            if (Instance != null)
                Instance.StartCoroutine(Instance.RemoveResourceGradually(amount, "stone"));
            else
                globalStone -= amount;
        }
    }

    public static void RemovePower(int amount)
    {
        amount = Mathf.Min(amount, globalPower);
        if (amount > 0)
        {
            if (Instance != null)
                Instance.StartCoroutine(Instance.RemoveResourceGradually(amount, "power"));
            else
                globalPower -= amount;
        }
    }

    private IEnumerator RemoveResourceGradually(int amountToRemove, string resourceName)
    {
        int removed = 0;
        int step = 1;

        if (resourceName == "iron" && ironUIGroup != null) StartCoroutine(PulseObject(ironUIGroup));
        else if (resourceName == "wood" && woodUIGroup != null) StartCoroutine(PulseObject(woodUIGroup));
        else if (resourceName == "stone" && stoneUIGroup != null) StartCoroutine(PulseObject(stoneUIGroup));
        else if (resourceName == "power" && powerUIGroup != null) StartCoroutine(PulseObject(powerUIGroup));

        while (removed < amountToRemove)
        {
            int remaining = amountToRemove - removed;
            int decrement = Mathf.Min(step, remaining);

            if (resourceName == "iron") { iron -= decrement; if (iron < 0) iron = 0; }
            else if (resourceName == "wood") { wood -= decrement; if (wood < 0) wood = 0; }
            else if (resourceName == "stone") { stone -= decrement; if (stone < 0) stone = 0; }
            else if (resourceName == "power") { power -= decrement; if (power < 0) power = 0; }

            removed += decrement;

            UpdateUI();
            OnResourcesChanged?.Invoke(GetTotalResourcesAmount(), GetTotalResourcesCapacity());

            if (TimeWorirRecqure.Instance != null)
                TimeWorirRecqure.Instance.UpdateIronDisplay();

            step *= 2;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator PulseObject(RectTransform target)
    {
        if (!originalScales.ContainsKey(target))
            originalScales[target] = target.localScale;

        if (activePulses.ContainsKey(target) && activePulses[target] != null)
            StopCoroutine(activePulses[target]);

        Coroutine newPulse = StartCoroutine(PulseCoroutine(target));
        activePulses[target] = newPulse;

        yield return null;
    }

    private IEnumerator PulseCoroutine(RectTransform target)
    {
        Vector3 originalScale = originalScales[target];
        Vector3 maxAllowedScale = originalScale * 1.1f;

        if (target.localScale.x > maxAllowedScale.x)
            yield break;

        Vector3 targetScale = maxAllowedScale;
        float duration = 0.5f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            yield return null;
        }

        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
            yield return null;
        }

        target.localScale = originalScale;
        activePulses[target] = null;
    }

    public void ResetAllResources()
    {
        iron = 0;
        wood = 0;
        stone = 0;
        power = 0;
        Instance.OnResourcesChanged?.Invoke(Instance.GetTotalResourcesAmount(), Instance.GetTotalResourcesCapacity());
        UpdateUI();
    }

    public int GetTotalResourcesAmount()
    {
        return iron + wood + stone + power;
    }

    public int GetTotalResourcesCapacity()
    {
        int singleResourceCapacity = GetTotalCapacity();
        return singleResourceCapacity * 4;
    }

    // ✅ دالة Static لتجميع الموارد من أي مشهد وإرسالها للحفظ (بالضبط مثل GetGold)
    public static ResourceSaveData GetGlobalResourceData()
    {
        return new ResourceSaveData
        {
            iron = globalIron,
            wood = globalWood,
            stone = globalStone,
            power = globalPower
        };
    }

    public void UpdateStorageValues()
    {
        int totalStorageBuildingsCapacity = 0;
        foreach (int capacity in storageBuildings)
        {
            totalStorageBuildingsCapacity += capacity;
        }
        storagePerBuilding = totalStorageBuildingsCapacity;
    }

    private string FormatShortNumber(int number)
    {
        if (number >= 1_000_000)
            return (number / 1_000_000f).ToString("0.#") + "M";
        else if (number >= 1_000)
            return (number / 1_000f).ToString("0.#") + "k";
        else
            return number.ToString();
    }
}