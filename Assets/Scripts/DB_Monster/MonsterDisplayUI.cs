using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class MonsterDisplayUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text speedText;
    public TMP_Text damageText;
    public TMP_Text healthText;
    public TMP_Text recruitTimeText;
    public TMP_Text Cost1TEXT;
    public TMP_Text COST2tEXT;
    public TMP_Text trainingTimeText;
    public Image monsterImage;

    [Header("Next Level Stats")]
    public TMP_Text speedAfterText;
    public TMP_Text damageAfterText;
    public TMP_Text healthAfterText;
    public TMP_Text recruitTimeAfterText;
    public Image monsterImageAfter;

    public TMP_Text Mondter_Info;



    void Start()
    {


        string monsterName = GetObjectNameAndText.objectName;
        if (monsterName == "CaveEnter" && HeroArcher2.isActiveHero == true)
        {
            monsterName = "DraginFly"; // ← تحقق من الاسم الصحيح للوحش
            Mondter_Info.text = "Trained in the desert heat by its ruthless master, this creature is known in mythology as the ruler of the skies. According to legends, no one has ever faced this mysterious monster, which draws power from fire, and survived. The only traces of its existence are burnt fields and villages engulfed in flames.";

        }
        else if (monsterName == "CaveEnter" && HeroArcher.isActiveHero == true)
        {
            monsterName = "Wom-kong"; // ← تحقق من الاسم الصحيح للوحش
            Mondter_Info.text = "Wom-Kong came from the endless jungles of Africa; he can attack both air and ground units from a long distance and destroy enemies with his deadly bananas!";

        }
        else if (monsterName == "CaveEnter" && HeroUnit.isActiveHero == true)
        {
            monsterName = "BearWolf"; // ← تحقق من الاسم الصحيح للوحش
            Mondter_Info.text = "Mentioned in Roman mythology, this powerful creature has teeth as sharp as a wolf's and claws as strong as a bear's. Spreading fear and terror onto the battlefields, this monster knows no rivals with its power and balance in both attack and defense.";

        }
        else if (monsterName == "CaveEnter" && HeroUnit2.isActiveHero == true)
        {
            monsterName = "Mightosour"; // ← تحقق من الاسم الصحيح للوحش
            Mondter_Info.text = "Hardly contained even with chains, this giant monster has the power to trample everything in its path under its feet. A nightmare for defenses with its massive size and long tusks, there is no target this creature cannot destroy within seconds!";

        }

        int level = GetObjectNameAndText.savedNumber;
        if (!string.IsNullOrEmpty(monsterName) && level > 0)
        {
            MonsterData monster = MonsterDatabase.GetMonsterByName(monsterName);

            if (monster != null)
            {
                MonsterLevel monsterLevel = monster.GetLevel(level);
                if (monsterLevel != null)
                {
                    nameText.text = monster.name;
                    speedText.text = monsterLevel.speed.ToString();
                    damageText.text = monsterLevel.damage.ToString();
                    healthText.text = monsterLevel.health.ToString();
                    recruitTimeText.text = monsterLevel.healTime.ToString();
                    Cost1TEXT.text = monsterLevel.cost1.ToString();
                    COST2tEXT.text = monsterLevel.cost2.ToString();
                    trainingTimeText.text = monsterLevel.spawnTime.ToString();

                    if (monsterLevel.image != null)
                    {
                        monsterImage.sprite = monsterLevel.image;
                        monsterImage.preserveAspect = true;
                    }

                    // المستوى التالي
                    if (level < 6) // فقط إذا لم نصل إلى أقصى مستوى
                    {
                        MonsterLevel nextLevel = monster.GetLevel(level + 1);
                        if (nextLevel != null)
                        {
                            speedAfterText.text = nextLevel.speed.ToString();
                            damageAfterText.text = nextLevel.damage.ToString();
                            healthAfterText.text = nextLevel.health.ToString();
                            recruitTimeAfterText.text = nextLevel.healTime.ToString();

                            if (nextLevel.image != null)
                            {
                                monsterImageAfter.sprite = nextLevel.image;
                                monsterImageAfter.preserveAspect = true;
                            }
                        }
                        else
                        {
                            speedAfterText.text = "-";
                            damageAfterText.text = "-";
                            healthAfterText.text = "-";
                            recruitTimeAfterText.text = "-";
                            monsterImageAfter.sprite = monsterLevel.image;
                            monsterImageAfter.preserveAspect = true;

                        }
                    }
                    else
                    {
                        // المستوى 6: لا يوجد مستوى بعده
                        speedAfterText.text = "-";
                        damageAfterText.text = "-";
                        healthAfterText.text = "-";
                        recruitTimeAfterText.text = "-";
                        monsterImageAfter.sprite = monsterLevel.image;
                        monsterImageAfter.preserveAspect = true;

                    }



                }
                else
                {
                    Debug.LogWarning($"لم يتم العثور على المستوى {level} للوحش {monsterName}");
                }
            }
            else
            {
                Debug.LogWarning($"لم يتم العثور على الوحش بالاسم: {monsterName}");
            }
        }

    }

    void Update()
    {
        string monsterName = GetObjectNameAndText.objectName;
        if (monsterName == "CaveEnter" && HeroArcher2.isActiveHero == true)
        {
            monsterName = "DraginFly"; // ← تحقق من الاسم الصحيح للوحش
            Mondter_Info.text = "Trained in the desert heat by its ruthless master, this creature is known in mythology as the ruler of the skies. According to legends, no one has ever faced this mysterious monster, which draws power from fire, and survived. The only traces of its existence are burnt fields and villages engulfed in flames.";
        }
        else if (monsterName == "CaveEnter" && HeroArcher.isActiveHero == true)
        {
            monsterName = "Wom-kong"; // ← تحقق من الاسم الصحيح للوحش
            Mondter_Info.text = "Wom-Kong came from the endless jungles of Africa; he can attack both air and ground units from a long distance and destroy enemies with his deadly bananas!";
        }
        else if (monsterName == "CaveEnter" && HeroUnit.isActiveHero == true)
        {
            monsterName = "BearWolf"; // ← تحقق من الاسم الصحيح للوحش
            Mondter_Info.text = "Mentioned in Roman mythology, this powerful creature has teeth as sharp as a wolf's and claws as strong as a bear's. Spreading fear and terror onto the battlefields, this monster knows no rivals with its power and balance in both attack and defense.";
        }
        else if (monsterName == "CaveEnter" && HeroUnit2.isActiveHero == true)
        {
            monsterName = "Mightosour"; // ← تحقق من الاسم الصحيح للوحش
            Mondter_Info.text = "Hardly contained even with chains, this giant monster has the power to trample everything in its path under its feet. A nightmare for defenses with its massive size and long tusks, there is no target this creature cannot destroy within seconds!";

        }
        else { return; }
        int level = GetObjectNameAndText.savedNumber;

        if (!string.IsNullOrEmpty(monsterName) && level > 0)
        {
            MonsterData monster = MonsterDatabase.GetMonsterByName(monsterName);

            if (monster != null)
            {
                MonsterLevel monsterLevel = monster.GetLevel(level);
                if (monsterLevel != null)
                {
                    nameText.text = monster.name;
                    speedText.text = monsterLevel.speed.ToString();
                    damageText.text = monsterLevel.damage.ToString();
                    healthText.text = monsterLevel.health.ToString();
                    recruitTimeText.text = monsterLevel.healTime.ToString();
                    Cost1TEXT.text = monsterLevel.cost1.ToString();
                    COST2tEXT.text = monsterLevel.cost2.ToString();
                    trainingTimeText.text = monsterLevel.spawnTime.ToString();
                    //levelText.text = monsterLevel.level.ToString();


                    if (monsterLevel.image != null)
                    {
                        monsterImage.sprite = monsterLevel.image;
                        monsterImage.preserveAspect = true;
                    }

                    // المستوى التالي
                    if (level < 6) // فقط إذا لم نصل إلى أقصى مستوى
                    {
                        MonsterLevel nextLevel = monster.GetLevel(level + 1);
                        if (nextLevel != null)
                        {
                            speedAfterText.text = nextLevel.speed.ToString();
                            damageAfterText.text = nextLevel.damage.ToString();
                            healthAfterText.text = nextLevel.health.ToString();
                            recruitTimeAfterText.text = nextLevel.healTime.ToString();

                            if (nextLevel.image != null)
                            {
                                monsterImageAfter.sprite = nextLevel.image;
                                monsterImageAfter.preserveAspect = true;
                            }
                        }
                        else
                        {
                            speedAfterText.text = "-";
                            damageAfterText.text = "-";
                            healthAfterText.text = "-";
                            recruitTimeAfterText.text = "-";
                            monsterImageAfter.sprite = monsterLevel.image;
                            monsterImageAfter.preserveAspect = true;

                        }
                    }
                    else
                    {
                        // المستوى 6: لا يوجد مستوى بعده
                        speedAfterText.text = "-";
                        damageAfterText.text = "-";
                        healthAfterText.text = "-";
                        recruitTimeAfterText.text = "-";
                        monsterImageAfter.sprite = monsterLevel.image;
                        monsterImageAfter.preserveAspect = true;

                    }


                }
                else
                {
                    Debug.LogWarning($"لم يتم العثور على المستوى {level} للوحش {monsterName}");
                }
            }
            else
            {
                Debug.LogWarning($"لم يتم العثور على الوحش بالاسم: {monsterName}");
            }


           
        }
    }
}
