using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoldPriceButton : MonoBehaviour
{
    [Header("ÇáãæÇÑÏ")]
    public TMP_Text woodText;
    public TMP_Text stoneText;
    public TMP_Text ironText;
    public TMP_Text energyText;

    [Header("ÒÑ ÇáĞåÈ")]
    public Button goldButton;
    public TMP_Text goldPriceText; // ÇáäÕ ÏÇÎá ÇáÈæÊä áÚÑÖ ÇáÓÚÑ

    private int goldPrice;

    private void Update()
    {
        UpdateGoldPrice();
    }

    void UpdateGoldPrice()
    {
        // 1. ÇÓÊÎÏÇã int.TryParse áÊİÇÏí ÇáÇäåíÇÑ ÚäÏ æÌæÏ ÚáÇãÉ "-"
        bool isWoodValid = int.TryParse(woodText.text, out int wood);
        bool isStoneValid = int.TryParse(stoneText.text, out int stone);

        // ÅĞÇ İÔá ÊÍæíá ÇáäÕ (íÚäí Ãä ÇáãÈäì æÕá ááÍÏ ÇáÃŞÕì)
        if (!isWoodValid || !isStoneValid)
        {
            goldButton.interactable = false; // ÊÚØíá ÇáÒÑ
            goldPriceText.text = "-"; // ÅÎİÇÁ ÇáÓÚÑ Ãæ ßÊÇÈÉ "MAX LEVEL"
            return; // ÅäåÇÁ ÇáÏÇáÉ åäÇ áãäÚ Ãí ÍÓÇÈÇÊ ÅÖÇİíÉ
        }

        // 2. ÅÚÇÏÉ ÊİÚíá ÇáÒÑ İí ÍÇá ßÇäÊ ÇáÈíÇäÇÊ ÕÍíÍÉ
        //goldButton.interactable = true;

        // äÃÎĞ ÃßÈÑ ŞíãÉ Èíä ÇáÎÔÈ æÇáÍÌÑ áÃäåÇ åí ÇáãÄËÑÉ
        int maxResource = Mathf.Max(wood, stone);

        // ÊÍÏíÏ ÇáÓÚÑ ÍÓÈ ÇáÌÏæá
        if (maxResource < 10000)
            goldPrice = 10;
        else if (maxResource < 50000)
            goldPrice = 15;
        else if (maxResource < 100000)
            goldPrice = 20;
        else if (maxResource < 500000)
            goldPrice = 40;
        else if (maxResource < 1000000)
            goldPrice = 60;
        else
        {
            // ßá 500 Ãáİ ÅÖÇİíÉ İæŞ Çáãáíæä ÊÒíÏ 20
            int extraSteps = (maxResource - 1000000) / 500000 + 1;
            goldPrice = 60 + (extraSteps * 20);
        }

        // ÊÍÏíË ÇáäÕ ÏÇÎá ÇáÈæÊä
        goldPriceText.text = "BUILD NOW!  " + goldPrice.ToString();
    }

    public int GetGoldPrice()
    {
        return goldPrice;
    }
}