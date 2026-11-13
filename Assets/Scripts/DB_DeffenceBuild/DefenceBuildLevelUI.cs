using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DefenceBuildLevelUI
{
    public int level;
    public float health;
    public float damage;
    public float shotsPerCharge;
    public float range;
    public float barrackSpace;
    public string timeRequire;
    public int woodRequired;
    public int stoneRequired;
    public int powerRequired;
    public int ironRequired;
    public Sprite image;
    public string requriment;

    public DefenceBuildLevelUI(int level, float health, float damage, float shotsPerCharge, float range, float barrackSpace,string timeRequire, int woodRequired,int stoneRequired, int powerRequired, int ironRequired, Sprite image, string requriment)
    {
        this.level = level;
        this.health = health;
        this.damage = damage;
        this.shotsPerCharge = shotsPerCharge;
        this.range = range;
        this.barrackSpace = barrackSpace;
        this.timeRequire = timeRequire;
        this.woodRequired = woodRequired;
        this.stoneRequired = stoneRequired;
        this.powerRequired = powerRequired;
        this.ironRequired = ironRequired;
        this.image = image;
        this.requriment = requriment;
    }
}

public class DefenceBuildData
{
    public string name;
    public List<DefenceBuildLevelUI> levels;

    public DefenceBuildData(string name)
    {
        this.name = name;
        this.levels = new List<DefenceBuildLevelUI>();
    }

    public DefenceBuildLevelUI GetLevel(int level)
    {
        return levels.Find(l => l.level == level);
    }
}

public static class DefenceBuilds
{
    public static List<DefenceBuildData> buildings = new List<DefenceBuildData>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;

        //lvl - HP - dmg - shotPer - range - spaceBarck -time - wood - stone - power - iron - requriment
        AddBuilding("ARCHER'S TOWER", "archerT", new[]
        {
            new DefenceBuildLevelData(1, 6000f, 100f, 1.5f, 7f, 0f,"0s",0,0,0,0, ""),
            new DefenceBuildLevelData(2, 9000f, 210f, 1.5f, 7.2f, 0f,"15M",7500,10000,2500,0, "City Center l2"),
            new DefenceBuildLevelData(3, 12600f, 320f, 1.5f, 7.4f, 0f,"45M",37500,50000,12500,0, "City Center l3"),
            new DefenceBuildLevelData(4, 17640f, 430f, 1.5f, 8f, 0f,"5H",187500,250000,62500,0, "City Center l4"),
            new DefenceBuildLevelData(5, 26460f, 540f, 1.5f, 8.2f, 0f,"12H",937500,1250000,312500,0, "City Center l5"),
            new DefenceBuildLevelData(6, 34400f, 650f, 1.5f, 8.4f, 0f,"1D",4687500,6250000,1562500,0, "City Center l6"),
            new DefenceBuildLevelData(7, 45500f, 760f, 1.60f, 8.8f, 0f,"1D",7031250,9375000,2343750,0, "City Center l7"),
            new DefenceBuildLevelData(8, 58000f, 870f, 1.6f, 9f, 0f,"2D",10547000,14062000,3515000,0, "City Center l8"),
            new DefenceBuildLevelData(9, 75500f, 980f, 1.7f, 9.2f, 0f,"2D",15820000,21095000,5275000,0, "City Center l8"),
            new DefenceBuildLevelData(10, 98200f, 1100f, 1.8f, 9.5f, 0f,"3D",32730000,31650000,7900000,0, "City Center l8"),
            new DefenceBuildLevelData(11, 110000f, 1200f, 2f, 10f, 0f,"4D",45000000,45000000,13000000,0, "City Center l10"),

        });

        //lvl - HP - dmg - shotPer - range - spaceBarck -time - wood - stone - power - iron - requriment
        AddBuilding("GALTING ARROW TOWER", "archerop", new[]
        {
            new DefenceBuildLevelData(1, 25000f, 100f, 0.2f, 6f, 0f,"0S",0,0,0,0, ""),
            new DefenceBuildLevelData(2, 27500f, 120f, 0.2f, 6.2f, 0f,"12H",750000,1000000,250000,0, "City Center l5"),
            new DefenceBuildLevelData(3, 30000f, 140f, 0.2f, 6.5f, 0f,"1D",2250000,3000000,750000,0, "City Center l6"),
            new DefenceBuildLevelData(4, 48000f, 160f, 0.2f, 7f, 0f,"1D",5250000,5000000,1250000,0, "City Center l7"),
            new DefenceBuildLevelData(5, 60000f, 180f, 0.2f, 7.5f, 0f,"2D",12000000,10000000,2000000,0, "City Center l8"),
            new DefenceBuildLevelData(6, 72000f, 200f, 0.2f, 8f, 0f,"3D",18000000,15000000,5000000,0, "City Center l9"),

        });

        //lvl - HP - dmg - shotPer - range - spaceBarck -time - wood - stone - power - iron - requriment
        AddBuilding("FLAMER TOWER", "flame", new[]
        {
            new DefenceBuildLevelData(1, 9000f, 67f, 0.5f, 4f, 0f,"0S",0,0,0,0, ""),
            new DefenceBuildLevelData(2, 12600f, 83f, 0.5f, 4.1f, 0f,"1D",1000000,500000,200000,0, "City Center l5"),
            new DefenceBuildLevelData(3, 17640f, 100f, 0.5f, 4.2f, 0f,"1D",2000000,1000000,400000,0, "City Center l6"),
            new DefenceBuildLevelData(4, 26460f, 111f, 0.5f, 4.3f, 0f,"2D",4000000,2000000,800000,0, "City Center l7"),
            new DefenceBuildLevelData(5, 34400f, 122f, 0.5f, 4.4f, 0f,"3D",8000000,4000000,1600000,0, "City Center l8"),
            new DefenceBuildLevelData(6, 42200f, 133f, 0.5f, 4.5f, 0f,"4D",16000000,8000000,3200000,0, "City Center l9"),


        });

        //lvl - HP - dmg - shotPer - range - spaceBarck -time - wood - stone - power - iron - requriment
        AddBuilding("SKY TOWER", "sky", new[]
        {
            new DefenceBuildLevelData(1, 15000f, 200f, 0.1f, 7f, 0f,"0S",0,0,0,0, ""),
            new DefenceBuildLevelData(2, 22000f, 250f, 0.1f, 7.5f, 0f,"12H",850000,1200000,250000,0, "City Center l5"),
            new DefenceBuildLevelData(3, 30000f, 250f, 0.1f, 7.8f, 0f,"1D",2750000,3400000,750000,0, "City Center l6"),
            new DefenceBuildLevelData(4, 48000f, 250f, 0.1f, 8f, 0f,"1D",5750000,5200000,1250000,0, "City Center l7"),
            new DefenceBuildLevelData(5, 60000f, 300f, 0.1f, 8.5f, 0f,"2D",13500000,11000000,2000000,0, "City Center l8"),
            new DefenceBuildLevelData(6, 72000f, 350f, 0.1f, 9f, 0f,"3D",16000000,14000000,4000000,0, "City Center l9"),

        });

        //lvl - HP - dmg - shotPer - range - spaceBarck -time - wood - stone - power - iron - requriment
        AddBuilding("WATCH POST", "watch", new[]
        {
            new DefenceBuildLevelData(1, 10000f, 0f, 0f, 8f, 380f,"0S",0,0,0,0, ""),
            new DefenceBuildLevelData(2, 24500f, 0f, 0f, 8.5f, 450f,"12S",1000000,1000000,500000,0, "City Center l4 , Mercenary Barracks L2"),
            new DefenceBuildLevelData(3, 52000f, 0f, 0f, 9f, 540f,"1D",2000000,2000000,1000000,0, "City Center l5 , Mercenary Barracks L3"),
            new DefenceBuildLevelData(4, 75000f, 0f, 0f, 10f, 660f,"2D",4000000,4000000,2000000,0, "City Center l9"),


        });

        //lvl - HP - dmg - shotPer - range - spaceBarck -time - wood - stone - power - iron - requriment
        AddBuilding("BOMBARD TOWER", "boomT", new[]
        {
            new DefenceBuildLevelData(1, 6000f, 20f, 1.1f, 5.5f, 0f,"0S",0,0,0,0, "City Center l2"),
            new DefenceBuildLevelData(2, 9000f, 40f, 1.2f, 5.6f, 0f,"12H",10000,7500,2500,0, "City Center l2"),
            new DefenceBuildLevelData(3, 12600f, 60f, 1.3f, 5.7f, 0f,"1D",50000,37500,12500,0, "City Center l3"),
            new DefenceBuildLevelData(4, 17640f, 80f, 1.4f, 5.8f, 0f,"1D",250000, 187500,62500,0, "City Center l4"),
            new DefenceBuildLevelData(5, 26460f, 100f, 1.5f, 6f, 0f,"2D",1250000,937500,312500,0, "City Center l5"),
            new DefenceBuildLevelData(6, 34400f, 120f, 1.5f, 6.1f, 0f,"2D",6250000, 4687500,1562500,0, "City Center l6"),
            new DefenceBuildLevelData(7, 45500f, 140f, 1.5f, 6.2f, 0f,"3D",9375000, 7031250,2343750,0, "City Center l7"),
            new DefenceBuildLevelData(8, 58000f, 160f, 1.6f, 6.3f, 0f,"3D",14062000, 10547000,3515000,0, "City Center l8"),
            new DefenceBuildLevelData(9, 75500f, 180f, 1.7f, 6.4f, 0f,"4D",21095000, 15820000,5275000,0, "City Center l8"),
            new DefenceBuildLevelData(10, 98200f, 200f, 1.8f, 6.5f, 0f,"5D",31650000,32730000,7900000,0, "City Center l8"),

        });

        //lvl - HP - dmg - shotPer - range - spaceBarck -time - wood - stone - power - iron - requriment
        AddBuilding("BEAST CANNON", "boomOp", new[]
        {
            new DefenceBuildLevelData(1, 35000f, 2.5f, 1.5f, 10f, 0f,"0S",0,0,0,0, ""),
            new DefenceBuildLevelData(2, 45000f, 3f, 1.5f, 10.5f, 0f,"1D",10000000,10000000,7500000,0, "City Center l7"),
            new DefenceBuildLevelData(3, 60000f, 3.5f, 1.5f, 11f, 0f,"1D",20000000,20000000,15000000,0, "City Center l8"),
            new DefenceBuildLevelData(4, 80000f, 4f, 1.5f, 11.5f, 0f,"2D",25000000,25000000,20000000,0, "City Center l8"),
            new DefenceBuildLevelData(5, 100000f, 4.5f, 1.5f, 11.8f, 0f,"2D",35000000,35000000,30000000,0, "City Center l9"),
            new DefenceBuildLevelData(6, 110000f, 5f, 1.5f, 12f, 0f,"3D",40000000,40000000,35000000,0, "City Center l10"),


        });

        //lvl - HP - dmg - shotPer - range - spaceBarck -time - wood - stone - power - iron - requriment
        AddBuilding("BURNING MIRRORS", "mirr", new[]
        {
            new DefenceBuildLevelData(1, 15000f, 400f, 3f, 8f, 0f,"0S",0,0,0,0, ""),
            new DefenceBuildLevelData(2, 22000f, 600f, 3f, 8.5f, 0f,"1D",3600000,4320000,2880000,0, "City Center l6"),
            new DefenceBuildLevelData(3, 30000f, 900f, 3f, 8.8f, 0f,"2D",6480000,7776000,5184000,0, "City Center l7"),
            new DefenceBuildLevelData(4, 48000f, 1200f, 3f, 9f, 0f,"3D",11664000,13996800,9331200,0, "City Center l7"),
            new DefenceBuildLevelData(5, 60000f, 1600f, 3f, 9.5f, 0f,"4D",16995200,18194240,16796160,0, "City Center l8"),
            new DefenceBuildLevelData(6, 72000f, 2000f, 3f, 10f, 0f,"5D",20220000,24202000,19000000,0, "City Center l9"),

        });


    }

    private static void AddBuilding(string name, string spritePrefix, DefenceBuildLevelData[] dataArray)
    {
        var building = new DefenceBuildData(name);
        foreach (var data in dataArray)
        {
            string spritePath = $"defenceIcons/{spritePrefix}{data.level}";
            Sprite sprite = Resources.Load<Sprite>(spritePath);

            var level = new DefenceBuildLevelUI(
                data.level, data.health, data.damage, data.shotsPerCharge, data.range, data.barrackSpace,data.timeRequire,data.woodRequired,data.stoneRequired,data.powerRequired,data.ironRequired, sprite, data.requriment
            );
            building.levels.Add(level);
        }
        buildings.Add(building);
    }

    private class DefenceBuildLevelData
    {
        public int level;
        public float health;
        public float damage;
        public float shotsPerCharge;
        public float range;
        public float barrackSpace;
        public string timeRequire;
        public int woodRequired;
        public int stoneRequired;
        public int powerRequired;
        public int ironRequired;
        public string requriment;

        public DefenceBuildLevelData(int level, float health, float damage, float shotsPerCharge, float range, float barrackSpace,string timeRequire, int woodRequired, int stoneRequired, int powerRequired, int ironRequired, string requriment)
        {
            this.level = level;
            this.health = health;
            this.damage = damage;
            this.shotsPerCharge = shotsPerCharge;
            this.range = range;
            this.barrackSpace = barrackSpace;
            this.timeRequire = timeRequire;
            this.woodRequired = woodRequired;
            this.stoneRequired = stoneRequired;
            this.powerRequired = powerRequired;
            this.ironRequired = ironRequired;
            this.requriment = requriment;
        }
    }

    public static DefenceBuildData GetBuildingByName(string name)
    {
        Initialize();
        return buildings.Find(b => b.name == name);
    }
}
