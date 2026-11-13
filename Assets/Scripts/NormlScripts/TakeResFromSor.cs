using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TakeResFromSor : MonoBehaviour
{
    public static TakeResFromSor instain;

    public TMP_Text WoodRes; // اربطها من الـ Inspector
    public TMP_Text StoneRes; // اربطها من الـ Inspector
    public TMP_Text PowerRes; // اربطها من الـ Inspector
    public TMP_Text IronRes; // اربطها من الـ Inspector

    [Header("Time Text")]
    public TMP_Text upgradeTimerText;

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
        if (towBTNforSor.buyGoldSorDone)
            return; // ❌ لا تخصم موارد إذا الشراء ذهب

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
}
