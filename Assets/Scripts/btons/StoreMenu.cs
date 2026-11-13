using UnityEngine;
using UnityEngine.UI;

public class StoreMenu : MonoBehaviour
{
    [Header("الأزرار والبنلات الرئيسية")]
    public GameObject mainPanel;
    public Button openStoreMainButton;
    public Button storeButton;
    public Button inventoryButton;

    [Header("بنل المتجر")]
    public GameObject storePanel;
    public Button[] storeButtons;
    public GameObject[] storeSubPanels;

    [Header("بنل المخزن")]
    public GameObject inventoryPanel;
    public Button[] inventoryButtons;
    public GameObject[] inventorySubPanels;

    void Start()
    {
        mainPanel.SetActive(false);
        storePanel.SetActive(false);
        inventoryPanel.SetActive(false);

        openStoreMainButton.onClick.AddListener(ToggleMainPanel);
        storeButton.onClick.AddListener(OpenStorePanel);
        inventoryButton.onClick.AddListener(OpenInventoryPanel);

        SetupButtons(storeButtons, storeSubPanels);
        SetupButtons(inventoryButtons, inventorySubPanels);

        CloseAllSubPanels();
    }

    void ToggleMainPanel()
    {
        bool isActive = !mainPanel.activeSelf;
        mainPanel.SetActive(isActive);

        if (isActive)
        {
            OpenStorePanel();      // ✅ فتح المتجر افتراضياً
            OpenStoreDefault();    // ✅ فتح أول بنل فرعي
        }
        else
        {
            CloseAllSubPanels();
        }
    }

    void OpenStorePanel()
    {
        storePanel.SetActive(true);
        inventoryPanel.SetActive(false);
        CloseAllSubPanels();
        OpenFirstStoreSubPanel();
    }

    void OpenInventoryPanel()
    {
        inventoryPanel.SetActive(true);
        storePanel.SetActive(false);
        CloseAllSubPanels();
        OpenFirstInventorySubPanel();
    }

    void OpenStoreDefault()
    {
        if (storeButtons.Length > 0 && storeSubPanels.Length > 0)
        {
            storeSubPanels[0].SetActive(true);
        }
    }

    void SetupButtons(Button[] buttons, GameObject[] panels)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() =>
            {
                CloseAllSubPanels();
                panels[index].SetActive(true);
            });
        }
    }

    void CloseAllSubPanels()
    {
        foreach (var panel in storeSubPanels)
            panel.SetActive(false);
        foreach (var panel in inventorySubPanels)
            panel.SetActive(false);
    }

    void OpenFirstStoreSubPanel()
    {
        if (storeButtons.Length > 0 && storeSubPanels.Length > 0)
        {
            storeSubPanels[0].SetActive(true);
        }
    }

    void OpenFirstInventorySubPanel()
    {
        if (inventoryButtons.Length > 0 && inventorySubPanels.Length > 0)
        {
            inventorySubPanels[0].SetActive(true);
        }
    }
}