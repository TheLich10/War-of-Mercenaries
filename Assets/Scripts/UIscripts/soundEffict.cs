using UnityEngine;

public class soundEffict : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlaySFX("make_Archive");
    }

}
