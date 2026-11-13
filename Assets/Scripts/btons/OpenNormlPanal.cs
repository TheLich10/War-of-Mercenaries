using UnityEngine;
using UnityEngine.UI;

public class OpenNormlPanal : MonoBehaviour
{
    public Button OpenBTN;
    public GameObject panal;

    void Start()
    {
        if (OpenBTN != null)
            OpenBTN.onClick.AddListener(OpenPanel);

        if (panal != null)
            panal.SetActive(false); // ‰Œ›ÌÂ »«·»œ«Ì…
    }

    void OpenPanel()
    {
        if (panal != null)
            panal.SetActive(true);
    }
}
