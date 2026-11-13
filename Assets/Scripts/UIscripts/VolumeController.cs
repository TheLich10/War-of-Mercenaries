using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    // 🟢 إضافة نسخة لتسهيل الوصول إليها من أي مكان
    public static VolumeController Instance;

    [Header("صور أشرطة الصوت (Fill Images)")]
    public Image musicBar;
    public Image sfxBar;

    [Header("مقدار التغيير في كل ضغطة")]
    [Range(0.05f, 0.2f)]
    public float changeStep = 0.1f;

    private void Awake()
    {
        Instance = this;
    }

    // 🟢 نستخدم OnEnable بدلاً من Start حتى تتحدث الواجهة في كل مرة تفتح فيها نافذة الإعدادات
    private void OnEnable()
    {
        RefreshUI();
    }

    // 🟢 دالة مخصصة لتحديث شكل الأشرطة بناءً على الأرقام الحقيقية في AudioManager
    public void RefreshUI()
    {
        if (AudioManager.Instance != null)
        {
            if (musicBar != null) musicBar.fillAmount = AudioManager.Instance.globalMusicVolume;
            if (sfxBar != null) sfxBar.fillAmount = AudioManager.Instance.globalSFXVolume;
        }
    }

    // ==========================================
    // دوال التحكم في الموسيقى (Music)
    // ==========================================
    public void IncreaseMusic()
    {
        musicBar.fillAmount += changeStep;
        ApplyMusicVolume();
    }

    public void DecreaseMusic()
    {
        musicBar.fillAmount -= changeStep;
        ApplyMusicVolume();
    }

    private void ApplyMusicVolume()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.globalMusicVolume = musicBar.fillAmount;
            AudioManager.Instance.musicSource.volume = musicBar.fillAmount;
        }
    }

    // ==========================================
    // دوال التحكم في المؤثرات (SFX)
    // ==========================================
    public void IncreaseSFX()
    {
        sfxBar.fillAmount += changeStep;
        ApplySFXVolume();
    }

    public void DecreaseSFX()
    {
        sfxBar.fillAmount -= changeStep;
        ApplySFXVolume();
    }

    private void ApplySFXVolume()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.globalSFXVolume = sfxBar.fillAmount;
            AudioManager.Instance.sfxSource.volume = sfxBar.fillAmount;
        }
    }
}