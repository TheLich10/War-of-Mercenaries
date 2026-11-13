using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; // تأكد من إضافة هذا السطر

public class StoneCollect : MonoBehaviour
{
    public static StoneCollect Instance { get; private set; }

    public TextMeshProUGUI stoneText; // مرجع إلى نص الحجر في UI
    public GameObject popupPrefab;
    public Sprite stoneicon;

    private int popupIndex = 0;
    private Transform lastClickedBuilding;

    void Awake()
    {
        // ضمان وجود كائن واحد فقط (Singleton)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
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

        if (hit.collider != null && hit.collider.CompareTag("stone"))
        {
            lastClickedBuilding = hit.collider.transform; // ✅ حفظ موقع آخر مبنى نُقر عليه
            CollectGold();
        }
    }

    public void CollectGold()
    {
        if (lastClickedBuilding != null)
        {
            buildStoneStroge clickedStoneBuilding = lastClickedBuilding.GetComponent<buildStoneStroge>();

            if (clickedStoneBuilding != null)
            {
                // 1. حساب السعة المتبقية في المخزون الرئيسي للحجر
                int currentStone = ResourceManager.Instance.stone;
                int maxCapacity = ResourceManager.Instance.GetTotalCapacity();
                int capacityLeft = maxCapacity - currentStone;

                // 2. التحقق مما إذا كان المخزون ممتلئاً
                if (capacityLeft <= 0)
                {
                    //Debug.LogWarning("مخزون الحجر ممتلئ! قم بترقية المخازن.");
                    PopupText.instan.ShowPopup("STONE full! Build storages or upgrade city.");

                    return; // إيقاف الدالة لمنع ضياع الموارد
                }

                // 3. نرسل المساحة المتبقية للمبنى ليتم الحصاد الجزئي
                int collectedStone = clickedStoneBuilding.CollectGold(capacityLeft);

                if (collectedStone > 0)
                {
                    ResourceManager.AddStone(collectedStone);
                    ShowPopup(stoneicon, collectedStone, clickedStoneBuilding.transform);
                }

                UpdateGoldUI();
            }
        }
    }

    void UpdateGoldUI()
    {
        ResourceManager.Instance.UpdateUI();
        // stoneText.text = ResourceManager.Instance.GetStone().ToString(); // (اختياري) إذا أردت عرض النص يدويًا
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
