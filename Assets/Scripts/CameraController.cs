using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public static CameraController current;
    public bool IsDragging { get; private set; } = false;

    public Transform ground;
    private SpriteRenderer groundSR; // لتفادي GetComponent في كل إطار
    private Camera cam;
    private Vector3 dragOrigin;

    public float zoomSpeed = 0.5f;
    public float minZoom = 3f, maxZoom = 8f;
    private float minX, maxX, minY, maxY;

    void Awake()
    {
        current = this;
    }

    void Start()
    {
        cam = Camera.main;
        if (ground != null)
        {
            groundSR = ground.GetComponent<SpriteRenderer>();
        }
        SetCameraBounds();
    }

    void Update()
    {
        IsDragging = false;

        // لا تحرك الكاميرا إذا كنا نسحب مبنى
        if (GridAttckC.current != null && GridAttckC.current.IsDragging)
            return;
        if (GridBuildingSystem.current != null && GridBuildingSystem.current.IsDragging)
            return;

        if (IsPointerOverUIButNotBlocker())
            return;

        // تشغيل الزوم أولاً
        HandleZoom();

        // لا تسمح بالسحب أبداً إذا كان هناك أكثر من إصبع على الشاشة (لمنع التطفر أثناء الزوم)
        if (Input.touchCount < 2)
        {
            HandleTouchMovement();
            HandleMouseMovement();
        }

        ClampCameraPosition();
    }

    void HandleTouchMovement()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragOrigin = cam.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 currentPos = cam.ScreenToWorldPoint(touch.position);
                Vector3 difference = dragOrigin - currentPos;

                // تحريك الكاميرا مع الحفاظ على محور Z ثابتًا
                Vector3 targetPos = cam.transform.position + difference;
                cam.transform.position = new Vector3(targetPos.x, targetPos.y, cam.transform.position.z);

                // إعادة تعيين dragOrigin لتجنب التراكم والقفزات المفاجئة
                dragOrigin = cam.ScreenToWorldPoint(touch.position);

                IsDragging = true;
            }
        }
    }

    void HandleMouseMovement()
    {
        // لتجربة اللعبة على الـ Editor بسلاسة
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = dragOrigin - currentPos;

            if (difference.magnitude > 0.01f)
            {
                Vector3 targetPos = cam.transform.position + difference;
                cam.transform.position = new Vector3(targetPos.x, targetPos.y, cam.transform.position.z);
                dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
                IsDragging = true;
            }
        }
    }

    void HandleZoom()
    {
        // زوم الموبايل (إصبعين)
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // حساب المسافة السابقة والحالية بين الإصبعين
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            float prevDistance = (touch1PrevPos - touch2PrevPos).magnitude;
            float currentDistance = (touch1.position - touch2.position).magnitude;
            float deltaDistance = prevDistance - currentDistance;

            // تطبيق الزوم
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + deltaDistance * zoomSpeed * 0.01f, minZoom, maxZoom);
            SetCameraBounds();
        }

        // زوم الكمبيوتر (عجلة الماوس للتجربة)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed * 10f, minZoom, maxZoom);
            SetCameraBounds();
        }
    }

    bool IsPointerOverUIButNotBlocker()
    {
        if (EventSystem.current == null) return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
#if UNITY_EDITOR || UNITY_STANDALONE
        pointerData.position = Input.mousePosition;
#else
        if (Input.touchCount > 0)
            pointerData.position = Input.GetTouch(0).position;
        else
            return false;
#endif

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject.CompareTag("InteractionBlocker"))
                continue;

            return true;
        }

        return false;
    }

    void SetCameraBounds()
    {
        if (groundSR == null) return;

        float halfCamWidth = cam.orthographicSize * cam.aspect;
        float halfCamHeight = cam.orthographicSize;

        minX = groundSR.bounds.min.x + halfCamWidth;
        maxX = groundSR.bounds.max.x - halfCamWidth;
        minY = groundSR.bounds.min.y + halfCamHeight;
        maxY = groundSR.bounds.max.y - halfCamHeight;

        // لمنع الكاميرا من الانقلاب أو الارتجاج إذا كان الزوم أكبر من حجم الأرضية
        if (minX > maxX) { float temp = minX; minX = maxX; maxX = temp; }
        if (minY > maxY) { float temp = minY; minY = maxY; maxY = temp; }
    }

    void ClampCameraPosition()
    {
        cam.transform.position = new Vector3(
            Mathf.Clamp(cam.transform.position.x, minX, maxX),
            Mathf.Clamp(cam.transform.position.y, minY, maxY),
            cam.transform.position.z
        );
    }
}