using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelOpener : MonoBehaviour
{
    [Header("“— «·› Õ")]
    public Button openButton;

    [Header("«·»«‰· «·–Ì  —Ìœ ≈ŸÂ«—Â")]
    public GameObject panel;

    void Start()
    {
        panel.SetActive(false); // ≈Œ›«¡ «·»«‰· »«·»œ«Ì…

        openButton.onClick.AddListener(OpenPanel);
    }

    void Update()
    {
        // ≈–« ﬂ«‰ «·»«‰· „›⁄¯· Ê«·‰ﬁ— ⁄·Ï «·‘«‘…
        if (panel.activeSelf && Input.GetMouseButtonDown(0))
        {
            //  Õﬁﬁ Â· «·‰ﬁ—… ›Êﬁ √Ì UI element
            if (!IsPointerOverUI(panel))
            {
                panel.SetActive(false);
            }
        }
    }

    void OpenPanel()
    {
        panel.SetActive(true);
    }

    // œ«·… ·›Õ’ ≈–« «·„ƒ‘— ›Êﬁ «·»«‰· √Ê √Õœ √ÿ›«·Â
    bool IsPointerOverUI(GameObject panelObject)
    {
        // „ﬂ«‰ «·‰ﬁ—…
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        // «·»ÕÀ ⁄‰ «·⁄‰«’—  Õ  «·„«Ê”
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // «· Õﬁﬁ ≈–« ﬂ«‰ √Ì ⁄‰’— „‰ «·‰ «∆Ã ÂÊ œ«Œ· «·»«‰·
        foreach (var result in results)
        {
            if (result.gameObject == panelObject || result.gameObject.transform.IsChildOf(panelObject.transform))
                return true;
        }

        return false;
    }
}
