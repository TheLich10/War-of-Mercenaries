using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WarriorLevel
{
    public int level;
    public float speed;
    public float damage;
    public float health;
    public int storageSpace;
    public string recruitTime;
    public int recruitCost;
    public int trainingCost;
    public string trainingTime;

    public WarriorLevel(int level, float speed, float damage, float health, int storageSpace,
                        string recruitTime, int recruitCost, int trainingCost, string trainingTime)
    {
        this.level = level;
        this.speed = speed;
        this.damage = damage;
        this.health = health;
        this.storageSpace = storageSpace;
        this.recruitTime = recruitTime;
        this.recruitCost = recruitCost;
        this.trainingCost = trainingCost;
        this.trainingTime = trainingTime;
    }
}

public class WarriorTring
{
    public string name;
    public Sprite image;
    public List<WarriorLevel> levels;

    public WarriorTring(string name, Sprite image)
    {
        this.name = name;
        this.image = image;
        this.levels = new List<WarriorLevel>();
    }

    public WarriorLevel GetLevel(int level)
    {
        return levels.Find(l => l.level == level);
    }
}

public static class WarriorTringDatabase
{
    public static List<WarriorTring> warriors = new List<WarriorTring>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» bedoiun ===
        Sprite bedoiunsprite = Resources.Load<Sprite>("imgeWoriors/bedoiun");
        var bedoiun = new WarriorTring("bedoiun", bedoiunsprite);
        bedoiun.levels.Add(new WarriorLevel(1, 1.2f, 60f, 200f, 10, "15S", 250, 0, "0S"));
        bedoiun.levels.Add(new WarriorLevel(2, 1.2f, 65f, 220f, 10, "14S", 450, 4000, "1H"));
        bedoiun.levels.Add(new WarriorLevel(3, 1.2f, 70f, 240f, 10, "8S", 650, 8000, "2H"));
        bedoiun.levels.Add(new WarriorLevel(4, 1.2f, 75f, 260f, 10, "7S", 800, 12000, "5H"));
        bedoiun.levels.Add(new WarriorLevel(5, 1.2f, 80f,280f, 10, "6S", 1000, 16000, "8H"));
        bedoiun.levels.Add(new WarriorLevel(6, 1.2f, 85f,300f, 10, "5S", 1250, 22000, "12H"));
        warriors.Add(bedoiun);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite ottmanSprite = Resources.Load<Sprite>("imgeWoriors/Ottoman");
        var Ottoman = new WarriorTring("Ottoman", ottmanSprite);
        Ottoman.levels.Add(new WarriorLevel(1, 1.4f, 15f, 1000f, 10, "15S", 500, 0, "0S"));
        Ottoman.levels.Add(new WarriorLevel(2, 1.4f, 15f, 1100f, 10, "15S", 525, 8000, "2H"));
        Ottoman.levels.Add(new WarriorLevel(3, 1.4f, 20f, 1300f, 10, "15S", 825, 16000, "4H"));
        Ottoman.levels.Add(new WarriorLevel(4, 1.4f, 25f, 1450f, 10, "15S", 1200, 24000, "10H"));
        Ottoman.levels.Add(new WarriorLevel(5, 1.4f, 30f, 1600f, 10, "15S", 1500, 48000, "16H"));
        Ottoman.levels.Add(new WarriorLevel(6, 1.4f, 35f, 1800f, 10, "15S", 2000, 64000, "1D"));
        warriors.Add(Ottoman);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite KnightOnHorseSpr = Resources.Load<Sprite>("imgeWoriors/KnightOnHorse");
        var KnightOnHorse = new WarriorTring("KnightOnHorse", KnightOnHorseSpr);
        KnightOnHorse.levels.Add(new WarriorLevel(1, 2.5f, 15f, 150f, 15, "23S", 350, 0, "0S"));
        KnightOnHorse.levels.Add(new WarriorLevel(2, 2.55f, 20f, 150f, 15, "23S", 475, 16000, "3H"));
        KnightOnHorse.levels.Add(new WarriorLevel(3, 2.6f, 25f, 150f, 15, "23S", 600, 32000, "6H"));
        KnightOnHorse.levels.Add(new WarriorLevel(4, 2.8f, 35f, 150f, 15, "23S", 900, 48000, "8H"));
        KnightOnHorse.levels.Add(new WarriorLevel(5, 3f, 45f, 150f, 15, "23S", 1250, 96000, "12H"));
        KnightOnHorse.levels.Add(new WarriorLevel(6, 3.2f, 55f, 200f, 15, "23S", 2200, 144000, "16H"));
        warriors.Add(KnightOnHorse);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite persianSprit = Resources.Load<Sprite>("imgeWoriors/persian");
        var persian = new WarriorTring("persian", persianSprit);
        persian.levels.Add(new WarriorLevel(1, 1.3f, 300f, 200f, 20, "1M", 1500, 0, "0S"));
        persian.levels.Add(new WarriorLevel(2, 1.3f, 330f, 200f, 20, "1M", 1700, 32000, "8H"));
        persian.levels.Add(new WarriorLevel(3, 1.3f, 380f, 200f, 20, "1M", 2000, 64000, "12H"));
        persian.levels.Add(new WarriorLevel(4, 1.3f, 430f, 200f, 20, "1M", 2500, 96000, "18H"));
        persian.levels.Add(new WarriorLevel(5, 1.3f, 470f, 220f, 20, "1M", 3000, 128000, "20H"));
        persian.levels.Add(new WarriorLevel(6, 1.3f, 520f, 240f, 20, "1M", 3500, 160000, "1D"));
        warriors.Add(persian);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite Kamikazespr = Resources.Load<Sprite>("imgeWoriors/Kamikaze");
        var Kamikaze = new WarriorTring("Kamikaze", Kamikazespr);
        Kamikaze.levels.Add(new WarriorLevel(1, 2f, 4000f, 600f, 60, "5M", 5000, 0, "0S"));
        Kamikaze.levels.Add(new WarriorLevel(2, 2.2f,8000f, 900f, 60, "5M", 15000, 64000, "5H"));
        Kamikaze.levels.Add(new WarriorLevel(3, 2.4f,12000f, 1200f, 60, "5M", 30000, 128000,"7H"));
        Kamikaze.levels.Add(new WarriorLevel(4, 2.6f,16000f, 1600f, 60, "5M", 45000, 192000, "12H"));
        Kamikaze.levels.Add(new WarriorLevel(5, 2.8f,20000f, 2000f, 60, "5M", 60000, 384000, "1D"));
        Kamikaze.levels.Add(new WarriorLevel(6, 3f,24000f, 2400f, 60, "5M", 80000, 512000, "2D"));
        warriors.Add(Kamikaze);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite ScalingEgyptianspr = Resources.Load<Sprite>("imgeWoriors/ScalingEgyptian");
        var ScalingEgyptian = new WarriorTring("ScalingEgyptian", ScalingEgyptianspr);
        ScalingEgyptian.levels.Add(new WarriorLevel(1, 0.9f, 1200f, 800f, 70, "10M", 60000, 0, "0S"));
        ScalingEgyptian.levels.Add(new WarriorLevel(2, 0.9f, 1400f, 900f, 70, "10M", 78750, 40960, "12H"));
        ScalingEgyptian.levels.Add(new WarriorLevel(3, 1f, 1600f, 950f, 70, "10M", 105000, 70000, "20H"));
        ScalingEgyptian.levels.Add(new WarriorLevel(4, 1.2f, 1800f, 1000f, 70, "10M", 150000, 120000, "1D"));
        ScalingEgyptian.levels.Add(new WarriorLevel(5, 1.2f, 2000f, 1100f, 70, "10M", 180000, 180000, "2H"));
        ScalingEgyptian.levels.Add(new WarriorLevel(6, 1.3f, 2200f, 1200f, 70, "10M", 240000, 240000, "3D"));
        warriors.Add(ScalingEgyptian);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite GentleHealerFullspr = Resources.Load<Sprite>("imgeWoriors/GentleHealerFull");
        var GentleHealerFull = new WarriorTring("GentleHealerFull", GentleHealerFullspr);
        GentleHealerFull.levels.Add(new WarriorLevel(1, 0.75f, 400f, 8000f, 200, "30M", 120000, 0, "0S"));
        GentleHealerFull.levels.Add(new WarriorLevel(2, 0.8f, 550f, 8000f, 200, "30M", 175000, 619200, "12H"));
        GentleHealerFull.levels.Add(new WarriorLevel(3, 0.85f, 700f, 8000f, 200, "30M", 240000, 780000, "1D"));
        GentleHealerFull.levels.Add(new WarriorLevel(4, 0.9f, 850f, 8000f, 200, "30M", 320000, 1200000, "2D"));
        GentleHealerFull.levels.Add(new WarriorLevel(5, 0.95f, 1000f, 8000f, 200, "30M", 380000, 1800000, "3D"));
        GentleHealerFull.levels.Add(new WarriorLevel(6, 1f, 1100f, 8000f, 200, "30M", 400000, 2000000, "4D"));
        warriors.Add(GentleHealerFull);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite PersianSapperspr = Resources.Load<Sprite>("imgeWoriors/PersianSapper");
        var PersianSapper = new WarriorTring("PersianSapper", PersianSapperspr);
        PersianSapper.levels.Add(new WarriorLevel(1, 3f, 700f, 1600f, 70, "20M", 20000, 0, "0S"));
        PersianSapper.levels.Add(new WarriorLevel(2, 4f, 800f, 1900f, 70, "20M", 25000, 409600, "2H"));
        PersianSapper.levels.Add(new WarriorLevel(3, 4f, 950f, 2100f, 70, "20M", 30000, 819200, "5H"));
        PersianSapper.levels.Add(new WarriorLevel(4, 4f, 1000f, 2400f, 70, "20M", 35000, 819200, "12H"));
        PersianSapper.levels.Add(new WarriorLevel(5, 4f, 1100f, 2700f, 70, "20M", 40000, 819200, "16H"));
        PersianSapper.levels.Add(new WarriorLevel(6, 4f, 1200f, 3000f, 70, "20M", 47500, 1280000, "1D"));
        warriors.Add(PersianSapper);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite flyingottomanspr = Resources.Load<Sprite>("imgeWoriors/flyingottoman");
        var flyingottoman = new WarriorTring("flyingottoman", flyingottomanspr);
        flyingottoman.levels.Add(new WarriorLevel(1, 2.5f, 300f, 1600f, 70, "25M", 70000, 0, "0S"));
        flyingottoman.levels.Add(new WarriorLevel(2, 2.75f,350f, 1900f, 70, "25M", 95000, 409600, "12H"));
        flyingottoman.levels.Add(new WarriorLevel(3, 3f,   400f, 2400f, 70, "25M", 145000, 700000, "1D"));
        flyingottoman.levels.Add(new WarriorLevel(4, 3.25f,500f, 3000f, 70, "25M", 200000, 1000000, "2D"));
        flyingottoman.levels.Add(new WarriorLevel(5, 3.5f, 600f, 3600f, 70, "25M", 300000, 1600000, "3D"));
        flyingottoman.levels.Add(new WarriorLevel(6, 3.5f, 700f, 4200f, 70, "25M", 400000, 2400000, "4D"));
        warriors.Add(flyingottoman);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite Battering_Gaulspr = Resources.Load<Sprite>("imgeWoriors/Battering_Gaul");
        var Battering_Gaul = new WarriorTring("Battering_Gaul", Battering_Gaulspr);
        Battering_Gaul.levels.Add(new WarriorLevel(1, 0.8f, 1500f, 8000f,  160, "1H", 150000, 0, "0S"));
        Battering_Gaul.levels.Add(new WarriorLevel(2, 0.85f, 1500f, 9100f, 160, "1H", 180000, 819200, "12H"));
        Battering_Gaul.levels.Add(new WarriorLevel(3, 0.9f, 1600f, 10000f, 160, "1H", 225000, 1000000, "1D"));
        Battering_Gaul.levels.Add(new WarriorLevel(4, 1f, 1700,  12000f,   160, "1H", 320000, 1220000, "2D"));
        Battering_Gaul.levels.Add(new WarriorLevel(5, 1.1f, 1800f, 16500f, 160, "1H", 500000, 1920000, "3D"));
        Battering_Gaul.levels.Add(new WarriorLevel(6, 1.2f, 1900f, 21000f, 160, "1H", 750000, 2800000, "5D"));
        warriors.Add(Battering_Gaul);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite EgyptianSavagespr = Resources.Load<Sprite>("imgeWoriors/EgyptianSavage");
        var EgyptianSavage = new WarriorTring("EgyptianSavage", EgyptianSavagespr);
        EgyptianSavage.levels.Add(new WarriorLevel(1, 1.2f, 50f, 2000f, 20, "3M", 5000, 0, "0S"));
        EgyptianSavage.levels.Add(new WarriorLevel(2, 1.2f, 60f, 2100f, 20, "3M", 5625, 128000, "5H"));
        EgyptianSavage.levels.Add(new WarriorLevel(3, 1.2f, 70f, 2200f, 20, "3M", 8440, 256000, "12H"));
        EgyptianSavage.levels.Add(new WarriorLevel(4, 1.2f, 80f, 2300f, 20, "3M", 10250, 409600, "20H"));
        EgyptianSavage.levels.Add(new WarriorLevel(5, 1.2f, 95f, 2500f, 20, "3M", 13000, 640000, "1D"));
        EgyptianSavage.levels.Add(new WarriorLevel(6, 1.2f, 110f, 2800f,20, "3M", 17000, 820000, "1D"));
        warriors.Add(EgyptianSavage);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite Destroyerspr = Resources.Load<Sprite>("imgeWoriors/Destroyer");
        var Destroyer = new WarriorTring("Destroyer", Destroyerspr);
        Destroyer.levels.Add(new WarriorLevel(1, 1f, 200f, 500f, 20, "4M", 2500, 0, "0S"));
        Destroyer.levels.Add(new WarriorLevel(2, 1f, 250f, 550f, 20, "4M", 3500, 256000, "12H"));
        Destroyer.levels.Add(new WarriorLevel(3, 1f, 300f, 600f, 20, "4M", 4500, 512000, "16H"));
        Destroyer.levels.Add(new WarriorLevel(4, 1f, 350f, 650f, 20, "4M", 5500, 756000, "1D"));
        Destroyer.levels.Add(new WarriorLevel(5, 1f, 400f, 750f, 20, "4M", 7500, 1024000, "2D"));
        Destroyer.levels.Add(new WarriorLevel(6, 1f, 450f, 900f, 20, "4M", 10000, 1444000, "3D"));
        warriors.Add(Destroyer);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite Poisonousspr = Resources.Load<Sprite>("imgeWoriors/Poisonous");
        var Poisonous = new WarriorTring("Poisonous", Poisonousspr);
        Poisonous.levels.Add(new WarriorLevel(1, 1.1f, 600f, 400f, 30, "6M", 18000, 0, "0S"));
        Poisonous.levels.Add(new WarriorLevel(2, 1.2f, 600f, 400f, 30, "6M", 25000, 512000, "2H"));
        Poisonous.levels.Add(new WarriorLevel(3, 1.3f, 620f, 400f, 30, "6M", 40000, 512000, "5H"));
        Poisonous.levels.Add(new WarriorLevel(4, 1.4f, 660f, 400f, 30, "6M", 50000, 756000, "10H"));
        Poisonous.levels.Add(new WarriorLevel(5, 1.5f, 720f, 400f, 30, "6M", 68000, 1024000, "12H"));
        Poisonous.levels.Add(new WarriorLevel(6, 1.6f, 800f, 400f, 30, "6M", 80000, 1140000, "16H"));
        warriors.Add(Poisonous);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite Sneakyspr = Resources.Load<Sprite>("imgeWoriors/Sneaky");
        var Sneaky = new WarriorTring("Sneaky", Sneakyspr);
        Sneaky.levels.Add(new WarriorLevel(1, 2f, 100f, 600f, 30, "4M", 4000, 0, "0S"));
        Sneaky.levels.Add(new WarriorLevel(2, 2f, 100f, 700f, 30, "4M", 5000, 102400, "2H"));
        Sneaky.levels.Add(new WarriorLevel(3, 2f, 200f, 750f, 30, "4M", 10000, 205600, "5H"));
        Sneaky.levels.Add(new WarriorLevel(4, 2f, 250f, 800f, 30, "4M", 12000, 287000, "10H"));
        Sneaky.levels.Add(new WarriorLevel(5, 2.1f, 300f, 1100f, 30, "4M", 16000, 450000, "12H"));
        Sneaky.levels.Add(new WarriorLevel(6, 3.2f, 350f, 1400f, 30, "4M", 25000, 600000, "15H"));
        warriors.Add(Sneaky);
        //lvl - speed - dmg - HP - strogeSpcae - timer - costIron - costTraing - TimeTraing
        // === «·„Õ«—» ottman ===
        Sprite ChineseMongolianspr = Resources.Load<Sprite>("imgeWoriors/ChineseMongolian");
        var ChineseMongolian = new WarriorTring("ChineseMongolian", ChineseMongolianspr);
        ChineseMongolian.levels.Add(new WarriorLevel(1, 1f, 100f, 4000f, 40, "8M", 30000, 0, "0S"));
        ChineseMongolian.levels.Add(new WarriorLevel(2, 1f, 120f, 4000f, 40, "8M", 36000, 204800, "12H"));
        ChineseMongolian.levels.Add(new WarriorLevel(3, 1f, 130f, 4300f, 40, "8M", 42000, 300000, "18H"));
        ChineseMongolian.levels.Add(new WarriorLevel(4, 1.2f, 140f, 4400f, 40, "8M", 55500, 440000, "1D"));
        ChineseMongolian.levels.Add(new WarriorLevel(5, 1.4f, 150f, 4600f, 40, "8M", 72500, 600000, "2D"));
        ChineseMongolian.levels.Add(new WarriorLevel(6, 1.5f, 170f, 4800f, 40, "8M", 92000, 750000, "3D"));
        warriors.Add(ChineseMongolian);

    }

    public static WarriorTring GetWarriorByName(string name)
    {
        Initialize();
        return warriors.Find(w => w.name == name);
    }
}
