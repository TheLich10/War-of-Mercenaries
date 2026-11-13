using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarriorDisplayUI : MonoBehaviour
{
    public TMP_Text warriorNameText;

    // ✅ الخصائص قبل الترقية
    public TMP_Text healthBeforeText;
    public TMP_Text speedBeforeText;
    public TMP_Text damageBeforeText;
    public TMP_Text storageBeforeText;
    public TMP_Text recruitTimeBeforeText;
    public TMP_Text recruitCostBeforeText;
    //public TMP_Text trainingCostBeforeText;
    //public TMP_Text trainingTimeBeforeText;

    // ✅ الخصائص بعد الترقية
    public TMP_Text healthAfterText;
    public TMP_Text speedAfterText;
    public TMP_Text damageAfterText;
    public TMP_Text storageAfterText;
    public TMP_Text recruitTimeAfterText;
    public TMP_Text recruitCostAfterText;
    public TMP_Text trainingCostAfterText;
    public TMP_Text trainingTimeAfterText;

    public Image warriorImage;


    [Header("Worriors TXT")]
    public static int BedionLevel;
    public TMP_Text BedionLevelTxt;

    public static int ottmanLvl;
    public TMP_Text OttmanLvlTxt;

    public static int ScalingEgyptianLvl;
    public TMP_Text ScalingEgyptianTXT;

    public static int DestroyerLvl;
    public TMP_Text DestroyerTXT;

    public static int flyingOttomanLVL;
    public TMP_Text flyingOttomanTXT;

    public static int GaulLVL;
    public TMP_Text GaulTXT;

    public static int HelaerLVL;
    public TMP_Text HelaerTXT;

    public static int KamikazeLVL;
    public TMP_Text KamikazeTXT;

    public static int knightLVL;
    public TMP_Text knightTXT;

    public static int persianLVL;
    public TMP_Text persianTXT;

    public static int postionLVL;
    public TMP_Text postionTXT;

    public static int sapperLVL;
    public TMP_Text sapperTXT;

    public static int SavgeLVL;
    public TMP_Text SavgeTXT;

    public static int sneakyLVL;
    public TMP_Text sneakyTXT;

    public static int MongolianLVL;
    public TMP_Text MongolianTXT;



    void Start()
    {
        //**********************************************//
        BedionLevel = int.Parse(BedionLevelTxt.text);
        ottmanLvl = int.Parse(OttmanLvlTxt.text);
        ScalingEgyptianLvl = int.Parse(ScalingEgyptianTXT.text);
        DestroyerLvl = int.Parse(DestroyerTXT.text);
        sneakyLVL = int.Parse(sneakyTXT.text);
        sapperLVL = int.Parse(sapperTXT.text);
        ottmanLvl = int.Parse(OttmanLvlTxt.text);
        SavgeLVL = int.Parse(SavgeTXT.text);
        postionLVL = int.Parse(postionTXT.text);
        persianLVL = int.Parse(persianTXT.text);
        KamikazeLVL = int.Parse(KamikazeTXT.text);
        GaulLVL = int.Parse(GaulTXT.text);
        knightLVL = int.Parse(knightTXT.text);
        HelaerLVL = int.Parse(HelaerTXT.text);
        flyingOttomanLVL = int.Parse(flyingOttomanTXT.text);
        MongolianLVL = int.Parse(MongolianTXT.text);

        //*********************************************//

        // هنا تأكد من أن البيانات تم تحديثها بشكل صحيح
        string warriorName = GetObjectNameAndText.objectName;
        if (warriorName == "CaveEnter")
        {
            return;
        }
        int level = GetObjectNameAndText.savedNumber;

        if (!string.IsNullOrEmpty(warriorName) && level > 0)
        {
            WarriorTring warrior = WarriorTringDatabase.GetWarriorByName(warriorName);
            if (warrior != null)
            {
                WarriorLevel currentLevel = warrior.GetLevel(level);
                WarriorLevel nextLevel = warrior.GetLevel(level + 1);

                if (currentLevel != null)
                {
                    warriorNameText.text = "UPGRADE " + warrior.name + " TO";

                    if (warrior.image != null)
                    {
                        warriorImage.sprite = warrior.image;
                        warriorImage.preserveAspect = true;

                    }

                    // ⬅️ قبل الترقية
                    healthBeforeText.text = currentLevel.health.ToString();
                    speedBeforeText.text = currentLevel.speed.ToString();
                    damageBeforeText.text = currentLevel.damage.ToString();
                    storageBeforeText.text = currentLevel.storageSpace.ToString();
                    recruitTimeBeforeText.text = currentLevel.recruitTime.ToString();
                    recruitCostBeforeText.text = currentLevel.recruitCost.ToString();
                    //trainingCostBeforeText.text = currentLevel.trainingCost.ToString();
                    //trainingTimeBeforeText.text = currentLevel.trainingTime.ToString();

                    // ➡️ بعد الترقية
                    if (nextLevel != null)
                    {
                        healthAfterText.text = nextLevel.health.ToString();
                        speedAfterText.text = nextLevel.speed.ToString();
                        damageAfterText.text = nextLevel.damage.ToString();
                        storageAfterText.text = nextLevel.storageSpace.ToString();
                        recruitTimeAfterText.text = nextLevel.recruitTime.ToString();
                        recruitCostAfterText.text = nextLevel.recruitCost.ToString();
                        trainingCostAfterText.text = nextLevel.trainingCost.ToString();
                        trainingTimeAfterText.text = nextLevel.trainingTime.ToString();
                    }
                    else
                    {
                        healthAfterText.text = "0";
                        speedAfterText.text = "0";
                        damageAfterText.text = "0";
                        storageAfterText.text = "0";
                        recruitTimeAfterText.text = "0";
                        recruitCostAfterText.text = "0";
                        trainingCostAfterText.text = "0";
                        trainingTimeAfterText.text = "0";
                    }
                }
            }
        }
    }

    private void Update()
    {
        // هنا تأكد من أن البيانات تم تحديثها بشكل صحيح
        string warriorName = GetObjectNameAndText.objectName;
        if (warriorName == "CaveEnter")
        {
            return;
        }
        int level = GetObjectNameAndText.savedNumber;

        if (!string.IsNullOrEmpty(warriorName) && level > 0)
        {
            WarriorTring warrior = WarriorTringDatabase.GetWarriorByName(warriorName);
            if (warrior != null)
            {
                WarriorLevel currentLevel = warrior.GetLevel(level);
                WarriorLevel nextLevel = warrior.GetLevel(level + 1);

                if (currentLevel != null)
                {
                    warriorNameText.text =warrior.name;

                    if (warrior.image != null)
                    {
                        warriorImage.sprite = warrior.image;
                        warriorImage.preserveAspect = true;

                    }

                    // ⬅️ قبل الترقية
                    healthBeforeText.text = currentLevel.health.ToString()+" +";
                    speedBeforeText.text = currentLevel.speed.ToString()+" +";
                    damageBeforeText.text = currentLevel.damage.ToString() + " +";
                    storageBeforeText.text = currentLevel.storageSpace.ToString() + " ->";
                    recruitTimeBeforeText.text = currentLevel.recruitTime.ToString() + " ->";
                    recruitCostBeforeText.text = currentLevel.recruitCost.ToString() + " ->";
                    //trainingCostBeforeText.text = currentLevel.trainingCost.ToString();
                    //trainingTimeBeforeText.text = currentLevel.trainingTime.ToString();

                    // ➡️ بعد الترقية
                    if (nextLevel != null)
                    {
                        healthAfterText.text = nextLevel.health.ToString();
                        speedAfterText.text = nextLevel.speed.ToString();
                        damageAfterText.text = nextLevel.damage.ToString();
                        storageAfterText.text = nextLevel.storageSpace.ToString();
                        recruitTimeAfterText.text = nextLevel.recruitTime.ToString();
                        recruitCostAfterText.text = nextLevel.recruitCost.ToString();
                        trainingCostAfterText.text = nextLevel.trainingCost.ToString();
                        trainingTimeAfterText.text = nextLevel.trainingTime.ToString();
                    }
                    else
                    {
                        healthAfterText.text = "0";
                        speedAfterText.text = "0";
                        damageAfterText.text = "0";
                        storageAfterText.text = "0";
                        recruitTimeAfterText.text = "0";
                        recruitCostAfterText.text = "0";
                        trainingCostAfterText.text = "0";
                        trainingTimeAfterText.text = "0";
                    }
                }
            }
        }
    }
}
