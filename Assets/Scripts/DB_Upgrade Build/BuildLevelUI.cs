using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class BuildLevelUI
{
    public int level;
    public float health;
    public float PerHoures;
    public int CityStroge;
    public float Barackspace;
    public float WoirroMakerNum;
    public string upgradeTime;
    public int woodRequired;
    public int stoneRequired;
    public int ironRequired;
    public int powerRequired;
    public string requriment;
    public Sprite image;

    public BuildLevelUI(int level, float health, float PerHoures, int CityStroge, float Barackspace, float WoirroMakerNum, string upgradeTime,
                        int woodRequired, int stoneRequired, int powerRequired, int ironRequired, string requriment, Sprite image)
    {
        //lvl - HP - PerH - CityStrg - BarckSpace - WarMaker - time - wood - stone - power - iron 
        this.level = level;
        this.health = health;
        this.PerHoures = PerHoures;
        this.CityStroge = CityStroge;
        this.Barackspace = Barackspace;
        this.WoirroMakerNum = WoirroMakerNum;
        this.upgradeTime = upgradeTime;
        this.woodRequired = woodRequired;
        this.stoneRequired = stoneRequired;
        this.ironRequired = ironRequired;
        this.powerRequired = powerRequired;
        this.requriment = requriment;
        this.image = image;
    }
}

public class BuildingData
{
    public string name;
    public List<BuildLevelUI> levels;

    public BuildingData(string name)
    {
        this.name = name;
        this.levels = new List<BuildLevelUI>();
    }

    public BuildLevelUI GetLevel(int level)
    {
        return levels.Find(l => l.level == level);
    }
}

public static class BuildingDatabase
{
    public static List<BuildingData> buildings = new List<BuildingData>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;

        //lvl - HP - PerH - CityStrg - BarckSpace - WarMaker - time - wood - stone - power - iron 
        AddBuilding("STONE GRINDER", "stonelvl", new[]
        {
            new BuildLevelData(1, 500f, 720f, 720, 0f, 0f, "0S", 0, 0, 0, 0,""),
            new BuildLevelData(2, 950f, 1440f, 2160, 0f, 0f, "10M", 1575, 0, 0, 0, "All Requirements Met"),
            new BuildLevelData(3, 1800f, 2520f, 5670, 0f, 0f, "20M", 3300, 0, 0, 0, "All Requirements Met"),
            new BuildLevelData(4, 3400f, 3960f, 13365, 0f, 0f, "30M", 6950, 0, 0, 0, "City Center L2"),
            new BuildLevelData(5, 6500f, 5760f, 29160, 0f, 0f, "1H", 14500, 0, 0, 0, "City Center L2"),
            new BuildLevelData(6, 12000f, 7920f, 60142, 0f, 0f, "3H", 30600, 0, 0, 0, "City Center L3"),
            new BuildLevelData(7, 24000f, 10440f, 118918, 0f, 0f, "5H", 64300, 0, 0, 0, "City Center L3"),
            new BuildLevelData(8, 45000f, 13320f, 227584, 0f, 0f, "12", 135000, 0, 0, 0, "City Center L4"),
            new BuildLevelData(9, 85000f, 16560f, 424414, 0f, 0f, "1D", 283600, 0, 0, 0, "City Center L4"),
            new BuildLevelData(10, 165000f, 20160f, 775018, 0f, 0f, "1D", 600000, 0, 0, 0, "City Center L5"),
            new BuildLevelData(11, 167500f, 30240f, 775018, 0f, 0f, "2D", 1200000, 0, 0, 0, "City Center L10"),
            new BuildLevelData(12, 170000f, 46360f, 775018, 0f, 0f, "2D", 2400000, 0, 0, 0, "City Center L10"),
            new BuildLevelData(13, 175000f, 60480f, 775018, 0f, 0f, "3D", 3600000, 0, 0, 0, "City Center L10"),


        });
        //lvl - HP - PerH - CityStrg - BarckSpace - WarMaker - time - wood - stone - power - iron
        AddBuilding("LUMBER BLADE", "woodlvl", new[]
        {
            new BuildLevelData(1, 500f, 720f, 720, 0f, 0f, "0S", 0, 0, 0, 0,""),
            new BuildLevelData(2, 950f, 1440f, 2160, 0f, 0f, "10M", 0, 1575, 0, 0, "All Requirements Met"),
            new BuildLevelData(3, 1800f, 2520f, 5670, 0f, 0f, "20M",  0,3300, 0, 0, "All Requirements Met"),
            new BuildLevelData(4, 3400f, 3960f, 13365, 0f, 0f, "30M",  0,6950, 0, 0, "City Center L2"),
            new BuildLevelData(5, 6500f, 5760f, 29160, 0f, 0f, "1H",  0,14500, 0, 0, "City Center L2"),
            new BuildLevelData(6, 12000f, 7920f, 60142, 0f, 0f, "3H",  0,30600, 0, 0, "City Center L3"),
            new BuildLevelData(7, 24000f, 10440f, 118918, 0f, 0f, "5H", 0, 64300, 0, 0, "City Center L3"),
            new BuildLevelData(8, 45000f, 13320f, 227584, 0f, 0f, "12H",  0,135000, 0, 0, "City Center L4"),
            new BuildLevelData(9, 85000f, 16560f, 424414, 0f, 0f, "1D",  0,283600, 0, 0, "City Center L4"),
            new BuildLevelData(10, 165000f, 20160f, 775018, 0f, 0f, "1D",  0,600000, 0, 0, "City Center L5"),
            new BuildLevelData(11, 167500f, 30240f, 775018, 0f, 0f, "2D",  0,1200000, 0, 0, "City Center L10"),
            new BuildLevelData(12, 170000f, 46360f, 775018, 0f, 0f, "2D",  0,2400000, 0, 0, "City Center L10"),
            new BuildLevelData(13, 175000f, 60480f, 775018, 0f, 0f, "3D",  0,3600000, 0, 0, "City Center L10"),
        });
        //lvl - HP - PerH - CityStrg - BarckSpace - WarMaker - time - wood - stone - power - iron
        AddBuilding("STATUE OF MIGHT", "powerlvl", new[]
        {
            new BuildLevelData(1, 500f, 720f, 720, 0f, 0f, "0S", 0, 0, 0, 0,""),
            new BuildLevelData(2, 950f, 1440f, 2160, 0f, 0f, "10M", 1102, 470, 0, 0, "All Requirements Met"),
            new BuildLevelData(3, 1800f, 2520f, 5670, 0f, 0f, "20M",  2315,992, 0, 0, "All Requirements Met"),
            new BuildLevelData(4, 3400f, 3960f, 13365, 0f, 0f, "30M",  4862,2086, 0, 0, "City Center L2"),
            new BuildLevelData(5, 6500f, 5760f, 29160, 0f, 0f, "1H",  10210,4375, 0, 0, "City Center L2"),
            new BuildLevelData(6, 12000f, 7920f, 60142, 0f, 0f, "3H",  21441,9160, 0, 0, "City Center L3"),
            new BuildLevelData(7, 24000f, 10440f, 118918, 0f, 0f, "5H", 45027, 19298, 0, 0, "City Center L3"),
            new BuildLevelData(8, 45000f, 13320f, 227584, 0f, 0f, "12H",  94557,40252, 0, 0, "City Center L4"),
            new BuildLevelData(9, 85000f, 16560f, 424414, 0f, 0f, "1D",  198570,85102, 0, 0, "City Center L4"),
            new BuildLevelData(10, 165000f, 20160f, 775018, 0f, 0f, "1D",  416997,178716, 0, 0, "City Center L5"),
            new BuildLevelData(11, 167500f, 30240f, 775018, 0f, 0f, "2D",  875693,375303, 0, 0, "City Center L10"),
            new BuildLevelData(12, 170000f, 46360f, 775018, 0f, 0f, "2D",  1838955,788136, 0, 0, "City Center L10"),
            new BuildLevelData(13, 175000f, 60480f, 775018, 0f, 0f, "3D",  3861806,1655086, 0, 0, "City Center L10"),
        });
        //lvl - HP - PerH - CityStrg - BarckSpace - WarMaker - time - wood - stone - power - iron
        AddBuilding("IRON SMELTER", "ironlvl", new[]
        {
            new BuildLevelData(1, 500f, 720f, 720, 0f, 0f, "0S", 0, 0, 0, 0,""),
            new BuildLevelData(2, 950f, 1440f, 2160, 0f, 0f, "10M", 1102, 470, 0, 0, "All Requirements Met"),
            new BuildLevelData(3, 1800f, 2520f, 5670, 0f, 0f, "20M",  2315,992, 0, 0, "All Requirements Met"),
            new BuildLevelData(4, 3400f, 3960f, 13365, 0f, 0f, "30M",  4862,2086, 0, 0, "City Center L2"),
            new BuildLevelData(5, 6500f, 5760f, 29160, 0f, 0f, "1H",  10210,4375, 0, 0, "City Center L2"),
            new BuildLevelData(6, 12000f, 7920f, 60142, 0f, 0f, "3H",  21441,9160, 0, 0, "City Center L3"),
            new BuildLevelData(7, 24000f, 10440f, 118918, 0f, 0f, "5H", 45027, 19298, 0, 0, "City Center L3"),
            new BuildLevelData(8, 45000f, 13320f, 227584, 0f, 0f, "12H",  94557,40252, 0, 0, "City Center L4"),
            new BuildLevelData(9, 85000f, 16560f, 424414, 0f, 0f, "1D",  198570,85102, 0, 0, "City Center L4"),
            new BuildLevelData(10, 165000f, 20160f, 775018, 0f, 0f, "1D",  416997,178716, 0, 0, "City Center L5"),
            new BuildLevelData(11, 167500f, 30240f, 775018, 0f, 0f, "2D",  875693,375303, 0, 0, "City Center L10"),
            new BuildLevelData(12, 170000f, 46360f, 775018, 0f, 0f, "2D",  1838955,788136, 0, 0, "City Center L10"),
            new BuildLevelData(13, 175000f, 60480f, 775018, 0f, 0f, "3D",  3861806,1655086, 0, 0, "City Center L10"),
        });
        //lvl - HP - PerH - CityStrg - BarckSpace - WarMaker - time - wood - stone - power - iron
        AddBuilding("STOCK PILE", "reslvl", new[]
        {
            new BuildLevelData(1, 750f, 0f, 7500, 0f, 0f, "0S", 0, 0, 0, 0,""),
            new BuildLevelData(2, 1400f, 0f, 15000, 0f, 0f, "30M", 7421, 3710, 0, 0,"All Requirements Met"),
            new BuildLevelData(3, 2550f, 0f, 30000, 0f, 0f, "45M", 14843, 7421, 0, 0,"City Center L2"),
            new BuildLevelData(4, 4750f, 0f, 60000, 0f, 0f, "1H", 29687, 14843, 0, 0,"City Center L2"),
            new BuildLevelData(5, 8800f, 0f, 120000, 0f, 0f, "12H", 59375, 29687, 0, 0,"City Center L2"),
            new BuildLevelData(6, 16250f, 0f, 240000, 0f, 0f, "1D", 118750, 59275, 0, 0,"City Center L3"),
            new BuildLevelData(7, 30000f, 0f, 480000, 0f, 0f, "1D", 237500, 118750, 0, 0,"City Center L3"),
            new BuildLevelData(8, 55600f, 0f, 960000, 0f, 0f, "2D", 475000, 237500, 0, 0,"City Center L4"),
            new BuildLevelData(9, 105000f, 0f, 1920000, 0f, 0f, "2D", 950000, 475999, 0, 0,"City Center L5"),
            new BuildLevelData(10, 190000f, 0f, 3840000, 0f, 0f, "3D", 1900000, 950000, 0, 0,"City Center L6"),
        });
        //lvl - HP - PerH - CityStrg - BarckSpace - WarMaker - time - wood - stone - power - iron
        AddBuilding("HIRING QUARTERS", "makerlvl", new[]
        {
            new BuildLevelData(1, 4000f, 0f, 0, 0f, 2f, "0S", 0, 0, 0, 0,""),
            new BuildLevelData(2, 16000f, 0f, 0, 0f, 3f, "1H", 21227, 49529, 0, 0,"City Center L3, Recultment Chamber"),
            new BuildLevelData(3, 32000f, 0f, 0, 0f, 4f, "12D", 93600, 218427, 0, 0,"City Center L4"),

        });
        //lvl - HP - PerH - CityStrg - BarckSpace - WarMaker - time - wood - stone - power - iron
        AddBuilding("MERCENARY BARRACKS", "barcklvl", new[]
        {
            new BuildLevelData(1, 4000f, 0f, 0, 200f, 0f, "0S", 0, 0, 0, 0,""),
            new BuildLevelData(2, 14000f, 0f, 0, 260f, 0f, "1H", 8640, 8640, 0, 0,"City Center L2"),
            new BuildLevelData(3, 25000f, 0f, 0, 320f, 0f, "3H", 34560, 34560, 0, 0,"City Center L3"),
            new BuildLevelData(4, 43000f, 0f, 0, 380f, 0f, "8H", 138240, 138240, 0, 0,"City Center L5"),
            new BuildLevelData(5, 75000f, 0f, 0, 450f, 0f, "20H", 552960, 552960, 0, 0,"City Center L6"),
            new BuildLevelData(6, 130000f, 0f, 0, 640f, 0f, "1D", 2211840, 2211840, 0, 0,"City Center L6"),
        });
        //lvl - HP - PerH - CityStrg - BarckSpace - WarMaker - time - wood - stone - power - iron
        AddBuilding("WALL", "wallvl", new[]
        {
            new BuildLevelData(1, 1000f, 0f, 0, 0f, 0f, "0S", 0, 0, 0, 0,""),
            new BuildLevelData(2, 2300f, 0f, 0, 0f, 0f, "0S", 0, 10000, 0, 0,"City Center L2"),
            new BuildLevelData(3, 5750f, 0f, 0, 0f, 0f, "0S", 100000, 100000, 0, 0,"City Center L3"),
            new BuildLevelData(4, 18000f, 0f, 0, 0f, 0f, "0S", 200000, 200000, 0, 0,"City Center L4"),
            new BuildLevelData(5, 27000f, 0f, 0, 0f, 0f, "0S", 400000, 400000, 0, 0,"City Center L5"),

        });

    }

    private static void AddBuilding(string name, string spritePrefix, BuildLevelData[] levelDataArray)
    {
        var building = new BuildingData(name);
        foreach (var data in levelDataArray)
        {
            string spritePath = $"buildingIcons/{spritePrefix}{data.level}";
            Sprite sprite = Resources.Load<Sprite>(spritePath);
            var level = new BuildLevelUI(
                data.level, data.health, data.PerHoures, data.CityStroge, data.Barackspace, data.WoirroMakerNum,
                data.upgradeTime, data.woodRequired, data.stoneRequired, data.powerRequired, data.ironRequired,data.requriment,  sprite
            );
            building.levels.Add(level);
        }
        buildings.Add(building);
    }

    private class BuildLevelData
    {
        public int level;
        public float health;
        public float PerHoures;
        public int CityStroge;
        public float Barackspace;
        public float WoirroMakerNum;
        public string upgradeTime;
        public int woodRequired;
        public int stoneRequired;
        public int powerRequired;
        public int ironRequired;
        public string requriment;

        public BuildLevelData(int level, float health, float PerHoures, int CityStroge, float Barackspace, float WoirroMakerNum, string upgradeTime,
                              int woodRequired, int stoneRequired, int powerRequired, int ironRequired, string requriment)
        {
            this.level = level;
            this.health = health;
            this.PerHoures = PerHoures;
            this.CityStroge = CityStroge;
            this.Barackspace = Barackspace;
            this.WoirroMakerNum = WoirroMakerNum;
            this.upgradeTime = upgradeTime;
            this.woodRequired = woodRequired;
            this.stoneRequired = stoneRequired;
            this.ironRequired = ironRequired;
            this.powerRequired = powerRequired;
            this.requriment = requriment;
        }
    }

    public static BuildingData GetBuildingByName(string name)
    {
        Initialize();
        return buildings.Find(b => b.name == name);
    }
}
