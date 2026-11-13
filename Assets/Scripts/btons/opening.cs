using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class opening : MonoBehaviour
{
    public Button closepanalMakeWorrior;
    public GameObject panal;

    private bool isPanelOpen = false;

    void Start()
    {
        closepanalMakeWorrior.onClick.AddListener(TogglePanel);
    }

    void Update()
    {
        if (isPanelOpen && Input.GetMouseButtonDown(0))
        {
            GameObject clickedObject = GetClickedUIObject();

            // ≈–« ·„ Ì „ «·‰ﬁ— ⁄·Ï «·“— √Ê ⁄·Ï √Ì ‘Ì¡ œ«Œ· «·»‰·
            if (clickedObject != null &&
                clickedObject != closepanalMakeWorrior.gameObject &&
                !IsChildOf(clickedObject, panal))
            {
                ClosePanel();
            }

            // ≈–« ·„ Ì „ «·‰ﬁ— ⁄·Ï √Ì ‘Ì¡ („ﬂ«‰ ›«—€ ›Ì «·‘«‘…)
            if (clickedObject == null)
            {
                ClosePanel();
            }
        }
    }

    void TogglePanel()
    {
        isPanelOpen = !isPanelOpen;
        panal.SetActive(isPanelOpen);
    }

    void ClosePanel()
    {
        isPanelOpen = false;
        panal.SetActive(false);
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

    // œ«·…   Õﬁﬁ „„« ≈–« ﬂ«‰ «·⁄‰’— ÷„‰ √»‰«¡ «·»‰·
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
