using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class MonsterAnimationBlendTree : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector3 lastPosition;

    [Header("Settings")]
    [Tooltip("ßáãÇ ÒÇÏ åĞÇ ÇáÑŞã¡ ßáãÇ ßÇä ÇáÇäÊŞÇá Èíä ÇáÇÊÌÇåÇÊ ÃäÚã æÃŞá ÊĞÈĞÈÇğ")]
    public float BrokerSmoothing = 10f;

    // --- äÙÇã ÊÛííÑ ÇáãÓÊæíÇÊ (New) ---
    [Header("Levels Animations")]
    [Tooltip("ÖÚ åäÇ ÇáÜ Override Controllers ÇáÎÇÕÉ ÈÇáãÓÊæíÇÊ ÇáÜ 6 ÈÇáÊÑÊíÈ")]
    public AnimatorOverrideController[] levelOverrides;

    private float smoothX = 0f;
    private float smoothY = 0f;

    public int leval = 1;

    // --- ãÊÛíÑÇÊ ÇáÊÍÏíË ÇáÊáŞÇÆí (New) ---
    private string cachedMonsterName = ""; // áÊÎÒíä ÇÓã ÇáæÍÔ ÇáãÓÊåÏİ
    private int currentAppliedLevel = -1;  // áãÚÑİÉ ÇáãÓÊæì ÇáãØÈŞ ÍÇáíÇğ æÊÌäÈ ÇáÊÍÏíË ÇáãßÑÑ

    // +++ ÇáãÊÛíÑÇÊ ÇáÌÏíÏÉ ÇáÊí ÓÊÖíİåÇ +++
    private int manualScriptType = 0; // áãÚÑİÉ Ãí ÓßÑÈÊ åÌæã äÓÊÎÏã (0 = áÇ íæÌÏ¡ 1 Åáì 4 ááÓßÑÈÊÇÊ ÇáÌÏíÏÉ)
    private Component manualScriptRef;

    [HideInInspector] public bool isAttacking = false; // áãÚÑİÉ åá ÇáæÍÔ íåÇÌã ÇáÂä Ãã áÇ
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;

        // 1. ÊÍÏíÏ ÇÓã ÇáæÍÔ ãÑÉ æÇÍÏÉ İí ÇáÈÏÇíÉ áÊÌäÈ ÇÓÊåáÇß ÇáÃÏÇÁ
        DetectMonsterName();

        // 2. ÊØÈíŞ ÇáãÓÊæì áÃæá ãÑÉ
        CheckAndUpdateLevel();
    }

    void Update()
    {
        // 1. ÍÓÇÈ ÇáÍÑßÉ ÇáİÚáíÉ
        Vector3 movement = transform.position - lastPosition;
        lastPosition = transform.position;

        float targetX = 0f;
        float targetY = 0f;



        if (Mathf.Abs(movement.x) > 0.005f) targetX = movement.x;
        if (Mathf.Abs(movement.y) > 0.005f) targetY = movement.y;

        if (targetX != 0f || targetY != 0f)
        {
            Vector2 direction = new Vector2(targetX, targetY).normalized;

            // 2. ÇáÊÍßã İí ÇáÜ Flip X
            if (direction.x > 0.01f)
            {
                spriteRenderer.flipX = false; // íãíä
            }
            else if (direction.x < -0.01f)
            {
                spriteRenderer.flipX = true;  // íÓÇÑ
            }

            // 3. ÇáÊäÚíã ÇáÊÏÑíÌí
            smoothX = Mathf.Lerp(smoothX, Mathf.Abs(direction.x), Time.deltaTime * BrokerSmoothing);
            smoothY = Mathf.Lerp(smoothY, direction.y, Time.deltaTime * BrokerSmoothing);
        }
        else
        {
            // ÅĞÇ ÊæŞİ ÊãÇãÇğ
            smoothX = Mathf.Lerp(smoothX, 0f, Time.deltaTime * BrokerSmoothing);
            smoothY = Mathf.Lerp(smoothY, 0f, Time.deltaTime * BrokerSmoothing);
        }

        // 4. ÅÑÓÇá ÇáŞíã ÇáãäÚãÉ æÇáãÓÊŞÑÉ Åáì ÇáÜ Animator
        anim.SetFloat("X", smoothX);
        anim.SetFloat("Y", smoothY);

        if (targetX == 0f && targetY == 0f && !isAttacking)
        {
            anim.SetBool("idl", true);
        }
        else
        {
            // ÅíŞÇİ ÇáÜ Idle ÈÔßá ÕÑíÍ æİæÑí ÅĞÇ ßÇä íÊÍÑß Ãæ íåÇÌã
            anim.SetBool("idl", false);
        }
        // 5. ÇáÊÍŞŞ ÇáãÓÊãÑ ãä ÇáÊÑŞíÉ (Îİíİ ÌÏÇğ Úáì ÇáÃÏÇÁ áÃäå íÊÍŞŞ ãä ÑŞã İŞØ)
        CheckAndUpdateLevel();
    }

    /// <summary>
    /// ÇÓÊßÔÇİ ÇÓã ÇáæÍÔ ãä ÇáÓßÑíÈÊÇÊ ÇáãÑİŞÉ æÊÎÒíäå ãÑÉ æÇÍÏÉ İŞØ
    /// </summary>
    private void DetectMonsterName()
    {
        // 1. ÇáÊÍŞŞ ãä ÓßÑíÈÊÇÊ ÇáÃÚÏÇÁ
        EnemyRNG_Advanced enemyRNG = GetComponent<EnemyRNG_Advanced>();
        Enemy enemy = GetComponent<Enemy>();
        y3soobEnemy y3soob = GetComponent<y3soobEnemy>();

        string detectedWarriorName = "";

        if (enemyRNG != null) detectedWarriorName = enemyRNG.warriorName;
        else if (enemy != null) detectedWarriorName = enemy.warriorName;
        else if (y3soob != null) detectedWarriorName = y3soob.warriorName;

        if (!string.IsNullOrEmpty(detectedWarriorName))
        {
            string[] monsterNames = { "Wom-kong", "DraginFly", "BearWolf", "Mightosour" };

            if (System.Array.Exists(monsterNames, name => name == detectedWarriorName))
            {
                cachedMonsterName = detectedWarriorName;
                return;
            }
        }

        // 2. ÇáÊÍŞŞ ãä ÇáÓßÑÈÊÇÊ ÇáÌÏíÏÉ (ÇáÊí ÊÍÊæí Úáì manualLevel)
        heroATK h1 = GetComponent<heroATK>();
        hero2ATK h2 = GetComponent<hero2ATK>();
        heroArcherATK h3 = GetComponent<heroArcherATK>();
        heroArcher2ATK h4 = GetComponent<heroArcher2ATK>();

        if (h1 != null) { cachedMonsterName = h1.monsterName; manualScriptType = 1; manualScriptRef = h1; return; }
        else if (h2 != null) { cachedMonsterName = h2.monsterName; manualScriptType = 2; manualScriptRef = h2; return; }
        else if (h3 != null) { cachedMonsterName = h3.monsterName; manualScriptType = 3; manualScriptRef = h3; return; }
        else if (h4 != null) { cachedMonsterName = h4.monsterName; manualScriptType = 4; manualScriptRef = h4; return; }

        // 2. ÇáÊÍŞŞ ãä ÓßÑíÈÊÇÊ ÇáÃÈØÇá æÊÚííä ÇáÇÓã ÇáãäÇÓÈ ááÈÍË
        if (GetComponent<HeroUnit>() != null) cachedMonsterName = "BearWolf";
        else if (GetComponent<HeroUnit2>() != null) cachedMonsterName = "Mightosour";
        else if (GetComponent<HeroArcher>() != null) cachedMonsterName = "Wom-kong";
        else if (GetComponent<HeroArcher2>() != null) cachedMonsterName = "DraginFly";

    }

    /// <summary>
    /// íÊÍŞŞ ããÇ ÅĞÇ ßÇä åäÇß ÊÛííÑ İí ÇáãÓÊæì¡ æíŞæã ÈÊÍÏíË ÇáÃäíãíÔä İæÑÇğ
    /// </summary>
    private void CheckAndUpdateLevel()
    {
        // ÅĞÇ áã íßä åäÇß ÇÓã ãÎÒä (áÇ íæÌÏ ÓßÑíÈÊ ãÚÑæİ)¡ äÚÊãÏ Úáì ÇáÑŞã ÇáÇİÊÑÇÖí
        if (string.IsNullOrEmpty(cachedMonsterName))
        {
            if (currentAppliedLevel != leval)
            {
                currentAppliedLevel = leval;
                SetMonsterLevel(leval);
            }
            return;
        }

        // ÌáÈ ÇáãÓÊæì ÇáÍÇáí ãä ãÏíÑ ÇáãÓÊæíÇÊ
        // ÌáÈ ÇáãÓÊæì ÇáÍÇáí ÈäÇÁğ Úáì äæÚ ÇáÓßÑÈÊ ÇáãÑİŞ
        int latestLevel = 1;

        if (manualScriptType == 1) latestLevel = ((heroATK)manualScriptRef).manualLevel;
        else if (manualScriptType == 2) latestLevel = ((hero2ATK)manualScriptRef).manualLevel;
        else if (manualScriptType == 3) latestLevel = ((heroArcherATK)manualScriptRef).manualLevel;
        else if (manualScriptType == 4) latestLevel = ((heroArcher2ATK)manualScriptRef).manualLevel;
        else latestLevel = MonsterLevelManager.Instance.GetLevel(cachedMonsterName); // ÇáØÑíŞÉ ÇáÇİÊÑÇÖíÉ
        // ÅĞÇ ßÇä ÇáãÓÊæì ÇáÌÏíÏ íÎÊáİ Úä ÇáãÓÊæì ÇáãØÈŞ¡ Şã ÈÊÛííÑå İæÑÇğ
        if (latestLevel != currentAppliedLevel)
        {
            currentAppliedLevel = latestLevel;
            leval = latestLevel; // ÊÍÏíË ÇáÑŞã áíÙåÑ ÕÍíÍÇğ İí ÇáÜ Inspector
            SetMonsterLevel(latestLevel);
        }
    }

    /// <summary>
    /// ÏÇáÉ áÊÛííÑ ãÓÊæì æÃäíãíÔä ÇáæÍÔ
    /// </summary>
    public void SetMonsterLevel(int level)
    {
        int index = level - 1;

        if (levelOverrides != null && index >= 0 && index < levelOverrides.Length)
        {
            if (levelOverrides[index] != null)
            {
                anim.runtimeAnimatorController = levelOverrides[index];
            }
            else
            {
                Debug.LogWarning($"ÇáÜ Override Controller ááãÓÊæì {level} ÛíÑ ãÚíä İí ÇáÜ Inspector!");
            }
        }
        else
        {
            Debug.LogError($"ÇáãÓÊæì {level} ÎÇÑÌ äØÇŞ ãÕİæİÉ ÇáÃäíãíÔä ÇáãÊÇÍÉ!");
        }
    }
}