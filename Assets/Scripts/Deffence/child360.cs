using UnityEngine;

public class child360 : MonoBehaviour
{
    private DefensiveBuilding parentScript;
    private Animator anim;

    [Header("ÅÚÏÇÏÇÊ ÇáÊÇÛÇÊ")]
    // ÇáÊÇÛÇÊ ÇáÊí ÓíÈÍË ÚäåÇ (ãØÇÈŞÉ ááÓßÑÈÊ ÇáÃÈ)
    private string[] targetTags = { "Enemy","monsterEnemy" };

    [Header("ÅÚÏÇÏÇÊ ÇáãäÙæÑ ÇáÃíÒæãÊÑí")]
    [Tooltip("äÓÈÉ ÖÛØ ãÍæÑ Y İí áÚÈÊß. ÇáŞíãÉ ÇáÇİÊÑÇÖíÉ ááÃíÒæãÊÑí åí ÚÇÏÉ 2.")]
    public float isometricYScale = 2f;

    private Transform currentTarget; // íÍİÙ ÇáÚÏæ ÇáĞí íÑßÒ Úáíå ÇáÃäíãíÔä ÍÇáíÇğ

    void Start()
    {
        // ÌáÈ ÓßÑÈÊ ÇáÃÈ æ Animator ÇáÎÇÕ ÈÇáÇÈä
        parentScript = GetComponentInParent<DefensiveBuilding>();
        anim = GetComponent<Animator>();

        if (parentScript == null)
        {
            Debug.LogError("áã íÊã ÇáÚËæÑ Úáì ÓßÑÈÊ DefensiveBuilding İí ÇáÃÈ!");
        }
    }

    void Update()
    {
        if (parentScript == null || anim == null) return;

        // 1. ÇáÊÍŞŞ ããÇ ÅĞÇ ßÇä ÇáåÏİ ÇáÍÇáí ãæÌæÏÇğ¡ ÍíÇğ¡ æãÇ ÒÇá Öãä ÇáäØÇŞ
        if (currentTarget != null && currentTarget.gameObject.activeInHierarchy)
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
            if (distanceToTarget > parentScript.attackRange)
            {
                currentTarget = null; // ÇáåÏİ ÎÑÌ ãä ÇáäØÇŞ¡ Şã ÈÊİÑíÛå
            }
        }
        else
        {
            currentTarget = null; // ÇáåÏİ ãÇÊ Ãæ ÇÎÊİì
        }

        // 2. ÅĞÇ áã íßä áÏíäÇ åÏİ ÍÇáí¡ äÈÍË Úä ÃŞÑÈ ÚÏæ æäŞİá Úáíå
        if (currentTarget == null)
        {
            currentTarget = GetNearestEnemyInRange();
        }

        // 3. ÊÍÏíË ÇáÃäíãíÔä ÈäÇÁğ Úáì ÍÇáÉ ÇáåÏİ
        if (currentTarget != null)
        {
            // ÅĞÇ ßÇä åäÇß åÏİ¡ Şã ÈÊÍÏíË ÇÊÌÇå ÇáÃäíãíÔä äÍæå
            SetDirectionalAnimation(currentTarget.position);
        }
        else
        {
            // ÅĞÇ áã íßä åäÇß åÏİ İí ÇáäØÇŞ¡ Şã ÈÊÚØíá ßá ÇáÇÊÌÇåÇÊ
            ResetAllAnimations();
        }
    }

    // ÏÇáÉ ááÈÍË Úä ÃŞÑÈ ÚÏæ Öãä äØÇŞ ÇáÃÈ
    Transform GetNearestEnemyInRange()
    {
        Transform nearestEnemy = null;
        float shortestDistance = parentScript.attackRange;

        foreach (string tag in targetTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemy in enemies)
            {
                // íãßääÇ ÇÓÊÎÏÇã Vector2 åäÇ áÃääÇ äåÊã ÈÇáãÓÇİÉ ËäÇÆíÉ ÇáÃÈÚÇÏ
                float distance = Vector2.Distance(transform.position, enemy.transform.position);

                if (distance <= shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }
        }

        return nearestEnemy;
    }

    // ÏÇáÉ áÊÍÏíÏ ÇáÇÊÌÇå æÊÔÛíá ÇáÃäíãíÔä ÇáãäÇÓÈ (ãÚÏáÉ ááÃíÒæãÊÑí)
    void SetDirectionalAnimation(Vector3 targetPos)
    {
        // äÍÕá Úáì ÇáÇÊÌÇå Èíä ÇáãÈäì æÇáåÏİ
        Vector2 dir = (targetPos - transform.position);

        // --- ÇáÊÚÏíá ÇáÓÍÑí ááãäÙæÑ ÇáÃíÒæãÊÑí ---
        // äÖÑÈ ãÍæÑ Y İí ŞíãÉ ÇáÜ Scale ÇáãÍÏÏÉ áÊÕÍíÍ ÇáÔßá ÇáÈíÖæí æÌÚáå ÏÇÆÑíÇğ İí ÇáÍÓÇÈÇÊ
        dir.y *= isometricYScale;

        // ÈÚÏ ÇáÊÚÏíá¡ äŞæã ÈÌÚá Øæá ÇáãÊÌå 1 (Normalize)
        dir.Normalize();

        // ÍÓÇÈ ÇáÒÇæíÉ (ãä -180 Åáì 180 ÏÑÌÉ)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // ÊÕİíÑ ÌãíÚ ÇáŞíã ÃæáÇğ
        ResetAllAnimations();

        // ÊÍÏíÏ ÇáÇÊÌÇåÇÊ ÇáËãÇäíÉ ÈäÇÁğ Úáì ÇáÒÇæíÉ ÇáãÕÍÍÉ
        if (angle > -22.5f && angle <= 22.5f)
            anim.SetBool("right", true);
        else if (angle > 22.5f && angle <= 67.5f)
            anim.SetBool("upRight", true);
        else if (angle > 67.5f && angle <= 112.5f)
            anim.SetBool("up", true);
        else if (angle > 112.5f && angle <= 157.5f)
            anim.SetBool("UpLeft", true);
        else if (angle > 157.5f || angle <= -157.5f)
            anim.SetBool("left", true);
        else if (angle > -157.5f && angle <= -112.5f)
            anim.SetBool("downLeft", true);
        else if (angle > -112.5f && angle <= -67.5f)
            anim.SetBool("down", true);
        else if (angle > -67.5f && angle <= -22.5f)
            anim.SetBool("DownRight", true);
    }

    // ÏÇáÉ áÅíŞÇİ ÊİÚíá ÌãíÚ ãÊÛíÑÇÊ ÇáÇÊÌÇåÇÊ
    void ResetAllAnimations()
    {
        anim.SetBool("up", false);
        anim.SetBool("down", false);
        anim.SetBool("right", false);
        anim.SetBool("left", false);
        anim.SetBool("upRight", false);
        anim.SetBool("UpLeft", false);
        anim.SetBool("DownRight", false);
        anim.SetBool("downLeft", false);
    }
}