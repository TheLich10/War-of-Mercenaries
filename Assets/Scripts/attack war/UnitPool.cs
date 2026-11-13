using System.Collections.Generic;
using UnityEngine;

public class UnitPool : MonoBehaviour
{
    public GameObject prefab;     // الجندي
    public int poolSize = 30;      // عدد الجنود المسبق

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Spawn(Vector3 position)
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("⚠️ Pool فارغ!");
            return null;
        }

        GameObject obj = pool.Dequeue();
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
