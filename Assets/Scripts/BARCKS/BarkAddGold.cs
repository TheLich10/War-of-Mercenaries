using UnityEngine;

public class BarkAddGold : MonoBehaviour
{
    // دالة تستقبل الاسم وتطبع رسالة حسب النتيجة
    public void PrintName(string playerName)
    {
        bool result = Barracks.instnce.AddWarrior(playerName);

        if (result)
        {
            if (playerName == "bedoiun") GoldCollector.RemoveGold(1);
            else if (playerName == "ScalingEgyptian") GoldCollector.RemoveGold(38);
            else if (playerName == "Destroyer") GoldCollector.RemoveGold(6);
            else if (playerName == "flyingOttoman") GoldCollector.RemoveGold(48);
            else if (playerName == "Gaul") GoldCollector.RemoveGold(98);
            else if (playerName == "Helaer") GoldCollector.RemoveGold(67);
            else if (playerName == "Kamikaze") GoldCollector.RemoveGold(37);
            else if (playerName == "knight") GoldCollector.RemoveGold(2);
            else if (playerName == "Mongolian") GoldCollector.RemoveGold(23);
            else if (playerName == "ottman") GoldCollector.RemoveGold(1);
            else if (playerName == "persian") GoldCollector.RemoveGold(3);
            else if (playerName == "postion") GoldCollector.RemoveGold(14);
            else if (playerName == "sapper") GoldCollector.RemoveGold(35);
            else if (playerName == "Savge") GoldCollector.RemoveGold(6);
            else if (playerName == "sneaky")  GoldCollector.RemoveGold(10);
            WarriorButtonHandler.instnce.SpawnWarriorOnlyByName(playerName);
        }
        else
        {
            //Debug.Log("❌ Fall add :" + playerName);
            PopupText.instan.ShowPopup("The barracks are full");
        }
    }
}
