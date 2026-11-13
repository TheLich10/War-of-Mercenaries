using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class towBTNforSor : MonoBehaviour
{
    [Header("UI Elements")]
    public Button button1;
    public Button button2;
    public TMP_Text GoldText; // اربطها من الـ Inspector

    private bool firstButtonPressed = false;

    void Start()
    {
        button1.onClick.AddListener(OnButton1Click);
        button2.onClick.AddListener(OnButton2Click);
    }

    void OnButton1Click()
    {
        if (WorkerUI.Instance != null && WorkerUI.Instance.HasAvailableWorker())
        {
            firstButtonPressed = true;

        }

        // تم الضغط على الزر الأول
    }
    public static bool buyGoldSorDone = false;
    public static int goldToRemove = 0;
    void OnButton2Click()
    {
        // تحقق إذا كان الزر الأول قد تم الضغط عليه قبله
        if (firstButtonPressed)
        {
            int gold = ExtractNumberFromText(GoldText != null ? GoldText.text : "0");
            //GoldCollector.RemoveGold(gold);
            goldToRemove = gold;
            //buyGoldSorDone = true;
            firstButtonPressed = false;

        }
        else
        {
            //Debug.Log("No worker in 2GoldBTN");
        }

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
    public static void ResetGoldPurchase()
    {
        buyGoldSorDone = false;
    }

}
