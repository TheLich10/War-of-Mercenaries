using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class HideButtonSimple : MonoBehaviour
{
    public static HideButtonSimple intstin;

    public Button targetButton;          // «”Õ» «·“— Â‰« „‰ «·‹ Inspector
    public float hideDuration = 10f;     // „œ… «·«Œ ›«¡ »«·ÀÊ«‰Ì

    public GameObject extraObject;       // «”Õ» «·√Ê»Ãﬂ  «·ÃœÌœ Â‰« „‰ «·‹ Inspector

    // „ €Ì—«  «·Õ›Ÿ
    [HideInInspector] public bool isButtonHidden = false;
    [HideInInspector] public long buttonShowTimeTicks = 0;
    [HideInInspector] public bool isExtraObjectActive = false;
    [HideInInspector] public long extraObjectHideTimeTicks = 0;

    public TextMeshProUGUI timeText;

    private void Awake()
    {
        intstin = this;

        // «· √ﬂœ „‰ √‰ «·√Ê»Ãﬂ  „⁄ÿ· ›Ì «·»œ«Ì… ⁄‰œ »œ¡ «··⁄»…
        if (extraObject != null)
        {
            extraObject.SetActive(false);
        }
        if (timeText != null && !isButtonHidden)
        {
            timeText.text = ""; // √Ê Ì„ﬂ‰ﬂ Ã⁄·Â ›«—€« ""
        }
    }

    private void Update()
    {
        // «· Õﬁﬁ „‰ “— «·≈Œ›«¡
        if (isButtonHidden)
        {
            // Õ”«» «·Êﬁ  «·„ »ﬁÌ
            long remainingTicks = buttonShowTimeTicks - System.DateTime.Now.Ticks;

            if (remainingTicks > 0)
            {
                //  ÕÊÌ· «·‹ Ticks ≈·Ï Êﬁ  „ﬁ—Ê¡ (”«⁄« :œﬁ«∆ﬁ:ÀÊ«‰Ì)
                if (timeText != null)
                {
                    System.TimeSpan ts = System.TimeSpan.FromTicks(remainingTicks);
                    timeText.text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                }
            }
            else // ≈–« «‰ ÂÏ «·Êﬁ 
            {
                isButtonHidden = false;
                if (targetButton != null) targetButton.gameObject.SetActive(true);

                // ≈›—«€ «·‰’ √Ê ﬂ «»… "Ã«Â“" ⁄‰œ «‰ Â«¡ «·Êﬁ 
                if (timeText != null) timeText.text = "";
            }
        }

        // «· Õﬁﬁ „‰ «·√Ê»Ãﬂ  «·≈÷«›Ì
        if (isExtraObjectActive && System.DateTime.Now.Ticks >= extraObjectHideTimeTicks)
        {
            isExtraObjectActive = false;
            if (extraObject != null) extraObject.SetActive(false);
        }
    }

    // œ«·… ·«” ﬁ»«· »Ì«‰«  «·Õ›Ÿ
    public void LoadData(bool btnHidden, long btnTicks, bool extraActive, long extraTicks)
    {
        isButtonHidden = btnHidden;
        buttonShowTimeTicks = btnTicks;
        isExtraObjectActive = extraActive;
        extraObjectHideTimeTicks = extraTicks;

        //  ÿ»Ìﬁ «·Õ«·… ›Ê—« ⁄‰œ »œ¡ «··⁄»… »‰«¡ ⁄·Ï «·Õ›Ÿ
        if (targetButton != null) targetButton.gameObject.SetActive(!isButtonHidden);
        if (extraObject != null) extraObject.SetActive(isExtraObjectActive);
    }

    // «·œ«·… «·√’·Ì…:  Œ›Ì «·“— »‰«¡ ⁄·Ï ﬁÌ„… hideDuration (10 ÀÊ«‰Ì)
    public void HideForSeconds()
    {
        if (targetButton != null)
        {
            isButtonHidden = true;
            // Õ”«» «·Êﬁ  ›Ì «·„” ﬁ»· («·ÀÊ«‰Ì «·„Õœœ… * ⁄œœ «· Ìﬂ” ›Ì «·À«‰Ì…)
            buttonShowTimeTicks = System.DateTime.Now.Ticks + (long)(hideDuration * System.TimeSpan.TicksPerSecond);
            targetButton.gameObject.SetActive(false);
        }
        hideBTNfor5s(); // · ‘€Ì· «·√Ê»Ãﬂ  «·≈÷«›Ì
    }

    public void HideFast()
    {
        if (targetButton != null)
        {
            isButtonHidden = true;
            buttonShowTimeTicks = System.DateTime.Now.Ticks + (long)(0.1f * System.TimeSpan.TicksPerSecond);
            targetButton.gameObject.SetActive(false);
        }
        hideBTNfor5s();
    }

    //  „  ⁄œÌ· «·ﬂÊ—Ê Ì‰ ·Ì” ﬁ»· «·„œ… «·“„‰Ì… ﬂ„ €Ì— (duration)

    public void hideBTNfor5s()
    {
        if (extraObject != null)
        {
            isExtraObjectActive = true;
            extraObjectHideTimeTicks = System.DateTime.Now.Ticks + (long)(5f * System.TimeSpan.TicksPerSecond);
            extraObject.SetActive(true);
        }
    }

}