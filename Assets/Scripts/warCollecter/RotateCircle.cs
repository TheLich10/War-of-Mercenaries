using UnityEngine;

public class RotateCircle : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 100f; // ”—⁄… «·œÊ—«‰ (œ—Ã…/À«‰Ì…)

    [Header("Effect Settings")]
    public float spawnInterval = 3f;   // ﬂ· ﬂ„ À«‰Ì… ÌŸÂ— «·„ƒÀ—
    public float moveUpSpeed = 1f;     // ”—⁄… ’⁄Êœ «·‰”Œ…
    public float fadeDuration = 1.5f;  // Êﬁ  «· ·«‘Ì ﬁ»· «·«Œ ›«¡

    private float timer;
    private SpriteRenderer childRenderer; // SpriteRenderer ··«»‰ "img"

    void Awake()
    {
        // «»ÕÀ ⁄‰ «·«»‰ »«·«”„ "img" ÊŒ– „‰Â SpriteRenderer
        Transform child = transform.Find("img");
        if (child != null)
        {
            childRenderer = child.GetComponent<SpriteRenderer>();
        }

        if (childRenderer == null)
        {
            Debug.LogWarning("·„ Ì „ «·⁄ÀÊ— ⁄·Ï «·«»‰ 'img' √Ê SpriteRenderer »œ«Œ·Â!");
        }
    }

    void Update()
    {
        // œÊ—«‰ ÕÊ· „ÕÊ— Z (2D)
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // „ƒﬁ  «·‰”Œ
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEffect();
            timer = 0f;
        }
    }

    void SpawnEffect()
    {
        if (childRenderer == null) return;

        Transform childT = childRenderer.transform;

        // ≈‰‘«¡ ‰”Œ… ÃœÌœ… »‰›” «·√» (Parent) Õ Ï  ÿ«»ﬁ «·„Êﬁ⁄ Ê«·ÕÃ„  „«„«
        GameObject clone = new GameObject("CircleEffect");
        clone.transform.SetParent(childT.parent, false); // false = Õ«›Ÿ ⁄·Ï localPosition
        clone.transform.localPosition = childT.localPosition;
        clone.transform.localRotation = childT.localRotation;
        clone.transform.localScale = childT.localScale;

        // ‰”Œ SpriteRenderer „‰ «·«»‰ "img"
        SpriteRenderer cloneRenderer = clone.AddComponent<SpriteRenderer>();
        cloneRenderer.sprite = childRenderer.sprite;
        cloneRenderer.color = childRenderer.color;
        cloneRenderer.flipX = childRenderer.flipX;
        cloneRenderer.flipY = childRenderer.flipY;
        cloneRenderer.sortingLayerID = childRenderer.sortingLayerID;
        cloneRenderer.sortingOrder = childRenderer.sortingOrder;
        cloneRenderer.material = childRenderer.material;

        // »œ¡ Õ—ﬂ… Ê ·«‘Ì «·‰”Œ…
        clone.AddComponent<EffectMover>().Init(moveUpSpeed, fadeDuration);
    }
}

public class EffectMover : MonoBehaviour
{
    private float moveUpSpeed;
    private float fadeDuration;
    private float timer;
    private SpriteRenderer spriteRenderer;
    private float startAlpha = 1f;

    public void Init(float moveSpeed, float fadeTime)
    {
        moveUpSpeed = moveSpeed;
        fadeDuration = Mathf.Max(0.01f, fadeTime); // „‰⁄ «·ﬁ”„… ⁄·Ï ’›—
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) startAlpha = spriteRenderer.color.a;
        timer = 0f;
    }

    void Update()
    {
        //  Õ—Ìﬂ ··√⁄·Ï »‘ﬂ· À«»  ›Ì «·⁄«·„ (·« Ì √À— »œÊ—«‰ «·√»)
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime, Space.World);

        // «· ·«‘Ì «· œ—ÌÃÌ
        timer += Time.deltaTime;
        if (spriteRenderer != null)
        {
            float t = Mathf.Clamp01(timer / fadeDuration);
            Color c = spriteRenderer.color;
            c.a = Mathf.Lerp(startAlpha, 0f, t);
            spriteRenderer.color = c;
        }

        // ⁄‰œ «‰ Â«¡ «· ·«‘Ì ->  œ„Ì—
        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }

}
