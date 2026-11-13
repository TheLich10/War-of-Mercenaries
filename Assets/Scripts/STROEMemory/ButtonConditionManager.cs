using UnityEngine;
using UnityEngine.UI;

public class ButtonConditionManager : MonoBehaviour
{
    [System.Serializable]
    public class ButtonWithImage
    {
        public Button button;  // «·“—
        public Image image;    // «·’Ê—… („„ﬂ‰   —ﬂÂ« ›«÷Ì…)
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

        // „’›Ê›… «·‘—Êÿ „‰ ”ﬂ—»  Conditions
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

            if (buttons[i].button != null)
            {
                //  ›⁄Ì·/ ⁄ÿÌ· «·“— Õ”» «·‘—ÿ
                buttons[i].button.interactable = condition;

                // ‰ Õﬂ„ »«·‹ alpha Õ”» «·‘—ÿ
                Color c = buttons[i].button.image.color;
                c.a = condition ? 1f : 1f;
                buttons[i].button.image.color = c;
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
