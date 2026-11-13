using System.Collections.Generic;
using UnityEngine;

public static class BuildingRegistry
{
    private static readonly List<BuildLvl> allBuildings = new List<BuildLvl>();

    public static void RegisterBuilding(BuildLvl building)
    {
        if (!allBuildings.Contains(building))
            allBuildings.Add(building);
    }

    public static void UnregisterBuilding(BuildLvl building)
    {
        if (allBuildings.Contains(building))
            allBuildings.Remove(building);
    }

    public static BuildLvl GetBuildingByTag(string tag)
    {
        // íÚíÏ Ãæá ãÈäì íÍãá ÇáÊÇÛ ÇáãØáæÈ
        return allBuildings.Find(b => b.gameObject.CompareTag(tag));
    }
}
