using TMPro;
using UnityEngine;

public class UIValueBinder : MonoBehaviour
{
    public static UIValueBinder Instance;
    [Header("—»ÿ ⁄‰«’— UI")]
    public TMP_Text rocutGoualText;
    public TMP_Text noubalGuradText;
    public TMP_Text nugaIronText;
    public TMP_Text gentaicHealingText;
    public TMP_Text ironPacketText;
    public TMP_Text stonePacketText;
    public TMP_Text hugeLumberText;
    public TMP_Text hugeMightText;
    public TMP_Text lumberPacketText;
    public TMP_Text beduin20Text;
    public TMP_Text hugeStoneText;
    public TMP_Text mightPacketText;

    [Header("«·ﬁÌ„ «·—ﬁ„Ì…")]
    private int rocutGoual = 0;
    private int noubalGurad = 0;
    private int nugaIron = 0;
    private int gentaicHealing = 0;
    private int ironPacket = 0;
    private int stonePacket = 0;
    private int hugeLumber = 0;
    private int hugeMight = 0;
    private int lumberPacket = 0;
    private int beduin20 = 0;
    private int hugeStone = 0;
    private int mightPacket = 0;

    private const int MAX_VALUE = 20;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        UpdateUIText();
    }

    public void UpdateUIText()
    {
        rocutGoualText.text = $"{rocutGoual} / {MAX_VALUE}";
        noubalGuradText.text = $"{noubalGurad} / {MAX_VALUE}";
        nugaIronText.text = $"{nugaIron} / {MAX_VALUE}";
        gentaicHealingText.text = $"{gentaicHealing} / {MAX_VALUE}";

        ironPacketText.text = $"{ironPacket} / {MAX_VALUE}";
        stonePacketText.text = $"{stonePacket} / {MAX_VALUE}";
        hugeLumberText.text = $"{hugeLumber} / {MAX_VALUE}";
        hugeMightText.text = $"{hugeMight} / {MAX_VALUE}";
        lumberPacketText.text = $"{lumberPacket} / {MAX_VALUE}";
        beduin20Text.text = $"{beduin20} / {MAX_VALUE}";
        hugeStoneText.text = $"{hugeStone} / {MAX_VALUE}";
        mightPacketText.text = $"{mightPacket} / {MAX_VALUE}";
    }

    public void IncreaseValue(string itemName)
    {
        SetValue(itemName, GetValue(itemName) + 1);
    }

    public void DecreaseValue(string itemName)
    {
        SetValue(itemName, GetValue(itemName) - 1);
    }

    private int GetValue(string itemName)
    {
        switch (itemName)
        {
            case "rocut Goual": return rocutGoual;
            case "Noubal Gurad": return noubalGurad;
            case "Nuga Iron": return nugaIron;
            case "Gentaic Healing": return gentaicHealing;

            case "Iron packet": return ironPacket;
            case "stone packet": return stonePacket;
            case "HUGE LUMBER": return hugeLumber;
            case "HUGE might": return hugeMight;
            case "LUMBER packet": return lumberPacket;
            case "20 beduin": return beduin20;
            case "HUGE stone": return hugeStone;
            case "Might Packet": return mightPacket;
            default: return 0;
        }
    }

    private void SetValue(string itemName, int value)
    {
        value = Mathf.Clamp(value, 0, MAX_VALUE); // ·« Ì Ã«Ê“ 20 Ê·« Ì‰ﬁ’ ⁄‰ 0

        switch (itemName)
        {
            case "rocut Goual": rocutGoual = value; break;
            case "Noubal Gurad": noubalGurad = value; break;
            case "Nuga Iron": nugaIron = value; break;
            case "Gentaic Healing": gentaicHealing = value; break;

            case "Iron packet": ironPacket = value; break;
            case "stone packet": stonePacket = value; break;
            case "HUGE LUMBER": hugeLumber = value; break;
            case "HUGE might": hugeMight = value; break;
            case "LUMBER packet": lumberPacket = value; break;
            case "20 beduin": beduin20 = value; break;
            case "HUGE stone": hugeStone = value; break;
            case "Might Packet": mightPacket = value; break;
        }

        UpdateUIText();
    }
}
