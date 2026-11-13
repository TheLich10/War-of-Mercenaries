using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceButtonController : MonoBehaviour
{
    [Header("√“—«— «·Œ‘»")]
    public Button woodButton1;
    public Button woodButton2;
    public Button woodButton3;

    [Header("√“—«— «·ÕÃ—")]
    public Button stoneButton1;
    public Button stoneButton2;
    public Button stoneButton3;
    public Button stoneButton4;

    [Header("√“—«— «·ÿ«ﬁ…")]
    public Button powerButton1;
    public Button powerButton2;
    public Button powerButton3;
    public Button powerButton4;

    [Header("‰’Ê’ «·√“—«—")]
    public TMP_Text[] woodButtonTexts;
    public TMP_Text[] stoneButtonTexts;
    public TMP_Text[] powerButtonTexts;

    private void Start()
    {
        CheckAndUpdateButtons();

    }

    private void Update()
    {
       
    }

    void CheckAndUpdateButtons()
    {
        int currentWood = GetIntFromText(ResourceManager.Instance.woodText);
        int currentStone = GetIntFromText(ResourceManager.Instance.stoneText);
        int currentPower = GetIntFromText(ResourceManager.Instance.powerText);

        CheckButtons(currentWood,
                     new Button[] { woodButton1, woodButton2, woodButton3 },
                     woodButtonTexts,
                     new int[] { 10000, 100000, 500000 });

        CheckButtons(currentStone,
                     new Button[] { stoneButton1, stoneButton2, stoneButton3, stoneButton4 },
                     stoneButtonTexts,
                     new int[] { 10000, 100000, 500000, 1000000 });

        CheckButtons(currentPower,
                     new Button[] { powerButton1, powerButton2, powerButton3, powerButton4 },
                     powerButtonTexts,
                     new int[] { 10000, 100000, 500000, 1000000 });
    }

    void CheckButtons(int currentValue, Button[] buttons, TMP_Text[] texts, int[] requiredValues)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < requiredValues.Length && currentValue >= requiredValues[i])
            {
                buttons[i].interactable = true;
                texts[i].color = Color.white;
            }
            else
            {
                buttons[i].interactable = false;
                texts[i].color = Color.red;
            }
        }
    }

    int GetIntFromText(TMP_Text text)
    {
        int result;
        if (int.TryParse(text.text, out result))
            return result;
        else
            return 0;
    }
}
