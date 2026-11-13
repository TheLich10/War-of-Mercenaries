using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheakMongelOpen : MonoBehaviour
{
    public GameObject woodButton;
    public GameObject stobeButton;
    public GameObject powerButton;
    
    void Start()
    {
        //Debug.Log("deffult is : " + deffult.stateMongel);
        if (deffult.stateMongel == 0)
        {
            woodButton.SetActive(true);
            stobeButton.SetActive(true);
            powerButton.SetActive(true);
        }
        if (deffult.stateMongel == 1) 
        {
            woodButton.SetActive(false);
            stobeButton.SetActive(true);
            powerButton.SetActive(true);
        }
        else if (deffult.stateMongel == 2) 
            {
            woodButton.SetActive(false);
            stobeButton.SetActive(false);
            powerButton.SetActive(true);

        }
        else if (deffult.stateMongel == 3)
        {
            woodButton.SetActive(false);
            stobeButton.SetActive(false);
            powerButton.SetActive(false);
        }

    }


}
