using UnityEngine;
using TMPro;
using System.Linq;

public class DefensiveBuildingInfoDisplay : MonoBehaviour
{
    public TMP_Text healthText;  // ‰’ TMP ·⁄—÷ «·’Õ… «·Õ«·Ì…
    public TMP_Text damageText;  // ‰’ TMP ·⁄—÷ «·÷——

    // «· «€«  «· Ì  ⁄»— ⁄‰ «·„»«‰Ì «·œ›«⁄Ì… «· Ì  —Ìœ «· Õﬁﬁ „‰Â«
    private string[] defensiveTags = new string[]
    {
        "Tower", "Cannon", "Trapfire", "flayTower", "boomTower","archer","archerOP","flameT","Mirror","wall","boomOP"
        // √÷› »«ﬁÌ «· «€«  «·Œ«’… »„»«‰Ìﬂ «·œ›«⁄Ì… Â‰«
    };

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);

            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);

            if (hit.collider != null)
            {
                // ‰»ÕÀ ≈–« «· «€ ›Ì ‰›” «·ﬂ«∆‰
                if (defensiveTags.Contains(hit.collider.tag))
                {
                    ShowBuildingInfo(hit.collider.gameObject);
                }
                else
                {
                    // ‰»ÕÀ ›Ì «·√»‰«¡ ≈–« √Ì „‰Â„ ÌÕ„· «· «€ «·„ÿ·Ê»
                    Transform childWithTag = null;
                    foreach (Transform child in hit.collider.transform)
                    {
                        if (defensiveTags.Contains(child.tag))
                        {
                            childWithTag = child;
                            break;
                        }
                    }

                    if (childWithTag != null)
                    {
                        ShowBuildingInfo(childWithTag.gameObject);
                    }
                    else
                    {
                        // ·„ ‰Ãœ  «€ „‰«”» ·« ›Ì «·√» Ê·« ›Ì «·√»‰«¡
                        ClearTexts();
                    }
                }
            }
            else
            {
                ClearTexts();
            }
        }
    }

    void ShowBuildingInfo(GameObject obj)
    {
        if (obj.tag == "wall")
        {
            ShowWallInfo(obj);
            return;
        }

        // ‰Õ«Ê· «·Õ’Ê· ⁄·Ï DefensiveBuilding „‰ Â–« «·ﬂ«∆‰ √Ê „‰ «·√»
        DefensiveBuilding defBuilding = obj.GetComponent<DefensiveBuilding>();

        if (defBuilding == null && obj.transform.parent != null)
        {
            defBuilding = obj.transform.parent.GetComponent<DefensiveBuilding>();
        }

        if (defBuilding != null)
        {
            healthText.text = defBuilding.health.ToString("F0");
            damageText.text = defBuilding.damage.ToString("F0");
        }
        else
        {
            healthText.text = "";
            damageText.text = "";
        }
    }

    void ShowWallInfo(GameObject obj)
    {
        building wallBuilding = obj.GetComponent<building>();

        if (wallBuilding == null && obj.transform.parent != null)
        {
            wallBuilding = obj.transform.parent.GetComponent<building>();
        }

        if (wallBuilding != null)
        {
            healthText.text = wallBuilding.health.ToString("F0");  // «› —÷ √‰ ”ﬂ—Ì»  building ÌÕ ÊÌ ⁄·Ï health
        }
        else
        {
            healthText.text = "";
        }
    }

    void ClearTexts()
    {
        healthText.text = "";
        damageText.text = "";
    }
}
