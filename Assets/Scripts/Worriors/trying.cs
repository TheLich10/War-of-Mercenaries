using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class SimpleWarriorPlacement : MonoBehaviour
{
    public Button runBtnWom;
    public Button runBtnfly;
    public Button runBtneleftnt;
    public Button runBtnbear;

    [Header("Info Panel UI")]
    public GameObject infoPanel; // اسحب الـ Panel هنا من الـ Inspector
    public TMP_Text healthText;  // نص الصحة
    public TMP_Text powerText;   // نص القوة
    public float panelYOffset = 170f; // ⬅️ أضف هذا المتغير (يمكنك تغييره من الإنسبيكتور لضبط الارتفاع)
    // متغيرات الضغط المطول
    private float pointerDownTimer = 0f;
    private bool isHoldingButton = false;
    private UnitTypeData holdingUnit = null;


    public HorizontalLayoutGroup layoutGroup; // اسحب الـ Layout Group هنا من الـ Inspector
    [System.Serializable]
    public class UnitTypeData
    {
        public string unitName; // اسم الجندي
        public GameObject prefab;
        public Button button;
        public TMP_Text buttonText;
        public TMP_Text lvelTxt;

        public int startingAmount;
        public int level;

        [HideInInspector]
        public int remainingAmount;

        [HideInInspector]
        public bool isSelected;

        public GameObject selectionIndicator; // ← هذا هو الـ GameObject الخاص بالتحديد
        public UnitPool pool; // ✅ أضف هذا

    }


    public GridLayout gridlayout;
    public Tilemap mainTilemap;
    public TileBase whiteTile;
    public GameObject validCirclePrefab;
    public GameObject invalidCirclePrefab;

    public List<UnitTypeData> unitTypes = new List<UnitTypeData>();

    private Camera mainCamera;
    private GameObject currentCircle;
    private UnitTypeData selectedUnit = null;
    public static bool isFirstUnitPlaced = false;
    void Start()
    {
        isFirstUnitPlaced = false; // ⬅️ أضف هذا السطر هنا لتصفير القيمة عند بدء المشهد
        mainCamera = Camera.main;
        AudioManager.Instance.PlayMusic("Second");


        // تحديث الأعداد و الأحداث
        foreach (var unit in unitTypes)
        {
            // جلب العدد من Barracks
            if (unit.unitName == "bedoiun")
            {
                unit.startingAmount = Barracks.bedoiun;
                unit.level = WarriorDisplayUI.BedionLevel;
            }
            else if (unit.unitName == "Destroyer")
            {
                unit.startingAmount = Barracks.Destroyer;
                unit.level = WarriorDisplayUI.DestroyerLvl;
            }
            else if (unit.unitName == "flyingOttoman")
            {
                unit.startingAmount = Barracks.flyingOttoman;
                unit.level = WarriorDisplayUI.flyingOttomanLVL;
            }
            else if (unit.unitName == "Gaul")
            {
                unit.startingAmount = Barracks.Gaul;
                unit.level = WarriorDisplayUI.GaulLVL;
            }
            else if (unit.unitName == "Helaer")
            {
                unit.startingAmount = Barracks.Helaer;
                unit.level = WarriorDisplayUI.HelaerLVL;
            }
            else if (unit.unitName == "Kamikaze")
            {
                unit.startingAmount = Barracks.Kamikaze;
                unit.level = WarriorDisplayUI.KamikazeLVL;
            }
            else if (unit.unitName == "knight")
            {
                unit.startingAmount = Barracks.knight;
                unit.level = WarriorDisplayUI.knightLVL;
            }
            else if (unit.unitName == "Mongolian")
            {
                unit.startingAmount = Barracks.Mongolian;
                unit.level = WarriorDisplayUI.MongolianLVL;
            }
            else if (unit.unitName == "ottman") 
            { 
                unit.startingAmount = Barracks.ottman;
                unit.level = WarriorDisplayUI.ottmanLvl;
            }
            else if (unit.unitName == "persian")
            {
                unit.startingAmount = Barracks.persian;
                unit.level = WarriorDisplayUI.persianLVL;
            }
            else if (unit.unitName == "postion")
            {
                unit.startingAmount = Barracks.postion;
                unit.level = WarriorDisplayUI.postionLVL;
            }
            else if (unit.unitName == "sapper")
            {
                unit.startingAmount = Barracks.sapper;
                unit.level = WarriorDisplayUI.sapperLVL;
            }
            else if (unit.unitName == "Savge")
            {
                unit.startingAmount = Barracks.Savge;
                unit.level = WarriorDisplayUI.SavgeLVL;
            }
            else if (unit.unitName == "sneaky")
            {
                unit.startingAmount = Barracks.sneaky;
                unit.level = WarriorDisplayUI.sneakyLVL;
            }
            else if (unit.unitName == "ScalingEgyptian")
            {
                unit.startingAmount = Barracks.ScalingEgyptian;
                unit.level = WarriorDisplayUI.ScalingEgyptianLvl;
            }
            else if (unit.unitName == "BearWolf")
            {
                unit.startingAmount = 1;

                // جلب مستوى الوحش الحالي من MonsterLevelManager حسب الاسم
                int currentLevel = MonsterLevelManager.Instance.GetLevel(unit.unitName);

                // الحصول على بيانات الوحش من قاعدة البيانات
                MonsterData monsterData = MonsterDatabase.GetMonsterByName(unit.unitName);

                if (monsterData != null)
                {
                    MonsterLevel monsterLevel = monsterData.GetLevel(currentLevel);

                    if (monsterLevel != null)
                    {
                        unit.level = monsterLevel.level;  // تعيين المستوى من بيانات الوحش

                    }
                    else
                    {
                        // إذا لم تجد المستوى، عيّن المستوى الحالي كما هو
                        unit.level = currentLevel;
                    }
                }
                else
                {
                    // لو لم يتم العثور على الوحش في قاعدة البيانات، عيّن المستوى الحالي
                    unit.level = currentLevel;
                }
            }
            else if (unit.unitName == "Mightosour")
            {
                unit.startingAmount = 1;

                // جلب مستوى الوحش الحالي من MonsterLevelManager حسب الاسم
                int currentLevel = MonsterLevelManager.Instance.GetLevel(unit.unitName);

                // الحصول على بيانات الوحش من قاعدة البيانات
                MonsterData monsterData = MonsterDatabase.GetMonsterByName(unit.unitName);

                if (monsterData != null)
                {
                    MonsterLevel monsterLevel = monsterData.GetLevel(currentLevel);

                    if (monsterLevel != null)
                    {
                        unit.level = monsterLevel.level;  // تعيين المستوى من بيانات الوحش

                    }
                    else
                    {
                        // إذا لم تجد المستوى، عيّن المستوى الحالي كما هو
                        unit.level = currentLevel;
                    }
                }
                else
                {
                    // لو لم يتم العثور على الوحش في قاعدة البيانات، عيّن المستوى الحالي
                    unit.level = currentLevel;
                }
            }
            else if (unit.unitName == "Wom-kong")
            {
                unit.startingAmount = 1;

                // جلب مستوى الوحش الحالي من MonsterLevelManager حسب الاسم
                int currentLevel = MonsterLevelManager.Instance.GetLevel(unit.unitName);

                // الحصول على بيانات الوحش من قاعدة البيانات
                MonsterData monsterData = MonsterDatabase.GetMonsterByName(unit.unitName);

                if (monsterData != null)
                {
                    MonsterLevel monsterLevel = monsterData.GetLevel(currentLevel);

                    if (monsterLevel != null)
                    {
                        unit.level = monsterLevel.level;  // تعيين المستوى من بيانات الوحش

                    }
                    else
                    {
                        // إذا لم تجد المستوى، عيّن المستوى الحالي كما هو
                        unit.level = currentLevel;
                    }
                }
                else
                {
                    // لو لم يتم العثور على الوحش في قاعدة البيانات، عيّن المستوى الحالي
                    unit.level = currentLevel;
                }
            }
            else if (unit.unitName == "DraginFly")
            {
                unit.startingAmount = 1;

                // جلب مستوى الوحش الحالي من MonsterLevelManager حسب الاسم
                int currentLevel = MonsterLevelManager.Instance.GetLevel(unit.unitName);

                // الحصول على بيانات الوحش من قاعدة البيانات
                MonsterData monsterData = MonsterDatabase.GetMonsterByName(unit.unitName);

                if (monsterData != null)
                {
                    MonsterLevel monsterLevel = monsterData.GetLevel(currentLevel);

                    if (monsterLevel != null)
                    {
                        unit.level = monsterLevel.level;  // تعيين المستوى من بيانات الوحش

                    }
                    else
                    {
                        // إذا لم تجد المستوى، عيّن المستوى الحالي كما هو
                        unit.level = currentLevel;
                    }
                }
                else
                {
                    // لو لم يتم العثور على الوحش في قاعدة البيانات، عيّن المستوى الحالي
                    unit.level = currentLevel;
                }
            }



            unit.lvelTxt.text = unit.level.ToString();

            // أضف أنواعًا أخرى حسب الحاجة

            unit.remainingAmount = unit.startingAmount;
            unit.isSelected = false;

            // ✅ إخفاء الزر إذا لم يكن هناك عدد منذ البداية
            if (unit.remainingAmount == 0)
                unit.button.gameObject.SetActive(false);

            UpdateButtonText(unit);

            // إضافة EventTrigger للزر لاكتشاف الضغط المطول
            EventTrigger trigger = unit.button.gameObject.GetComponent<EventTrigger>() ?? unit.button.gameObject.AddComponent<EventTrigger>();

            // حدث عند بدء الضغط (PointerDown)
            EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDown.callback.AddListener((data) => {
                isHoldingButton = true;
                pointerDownTimer = 0f;
                holdingUnit = unit;
            });
            trigger.triggers.Add(pointerDown);

            // حدث عند رفع الإصبع (PointerUp أو PointerExit)
            EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            pointerUp.callback.AddListener((data) => {
                isHoldingButton = false;
                infoPanel.SetActive(false); // إخفاء اللوحة بمجرد رفع الإصبع
            });
            trigger.triggers.Add(pointerUp);

            // 👇👇👇 أضف هذا الكود لحل مشكلة السحب (Scroll) 👇👇👇

            // 1. حدث عند بدء السحب
            EventTrigger.Entry beginDrag = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
            beginDrag.callback.AddListener((data) => {
                isHoldingButton = false; // إلغاء الضغط المطول حتى لا تظهر اللوحة أثناء التمرير
                infoPanel.SetActive(false);
                // تمرير الحدث للـ ScrollRect الأب
                ExecuteEvents.ExecuteHierarchy(unit.button.transform.parent.gameObject, data, ExecuteEvents.beginDragHandler);
            });
            trigger.triggers.Add(beginDrag);

            // 2. حدث أثناء السحب
            EventTrigger.Entry drag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
            drag.callback.AddListener((data) => {
                ExecuteEvents.ExecuteHierarchy(unit.button.transform.parent.gameObject, data, ExecuteEvents.dragHandler);
            });
            trigger.triggers.Add(drag);

            // 3. حدث عند انتهاء السحب
            EventTrigger.Entry endDrag = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
            endDrag.callback.AddListener((data) => {
                ExecuteEvents.ExecuteHierarchy(unit.button.transform.parent.gameObject, data, ExecuteEvents.endDragHandler);
            });
            trigger.triggers.Add(endDrag);

            // 👆👆👆 نهاية كود التمرير 👆👆👆

            unit.button.onClick.AddListener(() =>
            {
                if (unit.isSelected)
                {
                    // إذا كان الزر نفسه محدد مسبقاً، إلغاء التحديد
                    unit.isSelected = false;
                    selectedUnit = null;

                    if (unit.selectionIndicator != null)
                        unit.selectionIndicator.SetActive(false);
                }
                else
                {
                    // إلغاء تحديد كل الآخرين أولاً
                    foreach (var other in unitTypes)
                    {
                        other.isSelected = false;
                        if (other.selectionIndicator != null)
                            other.selectionIndicator.SetActive(false);
                    }

                    // تحديد هذا الجندي
                    if (unit.remainingAmount > 0)
                    {
                        unit.isSelected = true;
                        selectedUnit = unit;

                        if (unit.selectionIndicator != null)
                            unit.selectionIndicator.SetActive(true);

                        //Debug.Log($"✔️ تم تحديد {unit.unitName}!"); ديبك
                    }
                    else
                    {
                        selectedUnit = null;
                    }
                }
            });


        }
        UpdatePaddingBasedOnActiveButtons();

    }

    private Vector3 mouseDownPos;
    private bool isDragging = false;
    public float clickThreshold = 1f; // المسافة المسموح بها لتعتبر نقرة

    void Update()
    {
        // 1. حساب وقت الضغط المطول (يجب أن يكون هنا قبل شرط الـ UI)
        if (isHoldingButton && holdingUnit != null)
        {
            pointerDownTimer += Time.deltaTime; // ⬅️ هذا السطر كان مفقوداً!

            if (pointerDownTimer >= 1f) // إذا استمر الضغط لثانية واحدة
            {
                infoPanel.SetActive(true);

                // ⬅️ السطر الجديد: اجعل موقع اللوحة هو نفس موقع الزر مع إضافة ارتفاع
                infoPanel.transform.position = holdingUnit.button.transform.position + new Vector3(0, panelYOffset, 0);


                float currentHealth = 0f;
                float currentDamage = 0f;

                string[] monsterNames = { "Wom-kong", "DraginFly", "BearWolf", "Mightosour" };
                bool isMonster = false;
                string matchedMonsterName = null;

                // التحقق مما إذا كان المحارب وحشاً
                foreach (string mName in monsterNames)
                {
                    if (holdingUnit.unitName.StartsWith(mName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        isMonster = true;
                        matchedMonsterName = mName;
                        break;
                    }
                }

                if (isMonster)
                {
                    MonsterData monster = MonsterDatabase.GetMonsterByName(matchedMonsterName);
                    if (monster != null)
                    {
                        MonsterLevel monsterLevel = monster.GetLevel(holdingUnit.level);
                        if (monsterLevel != null)
                        {
                            currentHealth = monsterLevel.health;
                            currentDamage = monsterLevel.damage;
                        }
                    }
                }
                else
                {
                    WarriorTring warrior = WarriorTringDatabase.GetWarriorByName(holdingUnit.unitName);
                    if (warrior != null)
                    {
                        WarriorLevel warriorLevel = warrior.GetLevel(holdingUnit.level);
                        if (warriorLevel != null)
                        {
                            currentHealth = warriorLevel.health;
                            currentDamage = warriorLevel.damage;
                        }
                    }
                }

                // عرض البيانات في اللوحة
                healthText.text = currentHealth.ToString();
                powerText.text = currentDamage.ToString();

                isHoldingButton = false; // نوقفه حتى لا يتكرر التحديث كل فريم أثناء الإمساك
            }
        }

        // 2. الآن نضع شرط الـ UI لمنع وضع الجندي عند النقر على الأزرار
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // ... (باقي كود Input.GetMouseButtonDown وما بعده) ...

        if (selectedUnit == null || !selectedUnit.isSelected || selectedUnit.remainingAmount <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseDownPos.z = 0f;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            currentPos.z = 0f;

            // تحقق من حركة الشاشة من سكربت الكاميرا
            if (CameraController.current != null && CameraController.current.IsDragging)
            {
                isDragging = true;
            }

            // أو لو تحركت الفأرة كثيرًا
            if (Vector3.Distance(mouseDownPos, currentPos) > 0.001f)
            {
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // فقط إذا لم يتم السحب إطلاق الجندي
            if (!isDragging)
            {
                PlaceUnit();
            }
        }
    }

    void PlaceUnit()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3Int cellPos = mainTilemap.WorldToCell(mousePos);
        TileBase clickedTile = mainTilemap.GetTile(cellPos);

        if (currentCircle != null)
            Destroy(currentCircle);

        if (clickedTile == whiteTile || clickedTile == null)
        {
            // 👇👇👇 أضف هذا الشرط هنا لتشغيل موسيقى الحرب عند وضع أول جندي فقط
            if (!isFirstUnitPlaced)
            {
                AudioManager.Instance.PlayMusic("war");
                AudioManager.Instance.PlaySFX("firsOne");

                isFirstUnitPlaced = true; // نغير القيمة حتى لا تعمل الموسيقى من جديد مع كل جندي
            }
            // 👆👆👆

            selectedUnit.pool.Spawn(mousePos);
            currentCircle = Instantiate(validCirclePrefab, mousePos, Quaternion.Euler(45, 0, 0));
            Destroy(currentCircle, 0.5f);

            selectedUnit.remainingAmount--;
            UpdateButtonText(selectedUnit);

            switch (selectedUnit.unitName)
            {
                case "Sneaky":
                case "GentleHealerFull":
                    // لا تفعل شيئاً (بدون صوت)
                    break;

                case "Mightosour":
                    AudioManager.Instance.PlaySFX("monster_SPN"); // استبدل بالاسم الصحيح للصوت
                    break;

                case "Wom-kong":
                    AudioManager.Instance.PlaySFX("monster_SPN"); // استبدل بالاسم الصحيح للصوت
                    break;

                case "DraginFly":
                    AudioManager.Instance.PlaySFX("y3sb_SPN2"); // استبدل بالاسم الصحيح للصوت
                    break;

                case "BearWolf":
                    AudioManager.Instance.PlaySFX("monster_SPN"); // استبدل بالاسم الصحيح للصوت
                    break;

                case "KnightOnHorse":
                    AudioManager.Instance.PlaySFX("knight_SPN"); // استبدل بالاسم الصحيح للصوت
                    break;

                default:
                    // الصوت المشترك لباقي الجنود (اختيار عشوائي بين صوتين)
                    string[] defaultSounds = { "bed_SPN", "bed_SPN2" }; // ضع اسم الصوت الثاني بدل هذه الكلمة

                    // اختيار رقم عشوائي بين 0 و طول المصفوفة (الذي هو 2)
                    int randomIndex = UnityEngine.Random.Range(0, defaultSounds.Length);

                    AudioManager.Instance.PlaySFX(defaultSounds[randomIndex]);
                    break;
            }
            // 👆👆👆 نهاية كود الأصوات 👆👆👆

            // ✅ هنا نطبع اسم الوحش فقط إذا كان من الوحوش الخاصة
            if (selectedUnit.unitName == "Mightosour")
            { 
                runBtneleftnt.gameObject.SetActive(true);
            }
            else if (selectedUnit.unitName == "Wom-kong")
            {
                runBtnWom.gameObject.SetActive(true);
            }
            else if (selectedUnit.unitName == "DraginFly")
            {
                runBtnfly.gameObject.SetActive(true);
            }
            else if (selectedUnit.unitName == "BearWolf")
            {
                runBtnbear.gameObject.SetActive(true);
            }
            else
            {
                //Debug.Log($"the Worioris is : {selectedUnit.unitName}!");
                Barracks.instnce.RemoveWarrior(selectedUnit.unitName);

            }

        }
        else
        {
            //Debug.Log("⚠️ لا يمكنك وضع الجندي هنا!"); ديبك
            PopupText.instan.ShowPopup("Cannot place here.");
            currentCircle = Instantiate(invalidCirclePrefab, mousePos, Quaternion.Euler(45, 0, 0));
            Destroy(currentCircle, 0.5f);
        }
    }





    void UpdateButtonText(UnitTypeData unit)
    {
        unit.buttonText.text = unit.remainingAmount.ToString();
        unit.lvelTxt.text = unit.level.ToString();
    }
    void UpdatePaddingBasedOnActiveButtons()
    {
        int activeButtons = 0;

        foreach (var unit in unitTypes)
        {
            if (unit.button.gameObject.activeSelf)
                activeButtons++;
        }

        int padding = 0;

        if (HeroArcher.isActiveHero == true || HeroArcher2.isActiveHero == true || HeroUnit.isActiveHero == true || HeroUnit2.isActiveHero == true )
        {
            switch (activeButtons)
            {
                case 0:
                    padding = -870;
                    break;
                case 1:
                    padding = -850;
                    break;
                case 2:
                    padding = -500;
                    break;
                case 3:
                    padding = -60;
                    break;
                case 4:
                    padding = 180;
                    break;
                case 5:
                    padding = 180;
                    break;
                default:
                    padding = 180; // القيمة الافتراضية لو ما في ولا زر
                    break;
            }
            layoutGroup.padding.left = padding;
            layoutGroup.SetLayoutHorizontal();
        }
        else { 
        switch (activeButtons)
        {
            case 1:
                padding = -850;
                break;
            case 2:
                padding = -850;
                break;
            case 3:
                padding = -500;
                break;
            case 4:
                padding = -40;
                break;
            case 5:
                padding = 180;
                break;
            case 6:
                padding = 180;
                break;
            case 7:
                padding = 180;
                break;
            case 8:
                padding = 180;
                break;
            case 9:
                padding = 180;
                break;
            case 10:
                padding = 180;
                break;
            default:
                padding = 180; // القيمة الافتراضية لو ما في ولا زر
                break;
        }

        layoutGroup.padding.left = padding;
        layoutGroup.SetLayoutHorizontal();
        }
    }


}
