using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class NewBuildDB
{
    public string Requriment;
    public string info;
    public string upgradeTime;
    public int woodRequired;
    public int stoneRequired;
    public int ironRequired;
    public int powerRequired;

    public NewBuildDB(string Requriment, string info, string upgradeTime,
                    int woodRequired, int stoneRequired, int powerRequired, int ironRequired)
    {
        this.Requriment = Requriment;
        this.info = info;
        this.upgradeTime = upgradeTime;
        this.woodRequired = woodRequired;
        this.stoneRequired = stoneRequired;
        this.ironRequired = ironRequired;
        this.powerRequired = powerRequired;
    }
}

public class NewBuildingData
{
    public string name;
    public Sprite icon;
    public List<NewBuildDB> upgradeLevels = new List<NewBuildDB>();

    public NewBuildingData(string name, Sprite icon)
    {
        this.name = name;
        this.icon = icon;
    }

    public void AddUpgradeLevel(NewBuildDB upgradeData)
    {
        upgradeLevels.Add(upgradeData);
    }
}

public static class NewBuildingDatabase
{
    public static List<NewBuildingData> buildings = new List<NewBuildingData>();
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;
        initialized = true;

        Sprite woodbuildsprt = Resources.Load<Sprite>("buildingIcons/woodlvl1");
        var woodbuild = new NewBuildingData("LUMBER BLADE", woodbuildsprt);
        woodbuild.AddUpgradeLevel(new NewBuildDB("City Center 5", "Produces Lumber. Used in the construction of buildings.", "5M", 0, 750, 0, 0));
        buildings.Add(woodbuild);

        Sprite stonebuildsprt = Resources.Load<Sprite>("buildingIcons/stonelvl1");
        var stonebuild = new NewBuildingData("STONE GRINDER", stonebuildsprt);
        stonebuild.AddUpgradeLevel(new NewBuildDB("City Center 5", "Produces Stone. Used in the construction of buildings.", "5M", 750, 0, 0, 0));
        buildings.Add(stonebuild);

        Sprite ironbuildsprt = Resources.Load<Sprite>("buildingIcons/ironlvl1");
        var ironbuild = new NewBuildingData("IRON SMELTER", ironbuildsprt);
        ironbuild.AddUpgradeLevel(new NewBuildDB("City Center 5", "Produces Iron. Used for hiring mercenaries!", "5M", 247, 577, 0, 0));
        buildings.Add(ironbuild);

        Sprite pwerbuildsprt = Resources.Load<Sprite>("buildingIcons/powerlvl1");
        var pwerbuild = new NewBuildingData("STATUE OF MIGHT", pwerbuildsprt);
        pwerbuild.AddUpgradeLevel(new NewBuildDB("City Center 5", "Produces Might used for unlocking and upgrading mercenaries & their special abilities and attacks.", "5M", 525, 224, 0, 0));
        buildings.Add(pwerbuild);

        Sprite resbuildsprt = Resources.Load<Sprite>("buildingIcons/reslvl1");
        var resbuild = new NewBuildingData("STOCK PILE", resbuildsprt);
        resbuild.AddUpgradeLevel(new NewBuildDB("City Center 19, Lumber Blade, Stone Grinder, Statue of Might, Iron", "This is what holds the gathered resources as it bulges up. The production of this requires Lumber & Stone.", "10M", 2000, 1855, 0, 0));
        buildings.Add(resbuild);

        Sprite archerSprt = Resources.Load<Sprite>("buildingIcons/archer_build");
        var archerBuild = new NewBuildingData("ARCHER'S TOWER", archerSprt);
        archerBuild.AddUpgradeLevel(new NewBuildDB("City Center L8", "These Archers will do their best to take down aerial and ground enemies! Beware of the long range, high damage, but the slow reload.", "30M", 1500, 2000, 500, 0));
        buildings.Add(archerBuild);
/////////////////////////////////////////////////////////////////////////////////////////
        Sprite GATAPULTsprt = Resources.Load<Sprite>("buildingIcons/mongel1");
        var GATAPULTbuild = new NewBuildingData("GATAPULT", GATAPULTsprt);
        GATAPULTbuild.AddUpgradeLevel(new NewBuildDB("City Center L3", "The perfect weapon to take over an enemy City. Hurl things into the enemy field to destroy buildings from above. Don't forget to upgrade this building to increase damage", "1H", 75000, 75000, 75000, 0));
        buildings.Add(GATAPULTbuild);

        Sprite WALLsprt = Resources.Load<Sprite>("buildingIcons/WALL");
        var WALLbuild = new NewBuildingData("WALL", WALLsprt);
        WALLbuild.AddUpgradeLevel(new NewBuildDB("archer", "is a good build for defent", "0S", 1000, 0, 0, 0));
        buildings.Add(WALLbuild);

        Sprite archerOPSprt = Resources.Load<Sprite>("buildingIcons/archerOP");
        var archerOPBuild = new NewBuildingData("GALTING ARROW TOWER", archerOPSprt);
        archerOPBuild.AddUpgradeLevel(new NewBuildDB("City Center L4", "Turn the lever, watch the arrows fly! This incredible arrow machine doesn't ever stop!", "5H", 187500, 250000, 62500, 0));
        buildings.Add(archerOPBuild);

        Sprite flameSprt = Resources.Load<Sprite>("buildingIcons/flame");
        var flameBuild = new NewBuildingData("FLAMER TOWER", flameSprt);
        flameBuild.AddUpgradeLevel(new NewBuildDB("City Center L4", "It is fire, and will burn your enemies to a crisp well done state. You ignite the fuel, pressurize and shoot it with a spark! Fatal plastic surgery for your enemies.", "10H", 500000, 250000, 100000, 0));
        buildings.Add(flameBuild);

        Sprite flaysprt = Resources.Load<Sprite>("buildingIcons/flay");
        var flaysbuild = new NewBuildingData("SKY TOWER", flaysprt);
        flaysbuild.AddUpgradeLevel(new NewBuildDB("City Center L9", "All hail the dozen spears of death! Airborne enemies should beware of this sharp, lightning-speed cluster of spears!", "10H", 215000, 280000, 62500, 0));
        buildings.Add(flaysbuild);

        Sprite mirrorsprt = Resources.Load<Sprite>("buildingIcons/mirorr");
        var mirrorbuild = new NewBuildingData("BURNING MIRRORS", mirrorsprt);
        mirrorbuild.AddUpgradeLevel(new NewBuildDB("City Center L5", "Concentrated sun to light up your enemies in a row!", "1D", 2000000, 2400000, 1600000, 0));
        buildings.Add(mirrorbuild);

        Sprite spysprt = Resources.Load<Sprite>("buildingIcons/spy");
        var spybuild = new NewBuildingData("WATCH POST", spysprt);
        spybuild.AddUpgradeLevel(new NewBuildDB("City Center L3", "Defense or Offense? Why not combine the two? This defense building allows you to disguise your protective Mercs, who will be unleashed to respond mercilessly to any attack.", "6H", 250000, 187500, 62500, 0));
        buildings.Add(spybuild);

        Sprite spyclansprt = Resources.Load<Sprite>("buildingIcons/clanspy");
        var spyclanbuild = new NewBuildingData("FREEND WATCH POST", spyclansprt);
        spyclanbuild.AddUpgradeLevel(new NewBuildDB("City Center L4", "Why not let your allies manage your defenses? Invite them to send you some defensive Mercs!", "6H", 250000, 187500, 62500, 0));
        buildings.Add(spyclanbuild);

        Sprite boomtsprt = Resources.Load<Sprite>("buildingIcons/boomtower");
        var boombuild = new NewBuildingData("BOMBARD TOWER", boomtsprt);
        boombuild.AddUpgradeLevel(new NewBuildDB("All Requirements Met", "Short, stubby, not enough range! It will still smash weak ground enemies to a pulp! But great for destroying tightly-packed Mercs!", "30M", 2000, 1500, 500, 0));
        buildings.Add(boombuild);

        Sprite barcksprt = Resources.Load<Sprite>("buildingIcons/bracks");
        var barcksbuild = new NewBuildingData("MERCENARY BARRACKS", barcksprt);
        barcksbuild.AddUpgradeLevel(new NewBuildDB("All Requirements Met", "Home of your mercenaries after they survive training! The better and plentier these are, the more mercenaries you can house. Be careful, if it burns all within burn too!", "5H", 2160, 2160, 0, 0));
        buildings.Add(barcksbuild);
        
        Sprite clanbrakspet = Resources.Load<Sprite>("buildingIcons/clanbrack");
        var clanbrakbuild = new NewBuildingData("CLAN BARRACKS", clanbrakspet);
        clanbrakbuild.AddUpgradeLevel(new NewBuildDB("City Center L5, House of Brotherhood L3", "Home of your reinforcements, sent by your alliance brothers! The higher level, the more mercenaries you can house. Be careful, if it burns all within burn too!", "5H", 138240, 138240, 138240, 0));
        buildings.Add(clanbrakbuild);

        Sprite exesprt = Resources.Load<Sprite>("buildingIcons/exe");
        var exebuild = new NewBuildingData("EXECUTIONAL CUILLOTINE", exesprt);
        exebuild.AddUpgradeLevel(new NewBuildDB("City Center 3, MERCENARY BARRACKS", "Lets you get rid of those unwanted uncontrollable mercenaries. The disobedient shall feed the devoted! The weak should give room to the strong!", "1H", 50000, 50000, 50000, 0));
        buildings.Add(exebuild);

        Sprite plansprt = Resources.Load<Sprite>("buildingIcons/plan");
        var planbuild = new NewBuildingData("CITY PLANNER", plansprt);
        planbuild.AddUpgradeLevel(new NewBuildDB("City Center 3", "Lets you see your whole City to better plan your defenses and buildings from up high.", "12H", 250000, 250000, 0, 0));
        buildings.Add(planbuild);

        Sprite postsprt = Resources.Load<Sprite>("buildingIcons/post");
        var postbuild = new NewBuildingData("PIGEON POST", postsprt);
        postbuild.AddUpgradeLevel(new NewBuildDB("City Center 2", "Lets you know about your city if you're not personally supervising it!", "5M", 500, 500, 0, 0));
        buildings.Add(postbuild);

        Sprite foursprt = Resources.Load<Sprite>("buildingIcons/four");
        var fourbuild = new NewBuildingData("CENTRAL HIRING", foursprt);
        fourbuild.AddUpgradeLevel(new NewBuildDB("City Center L3, 3x Hiring Quarters L2", "Centralize all your Mercenary hiring! Helps you in Mercenary hiring process, and controls all your Hiring Quarters, forwarding your Merc to whichever quarter is available once in the queue!", "1D", 4000000, 4000000, 4000000, 0));
        buildings.Add(fourbuild);

        Sprite hornsprt = Resources.Load<Sprite>("buildingIcons/horn");
        var hornbuild = new NewBuildingData("TUSK HORN", hornsprt);
        hornbuild.AddUpgradeLevel(new NewBuildDB("City Center L4, Recruitment Chamber", "Lets you test the mettle of your City- let us see if these defenses will hold by attracting Wild Savages! If you are way above your head, you can always call off the self-inflicted attack.", "5H", 25000, 25000, 25000, 0));
        buildings.Add(hornbuild);

        Sprite newWorrsprt = Resources.Load<Sprite>("buildingIcons/newor");
        var newWorrbuild = new NewBuildingData("RECRUITMENT CHAMBER", newWorrsprt);
        newWorrbuild.AddUpgradeLevel(new NewBuildDB("City Center 2", "Find and recruit new mercenaries!", "10M", 1800, 2300, 0, 0));
        buildings.Add(newWorrbuild);

        Sprite tringsprt = Resources.Load<Sprite>("buildingIcons/train");
        var tringbuild = new NewBuildingData("TRAINGING CHAMBER", tringsprt);
        tringbuild.AddUpgradeLevel(new NewBuildDB("City Center L3, Recruitment Chamber L2", "Develop your mercenaries' different stats. Train them, so they become the ultimate killing machines!", "3H", 100000, 100000, 0, 0));
        buildings.Add(tringbuild);

        Sprite cavesprt = Resources.Load<Sprite>("buildingIcons/cave");
        var cavebuild = new NewBuildingData("BEAST KEEPER", cavesprt);
        cavebuild.AddUpgradeLevel(new NewBuildDB("City Center L4, BEAST CAVE", "Lets you cage your beasts, 3 at a time, allowing you to change your attacking Beast and protect the rest from attack.", "1D", 500000, 500000, 500000, 0));
        buildings.Add(cavebuild);

        Sprite cavebraksprt = Resources.Load<Sprite>("buildingIcons/cavebrack");
        var cavebrakbuild = new NewBuildingData("BEAST CAVE", cavebraksprt);
        cavebrakbuild.AddUpgradeLevel(new NewBuildDB("City Center L4", "The Cave is home to the deadly beasts: Dragonfly, Mightosour and Bearwolf! You can keep your wild Beasts on a tight leash in this chamber.", "0S", 250000, 250000, 250000, 0));
        buildings.Add(cavebrakbuild);

        Sprite makewspprt = Resources.Load<Sprite>("buildingIcons/make");
        var makewbuild = new NewBuildingData("HIRING QUARTERS", makewspprt);
        makewbuild.AddUpgradeLevel(new NewBuildDB("Mercenary Barracks", "Mercs are hired (produced) in this building", "15M", 2000, 2000, 0, 0));
        buildings.Add(makewbuild);

        Sprite HADAsprr = Resources.Load<Sprite>("buildingIcons/hadad");
        var HADAbuild = new NewBuildingData("BLACKSMITH", HADAsprr);
        HADAbuild.AddUpgradeLevel(new NewBuildDB("City Center L3", "You can prepare unlocked Event Rewards in this building.", "3H", 100000, 100000, 100000, 150000));
        buildings.Add(HADAbuild);

        Sprite clansprt = Resources.Load<Sprite>("buildingIcons/clan");
        var clanbuild = new NewBuildingData("CLAN", clansprt);
        clanbuild.AddUpgradeLevel(new NewBuildDB("City Center L3", "House of Brotherhood is the center of your Alliance. It is required for creating and joining Alliances. You can browse through Alliances, view their members and see the members' various statistics.","5H", 2000, 2000, 0, 0));
        buildings.Add(clanbuild);

        Sprite bomberOPsprt = Resources.Load<Sprite>("buildingIcons/towerBoom");
        var bomberOPbuild = new NewBuildingData("BEAST CANNON", bomberOPsprt);
        bomberOPbuild.AddUpgradeLevel(new NewBuildDB("archer", "is a good build for defent", "2D", 5000000, 5000000, 5000000, 0));
        buildings.Add(bomberOPbuild);

        Sprite BuriedSpikessprt = Resources.Load<Sprite>("buildingIcons/BuriedSpikes");
        var BuriedSpikesBuild = new NewBuildingData("BURIED SPIKES", BuriedSpikessprt);
        BuriedSpikesBuild.AddUpgradeLevel(new NewBuildDB("City Center L2", "A booby trap that will spike the enemy forces from below and impale them! Place these near your buildings or design your walls to tunnel attacking units towards these invisible death traps.", "0S", 1000, 1000, 1000, 0));
        buildings.Add(BuriedSpikesBuild);

        Sprite FireTrapSprt = Resources.Load<Sprite>("buildingIcons/FireTrap");
        var FireTrap = new NewBuildingData("FIRE TRAP", FireTrapSprt);
        FireTrap.AddUpgradeLevel(new NewBuildDB("City Center L4", "These traps will burn only the most powerful of enemies to a cinder and cause splash damage to its surroundings.", "0S", 50000, 50000, 50000, 0));
        buildings.Add(FireTrap);



    }

    public static NewBuildingData GetBuildingByName(string name)
    {
        Initialize();
        return buildings.Find(b => b.name == name);
    }
}
