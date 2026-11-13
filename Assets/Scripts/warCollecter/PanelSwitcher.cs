using UnityEngine;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{
    [Header("«·√“—«—")]
    public GameObject buttonWar;  // “— «·Õ—»
    public GameObject buttonSecond; // «·“— «·À«‰Ì

    [Header("«·»«‰·“")]
    public GameObject waroriPanal;
    public GameObject mongelPanal;
    public GameObject timerPanal;
    public GameObject bgPanal;
    public GameObject backBtn;



    // Ì „ —»ÿ Â–« »«·œ«·… ›Ì “— «·Õ—» „‰ «·‹ Inspector
    public void OnWarButtonClick()
    {
        // ≈Œ›«¡ «·»«‰·“ «·√Ê·Ï Ê«·À«‰Ì…
        waroriPanal.SetActive(false);
        mongelPanal.SetActive(false);
        bgPanal.SetActive(false);
        backBtn.SetActive(false);


        // ≈ŸÂ«— «·»«‰· «·À«·À…
        timerPanal.SetActive(true);

        // ≈Œ›«¡ “— «·Õ—»
        buttonWar.SetActive(false);

        // ≈ŸÂ«— «·“— «·À«‰Ì
        buttonSecond.SetActive(true);
    }
}
