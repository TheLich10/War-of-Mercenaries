using UnityEngine;

public class CheakWhoIsOpen : MonoBehaviour
{
    public GameObject bearObj;
    public GameObject y3soobObj;
    public GameObject mightsoorObj;
    public GameObject womKongObj;

    void Start()
    {
        if (bearObj != null)
        {
            // ≈–« ﬂ«‰ «·√Ê»Ãﬂ  „›⁄·«
            if (!bearObj.activeSelf)
            {
                heroATK.isActiveHero = false;
            }
        }
        if (y3soobObj != null)
        {
            // ≈–« ﬂ«‰ «·√Ê»Ãﬂ  „›⁄·«
            if (!y3soobObj.activeSelf)
            {
                heroArcher2ATK.isActiveHero = false;
            }
        }
        if (womKongObj != null)
        {
            // ≈–« ﬂ«‰ «·√Ê»Ãﬂ  „›⁄·«
            if (!womKongObj.activeSelf)
            {
                heroArcherATK.isActiveHero = false;
            }
        }
        if (mightsoorObj != null)
        {
            // ≈–« ﬂ«‰ «·√Ê»Ãﬂ  „›⁄·«
            if (!mightsoorObj.activeSelf)
            {
                hero2ATK.isActiveHero = false;
            }
        }
    }

}
