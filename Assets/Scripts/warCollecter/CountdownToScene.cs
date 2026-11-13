using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Drawing;

public class CountdownToScene : MonoBehaviour
{
    public TMP_Text timerText;   // ضع نص الوقت هنا
    public string sceneName;     // اسم المشهد الذي سيتم تحميله
    public float startTime = 60f; // دقيقة واحدة = 60 ثانية

    private float currentTime;

    void Start()
    {
        currentTime = startTime;
        UpdateTimerText();
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < 0)
                currentTime = 0;

            UpdateTimerText();

        }
        else
        {
            if (!string.IsNullOrEmpty(sceneName)) // التأكد من أن اسم المشهد موجود
            {
                EndLoader.isWar = true;
                if (WarProgressBar.WarPoints == 1)
                {
                    EndLoader.WinOrLost = true;
                    EndLoader.point = 1;
                }
                if (WarProgressBar.WarPoints == 2)
                {
                    EndLoader.WinOrLost = true;
                    EndLoader.point = 2;
                }
                if (WarProgressBar.WarPoints == 3)
                {
                    EndLoader.WinOrLost = true;
                    EndLoader.point = 3;
                }
                if (WarProgressBar.WarPoints == 0)
                {
                    EndLoader.WinOrLost = false;
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

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = minutes.ToString() + ":" + seconds.ToString("00");
    }
}
