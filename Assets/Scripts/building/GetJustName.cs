using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetJustName : MonoBehaviour
{
    public TMP_Text NameBuilding;
    public static string objectName;

    // œ«·…  ı” œ⁄Ï ⁄‰œ «·‰ﬁ— ·√Œ– «·«”„ ›ﬁÿ
    public void OnClick()
    {
        if (NameBuilding != null)
        {
            objectName = NameBuilding.text;
            //Debug.Log("Just name: " + objectName);  œÌ»ﬂ
        }
    }
}
