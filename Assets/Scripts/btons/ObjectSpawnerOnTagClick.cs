using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ObjectSpawnerOnTagClick : MonoBehaviour
{
    public GameObject objectToSpawn;

    private GameObject spawnedObject;

    // ✅ متغير جديد لحفظ مرجع المبنى الذي تم النقر عليه لتتبعه لاحقاً
    private Collider2D trackedCollider;

    private float scaleDuration = 0.5f;
    private float scaleTimer = 0f;
    private Vector3 initialScale = Vector3.zero;
    private Vector3 targetScale = Vector3.zero;
    private bool isScaling = false;

    public float addRange = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                DestroySpawnedObject();
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseWorldPos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos2D, Vector2.zero);

            if (hit.collider != null)
            {
                float range = 0f;

                // ✅ إذا التاغ spy -> خذ الـ range من DefensiveWatchPost
                if (hit.collider.CompareTag("spy"))
                {
                    DefensiveWatchPost wp = hit.collider.GetComponent<DefensiveWatchPost>();
                    if (wp != null)
                    {
                        range = wp.attackRange;

                        Debug.Log("Spy building detected | range: " + range);
                        trackedCollider = hit.collider; // ✅ حفظ المرجع لتتبع الحركة
                        SpawnObjectAt(hit.collider.bounds.center, range + addRange);
                        return;
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ لم يتم العثور على DefensiveWatchPost في spy");
                        DestroySpawnedObject();
                        return;
                    }
                }
                if (hit.collider.CompareTag("CaveBrack"))
                {
                    if (SceneManager.GetActiveScene().name.Contains("AttackScenes"))
                    {
                        HeroRangeControllerATK wp2 = hit.collider.GetComponent<HeroRangeControllerATK>();
                        if (wp2 != null)
                        {
                            range = wp2.attackRange;

                            //Debug.Log("cave building detected | range: " + range);
                            SpawnObjectAt(hit.collider.bounds.center, range + addRange);
                            return;
                        }
                        else
                        {
                            Debug.LogWarning("no Cave found");
                            DestroySpawnedObject();
                            return;
                        }
                    }
                    else
                    {
                    HeroRangeController wp = hit.collider.GetComponent<HeroRangeController>();
                    if (wp != null)
                    {
                        range = wp.attackRange;

                        //Debug.Log("cave building detected | range: " + range);
                        trackedCollider = hit.collider; // ✅ حفظ المرجع لتتبع الحركة
                        SpawnObjectAt(hit.collider.bounds.center, range + addRange);
                        return;
                    }
                    else
                    {
                        Debug.LogWarning("no Cave found");
                        DestroySpawnedObject();
                        return;
                    }

                    }

                }

                // ✅ إذا ليس spy -> تعامل مع المباني الدفاعية
                building targetBuilding = hit.collider.GetComponent<building>();
                DefensiveBuilding defensiveBuilding = hit.collider.GetComponent<DefensiveBuilding>();

                if (targetBuilding != null && defensiveBuilding != null)
                {
                    int targetLevel = targetBuilding.level;
                    string buildingName = defensiveBuilding.buildingName;

                    var buildingData = DefenceBuilds.GetBuildingByName(buildingName);
                    if (buildingData != null)
                    {
                        var levelData = buildingData.GetLevel(targetLevel);
                        if (levelData != null)
                        {
                            range = levelData.range;

                            //Debug.Log("Defensive building: " + buildingName + " | level: " + targetLevel + " | range: " + range);  ديبك
                            trackedCollider = hit.collider; // ✅ حفظ المرجع لتتبع الحركة
                            SpawnObjectAt(hit.collider.bounds.center, range + addRange);
                        }
                        else
                        {
                            Debug.LogWarning("No data for level " + targetLevel + " in building " + buildingName);
                            DestroySpawnedObject();
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Building not found in database: " + buildingName);
                        DestroySpawnedObject();
                    }
                }
                else
                {
                    DestroySpawnedObject();
                }
            }
            else
            {
                DestroySpawnedObject();
            }
        }

        // ✅ تحديث موقع الكائن المولد ليتبع المبنى المتبّع (trackedCollider) دائماً
        if (spawnedObject != null && trackedCollider != null)
        {
            spawnedObject.transform.position = trackedCollider.bounds.center;
        }

        // عملية التكبير التدريجي
        if (isScaling && spawnedObject != null)
        {
            scaleTimer += Time.deltaTime;
            float t = Mathf.Clamp01(scaleTimer / scaleDuration);
            spawnedObject.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

            if (t >= 1f)
            {
                isScaling = false;
            }
        }
    }

    void SpawnObjectAt(Vector3 position, float range)
    {
        DestroySpawnedObject();

        // إنشاء الكائن بالاستدارة المطلوبة
        spawnedObject = Instantiate(objectToSpawn, position, Quaternion.Euler(45, 0, 0));

        // 1. تحديد حجم الصورة الأساسي
        float baseSize = 1f;
        SpriteRenderer sr = spawnedObject.GetComponent<SpriteRenderer>();

        if (sr != null && sr.sprite != null)
        {
            // نأخذ عرض الصورة الفعلي بالوحدات
            baseSize = sr.sprite.bounds.size.x;
        }

        // 2. حساب القطر الكامل المطلوب (الرينج يمثل نصف القطر، لذا نضربه في 2)
        // إذا كان الاستدارة 45 درجة تؤثر على الحجم البصري وتود تعويضها، يمكن تعديل الحسابات هنا لاحقاً
        float desiredDiameter = range * 2f;

        // 3. حساب الـ Scale النهائي المطلوب للوصول للقطر الدقيق
        float finalScale = desiredDiameter / baseSize;

        targetScale = new Vector3(finalScale, finalScale, finalScale);
        spawnedObject.transform.localScale = initialScale;

        scaleTimer = 0f;
        isScaling = true;
    }

    void DestroySpawnedObject()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
            trackedCollider = null; // ✅ تفريغ المرجع عند حذف الكائن
            isScaling = false;
        }
    }
}