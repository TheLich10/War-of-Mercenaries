using UnityEngine;
using UnityEngine.UI;

public class ButtonReplaceConditionManager : MonoBehaviour
{
    [System.Serializable]
    public class ButtonWithImage
    {
        public Button originalButton;       // «·“— «·√’·Ì
        public Button replacementButton;    // «·“— «·»œÌ· („« Ì”ÊÌ ‘Ì)
        public Image image;                 // ’Ê—… ≈÷«›Ì… («Œ Ì«—Ì)
    }

    [Header("«·√“—«— + «·’Ê—")]
    public ButtonWithImage[] buttons = new ButtonWithImage[14];

    void Update()
    {
        UpdateButtons();
    }

    void UpdateButtons()
    {
        if (openNewWorior.instain == null) return;

        // „’›Ê›… «·‘—Êÿ „‰ ”ﬂ—»  openNewWorior
        bool[] conds = new bool[14]
        {
            openNewWorior.OtomanIsOpen,
            openNewWorior.KnightIsOpen,
            openNewWorior.PersonIsOpen,
            openNewWorior.KamkaziIsOpen,
            openNewWorior.SavgeIsOpen,
            openNewWorior.DestroyIsOpen,
            openNewWorior.PositionIsOpen,
            openNewWorior.SneakyIsOpen,
            openNewWorior.MongaleIsOpen,
            openNewWorior.EgypetIsOpen,
            openNewWorior.HealerIsOpen,
            openNewWorior.SapperIsOpen,
            openNewWorior.FlayingIsOpen,
            openNewWorior.GaulIsOpen,
        };

        for (int i = 0; i < buttons.Length; i++)
        {
            bool condition = conds[i];

            if (buttons[i].originalButton != null && buttons[i].replacementButton != null)
            {
                // ‰‰”Œ „ﬂ«‰ ÊÕÃ„ «·“— «·√’·Ì ··“— «·»œÌ·
                RectTransform origRect = buttons[i].originalButton.GetComponent<RectTransform>();
                RectTransform replRect = buttons[i].replacementButton.GetComponent<RectTransform>();

                replRect.position = origRect.position;
                replRect.rotation = origRect.rotation;
                replRect.sizeDelta = origRect.sizeDelta;
                replRect.anchorMin = origRect.anchorMin;
                replRect.anchorMax = origRect.anchorMax;
                replRect.pivot = origRect.pivot;

                //  ›⁄Ì· «·√’·Ì √Ê «·»œÌ· Õ”» «·‘—ÿ
                buttons[i].originalButton.gameObject.SetActive(condition);
                buttons[i].replacementButton.gameObject.SetActive(!condition);
            }

            if (buttons[i].image != null)
            {
                Color c = buttons[i].image.color;
                c.a = condition ? 1f : 0.8f;
                buttons[i].image.color = c;
            }
        }
    }
}