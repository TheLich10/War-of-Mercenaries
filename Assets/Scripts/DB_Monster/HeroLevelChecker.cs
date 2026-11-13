using UnityEngine;
using UnityEngine.UI;

public class HeroLevelChecker : MonoBehaviour
{
    [Header("Level GameObjects")]
    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public GameObject level4;
    public GameObject level5;

    [Header("Target UI Images to Change")]
    public Image monsterImage1;
    public Image monsterImage2;

    [Header("Sprites for Each Monster (2 per monster)")]
    public Sprite womKongSprite1;
    public Sprite womKongSprite2;

    public Sprite mightosourSprite1;
    public Sprite mightosourSprite2;

    public Sprite bearWolfSprite1;
    public Sprite bearWolfSprite2;

    public Sprite draginFlySprite1;
    public Sprite draginFlySprite2;

    void Update()
    {
        // إيقاف جميع المستويات
        DeactivateAllLevels();

        // التحقق من البطل النشط وتغيير الصور والمستوى
        if (HeroArcher2.isActiveHero)
        {
            CheckLevelAndActivate("DraginFly", draginFlySprite1, draginFlySprite2);
        }
        else if (HeroArcher.isActiveHero)
        {
            CheckLevelAndActivate("Wom-kong", womKongSprite1, womKongSprite2);
        }
        else if (HeroUnit.isActiveHero)
        {
            CheckLevelAndActivate("BearWolf", bearWolfSprite1, bearWolfSprite2);
        }
        else if (HeroUnit2.isActiveHero)
        {
            CheckLevelAndActivate("Mightosour", mightosourSprite1, mightosourSprite2);
        }
    }

    void CheckLevelAndActivate(string monsterName, Sprite sprite1, Sprite sprite2)
    {
        int level = MonsterLevelManager.Instance.GetLevel(monsterName);

        // تغيير الصورتين المخصصتين لهذا الوحش
        if (monsterImage1 != null) monsterImage1.sprite = sprite1;
        if (monsterImage2 != null) monsterImage2.sprite = sprite2;

        // تفعيل المستوى المناسب
        switch (level)
        {
            case 1:
                level1.SetActive(true);
                break;
            case 2:
                level2.SetActive(true);
                break;
            case 3:
                level3.SetActive(true);
                break;
            case 4:
                level4.SetActive(true);
                break;
            case 5:
                level5.SetActive(true);
                break;
            case 6:
                level5.SetActive(false);
                level4.SetActive(false);
                level3.SetActive(false);
                level2.SetActive(false);
                level1.SetActive(false);
                break;
            default:
                Debug.LogWarning("⚠️ مستوى غير متوقع: " + level + " للوحش: " + monsterName);
                break;
        }
    }

    void DeactivateAllLevels()
    {
        level1.SetActive(false);
        level2.SetActive(false);
        level3.SetActive(false);
        level4.SetActive(false);
        level5.SetActive(false);
    }
}
