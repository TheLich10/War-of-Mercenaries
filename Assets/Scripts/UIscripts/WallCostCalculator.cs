using UnityEngine;
using TMPro;

public class WallCostCalculator : MonoBehaviour
{
    public static WallCostCalculator Instance;
    [Header("نصوص عرض التكلفة (TextMeshPro)")]
    public TextMeshProUGUI level1CostText;
    public TextMeshProUGUI level2CostText;
    public TextMeshProUGUI level3CostText;
    public TextMeshProUGUI level4CostText;
    public GameObject panalToclose;
    private int levelCCNow = 1;

    private void Awake()
    {
        Instance = this;
    }

    // ----------------------------------------------------
    // 1. حساب تكلفة الجدران
    // ----------------------------------------------------
    public void CalculateWallsCost()
    {
        GameObject[] walls = GameObject.FindGameObjectsWithTag("wall");
        GameObject CC = GameObject.FindGameObjectWithTag("CenterCity");
        int totalCostLevel1 = 0;
        int totalCostLevel2 = 0;
        int totalCostLevel3 = 0;
        int totalCostLevel4 = 0;

        foreach (GameObject wall in walls)
        {
            building buildingScript = wall.GetComponent<building>();

            if (buildingScript != null)
            {
                int cost = buildingScript.level * 10;

                switch (buildingScript.level)
                {
                    case 1: totalCostLevel1 += cost; break;
                    case 2: totalCostLevel2 += cost; break;
                    case 3: totalCostLevel3 += cost; break;
                    case 4: totalCostLevel4 += cost; break;
                }
            }
        }

        building CClvl = CC.GetComponent<building>();
        if(CClvl != null)
        {
            levelCCNow = CClvl.level;
        }


        if (level1CostText != null) level1CostText.text = totalCostLevel1.ToString();
        if (level2CostText != null) level2CostText.text = totalCostLevel2.ToString();
        if (level3CostText != null) level3CostText.text = totalCostLevel3.ToString();
        if (level4CostText != null) level4CostText.text = totalCostLevel4.ToString();

    }

    // ----------------------------------------------------
    // 2. دالة الترقية الجماعية (الأساسية)
    // ----------------------------------------------------
    public void UpgradeSpecificBuildings(string targetTag, int targetLevel, int maxAmountToUpgrade)
    {
        BuildLvl[] allBuildings = FindObjectsByType<BuildLvl>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int upgradedCount = 0;

        foreach (BuildLvl building in allBuildings)
        {
            if (upgradedCount >= maxAmountToUpgrade) break;

            if (building.GetBuildingTag() == targetTag && building.GetCurrentLevel() == targetLevel)
            {
                building.InstantUpgrade();
                upgradedCount++;
            }
        }

        //Debug.Log($"✅ تمت ترقية {upgradedCount} مباني من نوع {targetTag} بنجاح!");

        // تحديث التكلفة المعروضة بعد الترقية مباشرة
        CalculateWallsCost();
    }

    // ----------------------------------------------------
    // 3. دوال أزرار ترقية الجدران حسب المستوى
    // ----------------------------------------------------

    // اربط هذه الدالة بالزر الأول (ترقية الجدران من مستوى 1 إلى 2)
    public void UpgradeAllLevel1Walls()
    {
        if(levelCCNow >= 2)
        {
            UpgradeSpecificBuildings("wall", 0, int.MaxValue);
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(int.Parse(level1CostText.text));
            panalToclose.gameObject.SetActive(false);
        }
        else return;

    }

    // اربط هذه الدالة بالزر الثاني (ترقية الجدران من مستوى 2 إلى 3)
    public void UpgradeAllLevel2Walls()
    {
        if (levelCCNow >= 3)
        {
            UpgradeSpecificBuildings("wall", 1, int.MaxValue);
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(int.Parse(level2CostText.text));
            panalToclose.gameObject.SetActive(false);
        }
        else return;
    }

    // اربط هذه الدالة بالزر الثالث (ترقية الجدران من مستوى 3 إلى 4)
    public void UpgradeAllLevel3Walls()
    {
        if (levelCCNow >= 4)
        {
            UpgradeSpecificBuildings("wall", 2, int.MaxValue);
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(int.Parse(level3CostText.text));
            panalToclose.gameObject.SetActive(false);
        }
        else return;
    }

    // اربط هذه الدالة بالزر الرابع (ترقية الجدران من مستوى 4 إلى 5)
    public void UpgradeAllLevel4Walls()
    {
        if (levelCCNow >= 5)
        {
            UpgradeSpecificBuildings("wall", 3, int.MaxValue);
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(int.Parse(level4CostText.text));
            panalToclose.gameObject.SetActive(false);
        }
        else return;

    }
}