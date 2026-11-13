using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("ÚäÇÕÑ æÇÌåÉ ÇáãÓÊÎÏã")]
    public GameObject loadingPanel;
    public Image progressBarFill; // ÇáÕæÑÉ ÇáÊí ÊãÊáÆ (Filled)
    public TMP_Text progressText;

    [Header("ÊÃËíÑÇÊ ÇáÌãÇáíÉ")]
    public RectTransform barBackgroundRect;
    public RectTransform movingIcon;

    private float _maxBarWidth;

    private void Start()
    {
        // ÍÓÇÈ ÇáÚÑÖ ÇáßÇãá ááÔÑíØ ÚäÏ ÇáÈÏÇíÉ áÇÓÊÎÏÇãå İí ÇáÊÍÑíß
        if (barBackgroundRect != null)
        {
            _maxBarWidth = barBackgroundRect.rect.width;
        }

        LoadScene("SampleScene");
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingPanel.SetActive(true);
        progressBarFill.fillAmount = 0f;

        // ÊÕİíÑ ãæŞÚ ÇáãÄÔÑ İí ÇáÈÏÇíÉ
        UpdateIconPosition(0f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            progressBarFill.fillAmount = progress;

            // ÇÓÊÏÚÇÁ ÏÇáÉ ÊÍÏíË ãæŞÚ ÇáãÄÔÑ ãÚ ÊÍÑß ÇáÈÇÑ
            UpdateIconPosition(progress);

            if (progressText != null)
            {
                progressText.text = (progress * 100f).ToString("F0") + "%";
            }

            if (operation.progress >= 0.9f)
            {
                progressBarFill.fillAmount = 1f;
                UpdateIconPosition(1f); // ÇáÊÃßÏ ãä æÕæá ÇáãÄÔÑ ááäåÇíÉ

                if (progressText != null) progressText.text = "100%";

                yield return new WaitForSeconds(0.5f);

                // --------------------------------------------------------
                // åäÇ ÇáÊÚÏíá: ÈÏáÇğ ãä ÊİÚíá ÇáãÔåÏ ãÈÇÔÑÉ ÚÈÑ ÇáÓßÑÈÊ ÇáÍÇáí¡
                // äŞæã ÈÇÓÊÏÚÇÁ ÇáßæÏ ÇáÌÇåÒ ÇáÎÇÕ Èß áíÊæáì ÇáÇäÊŞÇá ÇáÊÇã.
                // --------------------------------------------------------
                GlobalSceneManager.LoadSceneViaLoading(sceneName);

                yield break; // ÅäåÇÁ ÇáßæÑæÊíä ÇáÍÇáí İæÑÇğ áÃä ÇáÇäÊŞÇá ÓíÊã ÚÈÑ ÓßÑÈÊß ÇáÌÇåÒ
            }

            yield return null;
        }
    }

    // ÏÇáÉ ÌÏíÏÉ áÍÓÇÈ æÊÍÑíß ãæŞÚ ÇáãÄÔÑ
    private void UpdateIconPosition(float progress)
    {
        if (movingIcon != null && barBackgroundRect != null)
        {
            // ÍÓÇÈ ÇáãæŞÚ ÇáÃİŞí ÇáÌÏíÏ (X) ÈäÇÁğ Úáì ÇáäÓÈÉ æÇáÚÑÖ ÇáßÇãá
            float newX = progress * _maxBarWidth;

            // ÊØÈíŞ ÇáãæŞÚ ÇáÌÏíÏ Úáì anchoredPosition
            // äÓÊÎÏã anchoredPosition.y ááÍİÇÙ Úáì ÇÑÊİÇÚ ÇáãÄÔÑ ßãÇ åæ İí ÇáÜ UI
            movingIcon.anchoredPosition = new Vector2(newX, movingIcon.anchoredPosition.y);
        }
    }
}