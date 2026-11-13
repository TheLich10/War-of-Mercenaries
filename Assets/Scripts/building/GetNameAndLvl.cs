using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic; // تأكد من وجود هذه المكتبة لاستخدام الـ Dictionary

public class GetNameAndLvl : MonoBehaviour
{
    public Button closepanalMakeWorrior;
    public GameObject panal;
    public TMP_Text linkedText;
    public TMP_Text NameBuilding;

    public GameObject specialPanel;
    public Image specialImageComponent;

    public static string savedText;
    public static int savedNumber;
    public static string objectName;

    private bool isPanelOpen = false;

    // --- الإضافة الجديدة: قاموس الشروط ---
    // نستخدم static لكي يُحفظ في الذاكرة مرة واحدة لجميع الكائنات
    private static readonly Dictionary<string, string> specialBuildingsMap = new Dictionary<string, string>
    {
        // أضف هنا جميع الـ 20 مبنى الخاص بك
        // طريقة الكتابة: { "اسم المبنى_رقم المستوى", "مسار الصورة" }
        { "CENTER CITY_10", "buildingIcons/lv10" },
        { "LUMBER BLADE_13", "buildingIcons/woodlvl11" },
        { "STONE GRINDER_13", "buildingIcons/stonelvl12" },
        { "IRON SMELTER_13", "buildingIcons/ironlvl8" },
        { "STATUE OF MIGHT_13", "buildingIcons/powerlvl10" },
        { "ARCHER'S TOWER_11", "buildingIcons/archer_build" },
        { "BOMBARD TOWER_11", "buildingIcons/boomtower" },
        { "STOCK PILE_10", "buildingIcons/reslvl1" },
        { "WALL_5", "buildingIcons/wallvl5" },
        { "BEAST CANNON_6", "buildingIcons/towerBoom" },
        { "GALTING ARROW TOWER_6", "buildingIcons/archerOP" },
        { "FLAMER TOWER_6", "buildingIcons/flame" },
        { "SKY TOWER_6", "buildingIcons/flay" },
        { "HIRING QUARTERS_3", "buildingIcons/makerlvl3" },
        { "BURNING MIRRORS_6", "buildingIcons/mirorr" },
        { "WATCH POST_4", "buildingIcons/spy" },
        { "MERCENARY BARRACKS_6", "buildingIcons/barcklvl1" },
        { "RECRUITMENT CHAMBER_4", "buildingIcons/NewWor4" },
        { "TRAINGING CHAMBER_4", "buildingIcons/training4" },
        { "GATAPULT_3", "buildingIcons/mongel3" },

        // ... يمكنك إضافة البقية بنفس هذا النمط
    };

    void Start()
    {
        closepanalMakeWorrior.onClick.AddListener(TogglePanel);
    }

    void Update()
    {
        if (isPanelOpen && Input.GetMouseButtonDown(0))
        {
            GameObject clickedObject = GetClickedUIObject();

            if (clickedObject != null &&
                clickedObject != closepanalMakeWorrior.gameObject &&
                !IsChildOf(clickedObject, panal))
            {
                ClosePanel();
            }
        }
    }

    void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;
        panal.SetActive(isPanelOpen);
    }

    void ClosePanel()
    {
        isPanelOpen = false;
        panal.SetActive(false);
    }

    GameObject GetClickedUIObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0 ? results[0].gameObject : null;
    }

    bool IsChildOf(GameObject child, GameObject parent)
    {
        Transform t = child.transform;
        while (t != null)
        {
            if (t.gameObject == parent)
                return true;
            t = t.parent;
        }
        return false;
    }

    public void OnWarriorButtonClick()
    {
        if (linkedText != null)
        {
            savedText = linkedText.text;

            string digits = System.Text.RegularExpressions.Regex.Match(savedText, @"\d+").Value;
            if (int.TryParse(digits, out int level))
            {
                savedNumber = level;
            }
            else
            {
                savedNumber = 0;
            }
        }

        objectName = NameBuilding.text;
        //Debug.Log("name: " + objectName + " lvl: " + savedNumber); ديبك

        // --- مفتاح البحث ---
        // نقوم بدمج الاسم مع المستوى لإنشاء مفتاح للبحث (مثال: "CENTER CITY_10")
        string lookupKey = objectName + "_" + savedNumber;

        // --- التحقق عبر القاموس ---
        // هل القاموس يحتوي على هذا المفتاح؟
        if (specialBuildingsMap.ContainsKey(lookupKey))
        {
            panal.SetActive(false);
            isPanelOpen = true;

            if (specialPanel != null)
            {
                specialPanel.SetActive(true);
            }

            if (specialImageComponent != null)
            {
                // جلب مسار الصورة من القاموس بناءً على المفتاح
                string imagePath = specialBuildingsMap[lookupKey];
                Sprite loadedSprite = Resources.Load<Sprite>(imagePath);

                if (loadedSprite != null)
                {
                    specialImageComponent.sprite = loadedSprite;
                    specialImageComponent.preserveAspect = true;
                }
                else
                {
                    Debug.LogWarning("لم يتم العثور على الصورة في المسار: " + imagePath);
                }
            }
        }
        else
        {
            // في حال لم يتحقق الشرط، يتم تفعيل البنل العادي
            isPanelOpen = false;
            panal.SetActive(true);

            // تأكد من إخفاء البنل المميز إذا لم يتحقق الشرط
            if (specialPanel != null)
            {
                specialPanel.SetActive(false);
            }
        }

        // تحقق للمبنى الأول 
        BuildingUpgradeManager.instant.CheckUpgradeForActivePanel(objectName, savedNumber);
    }
}