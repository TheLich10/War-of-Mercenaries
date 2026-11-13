using UnityEngine;
using UnityEngine.UI;

public class UpgradeBarFollow : MonoBehaviour
{

    public Transform target;       // المبنى الذي نتبعه
    public Vector3 offset;         // موضع الشريط فوق المبنى
    public Image fillImage;        // الصورة الداخلية التي تمثل التقدم

    private RectTransform rectTransform;
    private Camera cam;

    public float duration;
    public float timer;
    private bool isActive = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cam = Camera.main;
    }

    public void StartUpgradeBar(Transform targetBuilding, float upgradeTime)
    {
        target = targetBuilding;
        duration = upgradeTime;
        timer = 0f;
        isActive = true;
        gameObject.SetActive(true);
    }
    // 🟢 دالة جديدة لاستئناف البار من حيث توقف
    public void ResumeUpgradeBar(Transform targetBuilding, float totalTime, float remainingTime)
    {
        target = targetBuilding;
        duration = totalTime;

        // حساب الوقت الذي مضى بالفعل لكي يبدأ البار ممتلئاً بنسبته الصحيحة
        timer = totalTime - remainingTime;

        isActive = true;
        gameObject.SetActive(true);
    }
    void Update()
    {
        if (!isActive || target == null)
            return;

        timer += Time.deltaTime;
        fillImage.fillAmount = timer / duration;

        // عند الانتهاء من الترقية
        if (timer >= duration)
        {
            isActive = false;
            gameObject.SetActive(false);
        }

        // تحريك البار مع المبنى
        Vector3 screenPos = cam.WorldToScreenPoint(target.position + offset);
        rectTransform.position = screenPos;
    }

    public void ForceFinishIn(float newDuration)
    {
        duration = newDuration;   // عدّل الوقت المتبقي
        timer = newDuration;   // عدّل الطول الكلي للتايمر
    }
    public void ForceCompleteUpgrade()
    {
        // إذا كنت تستخدم Coroutine لتعبئة البار، يمكننا إنهاؤها فورًا
        duration = 0f; // افترض أن لديك متغير remainingTime داخل UpgradeBarFollow
    }



}
