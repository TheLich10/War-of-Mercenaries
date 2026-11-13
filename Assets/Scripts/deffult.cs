using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class deffult : MonoBehaviour
{
    public Button WOM_Button;
    public Button BTN_CAVE1;
    public Button BTN_CAVE2;
    public static int stateMongel = 0;


    void Start()
    {
        SaveManager.Instance.LoadGame();

        // «· Õﬁﬁ „„« ≈–« ﬂ«‰  Â–Â ÂÌ «·„—… «·√Ê·Ï · ‘€Ì· «··⁄»…
        if (PlayerPrefs.GetInt("IsFirstRun", 0) == 0)
        {
            // Â–Â «·√ﬂÊ«œ ” ⁄„· „—… Ê«Õœ… ›ﬁÿ
            HeroArcher.isActiveHero = false;
            HeroUnit.isActiveHero = false;
            HeroArcher2.isActiveHero = false;
            HeroUnit2.isActiveHero = false;
            Roulette.WOM_KONG_OPEN = false;
            stateMongel = 0;

            // Õ›Ÿ Õ«·… √‰ «··⁄»…  „  ‘€Ì·Â« ·ﬂÌ ·« Ì ﬂ—— Â–« «·ﬂÊœ „” ﬁ»·«
            PlayerPrefs.SetInt("IsFirstRun", 1);
            PlayerPrefs.Save();
        }
    }

    void Update()
    {
        if(Roulette.WOM_KONG_OPEN == false)
        {
            WOM_Button.interactable = false;
        }
        else if (Roulette.WOM_KONG_OPEN == true)
        {
            WOM_Button.interactable = true;

        }
        if (HeroArcher.isActiveHero == false && HeroUnit.isActiveHero == false && HeroArcher2.isActiveHero == false && HeroUnit2.isActiveHero == false)
        {
            BTN_CAVE1.interactable = false;
            BTN_CAVE2.interactable = false;
        }
        else
        {
            BTN_CAVE1.interactable = true;
            BTN_CAVE2.interactable = true;
        }
    }

}
