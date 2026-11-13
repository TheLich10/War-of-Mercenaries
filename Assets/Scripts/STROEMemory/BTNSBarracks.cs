using UnityEngine;

public class BTNSBarracks : MonoBehaviour
{

    public static BTNSBarracks Instance;

    void Awake()
    {
        Instance = this;
    }
    // œ«·… ·≈÷«›… „Õ«—»
    public void AddUnit(string unitName)
    {
        Barracks.instnce.AddWarrior(unitName);
    }
    public void goldCost(int cost)
    {
        GoldCollector.RemoveGold(cost);
    }

    // œ«·… ·≈“«·… „Õ«—»
    public void RemoveUnit(string unitName)
    {
        bool removed = Barracks.instnce.RemoveWarrior(unitName);

        if (removed)
        {
            // ﬂ«›∆ ›ﬁÿ ≈–«  „ «·Õ–›
            //Debug.Log("removel is TRRUE");
            ResourceManager.AddIron(GetIronReward(unitName));
        }
    }
    public void RemoveUnitMons(string unitName)
    {
        bool removed = Barracks.instnce.RemoveWarrior(unitName, true);
        if (removed)
        {
            // ﬂ«›∆ ›ﬁÿ ≈–«  „ «·Õ–›
            //Debug.Log("removel to monster");

        }
    }

    int GetIronReward(string unitName)
    {
        if (unitName == "bedoiun") return 125;
        if (unitName == "ottman") return 250;
        if (unitName == "persian") return 1000;
        if (unitName == "knight") return 300;
        if (unitName == "Kamikaze") return 3000;
        if (unitName == "ScalingEgyptian") return 4000;
        if (unitName == "Helaer") return 80000;
        if (unitName == "sapper") return 15000;
        if (unitName == "flyingOttoman") return 70000;
        if (unitName == "Gaul") return 120000;
        if (unitName == "Savge") return 3000;
        if (unitName == "Destroyer") return 2000;
        if (unitName == "postion") return 10000;
        if (unitName == "sneaky") return 4000;
        if (unitName == "Mongolian") return 28000;

        return 0;
    }
    //public void ironCost(int cost)
    //{
    //    ResourceManager.AddIron(cost);
    //}
}
