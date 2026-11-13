using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonLevelManager : MonoBehaviour
{
    [Header("مستوى المبنى (1-4)")]
    private TrainingBuilding currentBuilding;

    [Header("الزر الوحيد")]
    public Button button;

    [Header("النص الذي يظهر على الشاشة")]
    public TMP_Text infoText;


    void Update()
    {
        if (UpgradeWarriorButton.Instance == null ||
            UpgradeWarriorButton.Instance.currentBuilding == null)
        {
            button.gameObject.SetActive(false);
            infoText.text = "";
            return;
        }

        currentBuilding = UpgradeWarriorButton.Instance.currentBuilding;

        UpdateButton(currentBuilding.level);
    }



    void UpdateButton(int buildingLevel)
    {
        if (button == null || infoText == null) return;

        int requiredLevel = GetObjectNameAndText.savedNumber;

        if (requiredLevel == 0)
        {
            button.gameObject.SetActive(false);
            infoText.text = "";
            return;
        }

        if (requiredLevel >= 6)
        {
            button.gameObject.SetActive(false);
            infoText.color = new Color(1f, 0.74f, 0f);
            infoText.text = "MAX LEVEL";
            return;
        }

        int maxAllowedLevel = GetMaxAllowedLevel(buildingLevel);

        if (requiredLevel > maxAllowedLevel)
        {
            button.gameObject.SetActive(false);
            int neededBuildingLevel = GetNeededBuildingLevel(requiredLevel);
            infoText.color = Color.red;
            infoText.text = "Upgrade Training Chamber To Level " + neededBuildingLevel;
            return;
        }

        button.gameObject.SetActive(true);
        infoText.text = "";
    }


    // يحدد أعلى مستوى محارب مسموح للمبنى الحالي
    int GetMaxAllowedLevel(int buildingLevel)
    {


        switch (buildingLevel)
        {
            case 1: return 2;  // مبنى لفل 1 → محارب لفل 1-2
            case 2: return 3;  // مبنى لفل 2 → محارب لفل 3
            case 3: return 4;  // مبنى لفل 3 → محارب لفل 4
            case 4: return 6;  // مبنى لفل 4 → محارب لفل 5-6
            default: return 0;
        }
    }

    // يحدد أي مستوى مبنى مطلوب لمستوى محارب معين
    int GetNeededBuildingLevel(int warriorLevel)
    {
        if (warriorLevel <= 2) return 1;
        if (warriorLevel == 3) return 2;
        if (warriorLevel == 4) return 3;
        if (warriorLevel >= 5) return 4;
        return 1;
    }
}
