using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    // ŞÇÆãÉ ÊÍÊæí Úáì ÌãíÚ ÇáÜ Pools İí ÇáãÔåÏ
    public List<UnitPool> allPools;

    private void Awake() { Instance = this; }

    public UnitPool GetPool(string name)
    {
        // íÈÍË Úä ÇáÜ Pool ÇáĞí íÍÊæí Úáì äİÓ ÇÓã ÇáÌäÏí
        return allPools.Find(p => p.name == name || p.gameObject.name.Contains(name));
    }
}