using TMPro;
using UnityEngine;

public class WarriorIronRequirementUI : MonoBehaviour
{
    [Header("‰’Ê’ TMP ·ﬂ· „Õ«—»")]
    public TMP_Text bedoiunText;
    public TMP_Text scalingEgyptianText;
    public TMP_Text destroyerText;
    public TMP_Text flyingOttomanText;
    public TMP_Text gaulText;
    public TMP_Text helaerText;
    public TMP_Text kamikazeText;
    public TMP_Text knightText;
    public TMP_Text mongolianText;
    public TMP_Text ottmanText;
    public TMP_Text persianText;
    public TMP_Text postionText;
    public TMP_Text sapperText;
    public TMP_Text savageText;
    public TMP_Text sneakyText;

    [Header("LEVELS")]
    public TMP_Text bedoiunTextlvl;
    public TMP_Text scalingEgyptianTextlvl;
    public TMP_Text destroyerTextlvl;
    public TMP_Text flyingOttomanTextlvl;
    public TMP_Text gaulTextlvl;
    public TMP_Text helaerTextlvl;
    public TMP_Text kamikazeTextlvl;
    public TMP_Text knightTextlvl;
    public TMP_Text mongolianTextlvl;
    public TMP_Text ottmanTextlvl;
    public TMP_Text persianTextlvl;
    public TMP_Text postionTextlvl;
    public TMP_Text sapperTextlvl;
    public TMP_Text savageTextlvl;
    public TMP_Text sneakyTextlvl;

    void Start()
    {
        UpdateIronTexts();
    }

    void UpdateIronTexts()
    {
        SetIronText("bedoiun", bedoiunTextlvl, bedoiunText);
        SetIronText("ScalingEgyptian", scalingEgyptianTextlvl, scalingEgyptianText);
        SetIronText("Destroyer", destroyerTextlvl, destroyerText);
        SetIronText("flyingottoman", flyingOttomanTextlvl, flyingOttomanText);
        SetIronText("Battering_Gaul", gaulTextlvl, gaulText);
        SetIronText("GentleHealerFull", helaerTextlvl, helaerText);
        SetIronText("Kamikaze", kamikazeTextlvl, kamikazeText);
        SetIronText("KnightOnHorse", knightTextlvl, knightText);
        SetIronText("ChineseMongolian", mongolianTextlvl, mongolianText);
        SetIronText("Ottoman", ottmanTextlvl, ottmanText);
        SetIronText("persian", persianTextlvl, persianText);
        SetIronText("Poisonous", postionTextlvl, postionText);
        SetIronText("PersianSapper", sapperTextlvl, sapperText);
        SetIronText("EgyptianSavage", savageTextlvl, savageText);
        SetIronText("Sneaky", sneakyTextlvl, sneakyText);
    }

    void SetIronText(string warriorName, TMP_Text levelText, TMP_Text ironText)
    {
        if (int.TryParse(levelText.text, out int level))
        {
            WarriorTring warrior = WarriorTringDatabase.GetWarriorByName(warriorName);
            if (warrior != null)
            {
                WarriorLevel data = warrior.GetLevel(level);
                if (data != null)
                {
                    ironText.text = data.recruitCost.ToString();
                }
                else
                {
                    ironText.text = "€Ì— „ÊÃÊœ";
                }
            }
            else
            {
                ironText.text = "„Õ«—» €Ì— „⁄—Ê›";
            }
        }
        else
        {
            ironText.text = "Œÿ√ ›Ì «·„” ÊÏ";
        }
    }
}
