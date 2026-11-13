using UnityEngine;
using Pathfinding;
using System.Collections; // 1. التعديل هنا: إضافة مكتبة مسارات الذكاء الاصطناعي

public class Warrior : MonoBehaviour
{
    public float speed = 5f; // سرعة حركة المحارب
    public string warriorName; // اسم المحارب (يتم تعيينه عند الإنشاء)
    public float roamRadius = 0.6f; // أقصى مسافة يتحرك فيها المحارب حول الثكنة
    public float roamOffsetY = 0.8f; // رفع أو خفض مركز التجول حول الثكنة على محور Y
    public float waitTimeAtDestination = 5f; // التعديل الجديد: وقت الانتظار بعد الوصول لكل نقطة (يمكنك زيادته من اليونيتي)

    private Barracks targetBarracks; // الثكنة المستهدفة
    private bool isMoving = true;
    private bool isRoaming = false; // حالة للتحقق مما إذا كان المحارب يتجول
    private AIPath aiPath; // 2. التعديل هنا: تعريف متغير AIPath
    private float roamTimer = 0f; // مؤقت لتغيير اتجاه الحركة العشوائية

    public bool isDismissed = false; // جعلناه public لنتمكن من فحصه من الثكنة
    private bool isGoingToExeBuild = false; // متغير لمعرفة ما إذا كان ذاهباً إلى مبنى الخروج



    // متغير ثابت يتذكره جميع المحاربين لتوزيعهم بالتسلسل
    private static int nextBarracksIndex = 0;
    // ابحث عن دالة void Start() وقم بتغييرها إلى:
    void OnEnable()
    {
        aiPath = GetComponent<AIPath>();
        if (aiPath != null) aiPath.maxSpeed = speed;

        // إعادة ضبط القيم لضمان بداية نظيفة
        isDismissed = false;
        isMoving = true;
        isRoaming = false;

        // البحث عن ثكنة (استخدمنا الكوروتين لتجنب Destroy)
        StartCoroutine(RetryFindBarracks());
    }

    void Update()
    {
        // --- الإضافة الجديدة هنا في بداية الـ Update ---
        if (isDismissed)
        {
            // إذا وصل لمبنى الخروج، قم بإلغاء تفعيله ليعود للـ Pool
            if (aiPath != null && aiPath.reachedDestination)
            {
                if (isGoingToExeBuild)
                {
                    // Random.Range(0, 2) سيقوم بإرجاع إما الرقم 0 أو 1 بشكل عشوائي
                    int randomNum = UnityEngine.Random.Range(0, 2);

                    if (randomNum == 0)
                    {
                        AudioManager.Instance.PlaySFX("die1");
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFX("die2");
                    }

                    isGoingToExeBuild = false; // إعادة ضبطه تحسباً إذا تم إعادة استخدام المحارب (Object Pooling)
                }

                gameObject.SetActive(false);

            }
            return; // نضع return لكي لا ينفذ أكواد الثكنة بالأسفل
        }

        if (isMoving && targetBarracks != null)
        {
            MoveToBarracks();

            // 4. التعديل هنا: استخدام دالة AIPath للتحقق من الوصول بدلاً من المسافة الثابتة (0.1f) 
            // لكي لا يعلق المحارب إذا توقف قبل 0.1 بقليل حسب إعدادات End Reached Distance
            if (aiPath != null && aiPath.reachedDestination)
            {
                AddToBarracks();
            }
        }
        else if (isRoaming && targetBarracks != null)
        {
            // إذا كان المحارب في حالة تجول، نقوم بتحديث حركته
            RoamAroundBarracks();
        }
    }

    // البحث عن أقرب ثكنة غير ممتلئة
    // استبدل هذه الدالة بالكامل في الكود الخاص بك
    private void FindNearestBarracks()
    {
        GameObject[] barracksObjects = GameObject.FindGameObjectsWithTag("Barracks");

        if (barracksObjects.Length > 0)
        {
            if (nextBarracksIndex >= barracksObjects.Length)
            {
                nextBarracksIndex = 0;
            }

            targetBarracks = barracksObjects[nextBarracksIndex].GetComponent<Barracks>();
            nextBarracksIndex++;
        }
        else
        {
            // 🟢 التعديل: بدلاً من Destroy، نستخدم StartCoroutine لمحاولة البحث مرة أخرى بعد ثانية
            // هذا يضمن أنه إذا تأخر تحميل المباني، سيعطي الجندي فرصة لنفسه
            //Debug.Log("⚠️ لا توجد ثكنات متاحة حالياً، سأنتظر...");
            StartCoroutine(RetryFindBarracks());
        }
    }

    // دالة مساعدة لتكرار البحث إذا لم يجد مبنى فوراً
    private IEnumerator RetryFindBarracks()
    {
        yield return new WaitForSeconds(1f);
        FindNearestBarracks();
    }

    // التحكم في حركة المحارب للوصول إلى الثكنة
    private void MoveToBarracks()
    {
        if (targetBarracks != null)
        {
            // 5. التعديل هنا: توجيه AIPath نحو الثكنة بدلاً من Vector3.MoveTowards
            if (aiPath != null)
            {
                aiPath.destination = targetBarracks.transform.position;
            }
        }
    }

    // إضافة المحارب إلى الثكنة
    private void AddToBarracks()
    {
        // تم إزالة شرط التحقق من الامتلاء CanAddWarrior() حسب طلبك
        //Debug.Log("✅ تمت إضافة المحارب (" + warriorName + ") إلى الثكنة وسيبدأ بالتجول حولها!");  ديبك
        isMoving = false;
        isRoaming = true;
        SetNewRoamDestination();

        if (!targetBarracks.activeWarriors.Contains(this))
        {
            targetBarracks.activeWarriors.Add(this);
        }
    }

    // ----------------------------------------------------
    // الدوال المسؤولة عن الحركة العشوائية حول الثكنة
    // ----------------------------------------------------

    private void RoamAroundBarracks()
    {
        // التعديل الجديد: التبديل البطيء بين الحركات
        if (aiPath != null && aiPath.reachedDestination)
        {
            // إذا وصل للنقطة العشوائية، يبدأ بإنقاص مؤقت الانتظار وهو واقف
            roamTimer -= Time.deltaTime;

            // عندما ينتهي وقت الانتظار المخصص، يختار نقطة جديدة
            if (roamTimer <= 0f)
            {
                SetNewRoamDestination();
            }
        }
        else
        {
            // إجراء حماية: إذا علق الجندي خلف عائق ولم يتمكن من الوصول للنقطة
            // نقوم بإنقاص المؤقت حتى إذا مر وقت طويل (مثلاً 5 ثواني إضافية) نعطيه نقطة جديدة لفك التعليق
            roamTimer -= Time.deltaTime;
            if (roamTimer <= -5f)
            {
                SetNewRoamDestination();
            }
        }
    }

    private void SetNewRoamDestination()
    {
        // توليد مسافة عشوائية (طول وعرض) لا تتجاوز المساحة المحددة (roamRadius)
        float randomX = Random.Range(-roamRadius, roamRadius);
        float randomY = Random.Range(-roamRadius, roamRadius);

        // تحديد مركز التجول مع إضافة قيمة roamOffsetY لرفع أو خفض المركز
        Vector3 centerPosition = targetBarracks.transform.position + new Vector3(0, roamOffsetY, 0);

        // حساب الموقع الجديد العشوائي بناءً على المركز المعدل
        Vector3 randomOffset = new Vector3(randomX, randomY, 0);
        Vector3 newDestination = centerPosition + randomOffset;

        if (aiPath != null)
        {
            aiPath.destination = newDestination;
        }

        // التعديل الجديد: إعادة ضبط المؤقت على قيمة الانتظار المطلوبة
        roamTimer = waitTimeAtDestination;
    }

    public void GoToExitAndDeactivate()
    {
        // إيقاف سلوك الثكنة
        isMoving = false;
        isRoaming = false;
        isDismissed = true; // تفعيل حالة المغادرة
        isGoingToExeBuild = true; // <-- الإضافة هنا: نخبره أنه متجه لمبنى exeBuild

        // البحث عن مبنى الخروج
        GameObject exitBuilding = GameObject.FindGameObjectWithTag("exeBuild");

        if (exitBuilding != null && aiPath != null)
        {
            aiPath.destination = exitBuilding.transform.position; // التوجه للمبنى
        }
        else
        {
            // إذا لم يجد المبنى لسبب ما، يتم إخفاؤه فوراً لتجنب الأخطاء
            gameObject.SetActive(false);
        }
    }

    public void GoToExitAndToMonster()
    {
        // إيقاف سلوك الثكنة
        isMoving = false;
        isRoaming = false;
        isDismissed = true; // تفعيل حالة المغادرة

        // البحث عن مبنى الخروج
        GameObject exitBuilding = GameObject.FindGameObjectWithTag("CaveBrack");

        if (exitBuilding != null && aiPath != null)
        {
            aiPath.destination = exitBuilding.transform.position; // التوجه للمبنى
        }
        else
        {
            // إذا لم يجد المبنى لسبب ما، يتم إخفاؤه فوراً لتجنب الأخطاء
            gameObject.SetActive(false);
        }
    }


}