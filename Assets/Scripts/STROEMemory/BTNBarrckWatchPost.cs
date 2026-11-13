using UnityEngine;

public class BTNBarrckWatchPost : MonoBehaviour
{
    // دالة لإضافة محارب
    public void AddUnit(string unitName)
    {
        // محاولة إضافة المحارب
        var barracks = WatchPostUIManager.Instance.GetCurrentBarracks();
        if (barracks == null) return;

        bool added = barracks.AddWarrior(unitName);

        if (!added)
        {
            Debug.Log("❌ لا توجد مساحة كافية - لم يتم خصم الذهب");
            return;
        }

        // تكلفة المحارب حسب الاسم
        int cost = GetGoldCost(unitName);

        // خصم الذهب فقط عند النجاح
        GoldCollector.RemoveGold(cost);

        Debug.Log("✅ تمت إضافة المحارب وخصم الذهب");
    }

    int GetGoldCost(string unitName)
    {
        switch (unitName)
        {
            case "bedoiun": return 1;
            case "ScalingEgyptian": return 38;
            case "Destroyer": return 6;
            case "Gaul": return 98;
            case "Kamikaze": return 37;
            case "knight": return 2;
            case "Mongolian": return 23;
            case "ottman": return 1;
            case "persian": return 3;
            case "postion": return 14;
            case "sapper": return 35;
            case "Savge": return 6;
            case "sneaky": return 10;

            default: return 1;
        }
    }


    // دالة لإزالة محارب
    public void RemoveUnit(string unitName)
    {
        var barracks = WatchPostUIManager.Instance.GetCurrentBarracks();
        if (barracks == null) return;

        barracks.RemoveWarrior(unitName);
    }
    public void ironCost(int cost)
    {
        ResourceManager.AddIron(cost);
    }

    public void HierWorrior(string unitName)
    {
        bool removed = Barracks.instnce.RemoveWarrior(unitName);
        // المبنى الهدف (WatchPost)
        var watchPost = WatchPostUIManager.Instance.GetCurrentBarracks();
        if (watchPost == null) return;

        if (removed)
        {
            watchPost.AddWarrior(unitName);
            Debug.Log("removal is TRUE");

        }
        else
        {
            Debug.Log("you dont have any woriors");
        }
    }
}
