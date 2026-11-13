using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MultiResourceChecker : MonoBehaviour
{
    [System.Serializable]
    public class ResourceBuildOption
    {
        public Button buildButton;

        [Header("«·„Ê«—œ «·„ÿ·Ê»… ·Â–« «·»‰«¡")]
        public TMP_Text requiredText1;
        public TMP_Text requiredText2;

        [Header("«·√Ê»Ãﬂ «  «· Ì ” Œ ›Ì ≈–« ﬂ«‰  «· ﬂ·›… 0")]
        public GameObject resourceObject1; // «·√Ê»Ãﬂ  «·„— »ÿ »«·‹ required 1
        public GameObject resourceObject2; // «·√Ê»Ãﬂ  «·„— »ÿ »«·‹ required 2
    }

    [Header("ﬁ«∆„… «·√“—«— Ê„ ÿ·»«  «·„Ê«—œ")]
    public List<ResourceBuildOption> buildOptions = new List<ResourceBuildOption>();

    [Header("‰’Ê’ «·„Ê«—œ «·Õ«·Ì…")]
    public TMP_Text kamaziText;
    public TMP_Text destroyText;
    public TMP_Text bediounText;
    public TMP_Text poisonText;
    public TMP_Text nabelText;
    public TMP_Text mongolenText;
    public TMP_Text sneakyText;
    public TMP_Text kningtText;

    void Update()
    {
        foreach (var option in buildOptions)
        {
            CheckResources(option);
        }
    }

    void CheckResources(ResourceBuildOption option)
    {
        TMP_Text current1 = null;
        TMP_Text current2 = null;

        //  ÕœÌœ «·„Ê«—œ »‰«¡ ⁄·Ï «·»ÿ· «·‰‘ÿ
        if (HeroArcher2.isActiveHero)
        {
            current1 = sneakyText;
            current2 = kningtText;
        }
        else if (HeroArcher.isActiveHero)
        {
            current1 = kamaziText;
            current2 = destroyText;
        }
        else if (HeroUnit.isActiveHero)
        {
            current1 = nabelText;
            current2 = mongolenText;
        }
        else if (HeroUnit2.isActiveHero)
        {
            current1 = bediounText;
            current2 = poisonText;
        }

        if (current1 != null && current2 != null)
        {
            // «” Œ—«Ã «·ﬁÌ„ «·—ﬁ„Ì… „»«‘—… „‰ «·‰’Ê’
            int required1 = int.Parse(option.requiredText1.text);
            int required2 = int.Parse(option.requiredText2.text);
            int currentAmount1 = int.Parse(current1.text);
            int currentAmount2 = int.Parse(current2.text);

            // «·√·Ê«‰
            Color okColor = Color.white;
            Color failColor = Color.red;
            Color fadedColor = new Color(1f, 1f, 1f, 0.5f);

            // «·„ﬁ«—‰…
            bool canBuild = true;

            // --- ›Õ’ «·„Ê—œ «·√Ê· Ê≈Œ›«¡/≈ŸÂ«— «·√Ê»Ãﬂ  ---
            if (option.resourceObject1 != null)
            {
                option.resourceObject1.SetActive(required1 != 0);
            }

            if (required1 == 0)
            {
                option.requiredText1.color = fadedColor;
            }
            else if (currentAmount1 < required1)
            {
                option.requiredText1.color = failColor;
                canBuild = false;
            }
            else
            {
                option.requiredText1.color = okColor;
            }

            // --- ›Õ’ «·„Ê—œ «·À«‰Ì Ê≈Œ›«¡/≈ŸÂ«— «·√Ê»Ãﬂ  ---
            if (option.resourceObject2 != null)
            {
                option.resourceObject2.SetActive(required2 != 0);
            }

            if (required2 == 0)
            {
                option.requiredText2.color = fadedColor;
            }
            else if (currentAmount2 < required2)
            {
                option.requiredText2.color = failColor;
                canBuild = false;
            }
            else
            {
                option.requiredText2.color = okColor;
            }

            //  ›⁄Ì· √Ê  ⁄ÿÌ· «·“—
            option.buildButton.interactable = canBuild;
        }
    }
}