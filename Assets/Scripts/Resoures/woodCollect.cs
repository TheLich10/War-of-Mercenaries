using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // تأكد من إضافة هذا السطر


public class woodCollect : MonoBehaviour
{
    public static woodCollect Instance { get; private set; }

    public TextMeshProUGUI woodText;
    private buildWoodStroge woodBuilding;

    public GameObject popupPrefab;
    public Sprite woodIcon;

    private int popupIndex = 0;

    // ✅ متغير لتخزين آخر مبنى تم النقر عليه
    private Transform lastClickedBuilding;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GameObject buildingObj2 = GameObject.FindWithTag("wood");
        if (buildingObj2 != null)
        {
            woodBuilding = buildingObj2.GetComponent<buildWoodStroge>();
        }
        UpdateGoldUI();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            // ✅ لا تكمل إذا كان المؤشر فوق عنصر UI
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            DetectBuilding();
        }
    }

    void DetectBuilding()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("wood"))
        {
            lastClickedBuilding = hit.collider.transform; // ✅ حفظ موقع آخر مبنى نُقر عليه
            CollectGold(); // استدعاء الدالة بدون متغير
        }
    }

    public void CollectGold()
    {
        if (lastClickedBuilding != null)
        {
            buildWoodStroge clickedWoodBuilding = lastClickedBuilding.GetComponent<buildWoodStroge>();

            if (clickedWoodBuilding != null)
            {
                // 1. حساب السعة المتبقية في المخزون
                int currentWood = ResourceManager.Instance.wood;
                int maxCapacity = ResourceManager.Instance.GetTotalCapacity();
                int capacityLeft = maxCapacity - currentWood;

                // 2. التحقق مما إذا كان المخزون ممتلئاً بالكامل
                if (capacityLeft <= 0)
                {
                    // إظهار رسالة تفيد بامتلاء المخزون
                    //Debug.LogWarning("مخزون الخشب ممتلئ! قم ببناء مخازن جديدة أو ترقية المدينة.");
                    PopupText.instan.ShowPopup("Wood full! Build storages or upgrade city.");

                    // TODO: يمكنك هنا استدعاء كود لإظهار رسالة (UI Text) على الشاشة للاعب

                    return; // إيقاف الدالة هنا لمنع حصد الموارد وضياعها
                }

                // 3. الحصاد إذا كان هناك مساحة
                int collectedwood = clickedWoodBuilding.CollectGold(capacityLeft); ResourceManager.AddWood(collectedwood);

                // إظهار الصورة والعدد فوق المبنى
                if (collectedwood > 0)
                {
                    ShowPopup(woodIcon, collectedwood, clickedWoodBuilding.transform);
                }

                UpdateGoldUI();
            }
        }
    }



    void UpdateGoldUI()
    {
        ResourceManager.Instance.UpdateUI();
    }

    public void ShowPopup(Sprite icon, int amount, Transform target)
    {
        GameObject popup = Instantiate(popupPrefab, GameObject.Find("Canvas").transform);
        Vector3 baseScreenPos = Camera.main.WorldToScreenPoint(target.position + new Vector3(0.5f, 2f, 0));
        Vector3 offset = new Vector3(popupIndex * 40f, 0f, 0f);
        popup.transform.position = baseScreenPos + offset;

        popup.GetComponent<ResourcePopup>().Setup(icon, amount);

        popupIndex++;
        Invoke(nameof(ResetPopupIndex), 0.1f);
    }

    void ResetPopupIndex()
    {
        popupIndex = 0;
    }
}
