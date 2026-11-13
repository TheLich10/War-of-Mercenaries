using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine.UI;

public class GridAttckC : MonoBehaviour
{
    public GameObject actionPanel;

    private bool isDragging = false;
    private Camera cam;
    public bool IsDragging => isDragging;

    public static GridAttckC current;

    public GridLayout gridlayout;
    public Tilemap MainTilemap;
    public Tilemap TempTilemap;

    private GameObject temp;
    private Vector3 prevPos;

    private bool canDragTemp = true; // يسمح بالسحب بشكل افتراضي

    public Button woodBtonActive;
    public Button stoneBtonActive;
    public Button powerBtonActive;

    private Coroutine destroyCoroutine;

    public Button enableRotateButton;

    public static bool isWoodAnime = false;
    public static bool isStoneAnime1 = false;
    public static bool isStoneAnime2 = false;
    public static bool isStoneAnime3 = false;
    public static bool isStoneAnime4 = false;

    private void Awake()
    {
        current = this;
        cam = Camera.main;

        //Debug.Log("✅ GridATack working");
    }

    private void Start()
    {
        if (enableRotateButton != null)
        {
            enableRotateButton.onClick.AddListener(EnableRotateCircle);
        }
    }

    private void Update()
    {
        if (temp == null) return;

        // ✅ لا تبدأ السحب إذا المؤشر فوق UI
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject())
            return;

        // بدء السحب عند الضغط على كائن قابل للسحب
        if (Input.GetMouseButtonDown(0) && canDragTemp)
        {
            Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

            if (hit != null && hit.CompareTag("Draggable"))
            {
                isDragging = true;
            }
        }

        // تنفيذ عملية السحب
        if (isDragging && canDragTemp)
        {
            Vector2 touchPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = gridlayout.LocalToCell(touchPos);

            if (prevPos != cellPos)
            {
                temp.transform.localPosition = gridlayout.CellToLocalInterpolated(cellPos + new Vector3(0.5f, 0.5f, 0f));
                prevPos = cellPos;
                FollowObject();
            }
        }

        // إنهاء السحب عند رفع الزر
        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                isDragging = false;
                Debug.Log("Object dropped at: " + temp.transform.position);
            }
        }
        // تأكد أن البنل يبقى فوق الكائن دائماً
        if (temp != null)
            FollowObject();
    }

    public void InitializeWithBuilding(GameObject building)
    {
        if (building == null)
        {
            Debug.LogError("!NO BUILDING");
            return;
        }

        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
        Vector3 worldPos = cam.ScreenToWorldPoint(screenCenter);
        worldPos.z = 0;

        temp = Instantiate(building, worldPos, Quaternion.Euler(45, 0, 0));

        // 🔹 التعديل هنا: استخدام GetComponentInChildren للبحث في الأب والابن
        RotateCircle rotateScript = temp.GetComponentInChildren<RotateCircle>();
        if (rotateScript != null)
        {
            rotateScript.enabled = false;
            Debug.Log("RotateCircle disabled on new object (Found in Parent or Child)");
        }
        else
        {
            Debug.LogWarning("RotateCircle script not found on Parent or Children!");
        }

        actionPanel.SetActive(true);
        canDragTemp = true;

        FollowObject();
    }

    // اربط هذا البوتن من Inspector
    // الدالة التي ستُستدعى عند الضغط على الزر
    public void EnableRotateCircle()
    {
        if (temp == null) return;

        // 🔹 التعديل هنا: استخدام GetComponentInChildren أيضاً
        RotateCircle rotateScript = temp.GetComponentInChildren<RotateCircle>();
        if (rotateScript != null)
        {
            rotateScript.enabled = true;
            Debug.Log("RotateCircle re-enabled");
        }
    }

    private void FollowObject()
    {
        if (temp == null)
        {
            Debug.LogError("⚠️ NO object to follow");
            return;
        }

        // تحويل موقع الكائن إلى إحداثيات الشاشة
        Vector3 screenPos = cam.WorldToScreenPoint(temp.transform.position);
        float yOffset = 50f;
        screenPos.y += yOffset;

        actionPanel.transform.position = screenPos;
    }

    // عند الضغط على زر "نعم"
    bool woodIsActive = false;
    bool stoneIsActive = false;
    bool powerIsActive = false;

    public void OnYesButtonPressed()
    {
        actionPanel.SetActive(false);
        canDragTemp = false;

        // ملاحظة: تأكد من تعريف هذه الكلاسات بشكل صحيح في مشروعك
        if (Temp.instaine != null) Temp.instaine.EnableTrigger();
        if (EnemyBuffZone.instaine != null) EnemyBuffZone.instaine.EnableTrigger();

        if (temp != null)
        {
            GameObject currentTemp = temp; // 🔹 نحفظ نسخة مستقلة من الكائن الحالي

            string tempName = currentTemp.name;

            if (tempName.Contains("smallWood") || tempName.Contains("midWood") || tempName.Contains("largeWood"))
            {
                woodIsActive = true;
                isActiveOrNot();
                isWoodAnime = true;
                if(tempName == "smallWood(Clone)")
                {
                    ResourceManager.RemoveWood(10000);
                }
                else if (tempName == "midWood(Clone)")
                {
                    ResourceManager.RemoveWood(100000);
                }
                else if (tempName == "largeWood(Clone)")
                {
                    ResourceManager.RemoveWood(500000);
                }
            }
            if (tempName.Contains("midenStone") || tempName.Contains("smallStone") || tempName.Contains("largeStone") || tempName.Contains("hugeStone"))
            {
                stoneIsActive = true;
                isActiveOrNot();
                Debug.Log("the texet i wnat is : " + tempName);
                if (tempName == "smallStone(Clone)") 
                {
                    isStoneAnime1 = true;
                    ResourceManager.RemoveStone(10000);

                }
                if (tempName == "midenStone(Clone)")
                {
                    isStoneAnime2 = true;
                    ResourceManager.RemoveStone(100000);

                }
                if (tempName == "largeStone(Clone)")
                {
                    isStoneAnime3 = true;
                    ResourceManager.RemoveStone(500000);

                }
                if (tempName == "hugeStone(Clone)")
                {
                    isStoneAnime4 = true;
                    ResourceManager.RemoveStone(1000000);

                }
            }
            if (tempName.Contains("smallpower") || tempName.Contains("midpower") || tempName.Contains("largepower") || tempName.Contains("hugepower"))
            {
                powerIsActive = true;
                isActiveOrNot();
                if (tempName == "smallpower(Clone)")
                {
                    ResourceManager.RemovePower(10000);
                }
                else if (tempName == "midpower(Clone)")
                {
                    ResourceManager.RemovePower(100000);
                }
                else if (tempName == "largepower(Clone)")
                {
                    ResourceManager.RemovePower(500000);
                }
                else if (tempName == "hugepower(Clone)")
                {
                    ResourceManager.RemovePower(1000000);
                }
            }

            // 🔹 نبدأ مؤقت التدمير للكائن الحالي، حتى لو تم إنشاء كائن جديد لاحقاً
            StartCoroutine(DestroyTempAfterSeconds(currentTemp, 10f));
        }
    }

    // عند الضغط على زر "لا"
    public void OnNoButtonPressed()
    {
        actionPanel.SetActive(false);
        isActiveOrNot();
        Destroy(temp);
    }

    public void isActiveOrNot()
    {
        if (woodBtonActive != null) woodBtonActive.interactable = !woodIsActive;
        if (stoneBtonActive != null) stoneBtonActive.interactable = !stoneIsActive;
        if (powerBtonActive != null) powerBtonActive.interactable = !powerIsActive;
    }

    // كوروتين لتأخير حذف الكائن
    private IEnumerator DestroyTempAfterSeconds(GameObject target, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (target != null)
        {
            Destroy(target);

            // 🔹 فقط لو الكائن الذي دمرناه هو نفسه `temp` الحالي، ننظف المؤشرات
            if (temp == target)
            {
                temp = null;
                canDragTemp = true;
            }
        }
    }

    public void RestALLanim()
    {
        isWoodAnime = false;
        isStoneAnime1 = false;
        isStoneAnime2 = false;
        isStoneAnime3 = false;
        isStoneAnime4 = false;
    }
}