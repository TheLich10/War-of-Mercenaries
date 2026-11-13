using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BarForSor : MonoBehaviour
{
    public GameObject panelTarget;       // «·»«— œ«Œ· «·»‰·
    public Image panelFillImage;         // «·’Ê—… «·œ«Œ·Ì… ··»«— ›Ì «·»‰·
    public TMP_Text timeText;            // ‰’ «·Êﬁ  «·„ »ﬁÌ ›Ì «·»‰·

    private UpgradeBarFollow currentBar;
    private bool isUpdating = false;

    void Update()
    {
        //  ÕœÌÀ «·»«— Ê«·‰’ ›Ì «·»‰·
        if (isUpdating && currentBar != null)
        {
            panelFillImage.fillAmount = currentBar.fillImage.fillAmount;

            if (timeText != null)
            {
                float remainingTime = Mathf.Max(0f, currentBar.duration - currentBar.timer);

                int hours = Mathf.FloorToInt(remainingTime / 3600f);
                int minutes = Mathf.FloorToInt((remainingTime % 3600f) / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);

                timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }


            // ≈–« «‰ ÂÏ «·»«— √Ê «·„»‰Ï  „  œ„Ì—Â
            if (!currentBar.gameObject.activeInHierarchy)
            {
                ResetPanelBar();
            }
        }

        // «·‰ﬁ— ⁄·Ï «·„»‰Ï
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && hit.name == "Capsule")
            {
                ShowBarForBuilding(hit.transform);
            }

        }
    }

    void ShowBarForBuilding(Transform building)
    {
        // «·»ÕÀ ⁄‰ ﬂ· «·»«—«  »«” Œœ«„ «· «Ã "Timer"
        GameObject[] bars = GameObject.FindGameObjectsWithTag("sorTimer");
        UpgradeBarFollow closestBar = null;
        float minDist = float.MaxValue;

        foreach (GameObject barObj in bars)
        {
            UpgradeBarFollow bar = barObj.GetComponent<UpgradeBarFollow>();
            if (bar != null)
            {
                //  Ã«Â· «·»«— «·Õ«·Ì «·„” Œœ„
                if (bar == currentBar)
                    continue;

                float dist = Vector3.Distance(building.position, bar.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestBar = bar;
                }
            }
        }

        if (closestBar != null)
        {
            currentBar = closestBar;
            isUpdating = true;
            panelFillImage.fillAmount = currentBar.fillImage.fillAmount;

            if (panelTarget != null)
                panelTarget.SetActive(true);
        }
    }


    void ResetPanelBar()
    {
        isUpdating = false;
        currentBar = null;
        panelFillImage.fillAmount = 0f;

        if (timeText != null)
            timeText.text = "";

        if (panelTarget != null)
            panelTarget.SetActive(false);
    }
    // œ«·… · ⁄ÌÌ‰ «·»«— «·Õ«·Ì ≈·Ï À«‰Ì… Ê«Õœ…

}
