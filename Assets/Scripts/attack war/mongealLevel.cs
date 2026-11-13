using UnityEngine;
using UnityEngine.SceneManagement; // ãåã ÌÏÇğ: ÃÖİäÇ åĞå ÇáãßÊÈÉ ááÊÚÇãá ãÚ ÇáãÔÇåÏ

public class mongealLevel : MonoBehaviour
{
    // ãÑÌÚ áÓßÑíÈÊ building ÇáãæÌæÏ Úáì äİÓ ÇáÜ GameObject
    private building buildingScript;
    // ãÊÛíÑ ááÇÍÊİÇÙ ÈÂÎÑ ãÓÊæì Êã ŞÑÇÁÊå (áÊÍÓíä ÇáÃÏÇÁ)
    private int lastKnownLevel = 0;

    void Start()
    {
        // ÇáÍÕæá Úáì ÓßÑíÈÊ building ÇáãÑÊÈØ ÈäİÓ ÇáÃæÈÌßÊ
        buildingScript = GetComponent<building>();
    }

    void Update()
    {
        // ÇáÔÑØ ÇáÌÏíÏ: ÇáÊÍŞŞ ãä ÇÓã ÇáãÔåÏ ÇáÍÇáí
        // ÅĞÇ áã íßä ÇÓã ÇáãÔåÏ "SampleScene"¡ ÃæŞİ ÇáÊäİíĞ æáÇ Êßãá ÈÇŞí ÇáßæÏ
        if (SceneManager.GetActiveScene().name != "SampleScene")
        {
            return;
        }

        // ÇáÊÃßÏ ãä Ãä ÇáÓßÑíÈÊÇÊ ãæÌæÏÉ áÊÌäÈ ÑÓÇÆá ÇáÎØÃ (NullReferenceException)
        if (buildingScript != null)
        {
            // ÇáÊÍŞŞ ããÇ ÅĞÇ ßÇä ãÓÊæì ÇáãÈäì ŞÏ ÊØæÑ Ãæ ÊÛíÑ
            if (buildingScript.level != lastKnownLevel)
            {
                // ÊÍÏíË ÇáãÓÊæì ÇáãÚÑæİ áÏíäÇ
                lastKnownLevel = buildingScript.level;

                // ÊÛííÑ ŞíãÉ stateMongel ÈäÇÁğ Úáì ÇáãÓÊæì ÇáÌÏíÏ
                switch (lastKnownLevel)
                {
                    case 1:
                        deffult.stateMongel = 1;
                        break;
                    case 2:
                        deffult.stateMongel = 2;
                        break;
                    case 3:
                        deffult.stateMongel = 3;
                        break;
                }
            }
        }
    }
}