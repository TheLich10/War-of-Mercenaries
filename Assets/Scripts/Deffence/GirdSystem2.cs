using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GirdSystem2 : MonoBehaviour
{
    private Camera cam;
    private bool isDragging = false;
    public bool IsDragging => isDragging;

    public static GirdSystem2 current;

    public GridLayout gridlayout;
    public Tilemap MainTilemap;
    public Tilemap TempTilemap;

    private static Dictionary<TileType2, TileBase> tileBases = new Dictionary<TileType2, TileBase>();

    private building temp;
    private Vector3 prevPos;
    private BoundsInt prevArea;

    private void Awake()
    {
        current = this;
        cam = Camera.main;
        //Debug.Log("✅ GridBuildingSystem working");
    }

    private void Start()
    {
        string tilepath = @"Tiles\";
        if (!tileBases.ContainsKey(TileType2.Empty))
            tileBases.Add(TileType2.Empty, null);
        if (!tileBases.ContainsKey(TileType2.White))
            tileBases.Add(TileType2.White, Resources.Load<TileBase>(tilepath + "White"));
        if (!tileBases.ContainsKey(TileType2.Green))
            tileBases.Add(TileType2.Green, Resources.Load<TileBase>(tilepath + "Green"));
        if (!tileBases.ContainsKey(TileType2.Red))
            tileBases.Add(TileType2.Red, Resources.Load<TileBase>(tilepath + "Red"));
        InitializeAttackSceneBuildings();
    }

    // دالة لتحرير مساحة المبنى عند تدميره وإعادتها للحالة الأصلية
    public void ClearBuildingArea(BoundsInt targetArea)
    {
        // نستخدم White لإعادة الأرضية لطبيعتها بدل مسحها بالكامل
        SetTilesBlock(targetArea, TileType2.White, MainTilemap);

        Debug.Log("🟩 تم مسح البلاط الأخضر وإعادة الأرضية للوضع الافتراضي!");
    }
    private void InitializeAttackSceneBuildings()
    {
        // البحث عن جميع المباني الموجودة في مشهد الهجوم الحالي فقط
        building[] attackSceneBuildings = UnityEngine.Object.FindObjectsByType<building>(FindObjectsSortMode.None);

        foreach (building b in attackSceneBuildings)
        {
            // 1. حساب موقع المبنى على شبكة الهجوم بناءً على مكانه الحالي التقريبي
            Vector3Int cellPos = gridlayout.WorldToCell(b.transform.position);

            // 2. تحديث منطقة المبنى
            b.area.position = cellPos;

            // 🌟 3. الحل هنا: إجبار المبنى على التمركز بدقة فوق البلاطات (Snapping)
            b.transform.localPosition = gridlayout.CellToLocalInterpolated(cellPos + new Vector3(0.5f, 0.5f, 0f));

            // 4. حجز المساحة باللون الأخضر
            TakeArea(b.area);

            //Debug.Log($"⚔️ تم محاذاة وحجز مبنى: {b.gameObject.name} في الموقع {cellPos}"); ديبك
        }
    }

    private void Update()
    {
        if (temp == null) return;
        // ✅ لا تكمل إذا كان المؤشر فوق عنصر UI
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // تحريك المبنى الجديد فقط
        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 touchPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = gridlayout.LocalToCell(touchPos);

            if (prevPos != cellPos)
            {
                temp.transform.localPosition = gridlayout.CellToLocalInterpolated(cellPos + new Vector3(0.5f, 0.5f, 0f));
                prevPos = cellPos;
                FollowBuilding();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
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

    private static void SetTilesBlock(BoundsInt area, TileType2 type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArry = new TileBase[size];
        FillTiles(tileArry, type);
        tilemap.SetTilesBlock(area, tileArry);
    }

    private static void FillTiles(TileBase[] arr, TileType2 type)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBases[type];
        }
    }

    public void InitializeWithBuilding(GameObject building)
    {
        if (building == null)
        {
            Debug.LogError("!NO BUILDING");
            return;
        }

        Debug.Log("BULIDNG CRATEING: " + building.name);

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenCenter);
        worldPos.z = 0;

        temp = Instantiate(building, worldPos, Quaternion.identity).GetComponent<building>();
        isDragging = true; // ✅ السحب يبدأ فقط عند إنشاء مبنى جديد

        FollowBuilding();
    }

    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[prevArea.size.x * prevArea.size.y * prevArea.size.z];
        FillTiles(toClear, TileType2.Empty);
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

        Vector3 screenPos = cam.WorldToScreenPoint(temp.transform.position);
        float yOffset = 250f;
        screenPos.y += yOffset;

        BoundsInt buildingArea = temp.area;

        TileBase[] baseArray = GetTileBlock(buildingArea, MainTilemap);
        TileBase[] tileArray = new TileBase[baseArray.Length];

        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == tileBases[TileType2.White])
            {
                tileArray[i] = tileBases[TileType2.Green];
            }
            else
            {
                FillTiles(tileArray, TileType2.Red);
                break;
            }
        }

        TempTilemap.SetTilesBlock(buildingArea, tileArray);
        prevArea = buildingArea;
    }

    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTileBlock(area, MainTilemap);
        foreach (var b in baseArray)
        {
            if (b != tileBases[TileType2.White])
            {
                Debug.Log("cant place here");
                return false;
            }
        }
        return true;
    }

    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType2.Empty, TempTilemap);
        SetTilesBlock(area, TileType2.Green, MainTilemap);
    }

    public void TryPlaceBuilding()
    {
        if (temp != null)
        {
            if (temp.CanBePlaced())
            {
                temp.Place();
                temp = null;
                YesNobTns.result = 0;
            }
            else
            {
                Debug.LogWarning("❌ لا يمكن وضع المبنى هنا.");
            }
        }
    }

    public void CancelBuilding()
    {
        if (temp != null)
        {
            Debug.Log("destroy the building done.");
            ClearArea();
            Destroy(temp.gameObject);
            temp = null;
            YesNobTns.result = 0;
        }
    }
}

public enum TileType2
{
    Empty,
    White,
    Green,
    Red
}
