using UnityEngine;
using UnityEngine.EventSystems;

public class AllCollect : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            DetectResourceBuilding();
        }
    }

    void DetectResourceBuilding()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Resoures"))
        {
            CollectAllResources();
        }
    }

    public void CollectAllResources()
    {
        CollectAllWood();
        CollectAllStone();
        CollectAllPower();
        CollectAllIron();
        AudioManager.Instance.PlaySFX("Res_Collect");
        PlayerLevel.instance.AddXP(20);

        Debug.Log("تم جمع جميع الموارد بنجاح!");
    }

    void CollectAllWood()
    {
        GameObject[] woodBuildings = GameObject.FindGameObjectsWithTag("wood");

        // 1. حساب السعة المتبقية الأساسية قبل بدء الجمع
        int currentWood = ResourceManager.Instance.wood;
        int maxCapacity = ResourceManager.Instance.GetTotalCapacity();
        int capacityLeft = maxCapacity - currentWood;

        foreach (GameObject building in woodBuildings)
        {
            // 2. إذا امتلأ المخزون بالكامل، نوقف عملية البحث والجمع فوراً لتوفير الأداء
            if (capacityLeft <= 0)
            {
                Debug.LogWarning("المخزون ممتلئ! تم إيقاف الجمع التلقائي.");
                break; // نخرج من الـ foreach
            }

            var comp = building.GetComponent<buildWoodStroge>();
            if (comp != null)
            {
                // 3. نرسل السعة المتبقية الحالية للمبنى
                int collected = comp.CollectGold(capacityLeft);

                if (collected > 0)
                {
                    ResourceManager.AddWood(collected);
                    woodCollect.Instance?.ShowPopup(woodCollect.Instance.woodIcon, collected, building.transform);

                    // 4. الأهم: ننقص الكمية التي جمعناها من السعة المتبقية حتى يعرف المبنى التالي المساحة الحقيقية
                    capacityLeft -= collected;
                }
            }
        }
    }

    void CollectAllStone()
    {
        GameObject[] stoneBuildings = GameObject.FindGameObjectsWithTag("stone");

        // 1. حساب السعة المتبقية الأساسية للحجر قبل بدء الجمع
        int currentStone = ResourceManager.Instance.stone;
        int maxCapacity = ResourceManager.Instance.GetTotalCapacity();
        int capacityLeft = maxCapacity - currentStone;

        foreach (GameObject building in stoneBuildings)
        {
            // 2. التوقف فوراً إذا امتلأ المخزون لتوفير موارد الجهاز
            if (capacityLeft <= 0)
            {
                Debug.LogWarning("مخزون الحجر ممتلئ! تم إيقاف الجمع التلقائي.");
                break; // نخرج من حلقة التكرار
            }

            var comp = building.GetComponent<buildStoneStroge>();
            if (comp != null)
            {
                // 3. إرسال السعة المتبقية للمبنى الحالي
                int collected = comp.CollectGold(capacityLeft);

                if (collected > 0)
                {
                    ResourceManager.AddStone(collected);
                    StoneCollect.Instance?.ShowPopup(StoneCollect.Instance.stoneicon, collected, building.transform);

                    // 4. الأهم: ننقص الكمية التي جمعناها من السعة المتبقية ليعرف المبنى التالي المساحة الحقيقية
                    capacityLeft -= collected;
                }
            }
        }
    }

    void CollectAllPower()
    {
        GameObject[] powerBuildings = GameObject.FindGameObjectsWithTag("power");

        // 1. حساب السعة المتبقية الأساسية للطاقة قبل بدء الجمع
        int currentPower = ResourceManager.Instance.power;
        int maxCapacity = ResourceManager.Instance.GetTotalCapacity();
        int capacityLeft = maxCapacity - currentPower;

        foreach (GameObject building in powerBuildings)
        {
            // 2. التوقف فوراً إذا امتلأ المخزون لتوفير موارد الجهاز
            if (capacityLeft <= 0)
            {
                Debug.LogWarning("مخزون الطاقة ممتلئ! تم إيقاف الجمع التلقائي.");
                break; // نخرج من حلقة التكرار
            }

            var comp = building.GetComponent<buildPowerStroge>();
            if (comp != null)
            {
                // 3. إرسال السعة المتبقية للمبنى الحالي
                int collected = comp.CollectGold(capacityLeft);

                if (collected > 0)
                {
                    ResourceManager.AddPower(collected);
                    PowerCollect.Instance?.ShowPopup(PowerCollect.Instance.powericon, collected, building.transform);

                    // 4. ننقص الكمية التي جمعناها من السعة المتبقية ليعرف المبنى التالي المساحة الحقيقية
                    capacityLeft -= collected;
                }
            }
        }
    }

    void CollectAllIron()
    {
        GameObject[] ironBuildings = GameObject.FindGameObjectsWithTag("iron");

        // 1. حساب السعة المتبقية الأساسية للحديد قبل بدء الجمع
        int currentIron = ResourceManager.Instance.iron;
        int maxCapacity = ResourceManager.Instance.GetTotalCapacity();
        int capacityLeft = maxCapacity - currentIron;

        foreach (GameObject building in ironBuildings)
        {
            // 2. التوقف فوراً إذا امتلأ المخزون لتوفير موارد الجهاز
            if (capacityLeft <= 0)
            {
                Debug.LogWarning("مخزون الحديد ممتلئ! تم إيقاف الجمع التلقائي.");
                break; // نخرج من حلقة التكرار
            }

            var comp = building.GetComponent<buildIronStorge>();
            if (comp != null)
            {
                // 3. إرسال السعة المتبقية للمبنى الحالي
                int collected = comp.CollectGold(capacityLeft);

                if (collected > 0)
                {
                    ResourceManager.AddIron(collected);
                    IronCollect.Instance?.ShowPopup(IronCollect.Instance.ironIcon, collected, building.transform);

                    // 4. الأهم: ننقص الكمية التي جمعناها من السعة المتبقية ليعرف المبنى التالي المساحة الحقيقية
                    capacityLeft -= collected;
                }
            }
        }
    }
}
