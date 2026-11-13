using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OpeningPanalMaker : MonoBehaviour
{
    public Button openPanelButton;

    // «—»ÿ ﬂ· «·»‰·«  „»«‘—… „‰ Inspector
    public GameObject[] panels; // «·’›Ì› ”ÌÕ ÊÌ ⁄·Ï «·»‰·« ° index = ID-1

    public GameObject fanalpanels;
    private GameObject currentPanel;
    private bool isPanelOpen = false;

    void Start()
    {
        openPanelButton.onClick.AddListener(TogglePanel);

        // «€·ﬁ ﬂ· «·»‰·«  ›Ì «·»œ«Ì…
        foreach (GameObject panel in panels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    void Update()
    {
        if (isPanelOpen && Input.GetMouseButtonDown(0))
        {
            GameObject clickedObject = GetClickedUIObject();

            if (clickedObject != null &&
                clickedObject != openPanelButton.gameObject &&
                !IsChildOf(clickedObject, currentPanel))
            {
                ClosePanel();
            }

            if (clickedObject == null)
            {
                ClosePanel();
            }
        }
    }

    void TogglePanel()
    {
        if (!isPanelOpen)
        {
            //  Õﬁﬁ √Ê·« ≈–« ﬂ«‰ Â‰«ﬂ √Ì ﬂ«∆‰ »«· «€ "fourMaker"
            GameObject fourMakerObj = GameObject.FindGameObjectWithTag("fourMaker");
            if (fourMakerObj != null)
            {
                // ≈–« ÊÃœ° ‰› Õ always «·‹ fanalpanels
                currentPanel = fanalpanels;
                currentPanel.SetActive(true);
                isPanelOpen = true;
                return; // ‰Œ—Ã „»«‘—… »œÊ‰ »«ﬁÌ «·ﬂÊœ
            }

            // «·ﬂÊœ «·ﬁœÌ„ »œÊ‰  €ÌÌ—
            if (BuildingSelector.targetBuilding != null)
            {
                BuildingID buildingIDScript = BuildingSelector.targetBuilding.GetComponent<BuildingID>();

                if (buildingIDScript != null)
                {
                    int id = buildingIDScript.buildingID;

                    // ID Ì»œ√ „‰ 1° √„« array index Ì»œ√ „‰ 0
                    if (id - 1 >= 0 && id - 1 < panels.Length)
                    {
                        currentPanel = panels[id - 1];
                        currentPanel.SetActive(true);
                        isPanelOpen = true;
                    }
                    else
                    {
                        Debug.LogWarning("·« ÌÊÃœ »‰· „⁄—› ·Â–« «·‹ ID: " + id);
                    }
                }
                else
                {
                    Debug.LogWarning("«·„»‰Ï ·« ÌÕ ÊÌ ⁄·Ï BuildingID script!");
                }
            }
            else
            {
                Debug.LogWarning("·„ Ì „  ÕœÌœ √Ì „»‰Ï!");
            }
        }
        else
        {
            ClosePanel();
        }
    }


    void ClosePanel()
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);

        isPanelOpen = false;
        currentPanel = null;
    }

    GameObject GetClickedUIObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0 ? results[0].gameObject : null;
    }

    bool IsChildOf(GameObject child, GameObject parent)
    {
        Transform t = child.transform;
        while (t != null)
        {
            if (t.gameObject == parent)
                return true;
            t = t.parent;
        }
        return false;
    }
}
