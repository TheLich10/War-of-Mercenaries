using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class takeResFromUp : MonoBehaviour
{
    public static takeResFromUp instain;
    public TMP_Text WoodRes; // «—»ÿÂ« „‰ «·‹ Inspector
    public TMP_Text StoneRes; // «—»ÿÂ« „‰ «·‹ Inspector
    public TMP_Text PowerRes; // «—»ÿÂ« „‰ «·‹ Inspector
    public TMP_Text IronRes; // «—»ÿÂ« „‰ «·‹ Inspector

    [Header("Time Text")]
    public TMP_Text upgradeTimerText;

    public static int whatPanlShouldTakeFrom = 1;
    private void Awake()
    {
        instain = this;
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

    public void RemovingRes()
    {
        int Wood = ExtractNumberFromText(WoodRes.text);
        int stone = ExtractNumberFromText(StoneRes.text);
        int power = ExtractNumberFromText(PowerRes.text);
        int iron = ExtractNumberFromText(IronRes.text);


        ResourceManager.RemoveWood(Wood);
        ResourceManager.RemoveStone(stone);
        ResourceManager.RemovePower(power);
        ResourceManager.RemoveIron(iron);
        Debug.Log("Removing done W : " + Wood + " S : " + stone + " P: " + power + " I: " + iron);

    }
    public void theInputButton()
    {
        whatPanlShouldTakeFrom = 1;
    }
    public void InputGold()
    {
        takeResFromUp.whatPanlShouldTakeFrom = 0;
    }
}
