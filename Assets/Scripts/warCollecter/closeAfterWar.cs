using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class closeAfterWar : MonoBehaviour
{
    public Button BTNClose;
    public Button BTNOkay;

    public GameObject makepanal;

    public TMP_Text ironText;
    public TMP_Text woodText;
    public TMP_Text stoneText;
    public TMP_Text powerText;


    void Start()
    {
        BTNClose.onClick.AddListener(closeMkaeWrrior);
        BTNOkay.onClick.AddListener(closeMkaeWrrior);


    }

    void closeMkaeWrrior()
    {
        PlayerResources.iron = 0;
        PlayerResources.wood = 0;
        PlayerResources.stone = 0;
        PlayerResources.power = 0;

        EndLoader.point = 0;
        ironText.text = "0";
        woodText.text = "0";
        stoneText.text = "0";
        powerText.text = "0";
        EndLoader.WinOrLost = false;

        makepanal.SetActive(false);
    }
}
