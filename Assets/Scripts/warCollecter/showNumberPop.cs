using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class showNumberPop : MonoBehaviour
{
    public GameObject popupPrefab; // prefab íÍÊæí Úáì äÕ (Text Ãæ TMP_Text)

    // ãÊÛíÑ áÍİÙ ÇáãÑÌÚ ááäÕ ÇáÙÇåÑ ÍÇáíÇğ
    private TextPopup activePopup;

    public void OnHit(float hit)
    {
        //Debug.Log("the hit in function is :" + hit);
        if (hit > 0)
        {
            ShowPopup(Mathf.Round(hit));
        }
    }

    void ShowPopup(float hitNumber)
    {
        // ÇáÊÍŞŞ ããÇ ÅĞÇ ßÇä åäÇß äÕ äÔØ æáã íÎÊİö ÈÚÏ
        if (activePopup != null)
        {
            // ÅĞÇ ßÇä ÇáäÕ ãæÌæÏÇğ¡ Şã ÈÅÖÇİÉ ÇáÖÑÑ Åáíå İŞØ
            activePopup.AddDamage(Mathf.Round(hitNumber));
        }
        else
        {
            // ÅĞÇ áã íßä åäÇß äÕ (Ãæ Êã ÊÏãíÑå)¡ Şã ÈÅäÔÇÁ æÇÍÏ ÌÏíÏ
            GameObject popup = Instantiate(popupPrefab, GameObject.Find("Canvas").transform);

            // ÍÓÇÈ ÇáãæŞÚ Úáì ÇáÔÇÔÉ
            Vector3 baseScreenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0.5f, 2f, 0));
            popup.transform.position = baseScreenPos;

            // ÇáÍÕæá Úáì ãßæä TextPopup æÊÎÒíäå İí ÇáãÊÛíÑ
            activePopup = popup.GetComponent<TextPopup>();

            // ÅÑÓÇá ŞíãÉ ÇáÖÑÑ ÇáÃæáì
            activePopup.AddDamage(hitNumber);
        }
    }
}