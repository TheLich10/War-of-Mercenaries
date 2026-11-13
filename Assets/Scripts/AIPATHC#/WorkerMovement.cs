using UnityEngine;
using Pathfinding;
using System.Collections;

public class WorkerMovement : MonoBehaviour
{
    private AIPath aiPath;
    private UpgradeTarget currentTarget; // حفظ سكريبت الهدف الحالي
    private Collider2D targetCollider;

    // --- متغيرات الأنيميشن الجديدة ---
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public string warriorName = "Worker"; // يمكنك تغيير الاسم من الـ Inspector ليتناسب مع الوحش

    void Start()
    {
        aiPath = GetComponent<AIPath>();

        // --- الإضافات الجديدة ---
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // -----------------------

        StartCoroutine(SearchForUpgradeRoutine());
    }

    IEnumerator SearchForUpgradeRoutine()
    {
        while (true)
        {
            // نبحث عن هدف جديد في حالتين:
            // 1. إذا لم يكن هناك هدف (null)
            // 2. إذا كان الهدف القديم قد اختفى أو تم إغلاقه (!activeInHierarchy)
            if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
            {
                // تنظيف البيانات القديمة للاستعداد للهدف الجديد
                currentTarget = null;
                targetCollider = null;

                // المرور على قائمة المباني المتاحة
                for (int i = 0; i < UpgradeTarget.AvailableUpgrades.Count; i++)
                {
                    UpgradeTarget upgrade = UpgradeTarget.AvailableUpgrades[i];

                    // التأكد أن المبنى موجود، واسمه مطابق لطلبك فقط، وغير محجوز
                    if (upgrade != null && upgrade.name == "upgrade (1)" && !upgrade.isClaimed)
                    {
                        // حجز المبنى بنجاح
                        upgrade.isClaimed = true;
                        currentTarget = upgrade;
                        targetCollider = upgrade.GetComponent<Collider2D>();

                        //Debug.Log(gameObject.name + " وجد مبنى مطور وحجزه!"); ديبك

                        break; // الخروج من الحلقة والتوجه للمبنى
                    }
                }
            }

            // فحص المشهد كل ثانية (يمكنك تقليلها إلى 0.5f إذا أردت استجابة أسرع)
            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {
        if (currentTarget != null && currentTarget.gameObject.activeInHierarchy)
        {
            Vector3 targetPos;

            // تحديد نقطة الهدف بدقة
            if (targetCollider != null)
            {
                targetPos = targetCollider.ClosestPoint(transform.position);
            }
            else
            {
                targetPos = currentTarget.transform.position;
            }

            aiPath.destination = targetPos;

            // نعتبر العامل في حالة هجوم/عمل إذا وصل إلى نقطة الهدف (أو اقترب جداً)
            bool isAttacking = aiPath.reachedDestination;

            // توجيه العامل وتشغيل الأنيميشن
            HandleAttackAnimation(targetPos, isAttacking);
        }
        else
        {
            // إيقاف الأنيميشن في حال عدم وجود هدف
            HandleAttackAnimation(transform.position, false);
        }
    }

    // تأمين: إذا مات العامل أو حُذف قبل أن يصل، يترك المبنى متاحاً للعمال الآخرين
    void OnDestroy()
    {
        if (currentTarget != null)
        {
            currentTarget.isClaimed = false;
        }
    }

    private void HandleAttackAnimation(Vector3 targetPos, bool isAttacking)
    {
        if (animator == null || spriteRenderer == null) return;

        // +++ أضف هذين السطرين هنا للربط بين السكربتين وإيقاف التذبذب +++
        PlayerAnimationOnly playerAnim = GetComponent<PlayerAnimationOnly>();
        if (playerAnim != null) playerAnim.isAttacking = isAttacking;

        // حساب اتجاه الهدف بالنسبة للعدو
        Vector3 direction = targetPos - transform.position;

        // 1. تحديد الانعكاس (Flip) يمين أو يسار
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;  // يسار (نعمل فلب)
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // يمين (بدون فلب)
        }



        // 2. تحديد نوع الأنيميشن (للوحوش العادية فقط)

        // إيقاف الأنيميشن إذا لم يكن في حالة هجوم
        if (!isAttacking)
        {
            animator.SetBool("isATKUP", false);
            animator.SetBool("isATKDOWN", false);
            animator.SetBool("isATKRIGHT", false); // إيقاف أنيميشن اليمين
            return;
        }

        // مقارنة أيهما أكبر: المسافة الأفقية أم العمودية لتحديد الاتجاه الدقيق
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // الهجوم أفقي (يمين أو يسار - الـ FlipX المكتوب بالأعلى سيتكفل باليسار)
            animator.SetBool("isATKRIGHT", true);
            animator.SetBool("isATKUP", false);
            animator.SetBool("isATKDOWN", false);
        }
        else
        {
            // الهجوم عمودي (أعلى أو أسفل)
            animator.SetBool("isATKRIGHT", false);

            if (direction.y > 0)
            {
                animator.SetBool("isATKUP", true);
                animator.SetBool("isATKDOWN", false);
            }
            else
            {
                animator.SetBool("isATKUP", false);
                animator.SetBool("isATKDOWN", true);
            }
        }
    }
}