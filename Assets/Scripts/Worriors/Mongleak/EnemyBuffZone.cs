using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffZone : MonoBehaviour
{
    public static EnemyBuffZone instaine;
    private Collider2D myCollider;

    public float healthMultiplier = 1.5f;
    public float speedMultiplier = 1.2f;
    public float damageMultiplier = 1.3f;
    public float buffDuration = 5f;

    private List<SpriteRenderer> affectedSprites = new List<SpriteRenderer>();

    private void Awake()
    {
        instaine = this;
    }

    private void Start()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
            myCollider.isTrigger = false;
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

        AudioManager.Instance.PlaySFX("powerCPTL");
        if (myCollider == null)
            myCollider = GetComponent<Collider2D>();

        if (myCollider != null)
            myCollider.isTrigger = true;
        else
            Debug.LogWarning($"EnableTrigger failed on {name} — no Collider2D found.");

        // 🔵 الكود الجديد: البحث عن الطفل "img" وتغيير لونه إلى الأزرق
        Transform imgChild = transform.Find("img");
        if (imgChild != null)
        {
            SpriteRenderer imgSprite = imgChild.GetComponent<SpriteRenderer>();
            if (imgSprite != null)
            {
                // يمكنك تغيير درجة اللون للدرجة التي استخدمتها في الأسفل new Color(0.6f, 0.8f, 1f, 1f) إذا أردت
                imgSprite.color = new Color(0.6f, 0.8f, 1f, 1f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") ||
            other.CompareTag("EnemyFly") ||
            other.CompareTag("monsterEnemy") ||
            other.CompareTag("monsterFly"))
        {
            // 🔵 تغيير اللون إلى الأزرق
            SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.6f, 0.8f, 1f, 1f); ;

            }

            // Enemy
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.HightWithSpeel(healthMultiplier, speedMultiplier, damageMultiplier, buffDuration);
                enemy.StartReturnColor(buffDuration);
            }


            // sapperEnemy
            sapperEnemy sapper = other.GetComponent<sapperEnemy>();
            if (sapper != null)
            {
                sapper.HightWithSpeel(healthMultiplier, speedMultiplier, damageMultiplier, buffDuration);
                sapper.StartReturnColor(buffDuration);

            }

            // y3soobEnemy
            y3soobEnemy y3soob = other.GetComponent<y3soobEnemy>();
            if (y3soob != null)
            {
                y3soob.HightWithSpeel(healthMultiplier, speedMultiplier, damageMultiplier, buffDuration);
                y3soob.StartReturnColor(buffDuration);

            }

            // flayingEnemy
            flayingEnemy flaying = other.GetComponent<flayingEnemy>();
            if (flaying != null)
            {
                flaying.HightWithSpeel(healthMultiplier, speedMultiplier, damageMultiplier, buffDuration);
                flaying.StartReturnColor(buffDuration);

            }

            // HealerWarrior
            HealerWarrior healer = other.GetComponent<HealerWarrior>();
            if (healer != null)
            {
                healer.HightWithSpeel(healthMultiplier, speedMultiplier, damageMultiplier, buffDuration);
                healer.StartReturnColor(buffDuration);

            }

            // EnemyRng
            EnemyRNG_Advanced enemyRng = other.GetComponent<EnemyRNG_Advanced>();
            if (enemyRng != null)
            {
                enemyRng.HightWithSpeel(healthMultiplier, speedMultiplier, damageMultiplier, buffDuration);
                enemyRng.StartReturnColor(buffDuration);

            }
        }
    }

}
