using UnityEngine;
using System.Collections.Generic;

public class Temp : MonoBehaviour
{
    public string nameThis;
    public static Temp instaine;
    public float damageAmount = 20f;
    private Collider2D myCollider;

    private List<building> currentTargets = new List<building>();
    private List<DefensiveBuilding> currentDefensiveTargets = new List<DefensiveBuilding>();

    // القائمة الخاصة بالمباني الثالثة (الجواسيس أو المراقبة)
    private List<DefensiveWatchPost> currentSpyTargets = new List<DefensiveWatchPost>();

    // قائمة التاغات للمباني العادية (building)
    private readonly HashSet<string> validTags = new HashSet<string>
    {
      "wall","CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker",
      "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr",
      "training", "Cave", "mongel"
    };

    // قائمة التاغات الخاصة بالمباني الدفاعية (DefensiveBuilding)
    private readonly HashSet<string> defensiveTags = new HashSet<string>
    {
        "boomOP", "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower"
    };

    // تاغ المبنى الثالث
    private const string spyTag = "spy";

    private void Awake()
    {
        instaine = this;
    }

    private void OnDestroy()
    {
        // التحقق من وجود AudioManager لتجنب أي أخطاء عند إغلاق اللعبة أو الانتقال لمشهد آخر
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopSFX();
        }
    }

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();

        if (myCollider != null)
        {
            myCollider.isTrigger = false;
        }
        else
        {
            Debug.LogWarning($"Temp ({name}) has no Collider2D attached!");
        }
    }

    public void EnableTrigger()
    {
        if (!SimpleWarriorPlacement.isFirstUnitPlaced)
        {
            AudioManager.Instance.PlayMusic("war");
            AudioManager.Instance.PlaySFX("firsOne");

            // تغيير القيمة إلى true حتى يعلم السكربت الأول أنه تم وضع جندي بالفعل
            SimpleWarriorPlacement.isFirstUnitPlaced = true;
        }

        // التحقق من المتغير nameThis لتشغيل الصوت المناسب
        if (nameThis == "wood")
        {
            AudioManager.Instance.PlaySFX("woodCPTL");
        }
        else if (nameThis == "stone")
        {
            AudioManager.Instance.PlaySFX("stoneCPTL");
        }

        if (myCollider == null)
            myCollider = GetComponent<Collider2D>();

        if (myCollider != null)
        {
            myCollider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning($"EnableTrigger failed on Temp ({name}) — no Collider2D found.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // مباني عادية
        if (validTags.Contains(other.tag))
        {
            building target = other.GetComponent<building>();
            if (target != null && !currentTargets.Contains(target))
            {
                currentTargets.Add(target);

                if (!IsInvoking(nameof(DealDamageRepeatedly)))
                {
                    InvokeRepeating(nameof(DealDamageRepeatedly), 0f, 0.5f);
                }
            }
        }
        // مباني دفاعية
        else if (defensiveTags.Contains(other.tag))
        {
            DefensiveBuilding defTarget = other.GetComponent<DefensiveBuilding>();
            if (defTarget != null && !currentDefensiveTargets.Contains(defTarget))
            {
                currentDefensiveTargets.Add(defTarget);

                if (!IsInvoking(nameof(DealDamageRepeatedly)))
                {
                    InvokeRepeating(nameof(DealDamageRepeatedly), 0f, 0.5f);
                }
            }
        }
        // مباني المراقبة (spy)
        else if (other.CompareTag(spyTag))
        {
            DefensiveWatchPost spyTarget = other.GetComponent<DefensiveWatchPost>();
            if (spyTarget != null && !currentSpyTargets.Contains(spyTarget))
            {
                currentSpyTargets.Add(spyTarget);
                if (!IsInvoking(nameof(DealDamageRepeatedly)))
                {
                    InvokeRepeating(nameof(DealDamageRepeatedly), 0f, 0.5f);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (validTags.Contains(other.tag))
        {
            building target = other.GetComponent<building>();
            if (target != null && currentTargets.Contains(target))
            {
                currentTargets.Remove(target);

                if (currentTargets.Count == 0 && currentDefensiveTargets.Count == 0 && currentSpyTargets.Count == 0)
                {
                    CancelInvoke(nameof(DealDamageRepeatedly));
                }
            }
        }
        else if (defensiveTags.Contains(other.tag))
        {
            DefensiveBuilding defTarget = other.GetComponent<DefensiveBuilding>();
            if (defTarget != null && currentDefensiveTargets.Contains(defTarget))
            {
                currentDefensiveTargets.Remove(defTarget);

                if (currentTargets.Count == 0 && currentDefensiveTargets.Count == 0 && currentSpyTargets.Count == 0)
                {
                    CancelInvoke(nameof(DealDamageRepeatedly));
                }
            }
        }
        else if (other.CompareTag("spy"))
        {
            DefensiveWatchPost spyTarget = other.GetComponent<DefensiveWatchPost>();
            if (spyTarget != null && currentSpyTargets.Contains(spyTarget))
            {
                currentSpyTargets.Remove(spyTarget);

                if (currentTargets.Count == 0 && currentDefensiveTargets.Count == 0 && currentSpyTargets.Count == 0)
                {
                    CancelInvoke(nameof(DealDamageRepeatedly));
                }
            }
        }
    }

    private void DealDamageRepeatedly()
    {
        // التعامل مع المباني العادية
        foreach (var target in new List<building>(currentTargets))
        {
            if (target != null)
            {
                target.TakeDamage(damageAmount);
            }
        }

        // التعامل مع المباني الدفاعية
        foreach (var defTarget in new List<DefensiveBuilding>(currentDefensiveTargets))
        {
            if (defTarget != null)
            {
                defTarget.TakeDamage(damageAmount);
            }
        }

        foreach (var spyTarget in new List<DefensiveWatchPost>(currentSpyTargets))
        {
            if (spyTarget != null)
            {
                spyTarget.TakeDamage(damageAmount);
            }
        }
    }
}
