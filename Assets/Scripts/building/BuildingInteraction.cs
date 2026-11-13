using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class BuildingInteraction : MonoBehaviour
{
    int level = 1;

    private RectTransform panelRectTransform;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    public GameObject actionPanel;
    public List<GameObject> subPanels; // القائمة التي تحتوي على البنلات الفرعية


    private GameObject selectedBuilding;

    public GameObject objectToSpawn; // الصورة التي ستظهر للمباني العادية
    public GameObject specialObjectToSpawn; // ✅ الصورة الجديدة للمباني المحددة
    public GameObject WALLObjectToSpawn; // ✅ الصورة الجديدة للمباني المحددة

    private GameObject spawnedImage; // المرجع للصورة الحالية

    public TMP_Text caveLevelText; // ✅ نص لعرض مستوى CaveBrack
    public TMP_Text levelText_Archer2;
    public TMP_Text levelText_Archer;
    public TMP_Text levelText_Unit;
    public TMP_Text levelText_Unit2;

    void Start()
    {
        actionPanel.SetActive(false);
        panelRectTransform = actionPanel.GetComponent<RectTransform>();
        HideAllSubPanels(); // ✅ تمت إضافته هنا
        AudioManager.Instance.PlayMusic("main");

    }

    private Transform trackedBuildingTransform; // ✅ نتابع هذا المبنى

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsClickOnUIInsidePanel())
                return;

            // ✅ التعديل هنا: استخدام الدالة الجديدة التي تدعم الهاتف والكمبيوتر
            if (IsPointerOverUI())
            {
                ClosePanel();
                DestroySpawnedImage();
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                ClosePanel();
                DestroySpawnedImage();
                return;
            }

            DetectBuilding();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
            DestroySpawnedImage();
        }

        // ✅ تابع تحديث موقع الصورة إذا كانت تتبع مبنى
        if (spawnedImage != null && trackedBuildingTransform != null)
        {
            spawnedImage.transform.position = trackedBuildingTransform.position;
        }
    }

    bool IsClickOnUIInsidePanel()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == null)
                continue;

            if (result.gameObject.transform.IsChildOf(actionPanel.transform))
                return true;
        }

        return false;
    }

    void DetectBuilding()
    {
        // ✅ التعديل هنا أيضاً
        if (IsPointerOverUI())
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        string[] validTags = { "trapfire", "trapN", "boomOP", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "archer", "archerOP", "flameT", "flayTower", "Mirror", "spy", "spyClan", "boomTower", "Barracks", "BrackClan", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "CaveBrack", "mongel", "wall" };

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // 🔹 إذا لم يصطدم بأي مبنى، دمر الصورة مباشرة
        if (hit.collider == null)
        {
            ClosePanel();
            DestroySpawnedImage();
            return;
        }

        if (hit.collider.gameObject.name == "upgrade (1)")
        {
            caveLevelText.text = ".";
        }

        if (hit.collider != null && validTags.Contains(hit.collider.tag))
        {
            AudioManager.Instance.PlaySFX("click");
            if (selectedBuilding != null) SetTransparency(selectedBuilding, 1f);

            selectedBuilding = hit.collider.gameObject;
            SetTransparency(selectedBuilding, 0.8f);

            Vector3 spawnPosition = hit.collider.bounds.center;

            DestroySpawnedImage();

            // 👇 مصفوفة التاغات التي تستدعي الأوبجكت الجديد
            string[] specialTags = { "CenterCity", "Barracks", "BrackClan", "Cave", "CaveBrack" };
            string[] WALLlTags = { "wall", "trapN", "trapfire"};


            // 👇 شرط فحص التاغ لإنشاء الأوبجكت المناسب
            if (specialTags.Contains(hit.collider.tag))
            {
                spawnedImage = Instantiate(specialObjectToSpawn, spawnPosition, Quaternion.identity);
            }
            else if (WALLlTags.Contains(hit.collider.tag))
            {
                spawnedImage = Instantiate(WALLObjectToSpawn, spawnPosition, Quaternion.identity);
            }
            else
            {
                spawnedImage = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
            }

            trackedBuildingTransform = selectedBuilding.transform; // ✅ تتبع المبنى

            ShowPanel(selectedBuilding.transform.position, hit.collider.tag); // ✅ أضفنا التاغ كمتغير ثاني

            if (hit.collider.tag == "wood")
            {
                buildWoodStroge woodStorage = hit.collider.GetComponent<buildWoodStroge>();
                if (woodStorage != null)
                {
                    UiAllBars.Instance.SetWoodStorage(woodStorage);
                }
            }
            if (hit.collider.CompareTag("power"))
            {
                buildPowerStroge powerStorage = hit.collider.GetComponent<buildPowerStroge>();
                if (powerStorage != null)
                {
                    UiAllBars.Instance.SetPowerStorage(powerStorage);
                }
            }
            if (hit.collider.CompareTag("iron"))
            {
                buildIronStorge ironStorage = hit.collider.GetComponent<buildIronStorge>();
                if (ironStorage != null)
                {
                    UiAllBars.Instance.SetIronStorage(ironStorage);
                }
            }
            if (hit.collider.CompareTag("stone"))
            {
                buildStoneStroge stoneStorage = hit.collider.GetComponent<buildStoneStroge>();
                if (stoneStorage != null)
                {
                    UiAllBars.Instance.SetStoneStorage(stoneStorage);
                }
            }
            if (hit.collider.CompareTag("CenterCity"))
            {
                BuildingGoldStorage GoldStorage = hit.collider.GetComponent<BuildingGoldStorage>();
                if (GoldStorage != null)
                {
                    UiAllBars.Instance.SetGoldStorage(GoldStorage);
                }
            }
            if (hit.collider.CompareTag("spy"))
            {
                BarracksWatchPost watchPost = hit.collider.GetComponent<BarracksWatchPost>();

                if (watchPost != null)
                {
                    UiAllBars.Instance.SetWatchPost(watchPost);
                    WatchPostUIManager.Instance.SetWatchPost(watchPost);
                }
            }

            if (hit.collider.tag == "CaveBrack")
            {
                // ثم: فحص البطل النشط وتفعيل النص المناسب فقط + قراءة المستوى
                if (HeroArcher2.isActiveHero)
                {
                    levelText_Archer2.gameObject.SetActive(true);
                    int.TryParse(levelText_Archer2.text, out level);

                    levelText_Archer.gameObject.SetActive(false);
                    levelText_Unit.gameObject.SetActive(false);
                    levelText_Unit2.gameObject.SetActive(false);
                }
                else if (HeroArcher.isActiveHero)
                {
                    levelText_Archer.gameObject.SetActive(true);
                    int.TryParse(levelText_Archer.text, out level);

                    levelText_Archer2.gameObject.SetActive(false);
                    levelText_Unit.gameObject.SetActive(false);
                    levelText_Unit2.gameObject.SetActive(false);
                }
                else if (HeroUnit.isActiveHero)
                {
                    levelText_Unit.gameObject.SetActive(true);
                    int.TryParse(levelText_Unit.text, out level);

                    levelText_Archer2.gameObject.SetActive(false);
                    levelText_Archer.gameObject.SetActive(false);
                    levelText_Unit2.gameObject.SetActive(false);
                }
                else if (HeroUnit2.isActiveHero)
                {
                    levelText_Unit2.gameObject.SetActive(true);
                    int.TryParse(levelText_Unit2.text, out level);

                    levelText_Archer2.gameObject.SetActive(false);
                    levelText_Archer.gameObject.SetActive(false);
                    levelText_Unit.gameObject.SetActive(false);
                }
                caveLevelText.text = level.ToString();

                // نحصل على الوحش المرتبط بالمبنى (مثال)
                MonsterHealth selectedMonsterHealth = hit.collider.GetComponent<MonsterHealth>();

                if (selectedMonsterHealth != null)
                {
                    UiMonsterHealthBar.Instance.SetCurrentMonster(selectedMonsterHealth);
                }
                else
                {
                    Debug.LogWarning("No MonsterHealth component found on this CaveBrack!");
                }
            }
            else
            {
                building targetBuilding = hit.collider.GetComponent<building>();
                if (targetBuilding != null)
                {
                    int trageLevel = targetBuilding.level; // أخذ مستوى المبنى
                    //Debug.Log("target building level: " + trageLevel); ديبك
                    caveLevelText.text = trageLevel.ToString();
                }
                else
                {
                    Debug.LogWarning("The object with tag 'wood' does not have a 'building' component!");
                }
            }
        }
        else
        {
            ClosePanel();
            DestroySpawnedImage(); // دمر الصورة إذا لم يتم النقر على مبنى
        }
    }

    void ShowPanel(Vector3 position, string tag) // ✅ أضفنا string tag
    {
        actionPanel.SetActive(true);

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        actionPanel.transform.position = new Vector3(screenWidth / 2, screenHeight * 0.02f, 0);

        ShowSubPanel(tag); // ✅ استدعاء دالة إظهار البانل الفرعي
    }

    void ClosePanel()
    {
        actionPanel.SetActive(false);
        HideAllSubPanels(); // ✅ تمت إضافته ليخفي البنلات الفرعية عند إغلاق الأب

        if (selectedBuilding != null)
        {
            SetTransparency(selectedBuilding, 1f);
        }

        selectedBuilding = null;
    }

    void ShowSubPanel(string tag)
    {
        HideAllSubPanels(); // إخفاء الجميع أولاً

        // البحث عن البانل المطابق للتاغ وتفعيله
        foreach (GameObject panel in subPanels)
        {
            if (panel != null && panel.name == tag)
            {
                panel.SetActive(true);
                break;
            }
        }
    }

    void HideAllSubPanels()
    {
        if (subPanels == null) return; // حماية إضافية لتفادي الأخطاء

        foreach (GameObject panel in subPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    void DestroySpawnedImage()
    {
        if (spawnedImage != null)
        {
            Destroy(spawnedImage);
            spawnedImage = null;
            trackedBuildingTransform = null; // ✅ أوقف التتبع
        }
    }
    void SetTransparency(GameObject target, float alpha)
    {
        foreach (var renderer in target.GetComponentsInChildren<SpriteRenderer>())
        {
            // 👇 إضافة هذا الشرط لتخطي العنصر الذي يحمل اسم الظل
            if (renderer.gameObject.name == "shod" || renderer.gameObject.name == "smoke" || renderer.gameObject.name == "eagle" || renderer.gameObject.name == "bear" || renderer.gameObject.name == "Mighty Elephant" || renderer.gameObject.name == "womKong")
            {
                continue; // الانتقال للعنصر التالي دون تغيير شفافية الظل
            }

            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }

    bool IsPointerOverUI()
    {
        // التحقق إذا كان اللاعب يستخدم شاشة لمس (هاتف)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        // التحقق إذا كان اللاعب يستخدم الماوس (كمبيوتر)
        return EventSystem.current.IsPointerOverGameObject();
    }
}