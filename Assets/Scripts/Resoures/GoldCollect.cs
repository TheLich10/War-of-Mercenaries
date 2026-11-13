using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class GoldCollector : MonoBehaviour
{
    public TextMeshProUGUI goldText; // مرجع إلى نص الذهب في UI
    public TextMeshProUGUI goldText2; // مرجع إلى نص الذهب في UI

    private static int goldAmount = 500; // الذهب الابتدائي (سيحتفظ بقيمته بين المشاهد بفضل static)
    private BuildingGoldStorage targetBuilding;

    public GameObject popupPrefab;
    public Sprite goldIcon;

    private int popupIndex = 0;
    private Transform lastClickedBuilding;

    private static GoldCollector instance; // ✅ مرجع للكائن نفسه

    void Start()
    {
        instance = this;
        UpdateGoldUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ✅ لا تكمل إذا كان المؤشر فوق عنصر UI
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            DetectBuilding();
        }
    }

    // 1. أضف دالة SetGold ليتمكن SaveManager من إرسال القيمة المخزنة
    public static void SetGold(int amount)
    {
        goldAmount = amount;
        if (instance != null) instance.UpdateGoldUI();
    }

    void DetectBuilding()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("CenterCity"))
        {
            lastClickedBuilding = hit.collider.transform;

            // ✅ جلب سكربت المبنى هنا فقط عند الضغط عليه (أفضل بكثير للأداء)
            targetBuilding = hit.collider.GetComponent<BuildingGoldStorage>();

            CollectGold();
        }
    }

    void CollectGold()
    {
        if (targetBuilding != null)
        {
            int collectedGold = targetBuilding.CollectGold();
            goldAmount += collectedGold;

            if (PlayerLevel.instance != null)
            {
                PlayerLevel.instance.AddXP(10);
            }

            if (collectedGold > 0)
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX("Gold_collect");
                }
                ShowPopup(goldIcon, collectedGold, lastClickedBuilding);
            }

            UpdateGoldUI();
        }
    }

    void UpdateGoldUI()
    {
        // ✅ التأكد من أن النصوص موجودة لتجنب الأخطاء إذا نسيت إضافتها في الـ Inspector
        if (goldText != null) goldText.text = goldAmount.ToString();
        if (goldText2 != null) goldText2.text = goldAmount.ToString();
    }

    void ShowPopup(Sprite icon, int amount, Transform target)
    {
        GameObject popup = Instantiate(popupPrefab, GameObject.Find("Canvas").transform);
        Vector3 baseScreenPos = Camera.main.WorldToScreenPoint(target.position + new Vector3(0.5f, 2f, 0));
        Vector3 offset = new Vector3(popupIndex * 40f, 0f, 0f);
        popup.transform.position = baseScreenPos + offset;

        popup.GetComponent<ResourcePopup>().Setup(icon, amount);

        popupIndex++;
        Invoke(nameof(ResetPopupIndex), 0.1f);
    }

    void ResetPopupIndex()
    {
        popupIndex = 0;
    }

    // ✅ دالة عامة لإرجاع قيمة الذهب الحالية
    public static int GetGold()
    {
        return goldAmount;
    }

    // ✅ إضافة ذهب
    public static void AddGold(int amount)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.AddGoldGradually(amount));
        }
        else
        {
            goldAmount += amount;
        }
    }

    private IEnumerator AddGoldGradually(int amountToAdd)
    {
        int added = 0;
        int step = 1;
        while (added < amountToAdd)
        {
            int remaining = amountToAdd - added;
            int increment = Mathf.Min(step, remaining);
            goldAmount += increment;
            added += increment;
            UpdateGoldUI();
            step *= 2;
            yield return new WaitForSeconds(0.05f);
        }
    }

    // ✅ إزالة ذهب تدريجيًا
    public static void RemoveGold(int amount)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.RemoveGoldGradually(amount));
        }
        else
        {
            goldAmount -= amount;
            if (goldAmount < 0) goldAmount = 0;
        }
    }

    private IEnumerator RemoveGoldGradually(int amountToRemove)
    {
        int removed = 0;
        int step = 1;
        while (removed < amountToRemove)
        {
            int remaining = amountToRemove - removed;
            int decrement = Mathf.Min(step, remaining);

            goldAmount -= decrement;
            if (goldAmount < 0) goldAmount = 0;

            removed += decrement;

            UpdateGoldUI();

            step *= 2;
            yield return new WaitForSeconds(0.05f);
        }
    }
}