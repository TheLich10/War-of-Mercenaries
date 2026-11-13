using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName; // اسم المشهد الذي سيتم تحميله

    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // ✅ التحقق مما إذا كان المشهد الحالي هو "SampleScene" لعملية الحفظ
            if (SceneManager.GetActiveScene().name == "SampleScene")
            {
                // ✅ 1. حفظ اللعبة بكل بياناتها قبل مغادرة المشهد
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.SaveGame();
                }
                else
                {
                    //Debug.LogWarning("⚠️ سكربت SaveManager غير موجود في هذا المشهد! لن يتم الحفظ.");
                }
            }

            // ✅ 2. الانتقال إلى المشهد الجديد
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            //Debug.LogError("⚠️ لم يتم تحديد اسم المشهد! تأكد من إدخال الاسم في الـ Inspector.");
        }
    }
}