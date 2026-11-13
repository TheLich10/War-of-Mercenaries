using UnityEngine;
using UnityEngine.UI;

public class ButtonListenerManager : MonoBehaviour
{
    void Start()
    {
        // ÇáÈÍË Úä ÌãíÚ ÇáÃÒÑÇÑ İí ÇáãÔåÏ (ÇáãİÚáÉ æÛíÑ ÇáãİÚáÉ)
        // áäÓÎ Unity 2023 æãÇ İæŞ:
        Button[] buttons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // áäÓÎ Unity ÇáÃŞÏã¡ ÇÓÊÎÏã ÇáÓØÑ ÇáÊÇáí ÈÏáÇğ ãä ÇáÓØÑ ÇáÃÚáì:
        // Button[] buttons = Object.FindObjectsOfType<Button>(true);

        // ÅÖÇİÉ ÍÏË ÇáäŞÑ áßá ÒÑ íÊã ÇáÚËæÑ Úáíå
        foreach (Button btn in buttons)
        {
            btn.onClick.AddListener(OnAnyButtonClicked);
        }
    }

    void OnAnyButtonClicked()
    {
        AudioManager.Instance.PlaySFX("click");

    }
}