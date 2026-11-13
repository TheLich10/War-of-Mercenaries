using UnityEngine;

public class SortingOrderByY : MonoBehaviour
{
    public int sortingOffset = 0; //  ⁄œÌ· »”Ìÿ ·Ê √—œ  ›—ﬁ À«» 

    // Â‰« ‰ﬂ » «·„”«— «·ﬂ«„· ·ﬂ· ÿ›· œ«Œ· «·√»
    private string[] childPaths = { "lvl1sor/front", "lvl2sor/front", "lvl3sor/front", "lvl4sor/front" };

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        int baseOrder = -(int)(transform.position.y * 100) + sortingOffset;
        spriteRenderer.sortingOrder = baseOrder;

        for (int i = 0; i < childPaths.Length; i++)
        {
            Transform child = transform.Find(childPaths[i]);
            if (child != null)
            {
                SpriteRenderer childSR = child.GetComponent<SpriteRenderer>();
                if (childSR != null)
                {
                    // «Ã⁄·  — Ì» «·ÿ›· √⁄·Ï »„ﬁœ«— 1 + i Õ Ï ÌﬂÊ‰ front √⁄·Ï „‰ anyb_0
                    childSR.sortingOrder = baseOrder + 1 + i;
                }
            }
        }
    }
}
