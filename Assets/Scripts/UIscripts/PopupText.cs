using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PopupText : MonoBehaviour
{
    public static PopupText instan;
    [Header("UI References")]
    public GameObject textOBj;
    public TMP_Text messageText;    // «·‰’ (TextMeshPro)

    [Header("Fade Settings")]
    public CanvasGroup canvasGroup; // ﬁ„ »≈÷«›… „ﬂÊ‰ CanvasGroup ··‹ textOBj Ê«”Õ»Â Â‰«
    public float fadeDuration = 1f; // „œ… «·«Œ ›«¡ «· œ—ÌÃÌ
    // Â–Â «·œ«·… ·«” ﬁ»«· «·‰’ Ê«·’Ê—… Ê⁄—÷Â«

    private void Awake()
    {
        instan = this;
    }
    public void ShowPopup(string text)
    {
        //  ⁄ÌÌ‰ «·‰’
        messageText.text = text;
        //  ›⁄Ì· «·‹ Panel
        textOBj.SetActive(true);
        canvasGroup.alpha = 1f; //  √ﬂœ „‰ √‰ «·‘›«›Ì… ﬂ«„·… ⁄‰œ «·ŸÂÊ—
        // ≈ÿ·«ﬁ Coroutine ·Ì€·ﬁ «·‹ Panel »⁄œ 3 ÀÊ«‰Ì
        StartCoroutine(HideAfterDelay(3f));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        // «·«‰ Ÿ«— ﬁ»· »œ¡ «·«Œ ›«¡
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;

        //  ﬁ·Ì· «·‘›«›Ì…  œ—ÌÃÌ« Õ Ï  ’· ≈·Ï 0
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null; // «‰ Ÿ«— «·≈ÿ«— (Frame) «· «·Ì
        }

        // ≈€·«ﬁ «·‹ Panel »⁄œ «‰ Â«¡ «· ·«‘Ì
        textOBj.SetActive(false);
    }
}
