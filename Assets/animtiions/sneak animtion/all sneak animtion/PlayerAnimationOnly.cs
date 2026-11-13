using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimationOnly : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    // متغير لتخزين موقع اللاعب في الإطار (الفريم) السابق
    private Vector3 lastPosition;

    [Header("Settings")]
    [Tooltip("كلما زاد هذا الرقم، كلما كان الانتقال بين الاتجاهات أنعم وأقل تذبذباً")]
    public float BrokerSmoothing = 10f;

    private float smoothX = 0f;
    private float smoothY = 0f;

    public bool isAttacking = false;


    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // تعيين الموقع الابتدائي
        lastPosition = transform.position;
    }

    void Update()
    {
        // 2. أضف هذا الشرط في بداية دالة Update 
        if (isAttacking)
        {
            // تحديث الموقع المستمر يمنع حدوث قفزة في الحركة بعد انتهاء الهجوم
            lastPosition = transform.position;

            // تصفير التنعيم وإجبار السكربت على إرسال حالة الوقوف للأنيمايتور
            smoothX = 0f;
            smoothY = 0f;
            HandleAnimations(0, 0);

            return; // إنهاء الدالة هنا لمنع أي تعارض
        }

        // 1. حساب مقدار التغير في الموقع لمعرفة اتجاه الحركة الفعلي
        Vector3 movement = transform.position - lastPosition;

        // 2. تحديث الموقع الأخير للفريم القادم
        lastPosition = transform.position;

        float targetX = 0f;
        float targetY = 0f;

        // تجاهل الحركات الدقيقة جداً لتجنب التذبذب
        if (Mathf.Abs(movement.x) > 0.005f) targetX = movement.x;
        if (Mathf.Abs(movement.y) > 0.005f) targetY = movement.y;

        // 3. التنعيم التدريجي
        smoothX = Mathf.Lerp(smoothX, targetX, Time.deltaTime * BrokerSmoothing);
        smoothY = Mathf.Lerp(smoothY, targetY, Time.deltaTime * BrokerSmoothing);

        // 4. تبسيط القيم إلى 1 أو -1 أو 0 لتسهيل تحديد الاتجاه
        float moveX = 0f;
        float moveY = 0f;

        if (smoothX > 0.001f) moveX = 1f;
        else if (smoothX < -0.001f) moveX = -1f;

        if (smoothY > 0.001f) moveY = 1f;
        else if (smoothY < -0.001f) moveY = -1f;

        HandleAnimations(moveX, moveY);
    }

    private void HandleAnimations(float x, float y)
    {
        // التحقق من اتجاه الوجه لعمل Flip
        if (x > 0) spriteRenderer.flipX = false;
        else if (x < 0) spriteRenderer.flipX = true;

        // === التعديل الجديد هنا ===
        // اللاعب يكون Idle فقط إذا كان لا يتحرك (x و y أصفار) وأيضاً لا يهجم (!isAttacking)
        bool isIdle = (x == 0 && y == 0) && !isAttacking;

        // تجهيز المتغيرات بناءً على حركة اللاعب في العالم
        bool isRight = (x != 0 && y == 0);
        bool isUp = (x == 0 && y > 0);
        bool isDown = (x == 0 && y < 0);
        bool isUpRight = (x != 0 && y > 0);
        bool isDownRight = (x != 0 && y < 0);

        // إرسال القيم للأنيمايتور دفعة واحدة
        anim.SetBool("isIdle", isIdle); // الآن يمكنك تفعيلها بأمان
        anim.SetBool("isrigh", isRight);
        anim.SetBool("isup", isUp);
        anim.SetBool("isdonw", isDown);
        anim.SetBool("isupright", isUpRight);
        anim.SetBool("isdownright", isDownRight);
    }
}