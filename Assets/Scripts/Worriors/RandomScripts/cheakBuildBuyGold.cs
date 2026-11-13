using UnityEngine;
using UnityEngine.UI;

public class cheakBuildBuyGold : MonoBehaviour
{
    public static cheakBuildBuyGold instan;

    public Button myButton; // زر التحقق في الشاشة
    public Button ExitBtn; // زر التحقق في الشاشة


    private bool buttonClicked = false; // حالة الزر
    private string pendingBuildingName;
    private int pendingGoldAmount;
    private string pendingString;

    private void Awake()
    {
        instan = this;
    }

    private void Start()
    {
        if (myButton != null)
        {
            myButton.onClick.AddListener(OnMyButtonClick);
        }
        else
        {
            Debug.LogWarning("⚠️ لم يتم تعيين الزر في Inspector!");
        }
        if (ExitBtn != null)
        {
            ExitBtn.onClick.AddListener(ExitBtnFun);
        }
        else
        {
            Debug.LogWarning("⚠️ لم يتم تعيين الزر في Inspector!");
        }

    }
    public static bool isGoldBuild = false;
    private void OnMyButtonClick()
    {
        if (WorkerUI.Instance != null && WorkerUI.Instance.HasAvailableWorker())
        {
            if (GoldBuildButton.ItsGoldBtn == true)
            {
                //Debug.Log("✔️ تم النقر على هذا الزر!"); ديبك

                // إذا كان هناك طلب سابق للـ cheaker، نفذ الدوال
                if (buttonClicked)
                {
                    ExecuteCheaker(pendingBuildingName, pendingGoldAmount, pendingString);
                    buttonClicked = false; // إعادة الحالة
                    GoldBuildButton.ItsGoldBtn = false;
                    isGoldBuild = true;
                }
            }
        }
        else Debug.Log("No Workers in GoldBtn");


    }

    // هذه الدالة تستقبل البيانات من سكريبت آخر
    public void cheaker(string buildingName, int goldAmount, string someString)
    {
        //Debug.Log($"Received in cheaker: Building: {buildingName}, Gold: {goldAmount}, Third: {someString}");  ديبك

        // إذا تم الضغط على الزر بالفعل، نبدأ التنفيذ مباشرة
        if (myButton != null && myButton.gameObject.activeInHierarchy)
        {
            buttonClicked = true;
            pendingBuildingName = buildingName;
            pendingGoldAmount = goldAmount;
            pendingString = someString;
            //Debug.Log("✔️ تم تحضير التنفيذ عند النقر على الزر!");  ديبك
        }
        else
        {
            // إذا تريد تنفيذه فورًا بدون انتظار الزر
            ExecuteCheaker(buildingName, goldAmount, someString);
        }
    }

    private void ExecuteCheaker(string buildingName, int goldAmount, string someString)
    {
        //Debug.Log($"Executing cheaker logic: {buildingName}, Gold: {goldAmount}, Third: {someString}"); ديبك

        // الشروط لكل مبنى
        if (buildingName == "LUMBER BLADE")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "BEAST CANNON")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "IRON SMELTER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "STONE GRINDER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "STATUE OF MIGHT")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "STOCK PILE")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "ARCHER'S TOWER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "GALTING ARROW TOWER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "HIRING QUARTERS")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "FLAMER TOWER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "SKY TOWER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "BURNING MIRRORS")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "WATCH POST")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "FREEND WATCH POST")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "BOMBARD TOWER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "MERCENARY BARRACKS")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "CLAN BARRACKS")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "CLAN")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "EXECUTIONAL CUILLOTINE")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "CITY PLANNER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "PIGEON POST")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "CENTRAL HIRING")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "BLACKSMITH")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "TUSK HORN")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "RECRUITMENT CHAMBER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "TRAINGING CHAMBER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "BEAST KEEPER")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "BEAST CAVE")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "GATAPULT")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "WALL")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "FIRE TRAP")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else if (buildingName == "BURIED SPIKES")
        {
            GoldCollector.RemoveGold(goldAmount);
        }
        else
        {
            Debug.Log("⚠️ لم يتم تعريف إجراء لهذا المبنى");
        }
    }

    public void ExitBtnFun()
    {
        GoldBuildButton.ItsGoldBtn = false;

    }

}
