using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roulette : MonoBehaviour
{
    public static Roulette intstin;
    public GameObject[] rewardObjects = new GameObject[16]; // كل كائن يمثل جائزة

    [System.Serializable]
    public class SpecialRewardSlot
    {
        public int rewardNumber;               // رقم الجائزة الخاصة (مثل 13)
        public UnityEngine.UI.Image targetSlot; // المكان الذي ستظهر فيه الصورة
        public Sprite rewardSprite;            // الصورة التي تمثل هذه الجائزة

    }
    public GameObject bigRewardObject; // يتم تفعيله عند اكتمال الجوائز الستة



    public List<SpecialRewardSlot> specialRewardSlots = new List<SpecialRewardSlot>();


    public float RotatePower;
    public float StopPower;

    public Rigidbody2D rbody;
    private int inRotate;
    private float t;

    private const int rewardCount = 16;
    private float segmentAngle = 360f / rewardCount;

    // الجوائز المخصصة
    private int[] rewards = new int[16] {
        1, 2, 3, 4,
        5, 6, 7, 8,
        9, 10, 11, 12,
        13, 14, 15, 16
    };

    // لتقليل التكرار
    private List<int> recentRewards = new List<int>();
    private const int maxRecent = 5;

    private void Awake()
    {
        intstin = this;
    }
    private void Start()
    {
        // استرجاع حالة الجائزة الكبرى (إذا كانت 1 يعني تم فتحها مسبقاً)
        if (PlayerPrefs.GetInt("BigRewardUnlocked", 0) == 1)
        {
            WOM_KONG_OPEN = true;
        }
    }

    private void Update()
    {
        if (rbody.angularVelocity > 0)
        {
            rbody.angularVelocity -= StopPower * Time.deltaTime;
            rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, 0, 1440);
        }

        if (rbody.angularVelocity == 0 && inRotate == 1)
        {
            t += Time.deltaTime;
            if (t >= 0.5f)
            {
                GetReward();
                inRotate = 0;
                t = 0;
            }
        }
    }

    public void Rotete()
    {
        AudioManager.Instance.PlaySFX("Roll");
        HideButtonSimple.intstin.hideBTNfor5s();
        if (inRotate == 0)
        {
            float randomPower = Random.Range(RotatePower * 0.8f, RotatePower * 1.2f); // قوة عشوائية
            rbody.AddTorque(randomPower, ForceMode2D.Impulse);
            inRotate = 1;
        }
    }


    public void GetReward()
    {
        // تم التعديل هنا: قراءة زاوية الكائن المرتبط بالـ rbody
        float rot = rbody.transform.eulerAngles.z;
        float angleOffset = segmentAngle / 2f;
        int segmentIndex = Mathf.FloorToInt((rot + angleOffset) % 360 / segmentAngle);

        // في حال تكرار الجائزة، نبحث عن أقرب جائزة غير مكررة (لليمين أو اليسار)
        if (recentRewards.Contains(segmentIndex))
        {
            int left = segmentIndex;
            int right = segmentIndex;
            int step = 1;
            while (step < rewardCount)
            {
                left = (segmentIndex - step + rewardCount) % rewardCount;
                right = (segmentIndex + step) % rewardCount;

                if (!recentRewards.Contains(left))
                {
                    segmentIndex = left;
                    break;
                }
                if (!recentRewards.Contains(right))
                {
                    segmentIndex = right;
                    break;
                }
                step++;
            }
        }

        float finalAngle = segmentIndex * segmentAngle;

        // تم التعديل هنا أيضاً: تطبيق الدوران النهائي على الكائن المرتبط بالـ rbody
        rbody.transform.eulerAngles = new Vector3(0, 0, finalAngle);

        // حفظ الجائزة لتجنب تكرارها لاحقًا
        if (!recentRewards.Contains(segmentIndex))
            recentRewards.Add(segmentIndex);
        if (recentRewards.Count > maxRecent)
            recentRewards.RemoveAt(0);

        int reward = rewards[segmentIndex];
        Win(reward);
    }


    public void Win(int Score)
    {
        string nameWin = "";

        if (Score == 7) { nameWin = "Irea spin";HideButtonSimple.intstin.HideFast(); }
        if (Score == 13) { nameWin = "rocut Goual";UIValueBinder.Instance.IncreaseValue("rocut Goual"); }
        if (Score == 3) { nameWin = "Noubal Gurad"; UIValueBinder.Instance.IncreaseValue("Noubal Gurad"); }
        if (Score == 9) { nameWin = "Nuga Iron"; UIValueBinder.Instance.IncreaseValue("Nuga Iron"); }
        if (Score == 15) { nameWin = "80 RB";RedCoinCollector.AddRedCoin(80); }
        if (Score == 5) { nameWin = "Gentaic Healing";UIValueBinder.Instance.IncreaseValue("Gentaic Healing"); }
        if (Score == 11) { nameWin = "75 Gold"; GoldCollector.AddGold(75); }
        if (Score == 1) { nameWin = "500 gold"; GoldCollector.AddGold(500); }
        if (Score == 12) { nameWin = "Iron packet";UIValueBinder.Instance.IncreaseValue("Iron packet"); }
        if (Score == 2) { nameWin = "stone packet";UIValueBinder.Instance.IncreaseValue("stone packet"); }
        if (Score == 14) { nameWin = "HUGE LUMBER";UIValueBinder.Instance.IncreaseValue("HUGE LUMBER"); }
        if (Score == 4) { nameWin = "HUGE might";UIValueBinder.Instance.IncreaseValue("HUGE might"); }
        if (Score == 10) { nameWin = "LUMBER packet";UIValueBinder.Instance.IncreaseValue("LUMBER packet"); }
        if (Score == 8) { nameWin = "20 beduin";UIValueBinder.Instance.IncreaseValue("20 beduin"); }
        if (Score == 16) { nameWin = "HUGE stone";UIValueBinder.Instance.IncreaseValue("HUGE stone"); }
        if (Score == 6) { nameWin = "Might Packet";UIValueBinder.Instance.IncreaseValue("Might Packet"); }

        print("You won: " + nameWin + " " + Score);

        ShowRewardObject(Score);
        SetSpecialRewardSlot(Score);

    }
    void ShowRewardObject(int score)
    {

        // إيقاف أي كوروتين سابق
        StopAllCoroutines();

        // إخفاء كل الكائنات أولاً
        foreach (GameObject obj in rewardObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        // عرض الكائن الصحيح وبدء المؤثر
        int index = score - 1;
        if (index >= 0 && index < rewardObjects.Length && rewardObjects[index] != null)
        {
            rewardObjects[index].SetActive(true);
            AudioManager.Instance.PlaySFX("win_point");
            StartCoroutine(AnimateReward(rewardObjects[index]));
        }
    }
    void SetSpecialRewardSlot(int score)
    {
        foreach (SpecialRewardSlot slot in specialRewardSlots)
        {
            if (slot.rewardNumber == score && slot.targetSlot != null && slot.rewardSprite != null)
            {
                // لا تُعيد نفس الجائزة مرتين (لو حصل عليها مسبقًا)
                if (slot.targetSlot.sprite == slot.rewardSprite)
                    return;

                slot.targetSlot.sprite = slot.rewardSprite;
                slot.targetSlot.enabled = true;

                CheckForBigReward(); // بعد إضافة كل جائزة خاصة، تحقق
            }
        }
    }
    public static bool WOM_KONG_OPEN = false;
    void CheckForBigReward()
    {
        int filledCount = 0;

        foreach (SpecialRewardSlot slot in specialRewardSlots)
        {
            if (slot.targetSlot != null && slot.targetSlot.sprite == slot.rewardSprite)
            {
                filledCount++;
            }
        }

        if (filledCount == specialRewardSlots.Count && bigRewardObject != null)
        {
            // هذا الشرط يضمن أن البانل يظهر ويُحفظ لمرة واحدة فقط
            if (!WOM_KONG_OPEN)
            {
                bigRewardObject.SetActive(true); // أظهر الجائزة الكبرى 🎉
                print("BIG REWARD UNLOCKED!");
                WOM_KONG_OPEN = true;

                // الحفظ باستخدام PlayerPrefs
                PlayerPrefs.SetInt("BigRewardUnlocked", 1);
                PlayerPrefs.Save();
            }
        }
    }



    IEnumerator AnimateReward(GameObject obj)
    {
        float duration = 2.5f;
        float time = 0f;

        // إعداد الحجم الابتدائي والنهائي
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * 1.1f;

        // نعيد الكائن لحجمه الابتدائي
        obj.transform.localScale = originalScale;

        while (time < duration)
        {
            float scaleFactor = Mathf.Lerp(1f, 1.2f, time / duration);
            obj.transform.localScale = originalScale * scaleFactor;

            time += Time.deltaTime;
            yield return null;
        }

        obj.SetActive(false); // إخفاء الكائن بعد انتهاء الوقت
    }

    // 🟢 دالة تجهيز البيانات للحفظ
    public RouletteSaveData GetSaveData()
    {
        RouletteSaveData data = new RouletteSaveData();
        data.isBigRewardUnlocked = WOM_KONG_OPEN;

        foreach (SpecialRewardSlot slot in specialRewardSlots)
        {
            // نحفظ رقم الجائزة فقط إذا كانت الصورة قد ظهرت فعلياً في المكان
            if (slot.targetSlot != null && slot.targetSlot.sprite == slot.rewardSprite)
            {
                data.unlockedRewardIDs.Add(slot.rewardNumber);
            }
        }
        return data;
    }

    // 🟢 دالة تحميل البيانات وتطبيقها على الواجهة
    public void LoadData(RouletteSaveData data)
    {
        if (data == null) return;

        WOM_KONG_OPEN = data.isBigRewardUnlocked;
        //if (bigRewardObject != null) bigRewardObject.SetActive(WOM_KONG_OPEN);

        foreach (SpecialRewardSlot slot in specialRewardSlots)
        {
            // إذا كان رقم الجائزة موجوداً في البيانات المحفوظة، نُظهرها
            if (data.unlockedRewardIDs.Contains(slot.rewardNumber))
            {
                if (slot.targetSlot != null && slot.rewardSprite != null)
                {
                    slot.targetSlot.sprite = slot.rewardSprite;
                    slot.targetSlot.enabled = true;
                }
            }
        }
    }

}
