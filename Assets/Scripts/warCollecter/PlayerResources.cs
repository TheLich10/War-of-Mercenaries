using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerResources : MonoBehaviour
{
    public static PlayerResources instance;

    public static int wood;
    public static int stone;
    public static int iron;
    public static int power;

    public TMP_Text woodText;
    public TMP_Text stoneText;
    public TMP_Text ironText;
    public TMP_Text powerText;

    public Image woodImage;
    public Image stoneImage;
    public Image ironImage;
    public Image powerImage;

    private Dictionary<Image, Coroutine> activePulses = new Dictionary<Image, Coroutine>();
    private Dictionary<Image, Vector3> originalScales = new Dictionary<Image, Vector3>();

    private void Awake()
    {
        instance = this;
    }

    public void AddResources(int w, int s, int i, int p)
    {
        if (w > 0) StartPulse(woodImage);
        if (s > 0) StartPulse(stoneImage);
        if (i > 0) StartPulse(ironImage);
        if (p > 0) StartPulse(powerImage);

        wood += w;
        stone += s;
        iron += i;
        power += p;

        UpdateUI();
    }

    public void UpdateUI()
    {
        woodText.text = FormatShortNumber(wood);
        stoneText.text = FormatShortNumber(stone);
        ironText.text = FormatShortNumber(iron);
        powerText.text = FormatShortNumber(power);
    }

    private string FormatShortNumber(int number)
    {
        if (number >= 1_000_000)
            return (number / 1_000_000f).ToString("0.#") + "M";
        else if (number >= 1_000)
            return (number / 1_000f).ToString("0.#") + "k";
        else
            return number.ToString();
    }

    private void StartPulse(Image img)
    {
        if (!originalScales.ContainsKey(img))
            originalScales[img] = img.rectTransform.localScale;

        if (activePulses.ContainsKey(img) && activePulses[img] != null)
            StopCoroutine(activePulses[img]);

        activePulses[img] = StartCoroutine(PulseImage(img));
    }

    private IEnumerator PulseImage(Image img)
    {
        Vector3 originalScale = originalScales[img];
        Vector3 maxAllowedScale = originalScale * 1.2f;

        if (img.rectTransform.localScale.x > maxAllowedScale.x)
            yield break;

        Vector3 targetScale = maxAllowedScale;

        float duration = 0.15f;
        float t = 0;

        // تكبير
        while (t < duration)
        {
            t += Time.deltaTime;
            img.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            yield return null;
        }

        // تصغير
        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            img.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
            yield return null;
        }

        img.rectTransform.localScale = originalScale;

        activePulses[img] = null;
    }
}
