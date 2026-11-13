using TMPro;
using UnityEngine;

public class BuildingNameOnlyUI : MonoBehaviour
{
    public static building currentBuilding; // المبنى الحالي

    [Header("Panels by SorLevel")]
    public GameObject panel0;
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;

    public void ShowPanel()
    {
        if (currentBuilding == null) return;

        // إغلاق جميع البانلات
        panel0.SetActive(false);
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(false);

        // فتح البانل حسب sorlevl
        switch (currentBuilding.sorlevl)
        {
            case 0: panel0.SetActive(true); break;
            case 1: panel1.SetActive(true); break;
            case 2: panel2.SetActive(true); break;
            case 3: panel3.SetActive(true); break;
            default: Debug.LogWarning("⚠️ sorlevl غير مدعوم: " + currentBuilding.sorlevl); break;
        }
    }
}
