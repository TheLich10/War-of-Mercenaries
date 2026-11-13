using UnityEngine;

public class IsometricSpriteShadow : MonoBehaviour
{
    // ÇÓÍÈ ßÇÆä ÇáÔÎÕíÉ ÇáÃÓÇÓíÉ åäÇ ãä ÇáãİÊÔ (Inspector)
    public SpriteRenderer playerSpriteRenderer;

    private SpriteRenderer shadowSpriteRenderer;

    [Header("ÅÚÏÇÏÇÊ ÇáÙá")]
    [Range(0f, 1f)] public float shadowOpacity = 0.3f; // ÏÑÌÉ ÔİÇİíÉ ÇáÙá
    public Vector3 offset = new Vector3(0f, -0.1f, 0f); // ÅÒÇÍÉ ÇáÙá Úä ÃŞÏÇã ÇáÔÎÕíÉ

    [Header("ÒÇæíÉ Çáãíá æÃÈÚÇÏ ÇáÙá (Isometric)")]
    public float skewX = 0.5f; // ÏÑÌÉ ãíáÇä ÇáÙá ÃİŞíÇğ áíäÇÓÈ ÇáãäÙæÑ
    public float scaleY = 0.4f; // ÊŞáíÕ ÇáØæá ÇáÑÃÓí ááÙá áíÙåÑ ßÃäå Úáì ÇáÃÑÖ
    [Tooltip("ÒíÇÏÉ åĞÇ ÇáÑŞã ÊÌÚá ÇáÙá ÃÚÑÖ")]
    public float scaleX = 1.3f; // ÇáãÊÛíÑ ÇáÌÏíÏ: ÇáÊÍßã ÈÚÑÖ ÇáÙá (ÇáŞíãÉ ÇáÇİÊÑÇÖíÉ 1.3 áÌÚáå ÃÚÑÖ ŞáíáÇğ)

    void Start()
    {
        // ÇáÍÕæá Úáì ÇáÜ SpriteRenderer ÇáÎÇÕ ÈÇáÙá
        shadowSpriteRenderer = GetComponent<SpriteRenderer>();

        // ÌÚá áæä ÇáÙá ÃÓæÏ ÔİÇİ
        shadowSpriteRenderer.color = new Color(0f, 0f, 0f, shadowOpacity);

        // ÊÑÊíÈ ÇáØÈŞÇÊ (Sorting Order) áíßæä ÇáÙá Îáİ ÇáÔÎÕíÉ ÏÇÆãÇğ
        shadowSpriteRenderer.sortingLayerName = playerSpriteRenderer.sortingLayerName;
        shadowSpriteRenderer.sortingOrder = playerSpriteRenderer.sortingOrder - 1;
    }

    void LateUpdate()
    {
        if (playerSpriteRenderer == null || shadowSpriteRenderer == null) return;

        // 1. ÊÍÏíË Ôßá ÇáÙá áíÊØÇÈŞ ãÚ ÅØÇÑ ÇáÃäãíÔä ÇáÍÇáí ááÔÎÕíÉ
        shadowSpriteRenderer.sprite = playerSpriteRenderer.sprite;
        shadowSpriteRenderer.flipX = playerSpriteRenderer.flipX;
        shadowSpriteRenderer.flipY = playerSpriteRenderer.flipY;

        // 2. ÊÍÏíË ãæŞÚ ÇáÙá ÈÇáäÓÈÉ ááÇÚÈ ãÚ ÇáÅÒÇÍÉ
        transform.position = playerSpriteRenderer.transform.position + offset;

        // 3. ÊØÈíŞ ãÕİæİÉ ÇáÊÍæíá (Matrix) áÚãá ÇáÅãÇáÉ (Skew) æÇáãäÙæÑ ÇáÇíÒæãÊÑí
        // ŞãäÇ åäÇ ÈÖÑÈ ÇáÜ Scale ÇáÎÇÕ ÈÇááÇÚÈ İí ãÊÛíÑ ÇáÜ scaleX ÇáÌÏíÏ áÊæÓíÚ ÇáÙá ÃİŞíÇğ
        float finalScaleX = playerSpriteRenderer.transform.localScale.x * scaleX;
        transform.localScale = new Vector3(finalScaleX, scaleY, 1f);

        // ÅãÇáÉ ÇáÙá ÈÒÇæíÉ ÊäÇÓÈ ÇÊÌÇå ÇáÅÖÇÁÉ ÇáÇİÊÑÇÖí İí áÚÈÊß
        transform.localRotation = Quaternion.Euler(0f, 0f, skewX * 90f);
    }
}