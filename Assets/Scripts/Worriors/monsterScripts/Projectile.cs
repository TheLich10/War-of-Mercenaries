using UnityEngine;

public class Projectile2 : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private float damage;

    public void SetTarget(Transform newTarget, float dmg)
    {
        target = newTarget;
        damage = dmg;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        Enemy enemy = target.GetComponent<Enemy>();
        y3soobEnemy y3ssob = target.GetComponent<y3soobEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        if (y3ssob != null)
        {
            y3ssob.TakeDamage(damage);
        }
        Destroy(gameObject); // Õ–› «·”Â„ »⁄œ «·«’ÿœ«„
    }
}
