using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BarracksUIManager : MonoBehaviour
{
    [Header("äÕæÕ æÇÌåÉ ÇáãÓÊÎÏã áÚÑÖ ÃÚÏÇÏ ÇáãÍÇÑÈíä")]
    public TMP_Text bedoiunText;
    public TMP_Text scalingEgyptianText;
    public TMP_Text destroyerText;
    public TMP_Text flyingOttomanText;
    public TMP_Text gaulText;
    public TMP_Text helaerText;
    public TMP_Text kamikazeText;
    public TMP_Text knightText;
    public TMP_Text mongolianText;
    public TMP_Text ottmanText;
    public TMP_Text persianText;
    public TMP_Text postionText;
    public TMP_Text sapperText;
    public TMP_Text savageText;
    public TMP_Text sneakyText;

    [Header(" img Fill)")]
    public Image capacityBarFill;         // ÕæÑÉ ÇáÔÑíØ (íÌÈ Ãä Êßæä ãä äæÚ Filled)
    public TMP_Text capacityText;         // ÇáäÕ ÏÇÎá ÇáÈÇÑ "40 / 200"

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        // ÊÍÏíË ÇáÃÚÏÇÏ
        bedoiunText.text = Barracks.bedoiun.ToString();
        scalingEgyptianText.text = Barracks.ScalingEgyptian.ToString();
        destroyerText.text = Barracks.Destroyer.ToString();
        flyingOttomanText.text = Barracks.flyingOttoman.ToString();
        gaulText.text = Barracks.Gaul.ToString();
        helaerText.text = Barracks.Helaer.ToString();
        kamikazeText.text = Barracks.Kamikaze.ToString();
        knightText.text = Barracks.knight.ToString();
        mongolianText.text = Barracks.Mongolian.ToString();
        ottmanText.text = Barracks.ottman.ToString();
        persianText.text = Barracks.persian.ToString();
        postionText.text = Barracks.postion.ToString();
        sapperText.text = Barracks.sapper.ToString();
        savageText.text = Barracks.Savge.ToString();
        sneakyText.text = Barracks.sneaky.ToString();

        // ÊÍÏíË ÔÑíØ ÇáÓÚÉ áÌãíÚ ÇáËßäÇÊ
        if (capacityBarFill != null && capacityText != null)
        {
            float current = BarracksManager.Instance.GetTotalCurrentCapacity();
            float max = BarracksManager.Instance.GetTotalMaxCapacity();

            float fill = max > 0 ? current / max : 0;
            capacityBarFill.fillAmount = fill;

            capacityText.text = current + " / " + max;
        }
    }

}
