using UnityEngine;

public class animtionSpell : MonoBehaviour
{
    public GameObject woodAnimtion;

    public GameObject StoneAnimtion1;
    public GameObject StoneAnimtion2;
    public GameObject StoneAnimtion3;
    public GameObject StoneAnimtion4;

    void Start()
    {
        //  „ ≈›—«€ Â–Â «·œ«·… ·√‰‰« ‰—Ìœ «· Õﬁﬁ «·„” „— Ê·Ì” ·„—… Ê«Õœ… ›ﬁÿ
    }

    // Update is called once per frame
    void Update()
    {
        // «· Õﬁﬁ „” „— ›Ì ﬂ· ≈ÿ«—° Ê·« Ì „ «· ›⁄Ì· ≈·« ≈–« ﬂ«‰ «·ﬂ«∆‰ „ÊÃÊœ« Ê€Ì— „›⁄· „”»ﬁ«

        if (GridAttckC.isWoodAnime && woodAnimtion != null && !woodAnimtion.activeSelf)
            woodAnimtion.SetActive(true);

        if (GridAttckC.isStoneAnime1 && StoneAnimtion1 != null && !StoneAnimtion1.activeSelf)
            StoneAnimtion1.SetActive(true);

        if (GridAttckC.isStoneAnime2 && StoneAnimtion2 != null && !StoneAnimtion2.activeSelf)
            StoneAnimtion2.SetActive(true);

        if (GridAttckC.isStoneAnime3 && StoneAnimtion3 != null && !StoneAnimtion3.activeSelf)
            StoneAnimtion3.SetActive(true);

        if (GridAttckC.isStoneAnime4 && StoneAnimtion4 != null && !StoneAnimtion4.activeSelf)
            StoneAnimtion4.SetActive(true);
    }
}