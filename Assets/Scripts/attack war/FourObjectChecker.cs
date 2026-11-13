using UnityEngine;

public class FourObjectChecker : MonoBehaviour
{
    [Header("Objects")]
    public GameObject bear;
    public GameObject elefent;
    public GameObject y3ssob;
    public GameObject womkong;

    // 🧠 حفظ القيم الأصلية
    bool hero1Old;
    bool hero2Old;
    bool archer1Old;
    bool archer2Old;

    void OnEnable()
    {
        // ✅ نحفظ الحالة الأصلية عند دخول السين
        hero1Old = HeroUnit.isActiveHero;
        hero2Old = HeroUnit2.isActiveHero;
        archer1Old = HeroArcher.isActiveHero;
        archer2Old = HeroArcher2.isActiveHero;
    }

    void Update()
    {
        if (bear != null && bear.activeInHierarchy)
            HeroUnit.isActiveHero = true;

        if (elefent != null && elefent.activeInHierarchy)
            HeroUnit2.isActiveHero = true;

        if (y3ssob != null && y3ssob.activeInHierarchy)
            HeroArcher2.isActiveHero = true;

        if (womkong != null && womkong.activeInHierarchy)
            HeroArcher.isActiveHero = true;
    }

    void OnDisable()
    {
        // 🔄 نرجّع القيم الأصلية عند الخروج من السين
        HeroUnit.isActiveHero = hero1Old;
        HeroUnit2.isActiveHero = hero2Old;
        HeroArcher.isActiveHero = archer1Old;
        HeroArcher2.isActiveHero = archer2Old;
    }
}
