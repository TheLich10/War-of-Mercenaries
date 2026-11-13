using UnityEngine;

public class openOrNot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject OpenObject;
    void Start()
    {
        int randomNum = UnityEngine.Random.Range(0, 2);

        if (randomNum == 0)
        {
            OpenObject.SetActive(true);
        }
        else
        {
            OpenObject.SetActive(false);
        }
    }


}
