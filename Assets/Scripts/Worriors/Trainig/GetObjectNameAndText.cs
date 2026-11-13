using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GetObjectNameAndText : MonoBehaviour
{
    public Button closepanalMakeWorrior;
    public GameObject panal;
    public TMP_Text linkedText; //  √ﬂœ „‰ —»ÿ Â–« «·‰’ »‘ﬂ· ’ÕÌÕ

    public static string savedText;     // «·‰’ ﬂ„« ÂÊ
    public static int savedNumber;      // «·—ﬁ„ «·„ÕÊ¯·
    public static string objectName;

    private bool isPanelOpen = false;

    void Start()
    {
        closepanalMakeWorrior.onClick.AddListener(TogglePanel);

        objectName = gameObject.name;
    }

    void Update()
    {
        if (isPanelOpen && Input.GetMouseButtonDown(0))
        {
            GameObject clickedObject = GetClickedUIObject();

            if (clickedObject != null &&
                clickedObject != closepanalMakeWorrior.gameObject &&
                !IsChildOf(clickedObject, panal))
            {
                ClosePanel();
            }

            //if (clickedObject == null)
            //{
            //    ClosePanel();
            //}
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

    // œ«·… · ÕœÌÀ «·‰’ Ê«·—ﬁ„ ⁄‰œ «·‰ﬁ— ⁄·Ï “— «·„Õ«—»
    public void OnWarriorButtonClick()
    {
        //  ÕœÌÀ «·«”„ Ê«·‰’
        if (linkedText != null)
        {
            savedText = linkedText.text;  // Õ›Ÿ «·‰’ «·„ÊÃÊœ ›Ì «·‹ TMP_Text
            if (int.TryParse(savedText, out int level))
            {
                savedNumber = level;
            }
        }

        // Õ›Ÿ «”„ «·„Õ«—» ›Ì «·„ €Ì— «·À«» 
        objectName = gameObject.name;  //  √ﬂœ „‰ √‰ «”„ «·„Õ«—» Ì „  ÕœÌÀÂ Â‰«
        Debug.Log(objectName + " ------ " +  savedNumber);
        // › Õ «·‹ Panel ·⁄—÷ «· ›«’Ì·
        panal.SetActive(true);
    }

}
