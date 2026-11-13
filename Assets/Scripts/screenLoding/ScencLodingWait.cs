using UnityEngine;
using UnityEngine.SceneManagement;

public class ScencLodingWait : MonoBehaviour
{
    public string sceneName; // اسم المشهد الذي سيتم تحميله

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // ✅ 2. استدعاء الدالة وتمرير اسم المشهد الجديد لها للذهاب لشاشة التحميل
            GlobalSceneManager.LoadSceneViaLoading(sceneName);
        }
        else
        {
            Debug.LogError("⚠️ لم يتم تحديد اسم المشهد! تأكد من إدخال الاسم في الـ Inspector.");
        }
    }
}