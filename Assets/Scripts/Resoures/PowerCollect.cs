using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerCollect : MonoBehaviour
{
    public static PowerCollect Instance { get; private set; }

    public TextMeshProUGUI powerText;
    public GameObject popupPrefab;
    public Sprite powericon;

    private int popupIndex = 0;
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
        UpdateGoldUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            DetectBuilding();
        }
    }

    void DetectBuilding()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("power"))
        {
            lastClickedBuilding = hit.collider.transform;
            CollectGold();
        }
    }

    public void CollectGold()
    {
        if (lastClickedBuilding != null)
        {
            buildPowerStroge clickedPowerBuilding = lastClickedBuilding.GetComponent<buildPowerStroge>();

            if (clickedPowerBuilding != null)
            {
                // 1. حساب السعة المتبقية في المخزون الرئيسي للطاقة
                int currentPower = ResourceManager.Instance.power;
                int maxCapacity = ResourceManager.Instance.GetTotalCapacity();
                int capacityLeft = maxCapacity - currentPower;

                // 2. التحقق مما إذا كان المخزون ممتلئاً
                if (capacityLeft <= 0)
                {
                    //Debug.LogWarning("مخزون الطاقة ممتلئ! قم بترقية المخازن.");
                    PopupText.instan.ShowPopup("MIGHT full! Build storages or upgrade city.");

                    return; // إيقاف الدالة لمنع ضياع الموارد
                }

                // 3. نرسل المساحة المتبقية للمبنى ليتم الحصاد الجزئي
                int collectedPower = clickedPowerBuilding.CollectGold(capacityLeft);

                if (collectedPower > 0)
                {
                    ResourceManager.AddPower(collectedPower);
                    ShowPopup(powericon, collectedPower, clickedPowerBuilding.transform);
                }

                UpdateGoldUI();
            }
        }
    }

    void UpdateGoldUI()
    {
        ResourceManager.Instance.UpdateUI();
        // powerText.text = ResourceManager.Instance.GetPower().ToString(); // (اختياري) عرض النص يدويًا
    }

    public void  ShowPopup(Sprite icon, int amount, Transform target)
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
