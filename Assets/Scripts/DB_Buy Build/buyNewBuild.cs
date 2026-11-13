using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class buyNewBuild : MonoBehaviour
{
    public TMP_Text buildingNameText;
    public TMP_Text buildingInfoText;
    public TMP_Text buildingRequrmentText;
    public TMP_Text upgradeTimeText;
    public TMP_Text woodRequiredText;
    public TMP_Text stoneRequiredText;
    public TMP_Text ironRequiredText;
    public TMP_Text powerRequiredText;
    public Image buildingIcon;

    public Button woodBtn;
    public Button stoneBtn;
    public Button powerBtn;
    public Button ironBtn;
    public Button ResourBtn;
    public Button archerBtn;

    public Button makerBtn;
    public Button archerOPBtn;
    public Button flameTBtn;
    public Button flayTowerBtn;
    public Button MirrorBtn;
    public Button spyBtn;
    public Button spyClanBtn;
    public Button boomTowerBtn;
    public Button BarracksBtn;
    public Button BrackClanBtn;
    public Button clanBtn;
    public Button exeBuildBtn;
    public Button planBtn;
    public Button postBtn;
    public Button fourMakerBtn;
    public Button hadadBtn;
    public Button hornBtn;
    public Button NewWorrBtn;
    public Button trainingBtn;
    public Button CaveBtn;
    public Button CaveBrackBtn;
    public Button wallBtn;
    public Button GATAPULTbtn;
    public Button bigBomberBtn;
    public Button fireTrapBtn;
    public Button trapNoremBtn;

    private Dictionary<string, Button> buildingButtonMap;
    private List<Button> allButtons;
    private string currentBuildingName = "";

    void Start()
    {
        //  Õ÷Ì— ﬁ«∆„… ﬂ· «·√“—«— · ⁄ÿÌ·Â« »”ÂÊ·…
        allButtons = new List<Button> { trapNoremBtn,fireTrapBtn, bigBomberBtn, GATAPULTbtn, wallBtn, archerOPBtn, flameTBtn, flayTowerBtn, MirrorBtn, spyBtn, spyClanBtn, boomTowerBtn, BarracksBtn, BrackClanBtn, woodBtn, stoneBtn, powerBtn, ironBtn, ResourBtn, archerBtn, makerBtn, CaveBrackBtn, CaveBtn, trainingBtn, NewWorrBtn, hornBtn, hadadBtn, fourMakerBtn, postBtn, planBtn, exeBuildBtn, clanBtn };

        // ≈⁄œ«œ Œ—Ìÿ… «·—»ÿ »Ì‰ «”„ «·„»‰Ï Ê«·“— «·„ÿ·Ê»
        buildingButtonMap = new Dictionary<string, Button>
        {
            { "LUMBER BLADE", woodBtn },
            { "STONE GRINDER", stoneBtn },
            { "IRON SMELTER", ironBtn },
            { "STATUE OF MIGHT", powerBtn },
            { "STOCK PILE", ResourBtn },
            { "ARCHER'S TOWER", archerBtn },

            { "WALL", wallBtn },
            { "GATAPULT", GATAPULTbtn },
            { "GALTING ARROW TOWER", archerOPBtn },
            { "FLAMER TOWER", flameTBtn },
            { "SKY TOWER", flayTowerBtn },
            { "BURNING MIRRORS", MirrorBtn },
            { "WATCH POST", spyBtn },
            { "FREEND WATCH POST", spyClanBtn },
            { "BOMBARD TOWER", boomTowerBtn },
            { "MERCENARY BARRACKS", BarracksBtn },
            { "CLAN BARRACKS", BrackClanBtn },
            { "CLAN", clanBtn },
            { "EXECUTIONAL CUILLOTINE", exeBuildBtn },
            { "CITY PLANNER", planBtn },
            { "PIGEON POST", postBtn },
            { "CENTRAL HIRING", fourMakerBtn },
            { "TUSK HORN", hornBtn },
            { "RECRUITMENT CHAMBER", NewWorrBtn },
            { "TRAINGING CHAMBER", trainingBtn },
            { "BEAST KEEPER", CaveBtn },
            { "BEAST CAVE", CaveBrackBtn },
            { "HIRING QUARTERS", makerBtn },
            { "BLACKSMITH", hadadBtn },
            { "BEAST CANNON", bigBomberBtn },
            { "FIRE TRAP", fireTrapBtn },
            { "BURIED SPIKES", trapNoremBtn }




        };

        UpdateUIIfNeeded();
    }

    void Update()
    {
        UpdateUIIfNeeded();
    }

    void UpdateUIIfNeeded()
    {
        string newBuildingName = GetJustName.objectName;

        // ·«  ÕœÀ √Ì ‘Ì¡ ≈–« ·„ Ì €Ì— «·«”„
        if (newBuildingName == currentBuildingName || string.IsNullOrEmpty(newBuildingName))
            return;

        currentBuildingName = newBuildingName;

        UpdateButtons(currentBuildingName);
        UpdateUI(currentBuildingName);
    }

    void UpdateButtons(string buildingName)
    {
        // ≈Œ›«¡ ﬂ· «·√“—«— √Ê·«
        foreach (Button btn in allButtons)
        {
            btn.gameObject.SetActive(false);
        }

        //  ›⁄Ì· «·“— «·Œ«’ ≈‰ ÊÃœ
        if (buildingButtonMap.ContainsKey(buildingName))
        {
            buildingButtonMap[buildingName].gameObject.SetActive(true);
        }
    }

    void UpdateUI(string buildingName)
    {
        NewBuildingData building = NewBuildingDatabase.GetBuildingByName(buildingName);

        if (building != null && building.upgradeLevels.Count > 0)
        {
            NewBuildDB upgradeData = building.upgradeLevels[0]; // «·„” ÊÏ «·√Ê·

            buildingNameText.text = "BUILD YOUR " + building.name;
            buildingInfoText.text = upgradeData.info;
            buildingRequrmentText.text = upgradeData.Requriment.ToString();
            upgradeTimeText.text = upgradeData.upgradeTime;
            woodRequiredText.text = upgradeData.woodRequired.ToString();
            stoneRequiredText.text = upgradeData.stoneRequired.ToString();
            ironRequiredText.text = upgradeData.ironRequired.ToString();
            powerRequiredText.text = upgradeData.powerRequired.ToString();

            if (building.icon != null)
            {
                buildingIcon.sprite = building.icon;
                buildingIcon.preserveAspect = true;
            }
        }
        else
        {
            Debug.LogWarning("·«  ÊÃœ »Ì«‰«   —ﬁÌ… ··„»‰Ï √Ê ·„ Ì „ «·⁄ÀÊ— ⁄·ÌÂ: " + buildingName);
        }
    }
}
