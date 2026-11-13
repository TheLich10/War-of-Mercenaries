using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // إنشاء نسخة واحدة (Singleton) للوصول إليها من أي سكربت آخر
    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    // أضف هذه المتغيرات للتحكم في المستوى العام للصوت
    [HideInInspector] public float globalMusicVolume = 1f;
    [HideInInspector] public float globalSFXVolume = 1f;

    private void Awake()
    {
        // التأكد من عدم وجود أكثر من مدير صوت واحد في اللعبة
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // هذه الدالة تمنع تدمير الكائن عند الانتقال لمشهد جديد
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // دالة لتشغيل الموسيقى
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        musicSource.clip = s.clip;
        musicSource.volume = s.volume * globalMusicVolume;
        musicSource.pitch = s.pitch;
        musicSource.loop = s.loop;
        musicSource.Play();
    }

    // دالة لتشغيل المؤثرات الصوتية
    // دالة لتشغيل المؤثرات الصوتية
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        // التحقق مما إذا كان الصوت يحتاج لتكرار
        if (s.loop)
        {
            sfxSource.clip = s.clip;
            sfxSource.volume = s.volume * globalSFXVolume;
            sfxSource.pitch = s.pitch;
            sfxSource.loop = true; // تفعيل التكرار في الـ AudioSource
            sfxSource.Play();      // استخدام Play بدلاً من PlayOneShot
        }
        else
        {
            // الأصوات العادية التي لا تتكرر
            sfxSource.PlayOneShot(s.clip, s.volume * globalSFXVolume);
        }
    }

    // دالة إضافية لإيقاف المؤثرات الصوتية (ستحتاجها لإيقاف الصوت المتكرر لاحقاً)
    public void StopSFX()
    {
        sfxSource.Stop();
    }

    // دالة لإيقاف الموسيقى
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // 🟢 دالة لتحميل إعدادات الصوت من ملف الحفظ
    public void LoadData(float savedMusicVolume, float savedSFXVolume)
    {
        globalMusicVolume = savedMusicVolume;
        globalSFXVolume = savedSFXVolume;

        // تحديث مستوى الصوت فوراً إذا كان هناك مقطع يعمل حالياً
        musicSource.volume = globalMusicVolume;
        sfxSource.volume = globalSFXVolume;

    }
}