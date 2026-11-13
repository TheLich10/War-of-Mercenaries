using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class SelectorSPY : MonoBehaviour
{
    private RectTransform panelRectTransform;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    public GameObject actionPanel;
    private GameObject selectedBuilding;
    public TMP_Text caveLevelText; // ✅ نص لعرض مستوى CaveBrack


    void Start()
    {
        actionPanel.SetActive(false);
        panelRectTransform = actionPanel.GetComponent<RectTransform>();
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

        var results = new System.Collections.Generic.List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == null)
                continue;

            if (result.gameObject.transform.IsChildOf(actionPanel.transform))
                return true;
        }

        return false;
    }

    void DetectBuilding()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        string[] validTags = { "boomOP","archer", "archerOP", "flameT", "flayTower", "Mirror", "spy", "spyClan", "boomTower"};

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        // 🔹 إذا لم يصطدم بأي مبنى، أغلق اللوحة
        if (hit.collider == null)
        {
            ClosePanel();
            return;
        }

        if (validTags.Contains(hit.collider.tag))
        {
            if (selectedBuilding != null) SetTransparency(selectedBuilding, 1f);

            selectedBuilding = hit.collider.gameObject;
            SetTransparency(selectedBuilding, 0.8f);

            ShowPanel(selectedBuilding.transform.position);

            // تحديث بيانات الموارد والوحوش بناءً على المبنى المحدد
            
            if (hit.collider.CompareTag("spy"))
            {
                BarracksWatchPost watchPost = hit.collider.GetComponent<BarracksWatchPost>();
                if (watchPost != null)
                {
                    //UiAllBars.Instance.SetWatchPost(watchPost);
                    //WatchPostUIManager.Instance.SetWatchPost(watchPost);
                }
            }
            building targetBuilding = hit.collider.GetComponent<building>();
            if (targetBuilding != null)
            {
                int trageLevel = targetBuilding.level; // أخذ مستوى المبنى
                //Debug.Log("target building level: " + trageLevel); ديبك
                caveLevelText.text = trageLevel.ToString();
            }
            else
            {
                Debug.LogWarning("The object with tag 'wood' does not have a 'building' component!");
            }
        }
        else
        {
            ClosePanel();
        }
    }

    void ShowPanel(Vector3 position)
    {
        actionPanel.SetActive(true);

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        actionPanel.transform.position = new Vector3(screenWidth / 2, screenHeight * 0.22f, 0);
    }

    void ClosePanel()
    {
        actionPanel.SetActive(false);
        if (selectedBuilding != null)
        {
            SetTransparency(selectedBuilding, 1f);
        }

        selectedBuilding = null;
    }

    void SetTransparency(GameObject target, float alpha)
    {
        foreach (var renderer in target.GetComponentsInChildren<SpriteRenderer>())
        {
            // 👇 إضافة هذا الشرط لتخطي العنصر الذي يحمل اسم الظل
            if (renderer.gameObject.name == "shod" || renderer.gameObject.name == "smoke")
            {
                continue; // الانتقال للعنصر التالي دون تغيير شفافية الظل
            }

            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }
}