using UnityEngine;
using TMPro;

public class TownHall : MonoBehaviour
{
    public int wood = 1000;
    public int stone = 800;
    public int iron = 500;
    public int power = 300;

    public GameObject popupPrefab; // prefab للصور مع النصوص
    public GameObject damageTextPrefab; // prefab للنص الأحمر فقط
    public Sprite woodIcon, stoneIcon, ironIcon, powerIcon, centerIcon;

    // مراجع لحفظ النصوص النشطة لكل نوع مورد لتتراكم الأرقام بداخلها
    private AccumulatingResourcePopup activeWoodPopup;
    private AccumulatingResourcePopup activeStonePopup;
    private AccumulatingResourcePopup activeIronPopup;
    private AccumulatingResourcePopup activePowerPopup;
    private AccumulatingResourcePopup activeCenterPopup;

    // مرجع نص الضرر الأحمر الحالي المعروض
    private TextPopup activeDamageText;

    // متغيرات التحكم بالسرقة
    public float plunderThreshold = 5000f; // أول 5000 من الصحة التي تتوزع عليها الموارد
    public float maxPlunderPercentage = 0.6f; // 60% كحد أقصى للسرقة (مثلاً 600 من أصل 1000)

    // لتخزين القيم الأصلية وما تم سرقته
    private int initialWood, initialStone, initialIron, initialPower;
    private int plunderedWood, plunderedStone, plunderedIron, plunderedPower;

    void Start()
    {
        initialWood = wood;
        initialStone = stone;
        initialIron = iron;
        initialPower = power;
    }

    public void OnHit(float hitTaken)
    {
        // حساب النسبة المأخوذة من هذه الضربة مقارنة بحد التوزيع (5000)
        float hitRatio = hitTaken / plunderThreshold;

        // حساب كمية الموارد لهذه الضربة (الكمية الأصلية × نسبة السرقة القصوى × نسبة الضربة)
        int w = Mathf.FloorToInt(initialWood * maxPlunderPercentage * hitRatio);
        int s = Mathf.FloorToInt(initialStone * maxPlunderPercentage * hitRatio);
        int i = Mathf.FloorToInt(initialIron * maxPlunderPercentage * hitRatio);
        int p = Mathf.FloorToInt(initialPower * maxPlunderPercentage * hitRatio);

        // تجنب خروج 0 إذا كانت الضربة خفيفة وما زال هناك موارد قابلة للسرقة
        if (w == 0 && plunderedWood < initialWood * maxPlunderPercentage && wood > 0) w = 1;
        if (s == 0 && plunderedStone < initialStone * maxPlunderPercentage && stone > 0) s = 1;
        if (i == 0 && plunderedIron < initialIron * maxPlunderPercentage && iron > 0) i = 1;
        if (p == 0 && plunderedPower < initialPower * maxPlunderPercentage && power > 0) p = 1;

        // التحقق من عدم تجاوز الحد الأقصى للسرقة (الـ 600 نقطة كمثال)
        w = Mathf.Min(w, Mathf.FloorToInt(initialWood * maxPlunderPercentage) - plunderedWood);
        s = Mathf.Min(s, Mathf.FloorToInt(initialStone * maxPlunderPercentage) - plunderedStone);
        i = Mathf.Min(i, Mathf.FloorToInt(initialIron * maxPlunderPercentage) - plunderedIron);
        p = Mathf.Min(p, Mathf.FloorToInt(initialPower * maxPlunderPercentage) - plunderedPower);

        // تحديث إجمالي ما تم سرقته حتى الآن
        plunderedWood += w;
        plunderedStone += s;
        plunderedIron += i;
        plunderedPower += p;

        bool resourcesDepleted = (w <= 0 && s <= 0 && i <= 0 && p <= 0);

        if (resourcesDepleted)
        {
            ShowDamageText(hitTaken);
            return;
        }

        wood -= w;
        stone -= s;
        iron -= i;
        power -= p;

        int totalCollected = w + s + i + p;

        // ... (باقي الكود الخاص بك هنا يبقى كما هو دون تغيير)
        PlayerResources.instance.AddResources(w, s, i, p);

        if (CompareTag("CenterCity") || CompareTag("Resoures"))
        {
            if (totalCollected > 0)
                // تمرير رقم 0 كمكان (Slot) لأن هذا المبنى يخرج مورداً واحداً فقط
                ShowPopup(ref activeCenterPopup, centerIcon, totalCollected, 0);
        }
        else
        {
            // إعطاء كل مورد رقم Slot مختلف لكي يظهروا بجانب بعضهم وليس فوق بعضهم
            if (w > 0) ShowPopup(ref activeWoodPopup, woodIcon, w, 0);
            if (s > 0) ShowPopup(ref activeStonePopup, stoneIcon, s, 1);
            if (i > 0) ShowPopup(ref activeIronPopup, ironIcon, i, 2);
            if (p > 0) ShowPopup(ref activePowerPopup, powerIcon, p, 3);
        }

        //Debug.Log($"🏹 Collected: {w} wood, {s} stone, {i} iron, {p} power"); ديبك
    }

    // استخدمنا (ref) لكي نقوم بتحديث المتغير الأصلي (مثل activeWoodPopup) مباشرة
    void ShowPopup(ref AccumulatingResourcePopup activePopup, Sprite icon, int amount, int slotIndex)
    {
        // التحقق مما إذا كان هناك نص نشط لهذا المورد تحديداً
        if (activePopup != null)
        {
            activePopup.AddResource(icon, amount);
        }
        else
        {
            // إنشاء نص جديد إذا لم يكن هناك نص نشط
            GameObject popupObj = Instantiate(popupPrefab, GameObject.Find("Canvas").transform);

            Vector3 baseScreenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.5f, 2f, 0));
            // الإزاحة تعتمد على نوع المورد (slotIndex) لترتيبهم أفقياً
            Vector3 offset = new Vector3(slotIndex * 40f, 0f, 0f);
            popupObj.transform.position = baseScreenPos + offset;

            activePopup = popupObj.GetComponent<AccumulatingResourcePopup>();
            activePopup.AddResource(icon, amount);
        }
    }

    void ShowDamageText(float damageAmount)
    {
        if (activeDamageText != null)
        {
            activeDamageText.AddDamage(damageAmount);
        }
        else
        {
            GameObject damageTextObj = Instantiate(damageTextPrefab, GameObject.Find("Canvas").transform);

            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.5f, 2f, 0));
            damageTextObj.transform.position = screenPos;

            activeDamageText = damageTextObj.GetComponent<TextPopup>();
            activeDamageText.AddDamage(damageAmount);
        }
    }
}