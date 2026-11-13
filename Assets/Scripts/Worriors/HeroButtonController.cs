using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroButtonManager : MonoBehaviour
{
    [Header("ÇáÃÒÑÇÑ ÇáÃÑÈÚÉ")]
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    //public int imageConditionNumber = 1;




    void Start()
    {
        // ÚäÏ ÇáÈÏÇíÉ äÚØøá ßá ÇáÃÒÑÇÑ
        DisableAllButtons();
        UpdateButtons();
    }

    void Update()
    {
        // ÅĞÇ ßÇäÊ ÇáÔÑæØ ÊÊÛíÑ ÃËäÇÁ ÇááÚÈ¡ äÍÏøË ÇáÃÒÑÇÑ
        UpdateButtons();
    }

    void UpdateButtons()
    {
        // åäÇ ÇáÔÑæØ ÇáÃÑÈÚÉ (íãßäß ÇÓÊÈÏÇáåÇ ÈÔÑæØß ÇáÍŞíŞíÉ)
        bool condition1 = HeroUnit.isActiveHero;
        bool condition2 = HeroUnit2.isActiveHero;
        bool condition3 = HeroArcher.isActiveHero;
        bool condition4 = HeroArcher2.isActiveHero;

        // äİÚá ÇáÒÑ ÇáãØÇÈŞ æäÛáŞ ÇáÈÇŞí
        if (condition1)
            ActivateOnly(button1);
        else if (condition2)
            ActivateOnly(button2);
        else if (condition3)
            ActivateOnly(button3);
        else if (condition4)
            ActivateOnly(button4);
        else
            DisableAllButtons();
    }

    void ActivateOnly(Button btn)
    {
        button1.gameObject.SetActive(btn == button1);
        button2.gameObject.SetActive(btn == button2);
        button3.gameObject.SetActive(btn == button3);
        button4.gameObject.SetActive(btn == button4);

        // ÊÛííÑ ÕæÑÉ ÇáÒÑ ÇáĞí Êã ÊİÚíáå ááÊæ
        UpdateButtonImage(btn);
    }

    void DisableAllButtons()
    {
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);
        button4.gameObject.SetActive(false);
    }

    // --- ÇáæÙíİÉ ÇáãÚÏáÉ áÊÛííÑ ÇáÕæÑÉ áßá ÒÑ ÈÔßá ãäİÕá ---
    // --- ÇáæÙíİÉ ÇáãÚÏáÉ áÃÎĞ ÇáãÓÊæì ãä MonsterLevelManager ---
    void UpdateButtonImage(Button activeBtn)
    {
        string buttonFolder = "";
        string matchedMonsterName = "";

        // ÊÍÏíÏ ÇÓã ÇáãÌáÏ æÇÓã ÇáæÍÔ ÈäÇÁğ Úáì ÇáÒÑ ÇáãİÚá
        if (activeBtn == button1)
        {
            buttonFolder = "Hero1";
            matchedMonsterName = "BearWolf";
        }
        else if (activeBtn == button2)
        {
            buttonFolder = "Hero2";
            matchedMonsterName = "Mightosour";
        }
        else if (activeBtn == button3)
        {
            buttonFolder = "Hero3";
            matchedMonsterName = "Wom-kong";
        }
        else if (activeBtn == button4)
        {
            buttonFolder = "Hero4";
            matchedMonsterName = "DraginFly";
        }

        // ÌáÈ ÑŞã ÇáãÓÊæì ãä ÇáÓßÑÈÊ ÇáÂÎÑ ÈäÇÁğ Úáì ÇÓã ÇáæÍÔ
        int currentLevel = MonsterLevelManager.Instance.GetLevel(matchedMonsterName);

        // ÊÍÏíÏ ÇáãÓÇÑ: ãÌáÏ ÇáÒÑ ÇáãÍÏÏ / ÑŞã ÇáãÓÊæì ÇáãÓÊÎÑÌ
        string imagePath = "HeroImages/" + buttonFolder + "/" + currentLevel;

        // ÊÍãíá ÇáÕæÑÉ ãä ÇáãÓÇÑ
        Sprite newImage = Resources.Load<Sprite>(imagePath);

        // ÊØÈíŞ ÇáÕæÑÉ ÅĞÇ ßÇäÊ ãæÌæÏÉ
        if (newImage != null)
        {
            activeBtn.GetComponent<Image>().sprite = newImage;
        }
        else
        {
            Debug.LogWarning("áã íÊã ÇáÚËæÑ Úáì ÇáÕæÑÉ İí ÇáãÓÇÑ: Resources/" + imagePath);
        }
    }
}