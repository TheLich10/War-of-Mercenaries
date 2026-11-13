using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class IronCollect : MonoBehaviour
{
    public static IronCollect Instance { get; private set; }

    public TextMeshProUGUI ironText; // مرجع إلى نص الحديد في UI
    public GameObject popupPrefab;
    public Sprite ironIcon;

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

        if (hit.collider != null && hit.collider.CompareTag("iron"))
        {
            lastClickedBuilding = hit.collider.transform; // ✅ حفظ موقع آخر مبنى نُقر عليه
            CollectGold();
        }
    }

    public void CollectGold()
    {
        if (lastClickedBuilding != null)
        {
            buildIronStorge clickedIronBuilding = lastClickedBuilding.GetComponent<buildIronStorge>();

            if (clickedIronBuilding != null)
            {
                // 1. حساب السعة المتبقية في المخزون الرئيسي للحديد
                int currentIron = ResourceManager.Instance.iron;
                int maxCapacity = ResourceManager.Instance.GetTotalCapacity();
                int capacityLeft = maxCapacity - currentIron;

                // 2. التحقق مما إذا كان المخزون ممتلئاً
                if (capacityLeft <= 0)
                {
                    //Debug.LogWarning("مخزون الحديد ممتلئ! قم بترقية المخازن.");
                    PopupText.instan.ShowPopup("IRON full! Build storages or upgrade city.");

                    return; // إيقاف الدالة لمنع ضياع الموارد
                }

                // 3. نرسل المساحة المتبقية للمبنى ليتم الحصاد الجزئي
                int collectedIron = clickedIronBuilding.CollectGold(capacityLeft);

                if (collectedIron > 0)
                {
                    ResourceManager.AddIron(collectedIron);
                    ShowPopup(ironIcon, collectedIron, clickedIronBuilding.transform);
                }

                UpdateGoldUI();
            }
        }
    }

    void UpdateGoldUI()
    {
        ResourceManager.Instance.UpdateUI();
        // إذا كنت تريد تخصيص النص هنا:
        // ironText.text = ResourceManager.Instance.GetIron().ToString();
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
