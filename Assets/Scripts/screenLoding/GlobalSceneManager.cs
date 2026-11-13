using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager : MonoBehaviour
{
    // ãÊÛíÑ ËÇÈÊ áÍİÙ ÇÓã ÇáãÔåÏ ÇáãÑÇÏ ÇáĞåÇÈ Åáíå
    public static string NextSceneName;

    void Start()
    {
        // ÈãÌÑÏ İÊÍ ãÔåÏ ÇáÊÍãíá¡ ÇÈÏÃ ÈÊÍãíá ÇáãÔåÏ ÇáãØáæÈ İí ÇáÎáİíÉ
        if (!string.IsNullOrEmpty(NextSceneName))
        {
            StartCoroutine(LoadNextSceneAsync());
        }
        spyButton.spyButtonState = false;
    }

    IEnumerator LoadNextSceneAsync()
    {
        // ÇáÇäÊÙÇÑ ŞáíáÇğ (ÇÎÊíÇÑí¡ ãËáÇğ ËÇäíÉ æÇÍÏÉ áíÑì ÇááÇÚÈ ÃäíãíÔä ÇáÏæÑÇä)
        yield return new WaitForSeconds(1f);
        // ÈÏÁ ÊÍãíá ÇáãÔåÏ ÇáÌÏíÏ İí ÇáÎáİíÉ
        AsyncOperation operation = SceneManager.LoadSceneAsync(NextSceneName);

        // ÇäÊÙÑ ÍÊì íßÊãá ÇáÊÍãíá ÊãÇãÇğ
        while (!operation.isDone)
        {
            yield return null;
        }
    }

    // ÏÇáÉ ÚÇãÉ äÓÊÏÚíåÇ ãä Ãí ãÔåÏ ÂÎÑ ááÇäÊŞÇá ÚÈÑ ãÔåÏ ÇáÊÍãíá
    public static void LoadSceneViaLoading(string targetSceneName)
    {
        // 1. äÎÒä ÇÓã ÇáãÔåÏ ÇáãÓÊåÏİ
        NextSceneName = targetSceneName;

        // 2. äİÊÍ ãÔåÏ ÇáÊÍãíá ÃæáÇğ
        SceneManager.LoadScene("loading");
    }
}