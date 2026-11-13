using UnityEngine;
using UnityEngine.UI;

public class HealthBarFollow : MonoBehaviour
{
    public Transform target;          // «·„»‰Ï √Ê GameObject «·–Ì Ì »⁄ ·Â «·‘—Ìÿ
    public Vector3 offset;            // ≈“«Õ… ·—›⁄ «·‘—Ìÿ ›Êﬁ «·„»‰Ï („À·«: (0, 2, 0))
    public Image healthBarFill;       // ’Ê—… «·‹ Fill «·Œ«’… »«·‘—Ìÿ

    private RectTransform rectTransform;
    private Camera mainCamera;

    private float maxHealth;
    private float currentHealth;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    public void Initialize(float maxHealthValue)
    {
        maxHealth = maxHealthValue;
        currentHealth = maxHealthValue;
        UpdateHealthBar();
    }

    public void UpdateHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float percent = currentHealth / maxHealth;
            healthBarFill.fillAmount = percent;

            if (percent > 0.6f)
                healthBarFill.color = Color.green;
            else if (percent > 0.3f)
                healthBarFill.color = Color.yellow;
            else
                healthBarFill.color = Color.red;
        }
    }

    void LateUpdate()
    {
        //  Õﬁﬁ ≈–« target „œ„¯— √Ê €Ì— „ÊÃÊœ
        if (target == null || target.gameObject == null)
        {
            //  œ„Ì— ‘—Ìÿ «·’Õ… ‰›”Â (»œ·« „‰ ≈Œ›«∆Â ›ﬁÿ)
            Destroy(gameObject);
            return;
        }

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position + offset);

        if (screenPos.z < 0)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        rectTransform.position = screenPos;
    }

}
