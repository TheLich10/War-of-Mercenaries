using UnityEngine;

public class ResourceStateTracker : MonoBehaviour
{
    [Header("state object")]
    [Tooltip("20 - 60")]
    public GameObject midLevelObject;

    [Tooltip("up 60 ")]
    public GameObject highLevelObject;

    private void Start()
    {
        // «· Õﬁﬁ „‰ «·Õ«·… «·«» œ«∆Ì… ⁄‰œ »œ¡ «··⁄»…
        if (ResourceManager.Instance != null)
        {
            CheckResourceLevels(ResourceManager.Instance.GetTotalResourcesAmount(), ResourceManager.Instance.GetTotalResourcesCapacity());

            // «·«‘ —«ﬂ ›Ì «·ÕœÀ «·–Ì Ì⁄„· ⁄‰œ √Ì  €ÌÌ— ›Ì «·„Ê«—œ
            ResourceManager.Instance.OnResourcesChanged += CheckResourceLevels;
        }
    }

    private void OnDestroy()
    {
        // ≈·€«¡ «·«‘ —«ﬂ · ›«œÌ √Ì  ”—Ì» ··–«ﬂ—… (Memory Leak) ⁄‰œ  œ„Ì— «·ﬂ«∆‰
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourcesChanged -= CheckResourceLevels;
        }
    }

    private void CheckResourceLevels(int currentTotalAmount, int maxTotalCapacity)
    {
        // Õ„«Ì… „‰ Œÿ√ «·ﬁ”„… ⁄·Ï ’›—
        if (maxTotalCapacity <= 0) return;

        // Õ”«» «·‰”»… «·„∆ÊÌ… ··„Ê«—œ «·„Ã„⁄…
        float percentage = ((float)currentTotalAmount / maxTotalCapacity) * 100f;

        //  ÿ»Ìﬁ «·‘—Êÿ »‰«¡ ⁄·Ï «·‰”»… «·„∆ÊÌ…
        if (percentage < 20f)
        {
            // √ﬁ· „‰ 20% - ·« ‘Ì¡ „›⁄·
            SetActiveState(midLevelObject, false);
            SetActiveState(highLevelObject, false);
        }
        else if (percentage >= 20f && percentage < 60f)
        {
            // »Ì‰ 20% Ê √ﬁ· „‰ 60% -  ›⁄Ì· «·√Ê· Ê≈·€«¡ «·À«‰Ì
            SetActiveState(midLevelObject, true);
            SetActiveState(highLevelObject, false);
        }
        else if (percentage >= 60f)
        {
            // „‰ 60% ›„« ›Êﬁ -  ›⁄Ì· «·À«‰Ì Ê≈·€«¡ «·√Ê·
            SetActiveState(midLevelObject, false);
            SetActiveState(highLevelObject, true);
        }
    }

    // œ«·… „”«⁄œ… · Ã‰» —”«∆· «·Œÿ√ ≈–« ‰”Ì‰« —»ÿ «·ﬂ«∆‰«  ›Ì «·‹ Inspector
    private void SetActiveState(GameObject obj, bool state)
    {
        if (obj != null && obj.activeSelf != state)
        {
            obj.SetActive(state);
        }
    }
}