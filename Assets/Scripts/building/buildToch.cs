using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class BuildingSelector : MonoBehaviour
{
    public TMP_Text buildingTitleText; // اربطها من الـ Inspector
    public static BuildLvl targetBuilding;

    string[] validTags = { "Timer", "trapfire", "trapN", "boomOP", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "archer", "archerOP", "flameT", "flayTower", "Mirror", "spy", "spyClan", "boomTower", "Barracks", "BrackClan", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "CaveBrack", "mongel", "wall" };

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            DetectBuildingByTag();
        }
    }

    void DetectBuildingByTag()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider == null)
            return; // لا شيء تم لمسه، نخرج من الدالة

        // 🔹 إذا الاسم هو "upgrade (1)"
        if (hit.collider.gameObject.name == "upgrade (1)")
        {
            NameUpgrdes buildingScript = hit.collider.GetComponent<NameUpgrdes>();
            if (buildingScript != null)
            {
                if (buildingTitleText != null)
                    buildingTitleText.text = buildingScript.buildingName;
            }
            else
            {
                if (buildingTitleText != null)
                    buildingTitleText.text = "TIMER BUILDING";
            }
            return; // توقف هنا، لا تحتاج للتأكد من التاغات الأخرى
        }
        if (hit.collider.gameObject.name == "Capsule")
        {
            NameUpgrdes buildingScript = hit.collider.GetComponent<NameUpgrdes>();
            if (buildingScript != null)
            {
                if (buildingTitleText != null)
                    buildingTitleText.text = buildingScript.buildingName;
            }
            else
            {
                if (buildingTitleText != null)
                    buildingTitleText.text = "TIMER BUILDING";
            }
            return; // توقف هنا، لا تحتاج للتأكد من التاغات الأخرى
        }


        // 🔹 التحقق حسب التاغات
        if (validTags.Contains(hit.collider.tag))
        {
            // البحث عن BuildLvl في الآباء
            Transform parent = hit.collider.transform;
            BuildLvl building = null;
            while (parent != null && building == null)
            {
                building = parent.GetComponent<BuildLvl>();
                parent = parent.parent;
            }

            if (building != null)
            {
                targetBuilding = building;
                BuildingNameOnlyUI.currentBuilding = building.GetComponent<building>();
                //Debug.Log("✅ تم تحديد المبنى: " + building.gameObject.name); ديبك
            }
            else
            {
                //Debug.LogWarning("❌ لم يتم العثور على مكون BuildLvl"); ديبك
                targetBuilding = null;
            }

            // 🔹 باقي الحالات العادية
            switch (hit.collider.tag)
            {
                case "wood":
                    buildingTitleText.text = "LUMBER BLADE";
                    break;
                case "boomOP":
                    buildingTitleText.text = "BEAST CANNON";
                    break;
                case "stone":
                    buildingTitleText.text = "STONE GRINDER";
                    break;
                case "iron":
                    buildingTitleText.text = "IRON SMELTER";
                    break;
                case "power":
                    buildingTitleText.text = "STATUE OF MIGHT";
                    break;
                case "CenterCity":
                    buildingTitleText.text = "CENTER CITY";
                    break;
                case "Resoures":
                    buildingTitleText.text = "STOCK PILE";
                    break;
                case "archer":
                    buildingTitleText.text = "ARCHER'S TOWER";
                    break;
                case "archerOP":
                    buildingTitleText.text = "GALTING ARROW TOWER";
                    break;
                case "maker":
                    buildingTitleText.text = "HIRING QUARTERS";
                    break;
                case "flameT":
                    buildingTitleText.text = "FLAMER TOWER";
                    break;
                case "flayTower":
                    buildingTitleText.text = "SKY TOWER";
                    break;
                case "Mirror":
                    buildingTitleText.text = "BURNING MIRRORS";
                    break;
                case "spy":
                    buildingTitleText.text = "WATCH POST";
                    break;
                case "spyClan":
                    buildingTitleText.text = "FREEND WATCH POST";
                    break;
                case "boomTower":
                    buildingTitleText.text = "BOMBARD TOWER";
                    break;
                case "Barracks":
                    buildingTitleText.text = "MERCENARY BARRACKS";
                    break;
                case "BrackClan":
                    buildingTitleText.text = "CLAN BARRACKS";
                    break;
                case "clan":
                    buildingTitleText.text = "CLAN";
                    break;
                case "exeBuild":
                    buildingTitleText.text = "EXECUTIONAL CUILLOTINE";
                    break;
                case "plan":
                    buildingTitleText.text = "CITY PLANNER";
                    break;
                case "post":
                    buildingTitleText.text = "PIGEON POST";
                    break;
                case "fourMaker":
                    buildingTitleText.text = "CENTRAL HIRING";
                    break;
                case "hadad":
                    buildingTitleText.text = "BLACKSMITH";
                    break;
                case "horn":
                    buildingTitleText.text = "TUSK HORN";
                    break;
                case "NewWorr":
                    buildingTitleText.text = "RECRUITMENT CHAMBER";
                    break;
                case "training":
                    buildingTitleText.text = "TRAINGING CHAMBER";
                    break;
                case "Cave":
                    buildingTitleText.text = "BEAST KEEPER";
                    break;
                case "CaveBrack":
                    buildingTitleText.text = "BEAST CAVE";
                    break;
                case "mongel":
                    buildingTitleText.text = "GATAPULT";
                    break;
                case "wall":
                    buildingTitleText.text = "WALL";
                    break;
                case "trapfire":
                    buildingTitleText.text = "FIRE TRAP";
                    break;
                case "trapN":
                    buildingTitleText.text = "BURIED SPIKES";
                    break;
                default:
                    buildingTitleText.text = "Building";
                    break;
            }
        }
    }
}
