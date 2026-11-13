using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowButtonForGold : MonoBehaviour
{
    [Header("UI Elements")]
    public Button button1;
    public Button button2;
    public Button NObtn;

    public TMP_Text GoldText; // «—»ÿÂ« „‰ «·‹ Inspector

    private bool firstButtonPressed = false;

    void Start()
    {
        button1.onClick.AddListener(OnButton1Click);
        button2.onClick.AddListener(OnButton2Click);
        NObtn.onClick.AddListener(OnButtonNOClick);

    }

    void OnButton1Click()
    {
        if (WorkerUI.Instance != null && WorkerUI.Instance.HasAvailableWorker())
        {
            firstButtonPressed = true;
        }

        //  „ «·÷€ÿ ⁄·Ï «·“— «·√Ê·
    }

    private static int gold = 0;
    void OnButton2Click()
    {
        //  Õﬁﬁ ≈–« ﬂ«‰ «·“— «·√Ê· ﬁœ  „ «·÷€ÿ ⁄·ÌÂ ﬁ»·Â
        if (firstButtonPressed)
        {
                gold = ExtractNumberFromText(GoldText != null ? GoldText.text : "0");
                GoldCollector.RemoveGold(gold);
                // ≈⁄«œ… «·÷»ÿ Õ Ï Ì⁄«œ «·‘—ÿ „—… √Œ—Ï
                firstButtonPressed = false;

        }
        else
        {
            //Debug.Log("No worker in 2GoldBTN");
        }

    }
    void OnButtonNOClick()
    {
        firstButtonPressed = false;

        gold = 0;
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
