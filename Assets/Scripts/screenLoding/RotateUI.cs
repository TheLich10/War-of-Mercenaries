using UnityEngine;

public class RotateUI : MonoBehaviour
{
    [Header("ÅÚÏÇÏÇÊ ÇáÏæÑÇä")]
    [Tooltip("ÓÑÚÉ ÇáÏæÑÇä - ÖÚ ŞíãÉ ÓÇáÈÉ áíÏæÑ ãÚ ÚŞÇÑÈ ÇáÓÇÚÉ¡ æãæÌÈÉ áÚßÓ ÚŞÇÑÈ ÇáÓÇÚÉ")]
    public float rotationSpeed = -200f;

    void Update()
    {
        // ÊÏæíÑ ÇáÚäÕÑ Íæá ãÍæÑ Z (æåæ ÇáãÍæÑ ÇáãÓÄæá Úä ÏæÑÇä ÇáÚäÇÕÑ ËäÇÆíÉ ÇáÃÈÚÇÏ æÇáÜ UI)
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}