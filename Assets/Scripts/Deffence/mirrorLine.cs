using System.Collections.Generic;
using UnityEngine;

public class mirrorLine : MonoBehaviour
{
    public float damage = 20f; // ŞíãÉ ÇáÖÑÑ ÓÊõÑÓá ãä ßáÇÓ ÂÎÑ
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // áÊÌäÈ ÊßÑÇÑ ÇáÖÑÑ Úáì äİÓ ÇáÚÏæ

    // íÌÈ Ãä íßæä Collider2D ãÚ ÎíÇÑ Is Trigger ãİÚá
    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        if (!hitEnemies.Contains(obj))
        {
            hitEnemies.Add(obj);

            if (other.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
            else if (other.TryGetComponent<EnemyRNG_Advanced>(out EnemyRNG_Advanced enemyRNG))
            {
                enemyRNG.TakeDamage(damage);
            }
            else if (other.TryGetComponent<sapperEnemy>(out sapperEnemy spaerEnemy))
            {
                spaerEnemy.TakeDamage(damage);
            }
            //else if (other.TryGetComponent<flayingEnemy>(out flayingEnemy flyeme))
            //{
            //    flyeme.TakeDamage(damage);
            //}
        }
    }

    public void DestroyAfterDelay(float delay = 2.5f)
    {
        Destroy(gameObject, delay);
    }
}
