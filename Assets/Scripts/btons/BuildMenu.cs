using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{
    public GameObject buildPanel; // «·ﬁ«∆„… «·Œ«’… »«·»‰«¡
    public Button buildButton; // «·“— «·—∆Ì”Ì ·› Õ «·ﬁ«∆„…

    public Button defensesButton; // “— «·œ›«⁄« 
    public Button resourcesButton; // “— «·„Ê«—œ
    public Button decorationsButton; // “— «·“Ì‰…
    public Button functionalbutton; // “— «·“Ì‰…


    public GameObject defensesPanel; // ﬁ«∆„… «·œ›«⁄« 
    public GameObject resourcesPanel; // ﬁ«∆„… «·„Ê«—œ
    public GameObject decorationsPanel; // ﬁ«∆„… «·“Ì‰…
    public GameObject functionalpanal; // “— «·“Ì‰…


    void Start()
    {
        buildPanel.SetActive(false); // ≈Œ›«¡ «·ﬁ«∆„… ⁄‰œ »œ¡ «··⁄»…

        buildButton.onClick.AddListener(ToggleBuildMenu);
        defensesButton.onClick.AddListener(() => OpenCategory(defensesPanel));
        resourcesButton.onClick.AddListener(() => OpenCategory(resourcesPanel));
        decorationsButton.onClick.AddListener(() => OpenCategory(decorationsPanel));
        functionalbutton.onClick.AddListener(() => OpenCategory(functionalpanal));



        CloseAllCategories(); // ≈Œ›«¡ Ã„Ì⁄ «·›∆«  ⁄‰œ «·»œ«Ì…
    }

    void ToggleBuildMenu()
    {
        bool isActive = !buildPanel.activeSelf;
        buildPanel.SetActive(isActive);

        if (isActive)
        {
            OpenCategory(resourcesPanel); // › Õ ﬁ«∆„… «·„Ê«—œ ⁄‰œ › Õ «·ﬁ«∆„…
        }
        else
        {
            CloseAllCategories(); // ≈€·«ﬁ «·ﬁÊ«∆„ ⁄‰œ ≈€·«ﬁ «·ﬁ«∆„…
        }
    }


    void OpenCategory(GameObject categoryPanel)
    {
        CloseAllCategories();
        categoryPanel.SetActive(true);
    }

    void CloseAllCategories()
    {
        defensesPanel.SetActive(false);
        resourcesPanel.SetActive(false);
        decorationsPanel.SetActive(false);
        functionalpanal.SetActive(false);
    }
}
