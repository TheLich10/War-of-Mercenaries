using UnityEngine;
using TMPro;
using UnityEngine.UI; // √÷›‰« Â–Â «·„ﬂ »… ·· ⁄«„· „⁄ «·’Ê— (UI Images)
using System.Linq;

public class WatchPostInfoDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text currentCapacityText; // ‰’ ·⁄—÷ «·⁄œœ «·Õ«·Ì
    public TMP_Text maxCapacityText;     // ‰’ ·⁄—÷ «·Õœ «·√ﬁ’Ï
    public Image capacityFillBar;        // ’Ê—… ‘—Ìÿ «· ⁄»∆… (Fill Bar)

    // «· «€«  «·Õ’—Ì… «·„ÿ·Ê»… ··„»«‰Ì
    private string[] watchPostTags = new string[] { "spy", "spyClan" };

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
                if (watchPostTags.Contains(hit.collider.tag))
                {
                    ShowWatchPostInfo(hit.collider.gameObject);
                }
                else
                {
                    // «·»ÕÀ ›Ì «·√»‰«¡
                    Transform childWithTag = null;
                    foreach (Transform child in hit.collider.transform)
                    {
                        if (watchPostTags.Contains(child.tag))
                        {
                            childWithTag = child;
                            break;
                        }
                    }

                    if (childWithTag != null)
                    {
                        ShowWatchPostInfo(childWithTag.gameObject);
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

    void ShowWatchPostInfo(GameObject obj)
    {
        BarracksWatchPost watchPost = obj.GetComponent<BarracksWatchPost>();

        if (watchPost == null && obj.transform.parent != null)
        {
            watchPost = obj.transform.parent.GetComponent<BarracksWatchPost>();
        }

        if (watchPost != null)
        {
            //  ÕœÌÀ «·‰’Ê’
            if (currentCapacityText != null) currentCapacityText.text = watchPost.currentCapacity.ToString("F0");
            if (maxCapacityText != null) maxCapacityText.text = watchPost.maxCapacity.ToString("F0");

            //  ÕœÌÀ ‘—Ìÿ «· ⁄»∆…
            if (capacityFillBar != null)
            {
                // ‰„‰⁄ «·ﬁ”„… ⁄·Ï ’›— · Ã‰» «·√Œÿ«¡
                if (watchPost.maxCapacity > 0)
                {
                    // Õ”«» ‰”»… «·«„ ·«¡ »Ì‰ 0 Ê 1
                    capacityFillBar.fillAmount = watchPost.currentCapacity / watchPost.maxCapacity;
                }
                else
                {
                    capacityFillBar.fillAmount = 0f;
                }
            }
        }
        else
        {
            ClearUI();
        }
    }

    //  „  €ÌÌ— «”„ «·œ«·… · ‘„·  ›—Ì€ «·»«— √Ì÷«
    void ClearUI()
    {
        if (currentCapacityText != null) currentCapacityText.text = "";
        if (maxCapacityText != null) maxCapacityText.text = "";

        //  ’›Ì— ‘—Ìÿ «· ⁄»∆… ⁄‰œ ⁄œ„  ÕœÌœ „»‰Ï
        if (capacityFillBar != null) capacityFillBar.fillAmount = 0f;
    }
}