using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WinOrLost : MonoBehaviour
{
    // ‰’Ê’ TMP
    public GameObject panal;


    public TMP_Text ironText;
    public TMP_Text woodText;
    public TMP_Text stoneText;
    public TMP_Text powerText;
    public TMP_Text Win;
    public TMP_Text lost;
    public TMP_Text pointText;

    public Image point1;
    public Image point2;
    public Image point3;

    public Image winImg;
    public Image loseImg;






    void Start()
    {
        if (EndLoader.isWar == true)
        {
            AudioManager.Instance.PlaySFX("win_war");
            panal.SetActive(true);
            // ‰ﬁ· «·„Ê«—œ
            ironText.text = PlayerResources.iron.ToString();
            woodText.text = PlayerResources.wood.ToString();
            stoneText.text = PlayerResources.stone.ToString();
            powerText.text = PlayerResources.power.ToString();


            if (EndLoader.WinOrLost == false)
            {
                pointText.text = WarProgressBar.ThepintWin.ToString()+" BATTLE POINTS";
                WarPointsManager.RemoveWarPoints(-WarProgressBar.ThepintWin);
                pointText.color = Color.red;
                Win.gameObject.SetActive(false);
                lost.gameObject.SetActive(true);
                loseImg.gameObject.SetActive(true);


            }
            else if (EndLoader.WinOrLost == true)
            {
                Win.gameObject.SetActive(true);
                lost.gameObject.SetActive(false);
                winImg.gameObject.SetActive(true);


                if (EndLoader.point == 1)
                {
                    point1.gameObject.SetActive(true);
                    pointText.text = "+" + WarProgressBar.ThepintWin.ToString() + " BATTLE POINTS";
                    WarPointsManager.AddWarPoints(WarProgressBar.ThepintWin);


                }
                else if (EndLoader.point == 2)
                {
                    point1.gameObject.SetActive(true);
                    point2.gameObject.SetActive(true);
                    pointText.text = "+" + WarProgressBar.ThepintWin.ToString() + " BATTLE POINTS";
                    WarPointsManager.AddWarPoints(WarProgressBar.ThepintWin);

                }
                else if (EndLoader.point == 3)
                {
                    point1.gameObject.SetActive(true);
                    point2.gameObject.SetActive(true);
                    point3.gameObject.SetActive(true);
                    pointText.text = "+" + WarProgressBar.ThepintWin.ToString() + " BATTLE POINTS";
                    WarPointsManager.AddWarPoints(WarProgressBar.ThepintWin);


                }
            }


            // ﬁ„ » ’›Ì— «·„Ê«—œ Â‰« »⁄œ  ÕÊÌ·Â«
            // 2. Õ›Ÿ «·ﬁÌ„ Ê ’›Ì—Â« ›Ê—«
            int wonIron = PlayerResources.iron;
            int wonWood = PlayerResources.wood;
            int wonStone = PlayerResources.stone;
            int wonPower = PlayerResources.power;

            PlayerResources.iron = 0;
            PlayerResources.power = 0;
            PlayerResources.stone = 0;
            PlayerResources.wood = 0;
            EndLoader.isWar = false;

            // 3. «” œ⁄«¡ «· √ŒÌ—
            StartCoroutine(AddResourcesDelayed(wonIron, wonWood, wonStone, wonPower));
        }
    }
    private IEnumerator AddResourcesDelayed(int iron, int wood, int stone, int power)
    {
        yield return new WaitForSeconds(1f);
        ResourceManager.AddIron(iron);
        ResourceManager.AddWood(wood);
        ResourceManager.AddStone(stone);
        ResourceManager.AddPower(power);
    }

}
