using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class buildSpySelcter : MonoBehaviour
{
    private RectTransform panelRectTransform;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    public GameObject actionPanel; // الأب الذي يحتوي البنلات
    private GameObject selectedBuilding;

    // كل البنلات الفرعية داخل actionPanel، اربطها في Inspector حسب الترتيب أو اسحب الأب وسيأخذهم تلقائيًا
    public List<GameObject> subPanels;

    // التاغات المطابقة حسب الترتيب
    private string[] validTags = { "boomOP", "archer", "archerOP", "flameT", "flayTower", "Mirror", "spy", "spyClan", "boomTower", "CaveBrack" };

    void Start()
    {
        actionPanel.SetActive(false);
        panelRectTransform = actionPanel.GetComponent<RectTransform>();
        HideAllSubPanels();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsClickOnUIInsidePanel())
                return;

            if (IsPointerOverUI())
            {
                ClosePanel();
                return;
            }

            DetectBuilding();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    bool IsClickOnUIInsidePanel()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject != null && result.gameObject.transform.IsChildOf(actionPanel.transform))
            {
                return true;
            }
        }

        return false;
    }

    void DetectBuilding()
    {
        if (IsPointerOverUI())
            return;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            string tag = hit.collider.tag;

            // 🔹 التحقق العادي بالـ Tag
            if (validTags.Contains(tag))
            {
                AudioManager.Instance.PlaySFX("click");
                selectedBuilding = hit.collider.gameObject;
                ShowPanel(selectedBuilding.transform.position, tag);
                return;
            }
        }

        ClosePanel();
    }

    void ShowPanel(Vector3 position, string tag)
    {
        actionPanel.SetActive(true);

        // موقع البانل في الشاشة (مثبت في الأسفل)
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        actionPanel.transform.position = new Vector3(screenWidth / 2, screenHeight * 0.18f, 0);

        ShowSubPanel(tag);
    }

    void ShowSubPanel(string tag)
    {
        HideAllSubPanels();

        // إيجاد البنل الذي اسمه يساوي التاغ وتفعيله
        foreach (GameObject panel in subPanels)
        {
            if (panel != null && panel.name == tag)
            {
                panel.SetActive(true);
                break;
            }
        }
    }

    void HideAllSubPanels()
    {
        foreach (GameObject panel in subPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    void ClosePanel()
    {
        actionPanel.SetActive(false);
        HideAllSubPanels();
        selectedBuilding = null;
    }

    bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        return EventSystem.current.IsPointerOverGameObject();
    }
}