using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildingClickChecker : MonoBehaviour
{
    public static BuildingClickChecker instaint;
    private BuildLvl selectedBuildLvl; // المرجع للمبنى المحدد حاليًا

    // 🟢 إضافة هذه الخاصية
    public BuildLvl SelectedBuildLvl
    {
        get { return selectedBuildLvl; }
    }

    [Header("أسماء المباني الصالحة لتسريع الترقية")]
    public List<string> validBuildingNames = new List<string>(); // ضع هنا جميع أسماء المباني

    private void Awake()
    {
        instaint = this;
    }

    void Update()
    {
        // النقر بالماوس مع التأكد أنه ليس فوق UI
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && (hit.name == "upgrade (1)" || hit.name == "Capsule"))
            {
                Debug.Log("✅ تم النقر على المبنى upgrade (1)");

                // جلب سكربت NameUpgrdes من نفس المبنى
                NameUpgrdes script = hit.GetComponent<NameUpgrdes>();
                if (script != null)
                {
                    string buildName = script.buildingName;
                    Debug.Log("🏗️ اسم المبنى من السكربت: " + buildName);

                    // التحقق إذا الاسم موجود في قائمة المباني الصالحة
                    if (validBuildingNames.Contains(buildName))
                    {
                        Debug.Log("🌲 can UPgrade");

                        // البحث عن سكربت BuildLvl في الأب وحفظه
                        selectedBuildLvl = hit.transform.parent.GetComponent<BuildLvl>();
                        if (selectedBuildLvl == null)
                        {
                            PurchaseConfirmation.ItsNewBuild = false;
                            Debug.LogWarning("⚠️ لم يتم العثور على سكربت BuildLvl في الأب");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ سكربت NameUpgrde غير موجود في المبنى upgrade (1)");
                }
            }
        }
    }

    public void OnSpeedUpButtonClicked()
    {
        if (selectedBuildLvl != null)
        {
            Debug.Log("⏱ تم الضغط على زر تسريع الترقية → استدعاء SetUpgradeToOneSecond");
            selectedBuildLvl.SetUpgradeToOneSecond();
        }
        else
        {
            Debug.LogWarning("⚠️ لم يتم تحديد أي مبنى لتسريع الترقية");
        }
    }

}
