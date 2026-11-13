using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GoldPanelChecker : MonoBehaviour
{
    [System.Serializable]
    public class GoldBuildOption
    {
        public Button buildButton;

        [Header("«·–Â» «·„ÿ·Ê» ·Â–« «·»‰«¡")]
        public TMP_Text requiredGoldText;
    }

    [Header("ﬁ«∆„… Ã„Ì⁄ √“—«— «·»‰«¡ Ê„ ÿ·»«  «·–Â» «·Œ«’… »Â«")]
    public List<GoldBuildOption> goldBuildOptions = new List<GoldBuildOption>();

    [Header("‰’ «·–Â» «·Õ«·Ì („‰ ResourceManager)")]
    public TMP_Text goldStorageText;

    void Update()
    {
        foreach (var option in goldBuildOptions)
        {
            CheckGold(option);
        }
    }

    void CheckGold(GoldBuildOption option)
    {
        Color okColor = Color.white;
        Color failColor = Color.red;
        Color fadedColor = new Color(1f, 1f, 1f, 0.5f);

        int requiredGold = ExtractNumberFromText(option.requiredGoldText.text);
        int currentGold = int.Parse(goldStorageText.text);

        bool canBuild = true;

        if (requiredGold == 0)
        {
            option.requiredGoldText.color = new Color(1f, 1f, 1f, 1f); ;
        }
        else if (currentGold < requiredGold)
        {
            option.requiredGoldText.color = failColor;
            canBuild = false;
        }
        else
        {
            option.requiredGoldText.color = okColor;
        }

        option.buildButton.interactable = canBuild;
    }

    int ExtractNumberFromText(string text)
    {
        string digitsOnly = "";
        foreach (char c in text)
        {
            if (char.IsDigit(c))
                digitsOnly += c;
        }

        if (string.IsNullOrEmpty(digitsOnly))
            return 0;

        return int.Parse(digitsOnly);
    }
}
