using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GoldBuildButton : MonoBehaviour
{
    public static GoldBuildButton instain;

    public Button goldButton;   // زر الذهب

    [System.Serializable]
    public class BuildingButton
    {
        public string buildingName;   // اسم المبنى (مثلاً "Wood", "Stone", "Iron")
        public Button resourceButton; // زر الموارد الخاص بهذا المبنى
        public int goldAmount;        // كمية الذهب المرتبطة بهذا المبنى

    }

    public List<BuildingButton> buildingButtonsList = new List<BuildingButton>();
    private Dictionary<string, Button> buildingButtons = new Dictionary<string, Button>();

    private void Awake()
    {
        instain = this;
    }
    private void Start()
    {
        // نخزن الأزرار في القاموس
        foreach (var item in buildingButtonsList)
        {
            if (!buildingButtons.ContainsKey(item.buildingName))
                buildingButtons.Add(item.buildingName, item.resourceButton);
        }

        // نربط زر الذهب
        goldButton.onClick.AddListener(OnGoldClick);
    }
    public static bool ItsGoldBtn = false;
    public void OnGoldClick()
    {
        string currentBuilding = GetJustName.objectName; // نأخذ الاسم من السكربت الثاني

        // البحث عن المبنى في القاموس
        if (buildingButtons.ContainsKey(currentBuilding))
        {
            // استدعاء زر الموارد (اختياري)
            buildingButtons[currentBuilding].onClick.Invoke();
            //Debug.Log("✔️ استدعيت زر الموارد لمبنى: " + currentBuilding); ديبك

            // إيجاد العنصر في القائمة للحصول على قيمة الذهب
            BuildingButton current = buildingButtonsList.Find(b => b.buildingName == currentBuilding);
            if (current != null)
            {
                cheakBuildBuyGold.instan.cheaker(current.buildingName, current.goldAmount, "GoldBuy");
                ItsGoldBtn = true;
                //Debug.Log($"✔️ تم إرسال البيانات إلى cheaker: {current.buildingName}, {current.goldAmount}, GoldBuy"); ديبك
            }
        }
        else
        {
            Debug.LogWarning("⚠️ لا يوجد زر موارد لهذا المبنى: " + currentBuilding);
        }
    }

}
