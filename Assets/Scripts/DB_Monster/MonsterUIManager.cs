using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterUIManager : MonoBehaviour
{
    [Header("‰’Ê’ «·„” ÊÌ« ")]
    public TMP_Text Wom_kong_LevelText;
    public TMP_Text Mightosour_LevelText;
    public TMP_Text BearWolf_LevelText;
    public TMP_Text DraginFly_LevelText;

    [Header("’Ê— «·ÊÕÊ‘")]
    public Image Wom_kong_Image;
    public Image Mightosour_Image;
    public Image BearWolf_Image;
    public Image DraginFly_Image;

    void Start()
    {
        UpdateAllMonstersUI();
    }
    private void Update()
    {
        UpdateAllMonstersUI();

    }
    public void UpdateAllMonstersUI()
    {
        UpdateMonsterUI("Wom-kong", Wom_kong_LevelText, Wom_kong_Image);
        UpdateMonsterUI("Mightosour", Mightosour_LevelText, Mightosour_Image);
        UpdateMonsterUI("BearWolf", BearWolf_LevelText, BearWolf_Image);
        UpdateMonsterUI("DraginFly", DraginFly_LevelText, DraginFly_Image);
    }

    void UpdateMonsterUI(string monsterName, TMP_Text levelText, Image monsterImage)
    {
        if (MonsterLevelManager.Instance == null) return;

        int level = MonsterLevelManager.Instance.GetLevel(monsterName);
        levelText.text = level.ToString();

        var monsterData = MonsterDatabase.GetMonsterByName(monsterName);
        if (monsterData != null)
        {
            var monsterLevel = monsterData.GetLevel(level);
            if (monsterLevel != null && monsterLevel.image != null)
            {
                monsterImage.sprite = monsterLevel.image;
                monsterImage.preserveAspect = true;
                monsterImage.enabled = true;
            }
            else
            {
                monsterImage.enabled = false;
            }
        }
        else
        {
            levelText.text = "Not Found";
            monsterImage.enabled = false;
        }
    }
}
