using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarriorInfoUI : MonoBehaviour
{
    public TMP_Text warriorNameText;   // «”„ «·„Õ«—»
    public TMP_Text levelText;         // «·„” ÊÏ «·Õ«·Ì

    public TMP_Text healthText;
    public TMP_Text speedText;
    public TMP_Text damageText;
    public TMP_Text storageText;
    public TMP_Text recruitTimeText;
    public TMP_Text recruitCostText;

    public Image warriorImage;

    public TMP_Text targetText;        // «·Âœ› „‰ «·„Õ«—»
    public TMP_Text infoWarriorText;   // „⁄·Ê„«  ⁄‰ «·„Õ«—»

    void Start()
    {
        string warriorName = GetObjectNameAndText.objectName;
        int level = GetObjectNameAndText.savedNumber;

        if (!string.IsNullOrEmpty(warriorName) && level > 0 && warriorName != "CaveEnter")
        {
            WarriorTring warrior = WarriorTringDatabase.GetWarriorByName(warriorName);
            if (warrior != null)
            {
                WarriorLevel currentLevel = warrior.GetLevel(level);
                if (currentLevel != null)
                {
                    // ⁄—÷ «·«”„ Ê«·„” ÊÏ
                    warriorNameText.text = warrior.name;
                    levelText.text =level.ToString();

                    // ⁄—÷ «·’Ê—…
                    if (warrior.image != null)
                    {
                        warriorImage.sprite = warrior.image;
                        warriorImage.preserveAspect = true;
                    }

                    // «·Œ’«∆’
                    healthText.text = currentLevel.health.ToString();
                    speedText.text = currentLevel.speed.ToString();
                    damageText.text = currentLevel.damage.ToString();
                    storageText.text = currentLevel.storageSpace.ToString()+" SPACES";
                    recruitTimeText.text = currentLevel.recruitTime.ToString() + "S";
                    recruitCostText.text = currentLevel.recruitCost.ToString();

                    // ‰’Ê’ „Œ’’… Õ”» «·„Õ«—»
                    SetCustomTexts(warriorName);
                }
            }
        }
    }
    private void Update()
    {
        string warriorName = GetObjectNameAndText.objectName;
        int level = GetObjectNameAndText.savedNumber;

        if (!string.IsNullOrEmpty(warriorName) && level > 0 && warriorName != "CaveEnter")
        {
            WarriorTring warrior = WarriorTringDatabase.GetWarriorByName(warriorName);
            if (warrior != null)
            {
                WarriorLevel currentLevel = warrior.GetLevel(level);
                if (currentLevel != null)
                {
                    // ⁄—÷ «·«”„ Ê«·„” ÊÏ
                    warriorNameText.text = warrior.name;
                    levelText.text = level.ToString();

                    // ⁄—÷ «·’Ê—…
                    if (warrior.image != null)
                    {
                        warriorImage.sprite = warrior.image;
                        warriorImage.preserveAspect = true;

                    }

                    // «·Œ’«∆’
                    healthText.text = currentLevel.health.ToString();
                    speedText.text = currentLevel.speed.ToString();
                    damageText.text = currentLevel.damage.ToString();
                    storageText.text = currentLevel.storageSpace.ToString() + " SPACES";
                    recruitTimeText.text = currentLevel.recruitTime.ToString() + "S";
                    recruitCostText.text = currentLevel.recruitCost.ToString();

                    // ‰’Ê’ „Œ’’… Õ”» «·„Õ«—»
                    SetCustomTexts(warriorName);
                }
            }
        }
    }

    void SetCustomTexts(string name)
    {
        switch (name)
        {
            case "bedoiun":
                targetText.text = "Anything";
                infoWarriorText.text = "These siege veterans survived countless desert battles at the forefronts. Although not very powerful, and seemingly highly expandable, their presence provides protection and inspiration to other warriors.";
                break;

            case "Ottoman":
                targetText.text = "Defensive Building";
                infoWarriorText.text = "Some of the Sultan's Janissaries left their ranks for fame and glory. These warriors are extremely tough and steadfast, having composed the backbone of the Ottoman army during their service. Since they live for honor, they hate the cowards in the towers!";
                break;

            case "KnightOnHorse":
                targetText.text = "Res. Producer, Stock Pile, City Center";
                infoWarriorText.text = "Many brave warriors fell before they could see this greedy Scandinavian Rider's face. Although he's not tough enough to see the end of most battles, he rides his horse as swiftly as the wind to get the first kill and loot in every battle!";
                break;
            case "Kamikaze":
                targetText.text = "Wall";
                infoWarriorText.text = "Devoted to his cause, this fearless Far Eastern does not hesitate to sacrifice his life for the greater good (or wage). Destroying walls is his speciality, and he is really good at it. Place his unit in the front lines to unleash destruction on enemy's fortifications!";
                break;
            case "persian":
                targetText.text = "Anything";
                infoWarriorText.text = "Originally trained as assassins, these dreaded and fanatically loyal warriors have now found a better use for their skills as bodyguards. Their exceptional dexterity and strength make them a balanced fighter, effective against any type of threat.";
                break;
            case "ScalingEgyptian":
                targetText.text = "Defensive Building";
                infoWarriorText.text = "Equipped with precise marksmanship and swift feet, these pharaoh warriorss live for taking down defense buildings. Thanks to their burning arrows, this mercenary unit is one of the deadliest in the field.";
                break;
            case "GentleHealerFull":
                targetText.text = "Wounded Uints";
                infoWarriorText.text = "These clerics are oath-bound not to harm any living creature. But this didn't prevent them from playing crucial roles in every epic war ever fought. Their healing abilities are unmatched and they can turn the tide of any battle in the blink of an eye.";
                break;
            case "PersianSapper":
                targetText.text = "Defensive Building";
                infoWarriorText.text = "One of the key units of Middles Eastern armies are these desert warriors who dig tunnels to take down structures in no time. Their low health makes them an easy target, so take care to use them wisely!";
                break;
            case "flyingottoman":
                targetText.text = "Anything";
                infoWarriorText.text = "A little curiosity and some lucky engineering feat granted this Ottoman the extraordinary ability of flight! He likes to drop death from above thanks to his timed bombs. His agility gives him an immunity to ground fire, and makes him a very deadly weapon for unprepared defenses and cities.";
                break;
            case "Battering_Gaul":
                targetText.text = "Anything";
                infoWarriorText.text = "This giant of a man comes from Alpine iron mines where he trains with trams and rocks to have an enormous strength! His durability and attack power make him probably the most powerful and feared Merc ever seen so far.";
                break;
            case "EgyptianSavage":
                targetText.text = "Defensive Building";
                infoWarriorText.text = "Protectors of the ancient pharaoh tombs for thousands of years, these brutal warriors are one of the strongest & deadliest in the field. They don't like getting hit by projectiles, but they will gladly launch themselves upon enemy soldiers!";
                break;
            case "Destroyer":
                targetText.text = "Anything";
                infoWarriorText.text = "Hailing from the wastelands of Britain, this half-Roman half-Saxon warrior-for-hire knows no fear in the battlefield! His heavy armor protects him against moderate damage, while his trusted crossbow and his cunning make him a deadly weapon against many enemies at the same time.";
                break;
            case "Poisonous":
                targetText.text = "Anything";
                infoWarriorText.text = "Only a few can resist an Aztec's poisonous darts. These seasoned hunters' offensive abilities compensate for their weak physiques. If protected well, they can kill almost anything in their path.";
                break;
            case "Sneaky":
                targetText.text = "Res. Producer, Stock Pile, City Center";
                infoWarriorText.text = "Having mastered the art of disguise, this devious woman can infiltrate any fortess (and can slice any watchman's throat). She prefers sneaking over a good old fashioned melee, so be careful when you plan your strategies with her!";
                break;
            case "ChineseMongolian":
                targetText.text = "Defensive Building";
                infoWarriorText.text = "These retired sumo wrestlers also found a place among the Mercenaries. They excel at destroying enemy towers with resilience thanks to their huge frame.";
                break;

            // √÷› «·„“Ìœ Õ”» «·Õ«Ã…

            default:
                targetText.text = "---";
                infoWarriorText.text = "---";
                break;
        }
    }
}
