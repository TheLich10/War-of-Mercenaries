using UnityEngine;

public class BarracksManagerWatchPost : MonoBehaviour
{
    public static BarracksManagerWatchPost Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Ì„ﬂ‰ﬂ  —ﬂÂ ﬂ≈œ«—… ⁄«„… ≈–« √—œ   ”ÃÌ· «·Àﬂ‰« ° ·ﬂ‰ ·‰ ÌﬂÊ‰ Â‰«ﬂ Ã„⁄ ··”⁄« 
    // public void RegisterBarracks(BarracksWatchPost b) { ... } €Ì— ÷—Ê—Ì
}
