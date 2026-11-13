using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class EnemyInteraction : MonoBehaviour
{
    private RectTransform panelRectTransform;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    // 🌟 استبدال البانل الواحد بمصفوفة تتسع لـ 5 بانيلات
    // رتبهم في الـ Inspector: العنصر 0 = بانل 1، العنصر 1 = بانل 2 ... وهكذا
    public GameObject[] actionPanels = new GameObject[5];

    private GameObject activePanel; // لتخزين البانل المفتوح حالياً للتحكم بموضعه وإغلاقه
    private GameObject selectedEnemy;

    void Start()
    {
        // إغلاق جميع البانيلات في البداية
        ClosePanel();
        AudioManager.Instance.PlayMusic("Second");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsClickOnUIInsidePanel())
                return;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                ClosePanel();
                return;
            }

            DetectEnemy();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }

        // تحديث موضع البانل النشط حالياً باستمرار إذا كان مفعلًا
        if (activePanel != null && activePanel.activeSelf && selectedEnemy != null)
        {
            UpdatePanelPosition();
        }
    }

    bool IsClickOnUIInsidePanel()
    {
        if (activePanel == null) return false;

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

            if (result.gameObject.transform.IsChildOf(activePanel.transform))
            {
                return true;
            }
        }

        return false;
    }

    void DetectEnemy()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("playerENM"))
        {
            // 🌟 البحث عن سكربت NamePlayer في العدو الذي تم النقر عليه
            NamePlayer enemyNameScript = hit.collider.GetComponent<NamePlayer>();

            if (enemyNameScript != null)
            {
                selectedEnemy = hit.collider.gameObject;

                // إغلاق أي بانل مفتوح سابقاً قبل فتح الجديد
                ClosePanel();
                selectedEnemy = hit.collider.gameObject; // إعادة تعيينه لأن ClosePanel تحذفه

                // 🌟 المقارنة وتحديد البانل المطلوب بناءً على متغير Name
                switch (enemyNameScript.Name)
                {
                    case "1": activePanel = actionPanels[0]; break;
                    case "2": activePanel = actionPanels[1]; break;
                    case "3": activePanel = actionPanels[2]; break;
                    case "4": activePanel = actionPanels[3]; break;
                    case "5": activePanel = actionPanels[4]; break;
                    default:
                        Debug.LogWarning("اسم اللاعب غير مطابق لأي بانل: " + enemyNameScript.Name);
                        break;
                }

                // تفعيل البانل المختار وتحديث موضعه
                if (activePanel != null)
                {
                    activePanel.SetActive(true);
                    UpdatePanelPosition();
                }
            }
        }
        else
        {
            ClosePanel();
        }
    }

    void UpdatePanelPosition()
    {
        if (activePanel == null) return;

        Vector3 worldPos = selectedEnemy.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        Vector3 offset = new Vector3(170f, 30f, 0f); // لليمين وأعلى قليلاً
        activePanel.transform.position = screenPos + offset;
    }

    void ClosePanel()
    {
        // إغلاق كافة البانيلات الخمسة للتأكد من عدم بقاء أي منها مفتوحاً
        foreach (GameObject panel in actionPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
        activePanel = null;
        selectedEnemy = null;
    }
}