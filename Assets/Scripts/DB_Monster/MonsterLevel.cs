using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterLevel
{
    public int level;
    public float health;
    public float speed;
    public float damage;
    public float range;
    public float healTime;
    public int cost1;
    public int cost2;
    public float spawnTime;
    public Sprite image; // ’Ê—… Œ«’… »Â–« «·„” ÊÏ

    public MonsterLevel(int level, float health, float speed, float damage, float range, float healTime, int cost1, int cost2, float spawnTime, Sprite image)
    {
        this.level = level;
        this.health = health;
        this.speed = speed;
        this.damage = damage;
        this.range = range;
        this.healTime = healTime;
        this.cost1 = cost1;
        this.cost2 = cost2;
        this.spawnTime = spawnTime;
        this.image = image;
    }
}

public class MonsterData
{
    public string name;
    public List<MonsterLevel> levels;

    public MonsterData(string name)
    {
        this.name = name;
        this.levels = new List<MonsterLevel>();
    }

    public MonsterLevel GetLevel(int level)
    {
        return levels.Find(l => l.level == level);
    }
}

public static class MonsterDatabase
{
    public static List<MonsterData> monsters = new List<MonsterData>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;

        // Monster: womknog
        var womknog = new MonsterData("Wom-kong");

        for (int i = 1; i <= 6; i++)
        {
            Sprite levelImage = Resources.Load<Sprite>($"monsterLevels/womkong/womkong{i}"); //  √ﬂœ „‰ √‰ «·’Ê— „”„«… »Â–« «·‘ﬂ·
            switch (i)
            {
                //                                level = HP - speed - Damg - range -  healTime - cost1 - cost2 - time
                case 1:
                    womknog.levels.Add(new MonsterLevel(1, 15000, 1.6f, 2000f,1, 10, 2, 2, 2f, levelImage));
                    break;
                case 2:
                    womknog.levels.Add(new MonsterLevel(2, 25000, 1.8f, 2400f, 1, 10, 2, 25, 6f, levelImage));
                    break;
                case 3:
                    womknog.levels.Add(new MonsterLevel(3, 40000, 2.0f, 2800f, 1, 10, 4, 25, 9f, levelImage));
                    break;
                case 4:
                    womknog.levels.Add(new MonsterLevel(4, 65000, 2.3f, 3600f, 1, 10, 6, 50, 12f, levelImage));
                    break;
                case 5:
                    womknog.levels.Add(new MonsterLevel(5, 90000, 2.6f, 4000f, 1, 10, 10, 10, 15f, levelImage));
                    break;
                case 6:
                    womknog.levels.Add(new MonsterLevel(6, 120000, 3.0f, 4800f, 1, 10, 20, 20, 4.0f, levelImage));
                    break;
            }
        }
        monsters.Add(womknog);

        // Monster: y3ssob
        var y3ssob = new MonsterData("DraginFly");

        for (int i = 1; i <= 6; i++)
        {
            Sprite levelImage = Resources.Load<Sprite>($"monsterLevels/berid/berid{i}"); //  √ﬂœ „‰ √‰ «·’Ê— „”„«… »Â–« «·‘ﬂ·
            switch (i)
            {
                //                                level = HP - speed - Damg - range -  healTime - cost1 - cost2 - time
                case 1:
                    y3ssob.levels.Add(new MonsterLevel(1, 15000, 1.2f, 70f, 40, 10, 10, 10, 2f, levelImage));
                    break;
                case 2:
                    y3ssob.levels.Add(new MonsterLevel(2, 17000, 1.4f, 80f, 40, 20, 20, 5, 6f, levelImage));
                    break;
                case 3:
                    y3ssob.levels.Add(new MonsterLevel(3, 20000, 2.0f, 90f, 40, 30, 40, 5, 9f, levelImage));
                    break;
                case 4:
                    y3ssob.levels.Add(new MonsterLevel(4, 22500, 2.1f, 100f, 40, 40, 30, 10, 12f, levelImage));
                    break;
                case 5:
                    y3ssob.levels.Add(new MonsterLevel(5, 25000, 2.2f, 110f, 40, 50, 10, 10, 15f, levelImage));
                    break;
                case 6:
                    y3ssob.levels.Add(new MonsterLevel(6, 40000, 2.3f, 120f, 40, 60, 10, 10, 4.0f, levelImage));
                    break;
            }
        }
        monsters.Add(y3ssob);

        // Monster: BearWolf
        var BearWolf = new MonsterData("BearWolf");

        for (int i = 1; i <= 6; i++)
        {
            Sprite levelImage = Resources.Load<Sprite>($"monsterLevels/bear/bear{i}"); //  √ﬂœ „‰ √‰ «·’Ê— „”„«… »Â–« «·‘ﬂ·
            switch (i)
            {
                //                                level = HP - speed - Damg - range -  healTime - cost1 - cost2 - time
                case 1:
                    BearWolf.levels.Add(new MonsterLevel(1, 40000, 1f, 1000f, 1, 0, 15, 0, 2f, levelImage));
                    break;
                case 2:
                    BearWolf.levels.Add(new MonsterLevel(2, 80000, 1.2f, 1200f, 1, 0, 10, 5, 6f, levelImage));
                    break;
                case 3:
                    BearWolf.levels.Add(new MonsterLevel(3, 120000, 1.4f, 1500f, 1, 0, 20, 0, 9f, levelImage));
                    break;
                case 4:
                    BearWolf.levels.Add(new MonsterLevel(4, 140000, 1.6f, 2000f, 1, 0, 10, 10, 12f, levelImage));
                    break;
                case 5:
                    BearWolf.levels.Add(new MonsterLevel(5, 160000, 1.8f, 2500f, 1, 0, 20, 0, 15f, levelImage));
                    break;
                case 6:
                    BearWolf.levels.Add(new MonsterLevel(6, 200000, 2.0f, 3000f, 1, 0, 20, 0, 4.0f, levelImage));
                    break;
            }
        }
        monsters.Add(BearWolf);

        // Monster: y3ssob
        var Mightosour = new MonsterData("Mightosour");

        for (int i = 1; i <= 6; i++)
        {
            Sprite levelImage = Resources.Load<Sprite>($"monsterLevels/elefnt/elefnt{i}"); //  √ﬂœ „‰ √‰ «·’Ê— „”„«… »Â–« «·‘ﬂ·
            switch (i)
            {
                //                                level = HP - speed - Damg - range -  healTime - cost1 - cost2 - time
                case 1:
                    Mightosour.levels.Add(new MonsterLevel(1, 12000, 2f, 3000f, 1, 0, 30, 0, 2f, levelImage));
                    break;
                case 2:
                    Mightosour.levels.Add(new MonsterLevel(2, 20000, 2.2f, 3600f, 1, 0, 20, 15, 6f, levelImage));
                    break;
                case 3:
                    Mightosour.levels.Add(new MonsterLevel(3, 36000, 2.5f, 4200f, 1, 0, 50, 9, 9f, levelImage));
                    break;
                case 4:
                    Mightosour.levels.Add(new MonsterLevel(4, 42000, 2.8f, 5500f, 1, 0, 10, 15, 12f, levelImage));
                    break;
                case 5:
                    Mightosour.levels.Add(new MonsterLevel(5, 52000, 3.2f, 6500f, 1, 0, 30, 0, 15f, levelImage));
                    break;
                case 6:
                    Mightosour.levels.Add(new MonsterLevel(6, 60000, 3.6f, 8000f, 1, 0, 30, 0, 4.0f, levelImage));
                    break;
            }
        }
        monsters.Add(Mightosour);
    }

    public static MonsterData GetMonsterByName(string name)
    {
        Initialize();
        return monsters.Find(m => m.name == name);
    }
}
