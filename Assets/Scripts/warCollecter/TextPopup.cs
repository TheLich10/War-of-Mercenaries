using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour
{
    public TMP_Text messageText;

    public float lifetime = 1.5f;
    public float floatSpeed = 40f;
    public float fadeDuration = 0.5f;

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private float timer;

    // „ €Ì— · Œ“Ì‰ „Ã„Ê⁄ «·÷——
    private float currentDamage = 0f;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void AddDamage(float damageAmount)
    {
        // ≈÷«›… «·÷—— «·ÃœÌœ ··÷—— «·„ —«ﬂ„
        currentDamage += damageAmount;
        messageText.text = currentDamage.ToString();

        //  „ Õ–› timer = 0f; Â‰«
        // ·ﬂÌ Ìﬂ„· «·‰’ œÊ—… ÕÌ« Â ÊÌŒ ›Ì »⁄œ «·Êﬁ  «·„Õœœ
    }

    void Update()
    {
        timer += Time.deltaTime;

        // «· Õ—ﬂ ··√⁄·Ï
        rect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;

        // «· ·«‘Ì
        if (timer > lifetime - fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, (timer - (lifetime - fadeDuration)) / fadeDuration);
            canvasGroup.alpha = alpha;
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}