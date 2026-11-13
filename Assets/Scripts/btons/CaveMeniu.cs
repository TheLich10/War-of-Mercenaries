using UnityEngine;
using UnityEngine.UI;

public class CaveMeniu : MonoBehaviour
{
    public GameObject cavePanal;
    public Button caveButton;
    public Button openChangeButton; // ✅ الزر الجديد

    public Button besastBTN;
    public Button tringBTN;
    public Button chnngeBTN;

    public GameObject BeastPanal;
    public GameObject tringPanal;
    public GameObject changePanal;

    void Start()
    {
        cavePanal.SetActive(false); // إخفاء القائمة عند بدء اللعبة

        caveButton.onClick.AddListener(ToggleBuildMenu);
        openChangeButton.onClick.AddListener(ToggleChangeMenu); // ✅ مستمع الزر الجديد

        besastBTN.onClick.AddListener(() => OpenCategory(BeastPanal));
        tringBTN.onClick.AddListener(() => OpenCategory(tringPanal));
        chnngeBTN.onClick.AddListener(() => OpenCategory(changePanal));

        CloseAllCategories(); // إخفاء جميع الفئات عند البداية
    }

    void ToggleBuildMenu()
    {
        bool isActive = !cavePanal.activeSelf;
        cavePanal.SetActive(isActive);

        if (isActive)
        {
            OpenCategory(BeastPanal); // فتح قائمة Beast عند فتح القائمة
        }
        else
        {
            CloseAllCategories(); // إغلاق القوائم عند إغلاق القائمة
        }
    }

    void ToggleChangeMenu()
    {
        bool isActive = !cavePanal.activeSelf;
        cavePanal.SetActive(isActive);

        if (isActive)
        {
            OpenCategory(changePanal); // ✅ فتح قائمة التغيير عند الضغط على الزر الثاني
        }
        else
        {
            CloseAllCategories();
        }
    }

    void OpenCategory(GameObject categoryPanel)
    {
        CloseAllCategories();
        categoryPanel.SetActive(true);
    }

    void CloseAllCategories()
    {
        BeastPanal.SetActive(false);
        tringPanal.SetActive(false);
        changePanal.SetActive(false);
    }
}
