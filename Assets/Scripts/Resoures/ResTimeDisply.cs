using UnityEngine;
using TMPro;
using System.Linq;

public class ResTimeDisply : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text woodTime; // ‰’ Ê«Õœ ›ﬁÿ ·⁄—÷ «·ﬁÌ„… «·„ÿ·Ê»… ·«Õﬁ«
    public TMP_Text powerTime; // ‰’ Ê«Õœ ›ﬁÿ ·⁄—÷ «·ﬁÌ„… «·„ÿ·Ê»… ·«Õﬁ«
    public TMP_Text stoneTime; // ‰’ Ê«Õœ ›ﬁÿ ·⁄—÷ «·ﬁÌ„… «·„ÿ·Ê»… ·«Õﬁ«
    public TMP_Text ironTime; // ‰’ Ê«Õœ ›ﬁÿ ·⁄—÷ «·ﬁÌ„… «·„ÿ·Ê»… ·«Õﬁ«
    public TMP_Text GoldTime;

    // «· «€«  «·Õ’—Ì… «·„ÿ·Ê»… ··„Ê«—œ
    private string[] resourceTags = new string[] { "wood", "stone", "iron", "power", "CenterCity" };

    void Update()
    {
        // «· Õﬁﬁ „‰ ‰ﬁ—… “— «·›√—… «·√Ì”— √Ê «··„”
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);

            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if (hit.collider != null)
            {
                // «· Õﬁﬁ „„« ≈–« ﬂ«‰ «·ﬂ«∆‰ ÌÕ„· √Õœ «· «€«  „»«‘—…
                if (resourceTags.Contains(hit.collider.tag))
                {
                    ShowResourceInfo(hit.collider.gameObject, hit.collider.tag);
                }
                else
                {
                    // «·»ÕÀ ›Ì «·√»‰«¡
                    Transform childWithTag = null;
                    string foundTag = "";

                    foreach (Transform child in hit.collider.transform)
                    {
                        if (resourceTags.Contains(child.tag))
                        {
                            childWithTag = child;
                            foundTag = child.tag; // Õ›Ÿ «· «€ «·–Ì  „ «·⁄ÀÊ— ⁄·ÌÂ
                            break;
                        }
                    }

                    if (childWithTag != null)
                    {
                        ShowResourceInfo(childWithTag.gameObject, foundTag);
                    }
                    else
                    {
                        ClearUI();
                    }
                }
            }
            else
            {
                ClearUI();
            }
        }
    }

    void ShowResourceInfo(GameObject obj, string objectTag)
    {
        // »‰«¡ ⁄·Ï «· «€° ”‰»ÕÀ ⁄‰ «·”ﬂ—Ì»  «·„‰«”»
        switch (objectTag)
        {
            case "wood":
                buildWoodStroge woodScript = GetScript<buildWoodStroge>(obj);
                if (woodScript != null)
                {
                    float timeRemaining = woodScript.GetTimeToFull();

                    //  ÕÊÌ· «·ÀÊ«‰Ì ≈·Ï ’Ì€… Êﬁ 
                    System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeRemaining);

                    // œ„Ã Ã„·… FULL IN „⁄ «·Êﬁ  «·„‰”ﬁ
                    woodTime.text = $"FULL IN : {timeSpan.ToString(@"hh\:mm\:ss")}";
                }
                break;

            case "stone":
                buildStoneStroge stoneScript = GetScript<buildStoneStroge>(obj);
                if (stoneScript != null)
                {
                    float timeRemaining = stoneScript.GetTimeToFull();

                    //  ÕÊÌ· «·ÀÊ«‰Ì ≈·Ï ’Ì€… Êﬁ 
                    System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeRemaining);

                    // œ„Ã Ã„·… FULL IN „⁄ «·Êﬁ  «·„‰”ﬁ
                    stoneTime.text = $"FULL IN : {timeSpan.ToString(@"hh\:mm\:ss")}";
                }
                break;

            case "iron":
                buildIronStorge ironScript = GetScript<buildIronStorge>(obj);
                if (ironScript != null)
                {
                    float timeRemaining = ironScript.GetTimeToFull();

                    //  ÕÊÌ· «·ÀÊ«‰Ì ≈·Ï ’Ì€… Êﬁ 
                    System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeRemaining);

                    // œ„Ã Ã„·… FULL IN „⁄ «·Êﬁ  «·„‰”ﬁ
                    ironTime.text = $"FULL IN : {timeSpan.ToString(@"hh\:mm\:ss")}";
                }
                break;

            case "power":
                buildPowerStroge powerScript = GetScript<buildPowerStroge>(obj);
                if (powerScript != null)
                {
                    float timeRemaining = powerScript.GetTimeToFull();

                    //  ÕÊÌ· «·ÀÊ«‰Ì ≈·Ï ’Ì€… Êﬁ 
                    System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeRemaining);

                    // œ„Ã Ã„·… FULL IN „⁄ «·Êﬁ  «·„‰”ﬁ
                    powerTime.text = $"FULL IN : {timeSpan.ToString(@"hh\:mm\:ss")}";
                }
                break;

            case "CenterCity":
                BuildingGoldStorage goldscript = GetScript<BuildingGoldStorage>(obj);
                if (goldscript != null)
                {
                    float timeRemaining = goldscript.GetTimeToFull();

                    //  ÕÊÌ· «·ÀÊ«‰Ì ≈·Ï ’Ì€… Êﬁ 
                    System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeRemaining);

                    // œ„Ã Ã„·… FULL IN „⁄ «·Êﬁ  «·„‰”ﬁ
                    GoldTime.text = $"FULL IN : {timeSpan.ToString(@"hh\:mm\:ss")}";
                }
                break;
        }
    }

    // œ«·… „”«⁄œ… (Generic) ··»ÕÀ ⁄‰ «·”ﬂ—Ì»  ›Ì «·ﬂ«∆‰ √Ê ›Ì «·√» (· Ã‰»  ﬂ—«— «·ﬂÊœ)
    private T GetScript<T>(GameObject obj) where T : MonoBehaviour
    {
        T script = obj.GetComponent<T>();

        // ≈–« ·„ ÌÃœÂ ›Ì «·ﬂ«∆‰° Ì»ÕÀ ›Ì «·√» (‰›” «·›ﬂ—… «· Ì «” Œœ„ Â« ›Ì ”ﬂ—Ì» ﬂ «·”«»ﬁ)
        if (script == null && obj.transform.parent != null)
        {
            script = obj.transform.parent.GetComponent<T>();
        }

        return script;
    }

    // œ«·… · ›—Ì€ Ê«ÃÂ… «·„” Œœ„
    void ClearUI()
    {
        if (powerTime != null)
        {
            powerTime.text = "";
        }
        if (woodTime != null)
        {
            woodTime.text = "";
        }
        if (stoneTime != null)
        {
            stoneTime.text = "";
        }
        if (ironTime != null)
        {
            ironTime.text = "";
        }
        if (GoldTime != null)
        {
            GoldTime.text = "";
        }
    }
}