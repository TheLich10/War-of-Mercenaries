using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class openNewBuild : MonoBehaviour
{
    public Button switchButton;
    public GameObject panelOpen;
    public GameObject panelClose;

    //public GameObject panel1; // «·»‰· «·√Ê·
    ////public GameObject panel2; // «·»‰· «·À«‰Ì
    //public Button switchButton; // «·“—

    private void Start()
    {
        //  √ﬂœ √‰ «·“— „ ’· ÊÌ÷Ì› «·ÕœÀ
        switchButton.onClick.AddListener(SwitchPanel);
    }

    void SwitchPanel()
    {
        panelClose.SetActive(false); // «€·ﬁ «·»‰· «·√Ê·
        panelOpen.SetActive(true);  // «› Õ «·»‰· «·À«‰Ì
    }


    //private bool isPanelOpen = false;

    //void Start()
    //{
    //    Opening.onClick.AddListener(TogglePanel);
    //}

    //void Update()
    //{
    //    if (isPanelOpen && Input.GetMouseButtonDown(0))
    //    {
    //        GameObject clickedObject = GetClickedUIObject();

    //        // ≈–« ·„ Ì „ «·‰ﬁ— ⁄·Ï «·“— √Ê ⁄·Ï √Ì ‘Ì¡ œ«Œ· «·»‰·
    //        if (clickedObject != null &&
    //            clickedObject != Opening.gameObject &&
    //            !IsChildOf(clickedObject, panal))
    //        {
    //            ClosePanel();
    //        }

    //        // ≈–« ·„ Ì „ «·‰ﬁ— ⁄·Ï √Ì ‘Ì¡ („ﬂ«‰ ›«—€ ›Ì «·‘«‘…)
    //        if (clickedObject == null)
    //        {
    //            ClosePanel();
    //        }
    //    }
    //}

    //void TogglePanel()
    //{
    //    isPanelOpen = !isPanelOpen;
    //    Canselpanal.SetActive(false);
    //    panal.SetActive(isPanelOpen);
    //}

    //void ClosePanel()
    //{
    //    isPanelOpen = false;
    //    panal.SetActive(false);
    //}

    //GameObject GetClickedUIObject()
    //{
    //    PointerEventData pointerData = new PointerEventData(EventSystem.current)
    //    {
    //        position = Input.mousePosition
    //    };

    //    var results = new System.Collections.Generic.List<RaycastResult>();
    //    EventSystem.current.RaycastAll(pointerData, results);

    //    return results.Count > 0 ? results[0].gameObject : null;
    //}

    //// œ«·…   Õﬁﬁ „„« ≈–« ﬂ«‰ «·⁄‰’— ÷„‰ √»‰«¡ «·»‰·
    //bool IsChildOf(GameObject child, GameObject parent)
    //{
    //    Transform t = child.transform;
    //    while (t != null)
    //    {
    //        if (t.gameObject == parent)
    //            return true;
    //        t = t.parent;
    //    }
    //    return false;
    //}
}
