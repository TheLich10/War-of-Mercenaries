using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefenceBuildingInfoUI : MonoBehaviour
{
    public TMP_Text buildingNameText;
    public TMP_Text buildingLevelText;
    public TMP_Text upgradeTimeText;
    public TMP_Text woodRequiredText;
    public TMP_Text stoneRequiredText;
    public TMP_Text ironRequiredText;
    public TMP_Text powerRequiredText;
    public TMP_Text RequiredText;

    public Image buildingIcon;

    // ✅ خصائص قبل الترقية
    public TMP_Text healthBeforeText;
    public TMP_Text damageBeforeText;
    public TMP_Text rangeBeforeText;
    public TMP_Text shotsPerChargetext;

    // ✅ خصائص بعد الترقية
    public TMP_Text healthAfterText;
    public TMP_Text damageAfterText;
    public TMP_Text rangeAfterText;
    public TMP_Text shotsPerChargeAftertext;

    private void Update()
    {
        string buildingName = GetNameAndLvl.objectName;
        int level = GetNameAndLvl.savedNumber;



        if (!string.IsNullOrEmpty(buildingName) && level > 0)
        {
            DefenceBuildData building = DefenceBuilds.GetBuildingByName(buildingName);
            if (building != null)
            {
                DefenceBuildLevelUI currentLevel = building.GetLevel(level);
                DefenceBuildLevelUI nextLevel = building.GetLevel(level + 1);

                if (currentLevel != null)
                {
                    // 🏗️ معلومات الترقية الأساسية
                    buildingNameText.text = "UPGRADE " + building.name + " TO";
                    buildingLevelText.text = (level + 1).ToString();
                    upgradeTimeText.text = nextLevel != null ? nextLevel.timeRequire.ToString() : "-";
                    woodRequiredText.text = nextLevel != null ? nextLevel.woodRequired.ToString() : "-";
                    stoneRequiredText.text = nextLevel != null ? nextLevel.stoneRequired.ToString() : "-";
                    ironRequiredText.text = nextLevel != null ? nextLevel.ironRequired.ToString() : "-";
                    powerRequiredText.text = nextLevel != null ? nextLevel.powerRequired.ToString() : "-";
                    RequiredText.text = nextLevel != null ? nextLevel.requriment.ToString() : "-";

                    if (nextLevel != null && nextLevel.image != null)
                    {
                        buildingIcon.sprite = nextLevel.image;
                        buildingIcon.preserveAspect = true;
                    }



                    // 🔢 الخصائص قبل الترقية
                    healthBeforeText.text = currentLevel.health.ToString();
                    damageBeforeText.text = currentLevel.damage.ToString();
                    rangeBeforeText.text = currentLevel.range.ToString();
                    shotsPerChargetext.text = currentLevel.shotsPerCharge.ToString();

                    // 🔼 الخصائص بعد الترقية
                    if (nextLevel != null)
                    {
                        healthAfterText.text = nextLevel.health.ToString();
                        damageAfterText.text = nextLevel.damage.ToString();
                        rangeAfterText.text = nextLevel.range.ToString();
                        shotsPerChargeAftertext.text = nextLevel.shotsPerCharge.ToString();
                    }
                    else
                    {
                        healthAfterText.text = "-";
                        damageAfterText.text = "-";
                        rangeAfterText.text = "-";
                        shotsPerChargeAftertext.text = "-";
                    }
                }
            }
        }
    }
}
