using UnityEngine;
using UnityEngine.UI;

public class spyButton : MonoBehaviour
{
    public static bool spyButtonState = false;
    public Button spyBTN;
    public GameObject spyObtionObj;

    public void SpyBTN()
    {
        spyButtonState = true;
        GoldCollector.RemoveGold(8);
        spyBTN.gameObject.SetActive(false);
        spyObtionObj.SetActive(true);
    }

}
