using UnityEngine;
using UnityEngine.UI;

public class open2panal : MonoBehaviour
{
    public Button openButton;       // «·“— «·–Ì ”Ì „ «·÷€ÿ ⁄·ÌÂ
    public GameObject panel1;       // «·»«‰· «·√Ê·
    public GameObject panel2;
    public GameObject panel3;// «·»«‰· «·À«‰Ì

    void Start()
    {
        // —»ÿ «·œ«·… »“— «·÷€ÿ
        openButton.onClick.AddListener(OpenBothPanels);
    }

    void OpenBothPanels()
    {
        panel1.SetActive(true);     // ⁄—÷ «·»«‰· «·√Ê·
        panel2.SetActive(true);     // ⁄—÷ «·»«‰· «·À«‰Ì
        panel3.SetActive(true);
    }
}
