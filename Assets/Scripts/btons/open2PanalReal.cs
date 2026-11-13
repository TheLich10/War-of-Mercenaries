using UnityEngine;
using UnityEngine.UI;
public class open2PanalReal : MonoBehaviour
{
    public Button openButton;       // «·“— «·–Ì ”Ì „ «·÷€ÿ ⁄·ÌÂ
    public GameObject panel1;       // «·»«‰· «·√Ê·
    public GameObject panel2;
    public GameObject panel3;


    void Start()
    {
        // —»ÿ «·œ«·… »“— «·÷€ÿ
        openButton.onClick.AddListener(OpenBothPanels);
    }

    void OpenBothPanels()
    {
        panel1.SetActive(true);     // ⁄—÷ «·»«‰· «·√Ê·
        if (HeroArcher.isActiveHero == false && HeroUnit.isActiveHero == false && HeroArcher2.isActiveHero == false && HeroUnit2.isActiveHero == false)
        {
            panel3.SetActive(true);

        }
        else panel2.SetActive(true);     // ⁄—÷ «·»«‰· «·À«‰Ì
    }
}
