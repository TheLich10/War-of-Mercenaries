using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseConfirmation : MonoBehaviour
{
    [Header("الـ Panel الخاص بالتأكيد")]
    public GameObject confirmationPanel;

    [Header("الأزرار داخل Panel التأكيد")]
    public Button yesButton;
    public Button noButton;

    private string pendingItemName; // ✅ لتخزين اسم الزر الذي تم الضغط عليه

    public GameObject closePanal;
    public GameObject closePanal2;
    public GameObject closePanal3;
    public GameObject closePanal4;

    public TMP_Text GOLDtEXT; // اربطها من الـ Inspector
    int ExtractNumberFromText(string text)
    {
        string digitsOnly = "";
        foreach (char c in text)
        {
            if (char.IsDigit(c))
                digitsOnly += c;
        }

        if (string.IsNullOrEmpty(digitsOnly))
            return 0;

        return int.Parse(digitsOnly);
    }

    void Start()
    {
        confirmationPanel.SetActive(false);

        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    // ✅ هذه تستدعى من زر الشراء (خشب، حجر، ذهب...)
    public void OnPurchaseButtonClicked(string itemName)
    {
        pendingItemName = itemName; // خزّن اسم العنصر
        confirmationPanel.SetActive(true);
    }

    private void OnYesClicked()
    {
        if (!string.IsNullOrEmpty(pendingItemName))
        {
            // ✅ تحقق من اسم الزر ونفذ الدالة المناسبة
            switch (pendingItemName)
            {
                case "Wood1":
                    BuyWood();
                    break;

                case "Wood2":
                    BuyWood2();
                    break;

                case "Wood3":
                    BuyWood3();
                    break;

                case "Stone1":
                    BuyStone();
                    break;

                case "Stone2":
                    BuyStone2();
                    break;

                case "Stone3":
                    BuyStone3();
                    break;

                case "Iron1":
                    BuyIron();
                    break;

                case "Iron2":
                    BuyIron2();
                    break;

                case "Iron3":
                    BuyIron3();
                    break;

                case "power1":
                    BuyPower();
                    break;

                case "power2":
                    BuyPower2();
                    break;

                case "power3":
                    BuyPower3();
                    break;

                case "buyWorker":
                    BuyWorker();
                    break;

                case "upgrade":
                    upgarde();
                    break;

                case "OpenNew":
                    OpenNew();
                    break;

                case "BuyBuild":
                    BuyBuild();
                    break;

                case "upLevel":
                    upLevel();
                    break;

                case "Sor":
                    Sor();
                    break;

                case "Roll":
                    Roule();
                    break;

                case "MonsterUp":
                    UpMonster();
                    break;

                case "wall2":
                    wallToLvl2();
                    break;

                case "wall3":
                    wallToLvl3();
                    break;

                case "wall4":
                    wallToLvl4();
                    break;

                case "wall5":
                    wallToLvl5();
                    break;

                case "triningBtn":
                    TrainLvl();
                    break;

                case "OpenNewW":
                    OpenNewtTimer();
                    break;

                case "SpeedMonster":
                    SpeedMon();
                    break;

                default:
                    Debug.LogWarning("⚠️ لا يوجد دالة معرفة لهذا الزر: " + pendingItemName);
                    break;
            }
        }

        confirmationPanel.SetActive(false);
    }

    private void OnNoClicked()
    {
        confirmationPanel.SetActive(false); // إغلاق بدون تنفيذ
    }

    private void BuyWood()
    {
        // ضع الكود الخاص بشراء الخشب هنا
        ResourceManager.AddWood(10000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(10);
    }

    private void BuyWood2()
    {
        // ضع الكود الخاص بشراء الخشب هنا
        ResourceManager.AddWood(50000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(15);
    }

    private void BuyWood3()
    {
        // ضع الكود الخاص بشراء الخشب هنا
        ResourceManager.AddWood(100000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(20);
    }

    private void BuyStone()
    {
        ResourceManager.AddStone(10000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(10);
    }

    private void BuyStone2()
    {
        ResourceManager.AddStone(50000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(15);
    }

    private void BuyStone3()
    {
        ResourceManager.AddStone(100000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(20);
    }

    private void BuyIron()
    {
        ResourceManager.AddIron(10000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(10);
    }
    private void BuyIron2()
    {
        ResourceManager.AddIron(50000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(15);
    }
    private void BuyIron3()
    {
        ResourceManager.AddIron(100000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(20);
    }
    private void BuyPower()
    {
        ResourceManager.AddPower(10000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(10);
    }

    private void BuyPower2()
    {
        ResourceManager.AddPower(50000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(15);
    }

    private void BuyPower3()
    {
        ResourceManager.AddPower(100000);
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(20);
    }
    private void TrainLvl()
    {
        // 1. الوصول إلى المبنى المحدد حالياً من سكربت Selector الخاص بك
        Transform selectedTransform = BuildingSelectorTrining.SelectedBuilding;

        if (selectedTransform != null)
        {
            // 2. البحث عن سكربت TrainingBuilding داخل المبنى المحدد (أو أبنائه)
            TrainingBuilding training = selectedTransform.GetComponentInChildren<TrainingBuilding>(true);

            // 3. التحقق من وجود السكربت وأن المبنى مشغول فعلاً بالتطوير
            if (training != null)
            {
                if (UpgradeWarriorButton.Instance.IsBuildingBusy(training))
                {
                    // 4. تسريع التطوير لهذا المبنى تحديداً
                    UpgradeWarriorButton.Instance.SpeedUpUpgrade(training);
                    Debug.Log("✅ تم تسريع تطوير المحارب بنجاح في: " + selectedTransform.name);
                }
                else
                {
                    Debug.Log("⚠️ المبنى المحدد لا يقوم بتطوير أي محارب حالياً.");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ المبنى المحدد لا يحتوي على سكربت التدريب (TrainingBuilding). تأكد من التاج (Tag)!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ لا يوجد مبنى محدد (يرجى النقر على مبنى تدريب أولاً).");
        }
    }

    private void BuyWorker()
    {
        WorkerUI.Instance.BuyWorker();
        if(WorkerUI.Instance.GetOwnedWorkers() == 2)
        {
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(50);
        }
        else if (WorkerUI.Instance.GetOwnedWorkers() == 3)
        {
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(500);

        }
        else if (WorkerUI.Instance.GetOwnedWorkers() == 4)
        {
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(1000);

        }
        else if (WorkerUI.Instance.GetOwnedWorkers() == 5)
        {
            AudioManager.Instance.PlaySFX("Gold_buy");
            GoldCollector.RemoveGold(2000);

        }

    }
    private void upgarde()
    {
        closePanal.SetActive(false);
        closePanal2.SetActive(false);
        closePanal3.SetActive(false);
        closePanal4.SetActive(false);
        // ✅ نستخدم المبنى المحدد فعليًا
        BuildLvl lvl = BuildingSelector.targetBuilding;

        if (lvl != null)
        {
            // 🚀 ترقية فورية بدون انتظار
            lvl.UpgradeBuilding();
            lvl.SetUpgradeToOneSecond();

            //Debug.Log("⚡ Instant Upgrade Done: " + lvl.gameObject.name); ديبك
        }
        else
        {
            //Debug.LogWarning("⚠️ لم يتم تحديد أي مبنى للترقية!");
        }
    }

    private void UpMonster()
    {
        UpgradeMonsterButton.instain.OnInstantUpgradeButtonClicked();
    }

    private void wallToLvl2()
    {
        WallCostCalculator.Instance.UpgradeAllLevel1Walls();
    }
    private void wallToLvl3()
    {
        WallCostCalculator.Instance.UpgradeAllLevel2Walls();
    }
    private void wallToLvl4()
    {
        WallCostCalculator.Instance.UpgradeAllLevel3Walls();
    }
    private void wallToLvl5()
    {
        WallCostCalculator.Instance.UpgradeAllLevel4Walls();
    }


    private void OpenNew()
    {
        // نحفظ الاندكس الحالي قبل محاولة الفتح لنعرف هل سيتغير أم لا
        int indexBefore = openNewWorior.instain.currentIndex;

        // 1. استدعاء دالة فتح المحارب أولاً
        openNewWorior.instain.FinishOpenWarrior();

        // 2. نتحقق: هل الاندكس الجديد أكبر من الاندكس القديم؟ 
        // إذا نعم، فهذا يعني أن الجندي فُتح بنجاح ولم يوقفه شرط الحد الأقصى
        if (openNewWorior.instain.currentIndex > indexBefore)
        {
            // نجلب الاندكس للمحارب الذي فتحناه للتو
            int openedIndex = openNewWorior.instain.currentIndex - 1;

            // نتحقق من أن الاندكس صحيح وأنه ضمن حدود مصفوفة الذهب
            if (openedIndex >= 0 && openedIndex < openNewWorior.instain.goldValues.Length)
            {
                // جلب القيمة الصحيحة للذهب
                int goldToDeduct = openNewWorior.instain.goldValues[openedIndex];

                // خصم الذهب
                AudioManager.Instance.PlaySFX("Gold_buy");
                GoldCollector.RemoveGold(goldToDeduct);
            }
        }
        else
        {
            // لم يتم خصم الذهب لأن الدالة توقفت عند شرط الحد الأقصى للمبنى
            // يمكنك ترك هذا الجزء فارغاً أو طباعة رسالة للتأكد
            //Debug.Log("dont have level enught "); ديبك
        }
    }
    
    private void OpenNewtTimer()
    {
        openNewWorior.instain.SpeedUpTimer();
    }

    private void BuyBuild()
    {
        GoldBuildButton.instain.OnGoldClick();
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(20);
    }

    private void Sor()
    {
        towBTNforSor.buyGoldSorDone = true;
        SorLevelButton.instain.OnLevelUpClick();
    }

    private void Roule()
    {
        Roulette.intstin.Rotete();
        AudioManager.Instance.PlaySFX("Gold_buy");
        GoldCollector.RemoveGold(50);
    }

    private void SpeedMon()
    {
        UpgradeMonsterButton.instain.SpeedUpWaitTime();
    }




    public static bool ItsNewBuild = false;
    private void upLevel()
    {
        // الحصول على المبنى الحالي
        BuildLvl lvl = BuildingClickChecker.instaint.SelectedBuildLvl;

        if (lvl != null)
        {
            building currentBuilding = lvl.GetComponent<building>();

            if (currentBuilding != null && currentBuilding.IsUnderConstruction)
            {
                GridBuildingSystem.current.OnSpeedUpButtonClicked();
            }
            else
            {
                int requiredGold = ExtractNumberFromText(GOLDtEXT.text);

                BuildingClickChecker.instaint.OnSpeedUpButtonClicked();
                AudioManager.Instance.PlaySFX("Gold_buy");
                GoldCollector.RemoveGold(requiredGold);
            }
        }
        else
        {
            if (ItsNewBuild == true)
            {
                int requiredGold = ExtractNumberFromText(GOLDtEXT.text);

                GridBuildingSystem.current.OnSpeedUpButtonClicked();
            }
            else
            {
                Debug.LogWarning("⚠️ لا يوجد مبنى محدد حالياً");

            }
        }
    }


}
