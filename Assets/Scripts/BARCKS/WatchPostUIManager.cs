using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WatchPostUIManager : MonoBehaviour
{
    public static WatchPostUIManager Instance;

    [Header("äÕæÕ æÇÌåÉ ÇáãÓÊÎÏã áÚÑÖ ÃÚÏÇÏ ÇáãÍÇÑÈíä")]
    public TMP_Text bedoiunText;
    public TMP_Text scalingEgyptianText;
    public TMP_Text destroyerText;
    //public TMP_Text flyingOttomanText;
    public TMP_Text gaulText;
    //public TMP_Text helaerText;
    public TMP_Text kamikazeText;
    public TMP_Text knightText;
    public TMP_Text mongolianText;
    public TMP_Text ottmanText;
    public TMP_Text persianText;
    public TMP_Text postionText;
    public TMP_Text sapperText;
    public TMP_Text savageText;
    public TMP_Text sneakyText;

    [Header("Img Fill")]
    public Image capacityBarFill;  // ÕæÑÉ ÇáÔÑíØ (íÌÈ Ãä Êßæä ãä äæÚ Filled)
    public TMP_Text capacityText;  // ÇáäÕ ÏÇÎá ÇáÈÇÑ "40 / 200"

    void Awake()
    {
        Instance = this;
    }

    public void SetWatchPost(BarracksWatchPost watchPost)
    {
        // Ýß ÇáÇÔÊÑÇß ãä ÇáãÈäì ÇáÓÇÈÞ
        if (currentWatchPost != null)
            currentWatchPost.OnWatchChanged -= OnWatchChanged;

        currentWatchPost = watchPost;

        // ÇáÇÔÊÑÇß ÈÇáãÈäì ÇáÌÏíÏ
        if (currentWatchPost != null)
            currentWatchPost.OnWatchChanged += OnWatchChanged;

        UpdateUI();
    }
    void OnWatchChanged(float current, float max)
    {
        UpdateUI();
    }



    private BarracksWatchPost currentWatchPost;

    void UpdateUI()
    {
        if (currentWatchPost == null)
            return;

        bedoiunText.text = currentWatchPost.bedoiun.ToString();
        scalingEgyptianText.text = currentWatchPost.ScalingEgyptian.ToString();
        destroyerText.text = currentWatchPost.Destroyer.ToString();
        gaulText.text = currentWatchPost.Gaul.ToString();
        kamikazeText.text = currentWatchPost.Kamikaze.ToString();
        knightText.text = currentWatchPost.knight.ToString();
        mongolianText.text = currentWatchPost.Mongolian.ToString();
        ottmanText.text = currentWatchPost.ottman.ToString();
        persianText.text = currentWatchPost.persian.ToString();
        postionText.text = currentWatchPost.postion.ToString();
        sapperText.text = currentWatchPost.sapper.ToString();
        savageText.text = currentWatchPost.Savge.ToString();
        sneakyText.text = currentWatchPost.sneaky.ToString();

        // ÊÍÏíË ÔÑíØ ÇáÓÚÉ
        float current = currentWatchPost.currentCapacity;
        float max = currentWatchPost.maxCapacity;

        float fill = max > 0 ? current / max : 0;
        capacityBarFill.fillAmount = fill;
        capacityText.text = current + " / " + max;
    }

    public BarracksWatchPost GetCurrentBarracks()
    {
        return currentWatchPost;
    }

}
