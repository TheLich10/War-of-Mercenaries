using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instan;
    [Header("UI References")]
    public GameObject panel;        // ÇáÜ Panel Çááí íÍÊæí Úáì ÇáäÕ æÇáÕæÑÉ
    public TMP_Text messageText;    // ÇáäÕ (TextMeshPro)
    public Image messageImage;      // ÇáÕæÑÉ (Image)

    [Header("Fade Settings")]
    public CanvasGroup canvasGroup; // ÃÖİ ãßæä CanvasGroup ááÜ Panel æÇÓÍÈå åäÇ
    public float fadeDuration = 1f; // ãÏÉ ÇáÇÎÊİÇÁ ÇáÊÏÑíÌí
    // åĞå ÇáÏÇáÉ áÇÓÊŞÈÇá ÇáäÕ æÇáÕæÑÉ æÚÑÖåÇ

    private void Awake()
    {
        instan = this;
    }
    public void ShowPopup(string text, Sprite sprite)
    {
        AudioManager.Instance.PlaySFX("openNew");
        PlayerLevel.instance.AddXP(100);

        // ÊÚííä ÇáäÕ
        messageText.text = text;

        // ÊÚííä ÇáÕæÑÉ
        messageImage.sprite = sprite;
        messageImage.preserveAspect = true;

        // ÊİÚíá ÇáÜ Panel
        panel.SetActive(true);

        canvasGroup.alpha = 1f;
        // ÅØáÇŞ Coroutine áíÛáŞ ÇáÜ Panel ÈÚÏ 3 ËæÇäí
        StartCoroutine(HideAfterDelay(5f));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        // ÇáÇäÊÙÇÑ ŞÈá ÈÏÁ ÇáÊáÇÔí
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;

        // ÊŞáíá ÇáÔİÇİíÉ ÊÏÑíÌíÇğ
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // ÅÛáÇŞ ÇáÜ Panel ÈÚÏ ÇäÊåÇÁ ÇáÇÎÊİÇÁ
        panel.SetActive(false);
    }
}
