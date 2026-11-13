using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingUpgradeManager : MonoBehaviour
{
    public static BuildingUpgradeManager instant;

    [Header("ربط قاعة المدينة")]
    public static int townHallLevel = 1;

    [Header("Panels")]
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;

    [Header("الأزرار + النصوص")]
    public Button upgradeButton1;
    public Button upgradeGold1;

    public TMP_Text requirementText1;

    public Button upgradeButton2;
    public Button upgradeGold2;
    public TMP_Text requirementText2;

    public Button upgradeButton3;
    public Button upgradeGold3;
    public TMP_Text requirementText3;

    private building townHallBuilding;

    private void Awake()
    {
        instant = this;
    }

    private void Start()
    {
        GameObject townHallObj = GameObject.FindGameObjectWithTag("CenterCity");
        if (townHallObj != null)
        {
            townHallBuilding = townHallObj.GetComponent<building>();
            townHallLevel = townHallBuilding.level;
        }
        else
        {
            //Debug.LogWarning("⚠️ لم يتم العثور على مبنى بوسم (CenterCity)!");
        }
    }

    // هذه تستدعيها عند الضغط على مبنى
    public void CheckUpgradeForActivePanel(string buildingName, int buildingLevel)
    {
        if (panel1.activeSelf)
        {
            CheckUpgradeAvailability(upgradeButton1, upgradeGold1, requirementText1, buildingName, buildingLevel);
        }
        else if (panel2.activeSelf)
        {
            CheckUpgradeAvailability(upgradeButton2, upgradeGold2, requirementText2, buildingName, buildingLevel);
        }
        else if (panel3.activeSelf)
        {
            CheckUpgradeAvailability(upgradeButton3, upgradeGold3, requirementText3, buildingName, buildingLevel);
        }
    }

    // دالة عامة للتحقق (لكل زر + نص)
    public void CheckUpgradeAvailability(Button upgradeBtn, Button upgradeGOldBtn, TMP_Text requirementTxt, string buildingName, int buildingLevel)
    {
        // ✅ شرط خاص: إذا المبنى هو Center City → دائماً الزر يشتغل ولا يطبق عليه شروط
        if (buildingName == "CENTER CITY")
        {
            upgradeBtn.gameObject.SetActive(true);
            upgradeGOldBtn.gameObject.SetActive(true);
            requirementTxt.text = "";
            //Debug.Log("CENTER CITY selected → No restrictions applied."); ديبك
            return;
        }

        int maxAllowedLevel = GetMaxLevelAllowedByTownHall(buildingName);

        int absoluteMax = GetAbsoluteMaxLevel(buildingName);

        if (buildingLevel >= absoluteMax)
        {
            upgradeBtn.gameObject.SetActive(false);
            upgradeGOldBtn.gameObject.SetActive(false);
            requirementTxt.color = Color.gray;
            requirementTxt.text = "Max Level Reached";
            return;
        }

        if (buildingLevel >= maxAllowedLevel)
        {
            upgradeBtn.gameObject.SetActive(false);
            upgradeGOldBtn.gameObject.SetActive(false);
            requirementTxt.color = Color.red;
            requirementTxt.text = $"Upgrade Center City to unlock next level";
        }
        else
        {
            upgradeBtn.gameObject.SetActive(true);
            upgradeGOldBtn.gameObject.SetActive(true);
            requirementTxt.text = "";
        }

        //Debug.Log($"Check Upgrade: {buildingName} | Currect Level: {buildingLevel} | Max Allow: {maxAllowedLevel} | CenterCity: {townHallLevel}");  ديبك
    }

    int GetAbsoluteMaxLevel(string buildingName)
    {
        switch (buildingName)
        {
            case "LUMBER BLADE": return 13;
            case "STONE GRINDER": return 13;
            case "IRON SMELTER": return 13;
            case "STATUE OF MIGHT": return 13;
            case "ARCHER'S TOWER": return 11;
            case "BOMBARD TOWER": return 11;
            case "STOCK PILE": return 10;
            case "WALL": return 5;
            case "BEAST CANNON": return 6;
            case "GALTING ARROW TOWER": return 6;
            case "FLAMER TOWER": return 6;
            case "SKY TOWER": return 6;
            case "HIRING QUARTERS": return 3;
            case "BURNING MIRRORS": return 6;
            case "WATCH POST": return 4;
            case "MERCENARY BARRACKS": return 6;
            case "RECRUITMENT CHAMBER": return 4;
            case "TRAINGING CHAMBER": return 4;
            case "GATAPULT": return 3;



                // أضف البقية
        }
        return 1;
    }


    int GetMaxLevelAllowedByTownHall(string buildingName)
    {
        GameObject townHallObj = GameObject.FindGameObjectWithTag("CenterCity");
        if (townHallObj != null)
        {
            townHallBuilding = townHallObj.GetComponent<building>();
            townHallLevel = townHallBuilding.level;
        }
        switch (buildingName)
        {
            case "LUMBER BLADE":
                if (townHallLevel >= 10) return 13;
                if (townHallLevel >= 5) return 10;
                if (townHallLevel >= 4) return 9;
                if (townHallLevel >= 3) return 7;
                if (townHallLevel >= 2) return 5;
                return 3;

            case "STONE GRINDER":
                if (townHallLevel >= 10) return 13;
                if (townHallLevel >= 5) return 10;
                if (townHallLevel >= 4) return 9;
                if (townHallLevel >= 3) return 7;
                if (townHallLevel >= 2) return 5;
                return 3;

            case "IRON SMELTER":
                if (townHallLevel >= 10) return 13;
                if (townHallLevel >= 5) return 10;
                if (townHallLevel >= 4) return 9;
                if (townHallLevel >= 3) return 7;
                if (townHallLevel >= 2) return 5;
                return 3;

            case "STATUE OF MIGHT":
                if (townHallLevel >= 10) return 13;
                if (townHallLevel >= 5) return 10;
                if (townHallLevel >= 4) return 9;
                if (townHallLevel >= 3) return 7;
                if (townHallLevel >= 2) return 5;
                return 3;

            case "STOCK PILE":
                if (townHallLevel >= 6) return 10;
                if (townHallLevel >= 5) return 9;
                if (townHallLevel >= 4) return 8;
                if (townHallLevel >= 3) return 7;
                if (townHallLevel >= 2) return 5;
                return 2;

            case "ARCHER'S TOWER":
                if (townHallLevel >= 10) return 11;
                if (townHallLevel >= 8) return 10;
                if (townHallLevel >= 7) return 7;
                if (townHallLevel >= 6) return 6;
                if (townHallLevel >= 5) return 5;
                if (townHallLevel >= 4) return 4;
                if (townHallLevel >= 3) return 3;
                if (townHallLevel >= 2) return 2;
                return 1;

            case "BOMBARD TOWER":
                if (townHallLevel >= 10) return 11;
                if (townHallLevel >= 8) return 10;
                if (townHallLevel >= 7) return 7;
                if (townHallLevel >= 6) return 6;
                if (townHallLevel >= 5) return 5;
                if (townHallLevel >= 4) return 4;
                if (townHallLevel >= 3) return 3;
                if (townHallLevel >= 2) return 2;
                return 1;

            case "GALTING ARROW TOWER":
                if (townHallLevel >= 9) return 6;
                if (townHallLevel >= 8) return 5;
                if (townHallLevel >= 7) return 4;
                if (townHallLevel >= 6) return 3;
                if (townHallLevel >= 5) return 2;
                return 1;

            case "FLAMER TOWER":
                if (townHallLevel >= 9) return 6;
                if (townHallLevel >= 8) return 5;
                if (townHallLevel >= 7) return 4;
                if (townHallLevel >= 6) return 3;
                if (townHallLevel >= 5) return 2;
                return 1;

            case "SKY TOWER":
                if (townHallLevel >= 9) return 6;
                if (townHallLevel >= 8) return 5;
                if (townHallLevel >= 7) return 4;
                if (townHallLevel >= 6) return 3;
                if (townHallLevel >= 5) return 2;
                return 1;

            case "HIRING QUARTERS":
                if (townHallLevel >= 4) return 3;
                if (townHallLevel >= 3) return 2;
                return 1;

            case "BURNING MIRRORS":
                if (townHallLevel >= 9) return 6;
                if (townHallLevel >= 8) return 5;
                if (townHallLevel >= 7) return 4;
                if (townHallLevel >= 6) return 3;
                if (townHallLevel >= 5) return 2;
                return 1;

            case "WATCH POST":
                if (townHallLevel >= 9) return 4;
                if (townHallLevel >= 5) return 3;
                if (townHallLevel >= 4) return 2;
                return 1;

            case "MERCENARY BARRACKS":
                if (townHallLevel >= 6) return 6;
                if (townHallLevel >= 5) return 4;
                if (townHallLevel >= 3) return 3;
                if (townHallLevel >= 2) return 2;
                return 1;

            case "RECRUITMENT CHAMBER":
                if (townHallLevel >= 5) return 4;
                if (townHallLevel >= 4) return 3;
                if (townHallLevel >= 3) return 2;
                return 1;

            case "TRAINGING CHAMBER":
                if (townHallLevel >= 5) return 4;
                if (townHallLevel >= 4) return 3;
                if (townHallLevel >= 3) return 2;
                return 1;

            case "GATAPULT":
                if (townHallLevel >= 3) return 3;
                return 1;

            case "WALL":
                if (townHallLevel >= 5) return 5;
                if (townHallLevel >= 4) return 4;
                if (townHallLevel >= 3) return 3;
                if (townHallLevel >= 2) return 2;
                return 1;

            case "BEAST CANNON":
                if (townHallLevel >= 10) return 6;
                if (townHallLevel >= 9) return 5;
                if (townHallLevel >= 8) return 4;
                if (townHallLevel >= 7) return 2;
                return 1;
        }

        return 1;

    }
}
