using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class closing : MonoBehaviour
{
    public Button closepanalMakeWorrior;
    public GameObject makepanal;


    void Start()
    {
        closepanalMakeWorrior.onClick.AddListener(closeMkaeWrrior);

    }

    void closeMkaeWrrior()
    {
        makepanal.SetActive(false); 
    }

}
