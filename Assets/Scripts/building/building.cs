using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class building : MonoBehaviour
{
    public string buildingName; // اسم المبنى الذي سيتم استخدامه للربط
    public int level = 1; // مستوى المبنى

    [Header("fire Effect")]
    public GameObject lowHealthEffect;

    [Header("destroy build")]
    public GameObject destroyedPrefab; // اسحب هنا الـ Prefab الخاص بالأنقاض

    public GameObject upgradeIcon; // اسحب الصورة من Inspector

    public float maxHealth = 0f;
    public float health;

    public bool Placed { get; private set; }
    public BoundsInt area;

    public GameObject healthBarPrefab; // Prefab مرجع شريط الصحة
    private HealthBarFollow healthBarUI;

    [HideInInspector]
    public bool IsUnderConstruction = false;

    public int sorlevl = 0;
    public GameObject sorlvl1; // اسحب الصورة من Inspector
    public GameObject sorlvl2; // اسحب الصورة من Inspector
    public GameObject sorlvl3; // اسحب الصورة من Inspector
    public GameObject sorlvl4; // اسحب الصورة من Inspector
    //////////////////////////////// SAVING /////////////////////////////////
    // 🟢 متغيرات حفظ وقت البناء
    public long buildEndTimeTicks = 0;
    public float buildTotalDuration = 0f;
    //////////////////////////////// SAVING /////////////////////////////////


    private void Start()
    {
        if (transform.parent != null)
        {
            building parent = transform.parent.GetComponent<building>();
            if (parent != null)
                sorlevl = parent.sorlevl;
        }
        if (lowHealthEffect != null)
        {
            lowHealthEffect.SetActive(false);
        }

        UpdateSorLevelVisual();
        InitializeBuildingStats();
        health = maxHealth;

        // إضافة سعة التخزين إذا كان المبنى من نوع STOCK PILE
        if (buildingName == "STOCK PILE" && SceneManager.GetActiveScene().name == "SampleScene")
        {
            ResourceManager.Instance.AddStorageBuilding(GetStorageCapacity());
        }


        if (healthBarPrefab != null)
        {
            GameObject barGO = Instantiate(healthBarPrefab, GameObject.Find("Canvas").transform);
            healthBarUI = barGO.GetComponent<HealthBarFollow>();

            healthBarUI.target = this.transform;
            healthBarUI.Initialize(maxHealth);
            healthBarUI.UpdateHealth(health);
            barGO.SetActive(false);
        }
    }



    private int GetStorageCapacity()
    {
        if (buildingName != "STOCK PILE") return 0;

        BuildingData data = BuildingDatabase.GetBuildingByName(buildingName);
        if (data != null)
        {
            BuildLevelUI levelData = data.GetLevel(level);
            if (levelData != null)
            {
                return levelData.CityStroge;
            }
        }
        return 0;
    }

    void InitializeBuildingStats()
    {
        if (buildingName == "CENTER CITY")
        {
            // الحصول على خصائص المبنى من قاعدة البيانات
            BuildingInfo data1 = BuildingInfoDatabase.GetBuildingByName(buildingName);
            if (data1 != null)
            {
                BuildingLevelInfo levelData = data1.GetLevel(level);
                if (levelData != null)
                {
                    health = levelData.health;
                    if (sorlevl == 1)
                    {
                        health = health + health * 0.20f;
                    }
                    else if (sorlevl == 2)
                    {
                        health = health + health * 0.30f;
                    }
                    else if (sorlevl == 3)
                    {
                        health = health + health * 0.40f;
                    }
                    else if (sorlevl == 4)
                    {
                        health = health + health * 0.50f;
                    }
                    maxHealth = health;
                    if (level == 1) { ResourceManager.Instance.cityStorageCapacity = 10000; }
                    if (level == 2) { ResourceManager.Instance.cityStorageCapacity = 50000; }
                    if (level == 3) { ResourceManager.Instance.cityStorageCapacity = 300000; }
                    if (level == 4) { ResourceManager.Instance.cityStorageCapacity = 2000000; }
                    if (level == 5) { ResourceManager.Instance.cityStorageCapacity = 8000000; }
                    if (level == 6) { ResourceManager.Instance.cityStorageCapacity = 12000000; }
                    if (level == 7) { ResourceManager.Instance.cityStorageCapacity = 15000000; }
                    if (level == 8) { ResourceManager.Instance.cityStorageCapacity = 20000000; }
                    if (level == 9) { ResourceManager.Instance.cityStorageCapacity = 30000000; }
                    if (level == 10) { ResourceManager.Instance.cityStorageCapacity = 40000000; }
                }
            }
        }

        BuildingData data = BuildingDatabase.GetBuildingByName(buildingName);
        if (data != null)
        {
            BuildLevelUI levelData = data.GetLevel(level);
            if (levelData != null)
            {
                health = levelData.health;

                // 🟢 تمت إضافة حسابات السور هنا لـ STOCK PILE
                if (buildingName == "STOCK PILE")
                {
                    if (sorlevl == 1) { health = health + health * 0.20f; }
                    else if (sorlevl == 2) { health = health + health * 0.30f; }
                    else if (sorlevl == 3) { health = health + health * 0.40f; }
                    else if (sorlevl == 4) { health = health + health * 0.50f; }
                }

                maxHealth = health;

                if (buildingName == "STOCK PILE")
                {
                    Debug.Log(levelData.CityStroge);
                }
            }
        }

        if (buildingName == "RECRUITMENT CHAMBER" || buildingName == "TRAINGING CHAMBER" || buildingName == "GATAPULT")
        {
            // الحصول على خصائص المبنى من قاعدة البيانات
            BuildingInfo data1 = BuildingInfoDatabase.GetBuildingByName(buildingName);
            if (data1 != null)
            {
                BuildingLevelInfo levelData = data1.GetLevel(level);
                if (levelData != null)
                {
                    health = levelData.health;
                    maxHealth = health;
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealth(health);
            healthBarUI.gameObject.SetActive(health < maxHealth && health > 0);
        }

        // الشرط الجديد: إذا كانت الصحة أقل من أو تساوي 30%
        if (lowHealthEffect != null)
        {
            if (health <= maxHealth * 0.30f && health > 0)
            {
                // نتحقق إذا كان التأثير غير مفعل لتشغيل الصوت لمرة واحدة فقط عند وصول الصحة لـ 30%
                if (!lowHealthEffect.activeSelf)
                {
                    AudioManager.Instance.PlaySFX("30HPbuild");
                    AudioManager.Instance.PlaySFX("30HPFire");

                    lowHealthEffect.SetActive(true); // إظهار الأوبجكت
                }
            }
            else if (health > maxHealth * 0.30f)
            {
                lowHealthEffect.SetActive(false); // إخفاء الأوبجكت إذا زادت الصحة
            }
        }

        if (health <= 0)
        {

            if (healthBarUI != null)
                Destroy(healthBarUI.gameObject); // حذف الشريط عند تدمير المبنى

            // نحفظ حدود المبنى قبل الحذف
            Bounds buildingBounds = GetComponent<Collider2D>().bounds;

            // 2. إظهار المبنى المهدم (الأنقاض) في نفس المكان والزاوية
            if (destroyedPrefab != null)
            {
                Instantiate(destroyedPrefab, transform.position, transform.rotation);
            }

            int randomNum = UnityEngine.Random.Range(0, 2);

            if (randomNum == 0)
            {
                AudioManager.Instance.PlaySFX("destroyBuild");
            }
            else
            {
                AudioManager.Instance.PlaySFX("boomberATK2");
            }

            // 🟢 التعديل الجديد: مسح البلاط الأخضر من خريطة الهجوم إذا كنا في مشهد الهجوم
            if (GirdSystem2.current != null)
            {
                GirdSystem2.current.ClearBuildingArea(area);
            }


            Destroy(gameObject);

            // تحديث المنطقة المحيطة بالمبنى فقط
            GraphUpdateObject guo = new GraphUpdateObject(buildingBounds);
            AstarPath.active.UpdateGraphs(guo);

        }
    }

    public bool CanBePlaced()
    {
        Vector3Int postionInt = GridBuildingSystem.current.gridlayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = postionInt;

        return GridBuildingSystem.current.CanTakeArea(areaTemp);
    }

    public void Place()
    {
        //occupiedArea = area; // احفظ المنطقة التي تمت السيطرة عليها بعد التثبيت

        Vector3Int postionInt = GridBuildingSystem.current.gridlayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = postionInt;
        Placed = true;
        GridBuildingSystem.current.TakeArea(areaTemp);
    }


    public void ShowUpgradeIcon()
    {
            upgradeIcon.SetActive(true);
    }

    public void HideUpgradeIcon()
    {
            upgradeIcon.SetActive(false);
    }

    public bool isUnderConstruction = true;
    //private float buildTimeRemaining = 10f; // الوقت الأصلي للبناء

    public void ForceFinishConstruction()
    {
        if (isUnderConstruction)
        {
            //buildTimeRemaining = 0f;
            isUnderConstruction = false;

            // إخفاء أي أيقونات أو مؤثرات البناء
            HideUpgradeIcon();

            // أي إجراءات إضافية عند الانتهاء فورًا
            OnConstructionComplete();
        }
    }

    private void OnConstructionComplete()
    {
        IsUnderConstruction = false; // 🟢 انتهى البناء

        // أي كود يجب أن ينفذ بعد انتهاء البناء
        Debug.Log("✅ البناء اكتمل فوراً!");
    }
    public void IncreaseSorLevel(int amount = 1)
    {
        sorlevl += amount;

        // نتأكد أنه ما يطلع عن الحدود (0 - 4)
        sorlevl = Mathf.Clamp(sorlevl, 0, 4);

        // تحديث الصور فورًا
        UpdateSorLevelVisual();

        // إذا كان الأب يحتوي على أبناء، نزامنهم مع الأب
        building[] children = GetComponentsInChildren<building>();
        foreach (building child in children)
        {
            if (child != this)
            {
                child.sorlevl = sorlevl;
                child.UpdateSorLevelVisual(); // تحديث الصور للأبناء مباشرة
            }
        }

        Debug.Log($"🔼 Sor Level الجديد للمبنى {buildingName}: {sorlevl}");
    }


    private void UpdateSorLevelVisual()
    {
        // نغلق كل المستويات إذا كانت موجودة
        if (sorlvl1 != null) sorlvl1.SetActive(false);
        if (sorlvl2 != null) sorlvl2.SetActive(false);
        if (sorlvl3 != null) sorlvl3.SetActive(false);
        if (sorlvl4 != null) sorlvl4.SetActive(false);

        // بعدين نشغل المستوى المناسب حسب القيمة إذا كان موجود
        switch (sorlevl)
        {
            case 1:
                if (sorlvl1 != null) sorlvl1.SetActive(true);
                break;
            case 2:
                if (sorlvl2 != null) sorlvl2.SetActive(true);
                break;
            case 3:
                if (sorlvl3 != null) sorlvl3.SetActive(true);
                break;
            case 4:
                if (sorlvl4 != null) sorlvl4.SetActive(true);
                break;
        }
    }

    // 🟢 دالة جديدة لاستقبال مستوى السور من ملف الحفظ وتحديث الصور والطاقة
    public void LoadSorLevel(int savedLevel)
    {
        sorlevl = savedLevel;
        UpdateSorLevelVisual();

        // هذا هو الجزء المفقود: إجبار الأبناء على تحديث وإظهار الأوبجكتات كما يحدث عند التطوير المباشر
        building[] children = GetComponentsInChildren<building>(true);
        foreach (building child in children)
        {
            if (child != this)
            {
                child.sorlevl = savedLevel;
                child.UpdateSorLevelVisual();
            }
        }

        // تحديث طاقة المبنى بناءً على مستوى السور الجديد
        InitializeBuildingStats();
        health = maxHealth;

        if (healthBarUI != null)
        {
            healthBarUI.Initialize(maxHealth);
            healthBarUI.UpdateHealth(health);
        }
    }


}
