using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePopup : MonoBehaviour
{
    public Image icon;
    public TMP_Text amountText;

    public float lifetime = 1.5f;
    public float floatSpeed = 40f;
    public float fadeDuration = 0.5f;

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private float timer;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(Sprite resourceIcon, int amount)
    {
        icon.sprite = resourceIcon;
        amountText.text = "+" + amount;
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // ÇáÊÍÑß ááÃÚáì
        rect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;

        // ÇáÊÃËíÑ ÇáÈÕÑí ááÇÎÊÝÇÁ
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
