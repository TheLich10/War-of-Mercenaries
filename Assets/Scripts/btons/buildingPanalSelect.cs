using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;

public class buildingPanalSelect : MonoBehaviour
{
    private RectTransform panelRectTransform;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    public GameObject panal; // خاص بتاغ "train"
    public GameObject actionPanel; // الأب الذي يحتوي البنلات
    private GameObject selectedBuilding;

    public GameObject UpgradePanal;
    public GameObject sorPanal;

    // كل البنلات الفرعية داخل actionPanel، اربطها في Inspector حسب الترتيب أو اسحب الأب وسيأخذهم تلقائيًا
    public List<GameObject> subPanels;

    // التاغات المطابقة حسب الترتيب
    private string[] validTags = { "boomOP", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "archer", "archerOP", "flameT", "flayTower", "Mirror", "spy", "spyClan", "boomTower", "Barracks", "BrackClan", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "CaveBrack", "mongel", "wall" };

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
            string objName = hit.collider.gameObject.name;

            // 🔹 التحقق العادي بالـ Tag
            if (validTags.Contains(tag))
            {
                // توقفنا عن فتح البانل هنا، دع السكريبت الجديد يتولى الأمر
                return;
            }
            // 🔹 التحقق إذا الاسم هو "upgrade (1)"
            else if (objName == "upgrade (1)")
            {
                selectedBuilding = hit.collider.gameObject;
                ShowPanel(selectedBuilding.transform.position, objName); // نرسل الاسم بدل التاغ
                UpgradePanal.SetActive(true);

                return;
            }
            else if (objName == "Capsule")
            {
                selectedBuilding = hit.collider.gameObject;
                ShowPanel(selectedBuilding.transform.position, objName); // نرسل الاسم بدل التاغ
                sorPanal.SetActive(true);

                return;
            }
            else if (tag == "train")
            {
                UpgradePanal.SetActive(false);
                panal.SetActive(true);
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
        actionPanel.transform.position = new Vector3(screenWidth / 2, screenHeight * 0.15f, 0);

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
        UpgradePanal.SetActive(false);
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
