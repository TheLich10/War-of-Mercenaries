using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildPanelCheckerPowerOnly : MonoBehaviour
{
    [System.Serializable]
    public class BuildOption
    {
        public Button buildButton;

        [Header("«·ÿ«ﬁ… «·„ÿ·Ê»… ·Â–« «·»‰«¡")]
        public TMP_Text requiredPowerText;
    }

    [Header("ﬁ«∆„… √“—«— «·»‰«¡")]
    public List<BuildOption> buildOptions = new List<BuildOption>();

    [Header("‰’ «·ÿ«ﬁ… «·Õ«·Ì…")]
    public TMP_Text powerStorageText;

    void Update()
    {
        foreach (var option in buildOptions)
        {
            CheckPower(option);
        }
    }

    void CheckPower(BuildOption option)
    {
        Color okColor = Color.white;
        Color failColor = Color.red;
        Color fadedColor = new Color(1f, 1f, 1f, 0.5f);

        int requiredPower = int.Parse(option.requiredPowerText.text);
        int currentPower = int.Parse(powerStorageText.text);

        bool canBuild = true;

        if (requiredPower == 0)
        {
            option.requiredPowerText.color = fadedColor;
        }
        else if (currentPower < requiredPower)
        {
            option.requiredPowerText.color = failColor;
            canBuild = false;
        }
        else
        {
            option.requiredPowerText.color = okColor;
        }

        option.buildButton.interactable = canBuild;
    }
}
