using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoUI_V2 : MonoBehaviour
{
    public TMP_Text buildingNameText;
    public TMP_Text buildingLevelText;
    public TMP_Text upgradeTimeText;
    public TMP_Text woodRequiredText;
    public TMP_Text stoneRequiredText;
    public TMP_Text ironRequiredText;
    public TMP_Text powerRequiredText;
    public Image buildingIcon;

    public TMP_Text openText1After;
    public TMP_Text openText2After;
    public TMP_Text openText3After;
    public TMP_Text requrimetTxt;

    public GameObject Text2;
    public GameObject Text3;



    private void Update()
    {
        string buildingName = GetNameAndLvl.objectName;
        int level = GetNameAndLvl.savedNumber;

        if (buildingName == "RECRUITMENT CHAMBER" || buildingName == "TRAINGING CHAMBER" || buildingName == "GATAPULT") { Text2.SetActive(false); Text3.SetActive(false); }
        else { Text2.SetActive(true); Text3.SetActive(true); }

        if (!string.IsNullOrEmpty(buildingName) && level > 0)
        {
            BuildingInfo building = BuildingInfoDatabase.GetBuildingByName(buildingName);
            if (building != null)
            {
                BuildingLevelInfo currentLevel = building.GetLevel(level);
                BuildingLevelInfo nextLevel = building.GetLevel(level + 1);

                if (currentLevel != null)
                {
                    // 🏗️ معلومات الترقية الأساسية
                    buildingNameText.text = "UPGRADE " + building.name + " TO";
                    buildingLevelText.text = (level + 1).ToString();

                    if (nextLevel != null)
                    {
                        upgradeTimeText.text = nextLevel.upgradeTime;
                        woodRequiredText.text = nextLevel.woodRequired.ToString();
                        stoneRequiredText.text = nextLevel.stoneRequired.ToString();
                        ironRequiredText.text = nextLevel.ironRequired.ToString();
                        powerRequiredText.text = nextLevel.powerRequired.ToString();


                        if (nextLevel != null && nextLevel.image != null)
                        {
                            buildingIcon.sprite = nextLevel.image;
                            buildingIcon.preserveAspect = true;
                        }
                    }

                    //// 🔢 خصائص قبل الترقية
                    //healthBeforeText.text = currentLevel.health.ToString();
                    //openText1Before.text = currentLevel.openText1;
                    //openText2Before.text = currentLevel.openText2;
                    //openText3Before.text = currentLevel.openText3;

                    // 🔼 خصائص بعد الترقية
                    if (nextLevel != null)
                    {
                        //healthAfterText.text = nextLevel.health.ToString();
                        openText1After.text = nextLevel.openText1;
                        openText2After.text = nextLevel.openText2;
                        openText3After.text = nextLevel.openText3;
                        requrimetTxt.text = nextLevel.requriment;
                    }
                    else
                    {
                        // أقصى ترقية
                        //healthAfterText.text = "-";
                        openText1After.text = "-";
                        openText2After.text = "-";
                        openText3After.text = "-";
                    }
                }
            }
        }
    }
}
