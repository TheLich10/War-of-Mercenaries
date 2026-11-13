using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingLevelInfo
{
    public int level;
    public float health;
    public string openText1;
    public string openText2;
    public string openText3;
    public string upgradeTime;
    public int woodRequired;
    public int stoneRequired;
    public int ironRequired;
    public int powerRequired;
    public string requriment;
    public Sprite image;

    public BuildingLevelInfo(int level, float health, string openText1, string openText2, string openText3,
                              string upgradeTime, int woodRequired, int stoneRequired, int powerRequired, int ironRequired,
                              string requriment, Sprite image)
    {
        this.level = level;
        this.health = health;
        this.openText1 = openText1;
        this.openText2 = openText2;
        this.openText3 = openText3;
        this.upgradeTime = upgradeTime;
        this.woodRequired = woodRequired;
        this.stoneRequired = stoneRequired;
        this.ironRequired = ironRequired;
        this.powerRequired = powerRequired;
        this.requriment = requriment;
        this.image = image;
    }
}

public class BuildingInfo
{
    public string name;
    public List<BuildingLevelInfo> levels;

    public BuildingInfo(string name)
    {
        this.name = name;
        this.levels = new List<BuildingLevelInfo>();
    }

    public BuildingLevelInfo GetLevel(int level)
    {
        return levels.Find(l => l.level == level);
    }
}

public static class BuildingInfoDatabase
{
    public static List<BuildingInfo> buildings = new List<BuildingInfo>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;

        AddBuilding("CENTER CITY", "lv", new[]
        {
            new LevelData(1, 10000f, "", "", "", "0s", 0, 0, 0, 0, "Requriment"),
            new LevelData(2, 20000f, "Recruitment Chamber, Pigeon Post, Buried Spikes, Wall", "Lumber Blade, Stone Grinder, Statue of Might, Iron Smelter, Stock Pile, Archer's Tower, Bombard Tower, Hiring Quarters", "Lumber Blade, Stone Grinder, Statue of Might, Iron Smelter, Stock Pile, Archer's Tower, Bombard Tower", "20M", 7000, 7000, 0, 0, "Requriment"),
            new LevelData(3, 30000f, "City Planner, Executional Guillotine, Catapult, Central Hiring, Watch Post", "Lumber Blade, Stone Grinder, Statue of Might, Iron Smelter, Stock Pile, Mercenary Barracks, Archer's Tower, Bombard Tower, Buried Spikes, Wall, Hiring Quarters, Training Chamber", "Lumber Blade, Stone Grinder, Statue of Might, Iron Smelter, Stock Pile, Recruitment Chamber, Mercenary Barracks, Assembly Area, Archer's Tower, Bombard Tower, Wall, Hiring Quarters", "2H", 40000, 40000, 0, 0, "Requriment"),
            new LevelData(4, 40000f, "Flamer Tower, Gatling Arrow Tower, Sky Tower, Beast Cave, Beast Keeper, Fire Trap", "Lumber Blade, Stone Grinder, Statue of Might, Iron Smelter, Stock Pile, Training Chamber, Archer's Tower, Bombard Tower, Buried Spikes, Wall, Hiring Quarters", "Lumber Blade, Stone Grinder, Statue of Might, Iron Smelter, Stock Pile, Recruitment Chamber, Training Chamber, Assembly Area, Archer's Tower, Bombard Tower, Watchpost, Wall, Catapult, Hiring Quarters", "12H", 240000, 240000, 0, 0, "Requriment"),
            new LevelData(5, 50000f, "Burning Mirrors", "Lumber Blade, Stone Grinder, Statue of Might, Iron Smelter, Stock Pile, Mercenary Barracks, Archer's Tower, Bombard Tower, Flamer Tower, Gatling Arrow Tower, Sky Tower, Watchpost, Buried Spikes, Fire Trap, Wall, Hiring Quarters", "Lumber Blade, Stone Grinder, Statue of Might, Iron Smelter, Stock Pile, Recruitment Chamber, Training Chamber, Mercenary Barracks, Archer's Tower, Bombard Tower, Flamer Tower, Gatling Arrow Tower, Sky Tower, Watchpost, Wall, Catapult", "1D", 1400000, 1400000, 0, 0, "Requriment"),
            new LevelData(6, 80000f, "", "Flamer Tower, Gatling Arrow Tower, Buried Spikes, Fire Trap, Wall", "Training Chamber, Mercenary Barracks, Archer's Tower, Bombard Tower, Flamer Tower, Gatling Arrow Tower, Sky Tower, Burning Mirrors, Wall, Catapult","1D", 7560000, 7560000, 0, 0, "Requriment"),
            new LevelData(7, 105000f, "", "Buried Spikes, Fire Trap, Wall", "Flamer Tower, Gatling Arrow Tower, Sky Tower, Burning Mirrors", "2D", 11340000, 11340000, 0, 0, "Requriment"),
            new LevelData(8, 125000f, "", "Mercenary Barracks, Watch Post, Buried Spikes, Fire Trap, Wall", "Flamer Tower, Burning Mirrors, Watchpost", "3D", 14420000, 14420000, 0, 0, "Requriment"),
            new LevelData(9, 175000f, "", "Stock Pile, Buried Spikes, Fire Trap, Wall", "Flamer Tower, Gatling Arrow Tower, Sky Tower, Burning Mirrors, Watchpost", "4D", 18000000, 18000000, 0, 0, "Requriment"),
            new LevelData(10, 275000f, "", "", "Lumber Blade, Stone Grinder, Statue of Might, Iron Smelter", "5D", 22500000, 22500000, 0, 0, "Requriment"),
            // Ì„ﬂ‰ﬂ ≈÷«›… «·„“Ìœ Â‰«
        });

        AddBuilding("RECRUITMENT CHAMBER", "NewWor", new[]
        {
            new LevelData(1, 10000f, "", "", "", "0S", 0,0 , 0, 0, ""),
            new LevelData(2, 10000f, "Each level enables new types of mercenaries to be recruited", "", "", "5H", 28800, 18400, 0, 0, "City Center L3"),
            new LevelData(3, 10000f, "Each level enables new types of mercenaries to be recruited", "", "", "15H", 115200, 147200, 0, 0, "City Center L4"),
            new LevelData(4, 10000f, "Each level enables new types of mercenaries to be recruited", "", "", "1D", 460800, 588800, 0, 0, "City Center L5"),

        });

        AddBuilding("TRAINGING CHAMBER", "training", new[]
        {
            new LevelData(1, 10000f, "", "", "", "0s", 0,0 , 0, 0, ""),
            new LevelData(2, 10000f, "Each level enables new types of mercenaries to be recruited", "", "", "5H", 28800, 18400, 0, 0, "City Center L3"),
            new LevelData(3, 10000f, "Each level enables new types of mercenaries to be recruited", "", "", "1D", 115200, 147200, 0, 0, "City Center L4"),
            new LevelData(4, 10000f, "Each level enables new types of mercenaries to be recruited", "", "", "2D", 460800, 588800, 0, 0, "City Center L5"),

        });

        AddBuilding("GATAPULT", "mongel", new[]
{
            new LevelData(1, 10000f, "", "", "", "0S", 0,0 , 0, 0, ""),
            new LevelData(2, 10000f, "Each upgrade enables you to deploy another type of salvo.", "", "", "3H", 128600,128600 , 128600, 0, ""),
            new LevelData(3, 10000f, "Each upgrade enables you to deploy another type of salvo.", "", "", "5H", 257200,257200 , 257200, 0, ""),

        });

        // √÷› «·„“Ìœ „‰ «·„»«‰Ì »‰›” «·ÿ—Ìﬁ… Â‰« ≈–« √—œ 
    }

    private static void AddBuilding(string name, string spritePrefix, LevelData[] levelDataArray)
    {
        var building = new BuildingInfo(name);
        foreach (var data in levelDataArray)
        {
            string spritePath = $"buildingIcons/{spritePrefix}{data.level}";
            Sprite sprite = Resources.Load<Sprite>(spritePath);

            var level = new BuildingLevelInfo(
                data.level, data.health, data.openText1, data.openText2, data.openText3,
                data.upgradeTime, data.woodRequired, data.stoneRequired, data.powerRequired, data.ironRequired,
                data.requriment, sprite
            );
            building.levels.Add(level);
        }
        buildings.Add(building);
    }

    private class LevelData
    {
        public int level;
        public float health;
        public string openText1;
        public string openText2;
        public string openText3;
        public string upgradeTime;
        public int woodRequired;
        public int stoneRequired;
        public int ironRequired;
        public int powerRequired;
        public string requriment;

        public LevelData(int level, float health, string openText1, string openText2, string openText3,
                         string upgradeTime, int woodRequired, int stoneRequired, int powerRequired, int ironRequired,
                         string requriment)
        {
            this.level = level;
            this.health = health;
            this.openText1 = openText1;
            this.openText2 = openText2;
            this.openText3 = openText3;
            this.upgradeTime = upgradeTime;
            this.woodRequired = woodRequired;
            this.stoneRequired = stoneRequired;
            this.powerRequired = powerRequired;
            this.ironRequired = ironRequired;
            this.requriment = requriment;
        }
    }

    public static BuildingInfo GetBuildingByName(string name)
    {
        Initialize();
        return buildings.Find(b => b.name == name);
    }
}
