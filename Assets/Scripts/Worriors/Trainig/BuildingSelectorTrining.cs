using UnityEngine;

public class BuildingSelectorTrining : MonoBehaviour
{
    public static Transform SelectedBuilding; // «·„»‰Ï «·„Õœœ Õ«·Ì«

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.CompareTag("training"))
            {
                SelectedBuilding = hit.collider.transform;
                Debug.Log("Selected building: " + SelectedBuilding.name);
            }
        }
    }
}
