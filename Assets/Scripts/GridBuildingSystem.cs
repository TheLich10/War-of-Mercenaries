using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GridBuildingSystem : MonoBehaviour
{
    public GameObject actionPanel;////
    private Camera cam;////
    private bool isDragging = false;////
    public bool IsDragging => isDragging;////



    public static GridBuildingSystem current;

    public GridLayout gridlayout;
    public Tilemap MainTilemap;
    public Tilemap TempTilemap;

    private static Dictionary<TileType, TileBase> tileBases = new Dictionary<TileType, TileBase>();

    private building temp;

    private GameObject currentBuildingPrefab; // 🟡 أضف هذا المتغير لحفظ نسخة المبنى الحالي

    private Vector3 prevPos;
    private BoundsInt prevArea;

    public GameObject buildMenuPanel;

    private GameObject selectedExistingBuilding = null;
    private float holdTimer = 0f;
    private float holdThreshold = 1f; // ثانية واحدة

    private Vector3 dragOffset; // ✅ الفرق بين الماوس والمبنى عند بداية السحب

    private Vector3 originalPositionBeforeDrag; // لتخزين موقع المبنى قبل السحب
    private bool isMovingExistingBuilding = false; // هل المستخدم يحرك مبنى موجود؟

    private Vector3Int originalCellPosition; // 🟡 هذا الجديد

    public GameObject upgradeBarPrefab; // 🟡 اربط البار من الـ Inspector

    public WorkerUI workerUI; // اربطه من الـInspector

    public Button confirmButton; // 🔘 اربطه من الـ Inspector بزر وضع البناء


    public PopupManager popupManager; // اربطها من الإنسبكتور

    #region Unity Methods
    public GameObject interactionBlocker;

    private Dictionary<GameObject, UpgradeBarFollow> buildingBars = new Dictionary<GameObject, UpgradeBarFollow>();
    private Dictionary<GameObject, Coroutine> buildingPopups = new Dictionary<GameObject, Coroutine>();


    [Header("Speed Up Icon")]
    public GameObject speedUpIconPrefab; // اربطه بالأيقونة في الـ Inspector
    private Dictionary<GameObject, GameObject> activeSpeedUpIcons = new Dictionary<GameObject, GameObject>();

    private Dictionary<GameObject, Coroutine> returnWorkerCoroutines = new Dictionary<GameObject, Coroutine>();

    /////////////////////////// SAVING /////////////////////////
    [System.Serializable]
    public struct BuildingPrefabData
    {
        public string buildingTag; // نكتب هنا مثلا "wood"
        public GameObject prefab;  // نسحب هنا الـ Prefab الخاص بمصنع الخشب
    }

    [Header("Save System Data")]
    // قائمة لربط التاغ بالبريفاب
    public List<BuildingPrefabData> buildingDatabase = new List<BuildingPrefabData>();

    // قائمة لتتبع كل المباني الموجودة في الخريطة حالياً
    public List<GameObject> activeBuildingsInScene = new List<GameObject>();
    /////////////////////////// SAVING /////////////////////////


    [Header("First Time Spawn System")]
    // قائمة لتحديد المباني التي ستظهر في أول مرة ومكانها
    public List<InitialBuildingData> firstTimeBuildings = new List<InitialBuildingData>();

    [System.Serializable]
    public struct InitialBuildingData
    {
        public GameObject prefab; // البريفاب الخاص بالمبنى (مثلاً المركز الرئيسي)
        public Vector3Int spawnPosition; // موقع الـ Cell (مثال: x=0, y=0, z=0)
    }


    private void EnableBlockingUI(bool enable)
    {
        interactionBlocker.SetActive(enable);
    }


    private void Awake()
    {

        current = this;
        cam = Camera.main;

        //Debug.Log("✅ GridBuildingSystem working");
    }

    private void Start()
    {
        string tilepath = @"Tiles\";
        if (!tileBases.ContainsKey(TileType.Empty))
        {
            tileBases.Add(TileType.Empty, null);
        }
        if (!tileBases.ContainsKey(TileType.White))
        {
            tileBases.Add(TileType.White, Resources.Load<TileBase>(tilepath + "White"));
        }
        if (!tileBases.ContainsKey(TileType.Green))
        {
            tileBases.Add(TileType.Green, Resources.Load<TileBase>(tilepath + "Green"));
        }
        if (!tileBases.ContainsKey(TileType.Red))
        {
            tileBases.Add(TileType.Red, Resources.Load<TileBase>(tilepath + "Red"));
        }
        SpawnInitialBuildingsFirstTime();
    }
    string[] validTags = { "trapfire", "trapN", "boomOP", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "archer", "archerOP", "flameT", "flayTower", "Mirror", "spy", "spyClan", "boomTower", "Barracks", "brackClan", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "CaveBrack", "mongel", "wall" };

    // 🟢 دالة توليد المباني لأول مرة فقط
    private void SpawnInitialBuildingsFirstTime()
    {
        // نتحقق إذا كانت هذه هي أول مرة يفتح فيها اللعبة (القيمة الافتراضية 1 تعني نعم)
        if (PlayerPrefs.GetInt("HasSpawnedInitialBuildings", 1) == 1)
        {

            foreach (var initData in firstTimeBuildings)
            {
                if (initData.prefab == null) continue;

                // تحديد المكان الدقيق بناءً على إحداثيات الخلايا (Grid)
                Vector3 worldPos = gridlayout.CellToLocalInterpolated(initData.spawnPosition + new Vector3(0.5f, 0.5f, 0f));

                // إنشاء المبنى في المشهد
                GameObject newBuilding = Instantiate(initData.prefab, worldPos, Quaternion.identity);
                building bScript = newBuilding.GetComponent<building>();

                if (bScript != null)
                {
                    // حجز مكان المبنى على الأرضية (لون أخضر بالخلفية)
                    bScript.area.position = initData.spawnPosition;
                    TakeArea(bScript.area);

                    // جعل المبنى مكتمل البناء فوراً 
                    bScript.IsUnderConstruction = false;
                    bScript.HideUpgradeIcon();

                    // تسجيل المبنى في قائمة الحفظ لكي يتم حفظه كأنه مبني من قبل اللاعب
                    activeBuildingsInScene.Add(newBuilding);
                }
            }

            // نحفظ أننا قمنا بتوليد المباني لكي لا تتكرر العملية في المرات القادمة
            PlayerPrefs.SetInt("HasSpawnedInitialBuildings", 0);
            PlayerPrefs.Save();
        }
    }

    // 🟢 الدالة الجديدة للتعرف على المباني الموضوعة يدوياً (محدثة للإصدارات الحديثة)
    // 🟢 الدالة الجديدة للتعرف على المباني الموضوعة يدوياً

    private void Update()
    {
        foreach (var kvp in activeSpeedUpIcons)
        {
            if (kvp.Value != null)
            {
                float floatY = Mathf.Sin(Time.time * 2f) * 0.2f;
                kvp.Value.transform.position = kvp.Key.transform.position + new Vector3(0, 2f + floatY, 0);
            }
        }


        HandleLongPressToMoveExistingBuilding();

        DetectClickOnUpgradeChild(); // ← أضف هذا السطر داخل Update


        if (!temp) return; // إذا لم يكن هناك مبنى محدد، لا تفعل شيئًا
        UpdateActionPanelPosition(); // ✅ تحديث موضع البنل بشكل دائم

        // 🚀 تحريك أيقونة SpeedUp إذا موجودة



        // عند الضغط على الكائن
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUIButNotOnTemp())
                return;


            Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

            if (hit != null && System.Array.Exists(validTags, tag => hit.CompareTag(tag)))
            {
                isDragging = true;
            }
        }


        // عندما تسحب الكائن
        if (isDragging)
        {
            Vector2 touchPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 adjustedPos = touchPos + (Vector2)dragOffset; // ✅ نعوّض الفرق
            Vector3Int cellPos = gridlayout.LocalToCell(adjustedPos);


            if (prevPos != cellPos)
            {
                temp.transform.localPosition = gridlayout.CellToLocalInterpolated(cellPos + new Vector3(0.5f, 0.5f, 0f));
                prevPos = cellPos;
                FollowBuilding();
            }
        }
        // عند إفلات الضغط
        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                isDragging = false;

            }
        }

    }
    private void HandleLongPressToMoveExistingBuilding()
    {
        Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos); // ⬅️ هنا نعرفه

        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUIButNotOnTemp())
                return;

            if (hit != null && System.Array.Exists(validTags, tag => hit.CompareTag(tag)))
            {
                selectedExistingBuilding = hit.transform.root.gameObject; // مهم استخدام root
                holdTimer = 0f;
            }
        }

        if (Input.GetMouseButton(0) && selectedExistingBuilding != null)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdThreshold)
            {

                var b = selectedExistingBuilding.GetComponent<building>();
                originalCellPosition = gridlayout.WorldToCell(b.transform.position); // 🟡 استخدمه بدل originalPositionBeforeDrag

                if (b != null)
                {
                    Vector3Int cellPos = gridlayout.WorldToCell(selectedExistingBuilding.transform.position);
                    BoundsInt area = new BoundsInt(cellPos, b.area.size);

                    TileBase[] currentTiles = GetTileBlock(area, MainTilemap);
                    TileBase[] newTiles = new TileBase[currentTiles.Length];

                    for (int i = 0; i < currentTiles.Length; i++)
                    {
                        // فقط إذا كانت البلاطة خضراء (أي محجوزة)، نعيدها إلى الأبيض
                        if (currentTiles[i] == tileBases[TileType.Green])
                        {
                            newTiles[i] = tileBases[TileType.White];
                        }
                        else
                        {
                            newTiles[i] = currentTiles[i]; // لا نغير البلاطة
                        }
                    }
                    MainTilemap.SetTilesBlock(area, newTiles);



                    // ✅ تعديل dragOffset بناءً على ما إذا كان الضغط على ابن
                    if (hit != null && hit.transform != selectedExistingBuilding.transform)
                    {
                        dragOffset = Vector3.zero;
                        b.transform.position = gridlayout.CellToWorld(gridlayout.WorldToCell(mouseWorldPos));
                    }
                    else
                    {
                        dragOffset = b.transform.position - (Vector3)mouseWorldPos;
                    }

                    temp = b;
                    originalPositionBeforeDrag = b.transform.position; // 🟡 احفظ الموقع الأصلي
                    isMovingExistingBuilding = true; // 🟡 علامة أن المبنى كان موجودًا
                    actionPanel.SetActive(true);
                    EnableBlockingUI(true);

                    isDragging = true;
                    selectedExistingBuilding = null;

                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectedExistingBuilding = null;
            holdTimer = 0f;
        }
    }




    public static TileBase[] GetTileBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, z: 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }

    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArry = new TileBase[size];
        FillTiles(tileArry, type);
        tilemap.SetTilesBlock(area, tileArry);

    }

    private static void FillTiles(TileBase[] arr, TileType type)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }
    #endregion


    #region Building placement

    private static bool isNewBuild = false;
    public void InitializeWithBuilding(GameObject building)
    {
        buildMenuPanel.SetActive(false);
        if (building == null)
        {
            Debug.LogError("!NO BUILDING");
            return;
        }

        currentBuildingPrefab = building; // 🟡 أضف هذا السطر هنا

        //Debug.Log("BULIDNG CREATING: " + building.name);  ديبك

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenCenter);
        worldPos.z = 0;

        temp = Instantiate(building, worldPos, Quaternion.identity).GetComponent<building>();

        actionPanel.SetActive(true);
        EnableBlockingUI(true);
        isNewBuild = true;
        FollowBuilding();
    }



    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(toClear, TileType.Empty);
        TempTilemap.SetTilesBlock(prevArea, toClear);

    }

    private void FollowBuilding()
    {
        if (temp == null)
        {
            Debug.LogError("⚠️ NO building ");
            return;
        }

        ClearArea();
        temp.area.position = gridlayout.WorldToCell(temp.transform.position);
        temp.transform.position = gridlayout.CellToLocalInterpolated(temp.area.position + new Vector3(0.5f, 0.5f, 0));

        // تحويل موقع المبنى من العالم إلى الشاشة
        Vector3 screenPos = cam.WorldToScreenPoint(temp.transform.position);
        float yOffset = 250f; // إزاحة رأسية
        screenPos.y += yOffset;
        actionPanel.transform.position = screenPos;

        BoundsInt buildingArea = temp.area;

        TileBase[] baseArray = GetTileBlock(buildingArea, MainTilemap);
        TileBase[] tileArray = new TileBase[baseArray.Length];

        bool isValid = true;
        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileBases[TileType.White])
            {
                tileArray[i] = tileBases[TileType.Green];
            }
            else
            {
                FillTiles(tileArray, TileType.Red);
                isValid = false;
                break;
            }
        }

        TempTilemap.SetTilesBlock(buildingArea, tileArray);

        // ✅ التحقق من صلاحية الموقع لتفعيل/تعطيل زر التأكيد
        bool canPlace = isValid && temp.CanBePlaced() && !IsOverlappingOtherBuilding(temp.area);

        if (confirmButton != null)
        {
            confirmButton.interactable = canPlace;
        }

        // 🎨 تغيير لون المبنى وأبنائه إذا كان المكان غير صالح
        Color targetColor = canPlace ? Color.white : Color.red;
        foreach (var renderer in temp.GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = targetColor;
        }

        prevArea = buildingArea;
    }


    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTileBlock(area, MainTilemap);
        foreach (var b in baseArray)
        {
            if (b != tileBases[TileType.White])
            {
                Debug.Log("cant place here");
                return false;
            }
        }
        return true;
    }

    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.Empty, TempTilemap);
        SetTilesBlock(area, TileType.Green, MainTilemap);
    }
    #endregion


    // عداد لكل مبنى محدد
    private int hiringQuartersCount = 0;
    public void TryPlaceBuilding(float timeIwant)
    {
        if (temp != null)
        {
            if (temp.CanBePlaced())
            {
                if (IsOverlappingOtherBuilding(temp.area))
                {
                    Debug.LogWarning("❌ لا يمكن وضع المبنى فوق مبنى آخر.");
                    return;
                }

                // ✅ تحقق هل هذا بناء جديد فقط
                if (isNewBuild)
                {
                    // تحقق من توفر العمال
                    if (workerUI != null && workerUI.TryUseWorker())
                    {
                        // بناء جديد => خصم عامل وبدء البناء
                        temp.Place();
                        /////////////////////////// SAVING /////////////////////////

                        activeBuildingsInScene.Add(temp.gameObject);

                        /////////////////////////// SAVING /////////////////////////

                        // عندما يبدأ البناء
                        temp.IsUnderConstruction = true; // 🟡 المبنى دخل في مرحلة البناء
                        // 🟢 السطرين الجدد: حساب متى سينتهي البناء في المستقبل وحفظ المدة
                        temp.buildTotalDuration = timeIwant;
                        temp.buildEndTimeTicks = System.DateTime.Now.AddSeconds(timeIwant).Ticks;

                        GameObject canvasObject = GameObject.FindWithTag("MainCanvas");
                        if (canvasObject != null && upgradeBarPrefab != null && temp != null)
                        {
                            GameObject bar = Instantiate(upgradeBarPrefab, canvasObject.transform);
                            bar.transform.SetAsFirstSibling();
                            UpgradeBarFollow barScript = bar.GetComponent<UpgradeBarFollow>();
                            buildingBars[temp.gameObject] = barScript;

                            //float buildTime = 10f;
                            float buildTime = timeIwant;

                            if (buildTime <= 300f && speedUpIconPrefab != null)
                            {
                                // إذا كان هناك أيقونة سابقة للمبنى نفسه، نحذفها
                                if (activeSpeedUpIcons.ContainsKey(temp.gameObject))
                                {
                                    Destroy(activeSpeedUpIcons[temp.gameObject]);
                                    activeSpeedUpIcons.Remove(temp.gameObject);
                                }

                                GameObject icon = Instantiate(speedUpIconPrefab, temp.transform.position + new Vector3(0, 2f, 0f), Quaternion.identity, temp.transform);
                                activeSpeedUpIcons[temp.gameObject] = icon;
                            }


                            barScript.StartUpgradeBar(temp.transform, buildTime);

                            temp.ShowUpgradeIcon();
                            StartCoroutine(HideUpgradeIconAfterDelay(temp, buildTime));

                            // مثلاً في دالة TryPlaceBuilding:
                            Sprite buildingSprite = GetSpriteFromDeepChild(temp.gameObject, "anyb_0");
                            if (buildingSprite != null)
                            {
                                string tagToCheck = GetTagFromBuildingOrChildren(temp.gameObject);

                                string NameBuild = "";
                                switch (tagToCheck)
                                {
                                    case "wood": NameBuild = "LUMBER BLADE"; break;
                                    case "boomOP": NameBuild = "BEAST CANNON"; break;
                                    case "stone": NameBuild = "STONE GRINDER"; break;
                                    case "iron": NameBuild = "IRON SMELTER"; break;
                                    case "power": NameBuild = "STATUE OF MIGHT"; break;
                                    case "CenterCity": NameBuild = "CENTER CITY"; break;
                                    case "Resoures": NameBuild = "STOCK PILE"; break;
                                    case "archer": NameBuild = "ARCHER'S TOWER"; break;
                                    case "archerOP": NameBuild = "GALTING ARROW TOWER"; break;
                                    case "maker": NameBuild = "HIRING QUARTERS"; break;
                                    case "flameT": NameBuild = "FLAMER TOWER"; break;
                                    case "flayTower": NameBuild = "SKY TOWER"; break;
                                    case "Mirror": NameBuild = "BURNING MIRRORS"; break;
                                    case "spy": NameBuild = "WATCH POST"; break;
                                    case "spyClan": NameBuild = "FREEND WATCH POST"; break;
                                    case "boomTower": NameBuild = "BOMBARD TOWER"; break;
                                    case "Barracks": NameBuild = "MERCENARY BARRACKS"; break;
                                    case "BrackClan": NameBuild = "CLAN BARRACKS"; break;
                                    case "clan": NameBuild = "CLAN"; break;
                                    case "exeBuild": NameBuild = "EXECUTIONAL CUILLOTINE"; break;
                                    case "plan": NameBuild = "CITY PLANNER"; break;
                                    case "post": NameBuild = "PIGEON POST"; break;
                                    case "fourMaker": NameBuild = "CENTRAL HIRING"; break;
                                    case "hadad": NameBuild = "BLACKSMITH"; break;
                                    case "horn": NameBuild = "TUSK HORN"; break;
                                    case "NewWorr": NameBuild = "RECRUITMENT CHAMBER"; break;
                                    case "training": NameBuild = "TRAINGING CHAMBER"; break;
                                    case "Cave": NameBuild = "BEAST KEEPER"; break;
                                    case "CaveBrack": NameBuild = "BEAST CAVE"; break;
                                    case "mongel": NameBuild = "GATAPULT"; break;
                                    case "trapfire": NameBuild = "FIRE TRAP"; break;
                                    case "trapN": NameBuild = "BURIED SPIKES"; break;
                                    case "wall": NameBuild = "WALL"; break;

                                    default: NameBuild = "Building"; break;
                                }
                                BuildingLimitSystem.instaine.Build(NameBuild);
                                AudioManager.Instance.PlaySFX("build1");

                                if (NameBuild == "HIRING QUARTERS")
                                {
                                    Debug.Log("its Maker build do somthing");
                                    hiringQuartersCount++; // زيادة العداد
                                    Debug.Log("Number make now is : " + hiringQuartersCount);

                                }

                                // ⏳ إظهار البوب أب بعد انتهاء البناء
                                if (popupManager != null)
                                {
                                    if (NameBuild == "BURNING MIRRORS")
                                    {
                                        buildingSprite = Resources.Load<Sprite>("buildingIcons/mirorr");

                                    }
                                    else if (NameBuild == "SKY TOWER")
                                    {
                                        buildingSprite = Resources.Load<Sprite>("buildingIcons/flay");
                                    }
                                    Coroutine popupCoroutine = StartCoroutine(ShowPopupAfterDelay(buildTime, NameBuild, buildingSprite));
                                    buildingPopups[temp.gameObject] = popupCoroutine; // حفظ المرجع
                                }

                            }
                            else
                            {
                                Debug.LogWarning("لم يتم العثور على صورة الابن anyb_0");
                            }

                            // بعد انتهاء البناء، يرجع العامل
                            Coroutine workerCoroutine = StartCoroutine(ReturnWorkerAfterDelay(buildTime));
                            returnWorkerCoroutines[temp.gameObject] = workerCoroutine;

                        }
                    }
                    else
                    {
                        Debug.LogWarning("❌ لا يوجد عمال متاحون حالياً!");
                        return; // 🛑 لا تكمل لأن العامل غير متاح
                    }
                }
                else
                {
                    // ✅ فقط تحريك مبنى موجود، لا نخصم عامل ولا نبدأ تطوير
                    temp.Place(); // فقط نضعه في المكان الجديد
                }

                // 🟡 التعديل هنا: نتحقق إذا كان البناء جديداً ويحمل التاغ "wall"
                if (isNewBuild && GetTagFromBuildingOrChildren(temp.gameObject) == "wall")
                {
                    // 1️⃣ التحقق من الخشب
                    bool hasEnoughWood = ResourceManager.Instance.wood >= 1000;


                    // 3️⃣ التحقق من الحد الأقصى لعدد الجدران المسموح ببنائها
                    bool hasNotReachedLimit = true;
                    if (BuildingLimitSystem.instaine != null)
                    {
                        // نبحث عن المبنى الذي اسمه "WALL" في القائمة
                        var wallData = BuildingLimitSystem.instaine.buildings.Find(b => b.buildingName == "WALL");
                        if (wallData != null)
                        {
                            // نتحقق إذا كان العدد الحالي لا يزال أقل من الحد الأقصى
                            hasNotReachedLimit = wallData.currentCount < wallData.maxAllowed;
                        }
                    }

                    // 4️⃣ جمع كل الشروط: هل يمكنه وضع جدار آخر؟
                    bool canAffordNextWall = hasEnoughWood && hasNotReachedLimit;

                    if (canAffordNextWall)
                    {
                        // كل شيء تمام: نستدعي دالة البناء مجدداً بنفس الجدار ليبقى متصلاً
                        ResourceManager.RemoveWood(1000);
                        temp = null;
                        InitializeWithBuilding(currentBuildingPrefab);
                    }
                    else
                    {
                        // إما أن الموارد نقصت أو وصلنا للحد الأقصى: ننهي البناء المستمر
                        Debug.LogWarning("❌ توقف البناء المستمر: نفدت الموارد أو وصلت للحد الأقصى لعدد الجدران.");

                        temp = null;
                        isMovingExistingBuilding = false;
                        isNewBuild = false;
                        YesNobTns.result = 0;
                        actionPanel.SetActive(false);
                        EnableBlockingUI(false);
                    }
                }
                else
                {
                    // في كل الحالات يتم إنهاء المؤقتات والـ UI
                    temp = null;
                    isMovingExistingBuilding = false;
                    isNewBuild = false;
                    YesNobTns.result = 0;
                    actionPanel.SetActive(false);
                    EnableBlockingUI(false);
                }

            }
            else
            {
                Debug.LogWarning("❌ لا يمكن وضع المبنى هنا.");
            }
        }
    }

    public string GetTagFromBuildingOrChildren(GameObject building)
    {
        // أولاً الأب
        if (!string.IsNullOrEmpty(building.tag) && building.tag != "Untagged")
        {
            return building.tag;
        }

        // ثم بحث عميق في الأبناء
        foreach (Transform child in building.GetComponentsInChildren<Transform>(true))
        {
            if (!string.IsNullOrEmpty(child.tag) && child.tag != "Untagged")
            {
                return child.tag;
            }
        }

        return "Untagged";
    }


    Sprite GetSpriteFromDeepChild(GameObject building, string childName = "anyb_0")
    {
        Transform childTransform = FindDeepChild(building.transform, childName);
        if (childTransform != null)
        {
            Image img = childTransform.GetComponent<Image>();
            if (img != null && img.sprite != null)
            {
                return img.sprite;
            }

            SpriteRenderer sr = childTransform.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                return sr.sprite;
            }
        }
        return null;
    }

    Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }






    public void CancelBuilding()
    {
        if (temp != null)
        {
            if (isMovingExistingBuilding && isNewBuild == false)
            {
                Debug.Log("⏪ تم إرجاع المبنى إلى مكانه الأصلي.");

                // 🟢 1. إعادة المبنى لموقعه القديم (بدقة)
                Vector3 returnPosition = gridlayout.CellToLocalInterpolated(originalCellPosition + new Vector3(0.5f, 0.5f, 0f));
                temp.transform.position = returnPosition;

                // 🟢 2. إعادة تحديد المنطقة بناءً على المكان الأصلي
                temp.area.position = originalCellPosition;

                // 🟢 3. إعادة الأرضية الأصلية إلى Green (لأنها كانت محجوزة لهذا المبنى)
                SetTilesBlock(temp.area, TileType.Green, MainTilemap);

                // 🟢 4. تنظيف الـ TempTilemap (المؤقتة)
                TempTilemap.ClearAllTiles();

                // 🟢 5. إغلاق البنل
                temp = null;
                isMovingExistingBuilding = false;
                actionPanel.SetActive(false);
                EnableBlockingUI(false);
                YesNobTns.result = 0;
            }

            else
            {
                Transform[] children = temp.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    if (child.CompareTag("Resoures"))
                    {
                        Debug.Log("🗑️resBuildReomve from child");
                        ResourceManager.Instance.RemoveStorageBuilding(7500);
                        break; // وجدناه، لا داعي للمتابعة
                    }
                }

                Debug.Log("🗑️ تم إلغاء البناء الجديد.");
                ClearArea(); // ← هذا ينظف الـ TempTilemap

                /////////////////////////// SAVING /////////////////////////
                if (activeBuildingsInScene.Contains(temp.gameObject))
                {
                    activeBuildingsInScene.Remove(temp.gameObject);
                }
                /////////////////////////// SAVING /////////////////////////

                Destroy(temp.gameObject);
                temp = null;
                isNewBuild = false;
                actionPanel.SetActive(false);
                EnableBlockingUI(false);
                YesNobTns.result = 0;

            }
        }
    }

    private bool IsPointerOverUIButNotOnTemp()
    {
        // إذا لا يوجد Temp نرجع الحالة العادية
        if (temp == null)
            return EventSystem.current.IsPointerOverGameObject();

        // نحصل على موقع الماوس في العالم
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // نفحص هل الماوس فوق كوليدر خاص بالمبنى
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null)
        {
            // ✅ إذا المبنى الذي تم الضغط عليه هو temp نفسه، نسمح بالضغط حتى لو فوق UI
            if (hit.gameObject == temp.gameObject || hit.transform.root.gameObject == temp.gameObject)
            {
                return false;
            }
        }

        // غير ذلك نرجع القيمة الحقيقية إن كان الماوس فوق أي UI
        return EventSystem.current.IsPointerOverGameObject();
    }



    private void UpdateActionPanelPosition()
    {
        if (temp == null || actionPanel == null) return;

        Vector3 screenPos = cam.WorldToScreenPoint(temp.transform.position);
        float yOffset = 250f;
        screenPos.y += yOffset;

        actionPanel.transform.position = screenPos;
    }

    private bool IsOverlappingOtherBuilding(BoundsInt area)
    {
        Vector3 min = gridlayout.CellToWorld(area.min);
        Vector3 max = gridlayout.CellToWorld(area.max);
        Vector2 size = max - min;
        Vector2 center = (min + max) / 2;

        // نقوم بالفحص في Layer معين مثلاً "Building" لتجنب الالتقاط الزائد
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f);

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            GameObject hitRoot = hit.transform.root.gameObject;

            // 🟡 تخطى الـ temp نفسه (لأنه تم إنشاؤه بالفعل قبل الوضع)
            if (temp != null && (hitRoot == temp.gameObject || hitRoot == temp.transform.root.gameObject))
                continue;

            // ✅ هذا كائن آخر غير temp
            return true;
        }

        return false;
    }

    private IEnumerator HideUpgradeIconAfterDelay(building buildingScript, float delay)
    {
        if (buildingScript == null)
            yield break;

        GameObject root = buildingScript.gameObject;

        // 🟡 نحفظ حالة كل Collider (هل كان مفعلاً أم لا)
        Dictionary<Collider2D, bool> colliderStates = new Dictionary<Collider2D, bool>();

        foreach (Collider2D col in root.GetComponentsInChildren<Collider2D>(true))
        {
            // ✅ إذا كان هذا الكوليدر موجود تحت طفل اسمه "upgrade (1)" نتخطاه
            if (col.transform.name == "upgrade (1)")
                continue;

            colliderStates[col] = col.enabled;
            col.enabled = false; // ⛔ تعطيل الكوليدر
        }

        buildingScript.ShowUpgradeIcon();

        yield return new WaitForSeconds(delay);

        buildingScript.HideUpgradeIcon();

        // ✅ إعادة تفعيل الكوليدرات حسب حالتها الأصلية
        foreach (var kvp in colliderStates)
        {
            if (kvp.Key != null)
                kvp.Key.enabled = kvp.Value;
        }
        // إخفاء أيقونة SpeedUp الخاصة بهذا المبنى
        if (activeSpeedUpIcons.TryGetValue(buildingScript.gameObject, out GameObject icon) && icon != null)
        {
            Destroy(icon);
            activeSpeedUpIcons.Remove(buildingScript.gameObject);
        }


    }

    private IEnumerator ShowPopupAfterDelay(float delay, string nameBuild, Sprite sprite)
    {
        yield return new WaitForSeconds(delay);
        popupManager.ShowPopup(nameBuild + " is built now!", sprite);


    }

    private IEnumerator ReturnWorkerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (workerUI != null)
        {
            workerUI.ReturnWorker();
            //Debug.Log("✅ تم إرجاع العامل بعد انتهاء البناء."); ديبك
        }
    }

    private GameObject selectedBuilding; // المبنى المحدد حاليا
    [Header("أسماء المباني الصالحة لتسريع الترقية")]
    public List<string> validBuildingNames = new List<string>();

    private void DetectClickOnUpgradeChild()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && hit.name == "upgrade (1)")
            {
                Debug.Log("✅ تم النقر على الابن upgrade (1)");

                // جلب سكربت NameUpgrdes
                NameUpgrdes script = hit.GetComponent<NameUpgrdes>();
                if (script != null)
                {
                    string buildName = script.buildingName;
                    Debug.Log("🏗️ اسم المبنى من السكربت: " + buildName);
                    if (validBuildingNames.Contains(buildName))
                    {
                        Debug.Log("🌲 المبنى صالح لتسريع الترقية");

                        // تعيين المبنى الأب كمحدد حاليا
                        selectedBuilding = hit.transform.parent.gameObject;
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ سكربت NameUpgrdes غير موجود في الابن upgrade (1)");
                }
            }
        }

    }

    public TMP_Text GOLDtEXT; // اربطها من الـ Inspector
    int ExtractNumberFromText(string text)
    {
        string digitsOnly = "";
        foreach (char c in text)
        {
            if (char.IsDigit(c))
                digitsOnly += c;
        }

        if (string.IsNullOrEmpty(digitsOnly))
            return 0;

        return int.Parse(digitsOnly);
    }

    // دالة تستدعيها عند الضغط على زر تسريع الترقية
    public void OnSpeedUpButtonClicked()
    {
        int requiredGold = ExtractNumberFromText(GOLDtEXT.text);

        if (selectedBuilding != null)
        {
            Debug.Log("⏱ تم الضغط على زر تسريع الترقية");
            GoldCollector.RemoveGold(requiredGold);
            // استدعاء دالة داخل GridBuildingSystem لتسريع الوقت
            ForceFinishUpgrade(selectedBuilding);
        }
        else
        {
            Debug.LogWarning("⚠️ لم يتم تحديد أي مبنى لتسريع الترقية");
        }
    }

    // دالة لتسريع وقت الترقية للبناء
    private void ForceFinishUpgrade(GameObject building)
    {
        if (building == null)
        {
            Debug.LogWarning("⚠️ المبنى غير موجود!");
            return;
        }

        // 1️⃣ تسريع البار
        if (buildingBars.TryGetValue(building, out UpgradeBarFollow barScript))
        {
            barScript.ForceFinishIn(0.1f);
            Debug.Log("⏱ تم تسريع البار!");
        }

        // 2️⃣ إنهاء وقت البناء الفعلي
        building buildingScript = building.GetComponent<building>();
        if (buildingScript != null)
        {
            buildingScript.ForceFinishConstruction();
            buildingScript.IsUnderConstruction = false; // 🟢 انتهى البناء

            foreach (Collider2D col in building.GetComponentsInChildren<Collider2D>(true))
            {
                col.enabled = true; // إعادة تفعيل الكوليدرات
            }

            buildingScript.HideUpgradeIcon();
        }



        if (returnWorkerCoroutines.TryGetValue(building, out Coroutine workerCoroutine) && workerCoroutine != null)
        {
            StopCoroutine(workerCoroutine);
            returnWorkerCoroutines.Remove(building);
        }

        // 3️⃣ إعادة العامل فورًا
        if (workerUI != null)
        {
            workerUI.ReturnWorker();
            Debug.Log("👷‍♂️ تم إرجاع العامل فورًا!");
        }

        // إخفاء أيقونة SpeedUp
        if (activeSpeedUpIcons.TryGetValue(building, out GameObject icon) && icon != null)
        {
            Destroy(icon);
            activeSpeedUpIcons.Remove(building);
        }

        // 4️⃣ إيقاف Coroutine البوب أب الأصلي إذا كان موجود
        if (buildingPopups.TryGetValue(building, out Coroutine popupCoroutine) && popupCoroutine != null)
        {
            StopCoroutine(popupCoroutine);
            buildingPopups.Remove(building);
        }

        // 5️⃣ إظهار البوب أب فورًا
        if (popupManager != null)
        {
            Sprite buildingSprite = GetSpriteFromDeepChild(building, "anyb_0");
            string buildingName = GetTagFromBuildingOrChildren(building);

            string NameBuild = "";
            switch (buildingName)
            {
                case "wood": NameBuild = "LUMBER BLADE"; break;
                case "boomOP": NameBuild = "BEAST CANNON"; break;
                case "stone": NameBuild = "STONE GRINDER"; break;
                case "iron": NameBuild = "IRON SMELTER"; break;
                case "power": NameBuild = "STATUE OF MIGHT"; break;
                case "CenterCity": NameBuild = "CENTER CITY"; break;
                case "Resoures": NameBuild = "STOCK PILE"; break;
                case "archer": NameBuild = "ARCHER'S TOWER"; break;
                case "archerOP": NameBuild = "GALTING ARROW TOWER"; break;
                case "maker": NameBuild = "HIRING QUARTERS"; break;
                case "flameT": NameBuild = "FLAMER TOWER"; break;
                case "flayTower": NameBuild = "SKY TOWER"; break;
                case "Mirror": NameBuild = "BURNING MIRRORS"; break;
                case "spy": NameBuild = "WATCH POST"; break;
                case "spyClan": NameBuild = "FREEND WATCH POST"; break;
                case "boomTower": NameBuild = "BOMBARD TOWER"; break;
                case "Barracks": NameBuild = "MERCENARY BARRACKS"; break;
                case "BrackClan": NameBuild = "CLAN BARRACKS"; break;
                case "clan": NameBuild = "CLAN"; break;
                case "exeBuild": NameBuild = "EXECUTIONAL CUILLOTINE"; break;
                case "plan": NameBuild = "CITY PLANNER"; break;
                case "post": NameBuild = "PIGEON POST"; break;
                case "fourMaker": NameBuild = "CENTRAL HIRING"; break;
                case "hadad": NameBuild = "BLACKSMITH"; break;
                case "horn": NameBuild = "TUSK HORN"; break;
                case "NewWorr": NameBuild = "RECRUITMENT CHAMBER"; break;
                case "training": NameBuild = "TRAINGING CHAMBER"; break;
                case "Cave": NameBuild = "BEAST KEEPER"; break;
                case "CaveBrack": NameBuild = "BEAST CAVE"; break;
                case "mongel": NameBuild = "GATAPULT"; break;
                case "trapfire": NameBuild = "FIRE TRAP"; break;
                case "trapN": NameBuild = "BURIED SPIKES"; break;
                case "wall": NameBuild = "WALL"; break;
                default: NameBuild = "Building"; break;
            }

            if (NameBuild == "BURNING MIRRORS")
            {
                Sprite buildingSprite2 = Resources.Load<Sprite>("buildingIcons/mirorr");
                popupManager.ShowPopup(NameBuild + " is built now!", buildingSprite);

            }
            else if (NameBuild == "SKY TOWER")
            {
                Sprite buildingSprite2 = Resources.Load<Sprite>("buildingIcons/flay");
                popupManager.ShowPopup(NameBuild + " is built now!", buildingSprite);

            }
            else popupManager.ShowPopup(NameBuild + " is built now!", buildingSprite);

            Debug.Log("🎉 تم عرض Popup البناء فورًا!");
        }
    }
    /////////////////////////// SAVING /////////////////////////

    // دالة لتحرير مساحة المبنى عند مسحه (للتحميل)
    public void FreeArea(BoundsInt area)
    {
        TileBase[] whiteTiles = new TileBase[area.size.x * area.size.y * area.size.z];
        FillTiles(whiteTiles, TileType.White);
        MainTilemap.SetTilesBlock(area, whiteTiles);
    }

    // دالة إنشاء المباني المحفوظة مباشرة
    // 🟢 أضفنا int savedLevel هنا
    // 🟢 قمنا بتعديل الأقواس لتستقبل bData بالكامل
    public void SpawnBuildingFromSave(BuildingSaveData data)
    {
        Vector3Int cellPos = new Vector3Int(data.x, data.y, data.z);
        GameObject prefabToSpawn = null;

        foreach (var mapping in buildingDatabase)
        {
            if (mapping.buildingTag == data.buildingTag)
            {
                prefabToSpawn = mapping.prefab;
                break;
            }
        }

        if (prefabToSpawn == null) return;

        Vector3 worldPos = gridlayout.CellToLocalInterpolated(cellPos + new Vector3(0.5f, 0.5f, 0f));
        GameObject newBuilding = Instantiate(prefabToSpawn, worldPos, Quaternion.identity);

        building bScript = newBuilding.GetComponent<building>();
        if (bScript != null)
        {
            bScript.area.position = cellPos;
            TakeArea(bScript.area);

            // 🟢 فحص البناء الجديد
            if (data.isBuildingNew)
            {
                // كم ثانية متبقية بناءً على الوقت الحقيقي؟
                System.TimeSpan diff = new System.DateTime(data.buildEndTimeTicks) - System.DateTime.Now;
                float remainingSeconds = (float)diff.TotalSeconds;

                if (remainingSeconds > 0)
                {
                    // لم ينتهِ الوقت بعد، نكمل البناء
                    bScript.IsUnderConstruction = true;
                    // 🟢 سحب عامل لأن البناء لا يزال مستمراً
                    if (WorkerUI.Instance != null) WorkerUI.Instance.OccupyWorkerDuringLoad();

                    bScript.buildEndTimeTicks = data.buildEndTimeTicks;
                    bScript.buildTotalDuration = data.buildTotalDuration;

                    GameObject canvasObject = GameObject.FindWithTag("MainCanvas");
                    if (canvasObject != null && upgradeBarPrefab != null)
                    {
                        GameObject bar = Instantiate(upgradeBarPrefab, canvasObject.transform);
                        bar.transform.SetAsFirstSibling();
                        UpgradeBarFollow barScript = bar.GetComponent<UpgradeBarFollow>();
                        buildingBars[newBuilding] = barScript;
                        // 🟢 استخدام الدالة الجديدة لاستئناف البار بالنسبة الصحيحة
                        barScript.ResumeUpgradeBar(newBuilding.transform, data.buildTotalDuration, remainingSeconds);
                    }
                    bScript.ShowUpgradeIcon();
                    StartCoroutine(HideUpgradeIconAfterDelay(bScript, remainingSeconds));
                }
                else
                {
                    // الوقت انتهى واللاعب غير موجود!
                    bScript.IsUnderConstruction = false;
                    bScript.HideUpgradeIcon();
                }
            }
            else
            {
                bScript.IsUnderConstruction = false;
                bScript.HideUpgradeIcon();
            }
        }

        // 🟢 فحص التطوير (الترقية)
        BuildLvl lvlScript = newBuilding.GetComponent<BuildLvl>();
        if (lvlScript != null)
        {
            lvlScript.SetLevelFromSave(data.level);

            if (data.isUpgrading)
            {
                System.TimeSpan diff = new System.DateTime(data.upgradeEndTimeTicks) - System.DateTime.Now;
                float remainingSeconds = (float)diff.TotalSeconds;

                if (remainingSeconds > 0)
                {
                    // استئناف الترقية
                    lvlScript.isUpgrading = true;
                    // 🟢 سحب عامل لأن الترقية لا تزال مستمرة
                    if (WorkerUI.Instance != null) WorkerUI.Instance.OccupyWorkerDuringLoad();

                    lvlScript.upgradeEndTimeTicks = data.upgradeEndTimeTicks;
                    lvlScript.upgradeTotalDuration = data.upgradeTotalDuration;


                    // 🟢 السطر الجديد: استدعاء الدالة لضمان تسجيل الكوروتين
                    lvlScript.ResumeUpgrade(remainingSeconds, data.upgradeTotalDuration);
                }
                else
                {
                    // الترقية انتهت أثناء إغلاق اللعبة! نرقي المبنى فوراً للمستوى التالي
                    lvlScript.InstantUpgrade();
                }
            }
        }
        // 🟢 فحص تطوير المحاربين (إذا كان المبنى يحتوي على التدريب)
        if (data.isTrainingWarrior && UpgradeWarriorButton.Instance != null)
        {
            // نمرر المبنى الرئيسي للكوروتين الجديد لكي ينتظر السكربت الداخلي
            UpgradeWarriorButton.Instance.StartCoroutine(UpgradeWarriorButton.Instance.WaitAndResumeWarrior(
                newBuilding,
                data.trainingWarriorName,
                data.trainingWarriorEndTimeTicks,
                data.trainingWarriorTotalDuration
            ));
        }

        activeBuildingsInScene.Add(newBuilding);
    }
    /////////////////////////// SAVING /////////////////////////

}
public enum TileType
{
    Empty,
    White,
    Green,
    Red
}
