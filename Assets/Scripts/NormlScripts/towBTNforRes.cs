using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowBTNforRes : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] firstButtons;   // „Ã„Ê⁄… «·√“—«— «·√Ê·Ï
    public Button secondButton;     // “— „‘ —ﬂ ··Ã„Ì⁄

    public TMP_Text WoodRes;
    public TMP_Text StoneRes;
    public TMP_Text PowerRes;
    public TMP_Text IronRes;

    private bool firstButtonPressed = false;

    void Start()
    {
        // —»ÿ Ã„Ì⁄ «·√“—«— «·√Ê·Ï
        foreach (var btn in firstButtons)
        {
            btn.onClick.AddListener(OnFirstButtonClick);
        }

        // —»ÿ «·“— «·À«‰Ì «·„‘ —ﬂ
        if (secondButton != null)
            secondButton.onClick.AddListener(OnSecondButtonClick);
    }

    void OnFirstButtonClick()
    {
        firstButtonPressed = true;
    }

    void OnSecondButtonClick()
    {
        if (WorkerUI.Instance != null && WorkerUI.Instance.HasAvailableWorker())
        {
            if (GoldBuildButton.ItsGoldBtn == false)
            {
                if (firstButtonPressed)
                {
                    int wood = ExtractNumberFromText(WoodRes.text != null ? WoodRes.text : "0");
                    int stone = ExtractNumberFromText(StoneRes.text != null ? StoneRes.text : "0");
                    int power = ExtractNumberFromText(PowerRes.text != null ? PowerRes.text : "0");
                    int iron = ExtractNumberFromText(IronRes.text != null ? IronRes.text : "0");

                    ResourceManager.RemoveWood(wood);
                    ResourceManager.RemoveStone(stone);
                    ResourceManager.RemovePower(power);
                    ResourceManager.RemoveIron(iron);

                    Debug.Log($"Removing done W: {wood} S: {stone} P: {power} I: {iron}");

                    // ≈⁄«œ… «·÷»ÿ
                    firstButtonPressed = false;
                }
            }

        }
        else Debug.Log("No Workeers Here in BTN2");


    }

    int ExtractNumberFromText(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        string digitsOnly = "";
        foreach (char c in text)
        {
            if (char.IsDigit(c))
                digitsOnly += c;
        }

        if (string.IsNullOrEmpty(digitsOnly)) return 0;

        return int.Parse(digitsOnly);
    }
}
