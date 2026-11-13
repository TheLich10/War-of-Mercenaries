using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLoader : MonoBehaviour
{
    public string sceneName; // اسم المشهد الذي سيتم تحميله
    public static bool isWar = false;
    public static bool WinOrLost = false;
    public static int point = 0;

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName)) // التأكد من أن اسم المشهد موجود
        {
            isWar = true;
                if(WarProgressBar.WarPoints == 1)
                {
                    WinOrLost = true;
                    point = 1;
                }
                if (WarProgressBar.WarPoints == 2)
                {
                    WinOrLost = true;
                    point = 2;
                }
                if (WarProgressBar.WarPoints == 3)
                {
                    WinOrLost = true;
                    point = 3;
                }
                if ( WarProgressBar.WarPoints == 0){
                WinOrLost = false;
                }
            spyButton.spyButtonState = false;
            SaveManager.Instance.SaveResourcesAndBarracksOnly();
            GlobalSceneManager.LoadSceneViaLoading(sceneName);

        }
        else
        {
            Debug.LogError("⚠️ لم يتم تحديد اسم المشهد! تأكد من إدخال الاسم في الـ Inspector.");
        }
    }
}
