using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GoldRequirementManager : MonoBehaviour
{
    [System.Serializable]
    public class BuildItemOption
    {
        public Button buildButton;

        [Header("«·–Â» «·„ÿ·Ê» ·Â–« «·»‰«¡")]
        public TMP_Text requiredGoldText;

        [Header("ÌŸÂ— Â–« «·√Ê»Ãﬂ  ⁄‰œ„« ÌﬂÊ‰ «·–Â» €Ì— ﬂ«›Ú")]
        public GameObject insufficientGoldIndicator;
    }

    [Header("ﬁ«∆„… Ã„Ì⁄ √“—«— «·»‰«¡ Ê„ ÿ·»« Â«")]
    public List<BuildItemOption> buildOptions = new List<BuildItemOption>();

    [Header("‰’ «·–Â» «·Õ«·Ì („‰ ResourceManager)")]
    public TMP_Text goldStorageText;

    void Update()
    {
        foreach (var option in buildOptions)
        {
            CheckGold(option);
        }
    }

    void CheckGold(BuildItemOption option)
    {
        Color okColor = Color.white;
        Color failColor = Color.red;

        int requiredGold = ExtractNumberFromText(option.requiredGoldText.text);
        int currentGold = 0;

        // · Ã‰» √Ì Œÿ√ ›Ì Õ«·… ﬂ«‰ «·‰’ ›«—€«
        if (!string.IsNullOrEmpty(goldStorageText.text))
        {
            currentGold = ExtractNumberFromText(goldStorageText.text);
        }

        bool canBuild = true;

        if (requiredGold == 0)
        {
            option.requiredGoldText.color = okColor;
        }
        else if (currentGold < requiredGold)
        {
            option.requiredGoldText.color = failColor;
            canBuild = false; // «·–Â» €Ì— ﬂ«›Ì
        }
        else
        {
            option.requiredGoldText.color = okColor;
        }

        //  ›⁄Ì· √Ê  ⁄ÿÌ· «·“—
        option.buildButton.interactable = canBuild;

        // ≈Œ›«¡ √Ê ≈ŸÂ«— «·√Ê»Ãﬂ  «·Œ«’ »⁄œ„ ﬂ›«Ì… «·–Â»
        if (option.insufficientGoldIndicator != null)
        {
            // ≈–« ﬂ«‰ ·« Ì„ﬂ‰Â «·»‰«¡ (!canBuild) ” ﬂÊ‰ «·ﬁÌ„… true ›ÌŸÂ— «·√Ê»Ãﬂ 
            // Ê≈–« ﬂ«‰ Ì„ﬂ‰Â «·»‰«¡ ” ﬂÊ‰ false ›ÌŒ ›Ì
            option.insufficientGoldIndicator.SetActive(!canBuild);
        }
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