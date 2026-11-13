using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonPanelController : MonoBehaviour
{
    [Header("Botns")]
    public List<Button> clickableButtons; // ← مرنة، تدعم أي عدد من الأزرار

    [Header("display botn")]
    public Button buttonToDisable;
    public Button buttonToDisable2;
    public Button buttonToDisable3;


    [Header("panlClose")]
    public GameObject panel;

    void Start()
    {
        foreach (Button btn in clickableButtons)
        {
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnButtonClicked());
            }
        }
    }

    void OnButtonClicked()
    {
        if (panel != null)
            panel.SetActive(false);

        if (buttonToDisable != null)
            buttonToDisable.interactable = false;

        if (buttonToDisable2 != null)
            buttonToDisable2.interactable = false;

        if (buttonToDisable3 != null)
            buttonToDisable3.interactable = false;

    }
}
