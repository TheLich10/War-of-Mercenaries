using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;


public class UpgradeButton : MonoBehaviour
{
    public Button upgradeButton;
    public GameObject closePanal;
    public GameObject closepanal2;

    //private BuildLvl targetBuilding;

    private void Start()
    {
        upgradeButton.onClick.AddListener(UpgradeSelectedBuilding);
    }

    void UpgradeSelectedBuilding()
    {
        closePanal.SetActive(false);

        if(closepanal2 != null)
          closepanal2.SetActive(false);

        if (BuildingSelector.targetBuilding != null)
        {
            BuildingSelector.targetBuilding.UpgradeBuilding();
            PlayerLevel.instance.AddXP(100);
            Debug.Log("🚀 Upgrade buildin Done : " + BuildingSelector.targetBuilding.gameObject.name);
        }
        else
        {
            Debug.LogWarning("⚠️ لم يتم تحديد أي مبنى للترقية!");
        }
    }
}