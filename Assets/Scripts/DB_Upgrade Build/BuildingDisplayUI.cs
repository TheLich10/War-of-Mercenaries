using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingInfoUI : MonoBehaviour
{
    public TMP_Text buildingNameText;
    public TMP_Text buildingLevelText;
    public TMP_Text upgradeTimeText;
    public TMP_Text woodRequiredText;
    public TMP_Text stoneRequiredText;
    public TMP_Text ironRequiredText;
    public TMP_Text powerRequiredText;
    public Image buildingIcon;

    // ✅ خصائص قبل الترقية
    public TMP_Text healthBeforeText;
    public TMP_Text perHourBeforeText;
    public TMP_Text cityStorageBeforeText;
    public TMP_Text barackSpaceBeforeText;
    public TMP_Text warriorMakerBeforeText;

    // ✅ خصائص بعد الترقية
    public TMP_Text healthAfterText;
    public TMP_Text perHourAfterText;
    public TMP_Text cityStorageAfterText;
    public TMP_Text barackSpaceAfterText;
    public TMP_Text warriorMakerAfterText;

    public GameObject panalBarStrge;
    public GameObject panalBarProductPer;
    public GameObject panakBarMakerNum;
    public GameObject panalBarBaeckSpace;

    private void Update()
    {
        string buildingName = GetNameAndLvl.objectName;
        int level = GetNameAndLvl.savedNumber;

        if (buildingName == "MERCENARY BARRACKS") 
        {
            panalBarBaeckSpace.SetActive(true);
            panalBarStrge.SetActive(false);
            panalBarProductPer.SetActive(false);
            panakBarMakerNum.SetActive(false);

        }
        else if (buildingName == "HIRING QUARTERS")
        {
            panakBarMakerNum.SetActive(true);
            panalBarBaeckSpace.SetActive(false);
            panalBarStrge.SetActive(false);
            panalBarProductPer.SetActive(false);
        }
        else
        {
            panalBarBaeckSpace.SetActive(false);
            panakBarMakerNum.SetActive(false);
            panalBarStrge.SetActive(true);
            panalBarProductPer.SetActive(true);
        }

        if (!string.IsNullOrEmpty(buildingName) && level > 0)
        {
            BuildingData building = BuildingDatabase.GetBuildingByName(buildingName);
            if (building != null)
            {
                BuildLevelUI currentLevel = building.GetLevel(level);
                BuildLevelUI nextLevel = building.GetLevel(level + 1);

                if (currentLevel != null && nextLevel != null)
                {
                    // 🏗️ معلومات الترقية الأساسية
                    buildingNameText.text = "UPGRADE " + building.name + " TO";
                    buildingLevelText.text = (level + 1).ToString();
                    upgradeTimeText.text = nextLevel.upgradeTime.ToString();
                    woodRequiredText.text = nextLevel.woodRequired.ToString();
                    stoneRequiredText.text = nextLevel.stoneRequired.ToString();
                    ironRequiredText.text = nextLevel.ironRequired.ToString();
                    powerRequiredText.text = nextLevel.powerRequired.ToString();
                    if (nextLevel != null && nextLevel.image != null)
                    {
                        buildingIcon.sprite = nextLevel.image;
                        buildingIcon.preserveAspect = true;
                    }


                    // 🔢 عرض الخصائص قبل الترقية
                    healthBeforeText.text = currentLevel.health.ToString();
                    perHourBeforeText.text = currentLevel.PerHoures.ToString();
                    cityStorageBeforeText.text = currentLevel.CityStroge.ToString();
                    barackSpaceBeforeText.text = currentLevel.Barackspace.ToString();
                    warriorMakerBeforeText.text = currentLevel.WoirroMakerNum.ToString();

                    // 🔼 عرض الخصائص بعد الترقية (إن وجدت)
                    if (nextLevel != null)
                    {
                        healthAfterText.text = nextLevel.health.ToString();
                        perHourAfterText.text = nextLevel.PerHoures.ToString();
                        cityStorageAfterText.text = nextLevel.CityStroge.ToString();
                        barackSpaceAfterText.text = nextLevel.Barackspace.ToString();
                        warriorMakerAfterText.text = nextLevel.WoirroMakerNum.ToString();
                    }
                    else
                    {
                        // 🛑 لا يوجد مستوى بعد الحالي (أقصى ترقية)
                        healthAfterText.text = "-";
                        perHourAfterText.text = "-";
                        cityStorageAfterText.text = "-";
                        barackSpaceAfterText.text = "-";
                        warriorMakerAfterText.text = "-";
                    }
                }
            }
        }
    }
}
