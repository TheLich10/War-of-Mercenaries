using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccumulatingResourcePopup : MonoBehaviour //  „  €ÌÌ— «”„ «·ﬂ·«” Â‰«
{
    public Image icon;
    public TMP_Text amountText;

    public float lifetime = 1.5f;
    public float floatSpeed = 40f;
    public float fadeDuration = 0.5f;

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private float timer;

    // „ €Ì— · Œ“Ì‰ „Ã„Ê⁄ «·„Ê«—œ «·„ —«ﬂ„…
    private int currentAmount = 0;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // œ«·… ÃœÌœ…  ﬁÊ„ »Ã„⁄ «·„Ê«—œ »œ·« „‰ «” »œ«·Â«
    public void AddResource(Sprite resourceIcon, int amount)
    {
        icon.sprite = resourceIcon; //  ÕœÌÀ «·√ÌﬁÊ‰… · √ﬂÌœ ‰Ê⁄ «·„Ê—œ
        currentAmount += amount; // ≈÷«›… «·„Ê—œ «·ÃœÌœ ··—ﬁ„ «·ﬁœÌ„
        amountText.text = "+" + currentAmount;

        // ·„ ‰ﬁ„ » ’›Ì— «·⁄œ«œ (timer = 0) ·ﬂÌ Ìﬂ„· «·‰’ œÊ—… ÕÌ« Â ÊÌŒ ›Ì »⁄œ «·Êﬁ  «·„Õœœ
    }

    void Update()
    {
        timer += Time.deltaTime;

        // «· Õ—ﬂ ··√⁄·Ï
        rect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;

        // «· √ÀÌ— «·»’—Ì ··«Œ ›«¡
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