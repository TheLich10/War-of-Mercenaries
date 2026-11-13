using UnityEngine;
using UnityEngine.UI;

public class AlwaysOnTop : MonoBehaviour
{
    void Awake()
    {
        // 1. ÇáÈÍË Úä ãßæä Canvas Ãæ ÅÖÇİÊå ÅĞÇ áã íßä ãæÌæÏÇğ
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }

        // 2. ÊİÚíá ÎÇÕíÉ ÊÌÇæÒ ÇáÊÑÊíÈ æÅÚØÇÆå ÃÚáì ŞíãÉ ããßäÉ
        canvas.overrideSorting = true;
        canvas.sortingOrder = 32767; // ÃŞÕì ŞíãÉ ááÜ Sorting Order

        // 3. ÅÖÇİÉ GraphicRaycaster áÖãÇä ÇÓÊãÑÇÑ Úãá ÇáÃÒÑÇÑ æÇáäŞÑÇÊ ÏÇÎá ÇáÜ Panel
        if (gameObject.GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }
    }
}