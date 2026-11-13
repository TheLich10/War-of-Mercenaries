using UnityEngine;

public class DiggingTrack : MonoBehaviour
{
    public float lifeTime = 3f; // ßã ËÇäíÉ íÈŞì ÇáÃËÑ ŞÈá Ãä íÎÊİí
    private UnitPool myPool;

    // ÏÇáÉ áÊÍÏíÏ ÇáÜ Pool ÇáĞí íÊÈÚ áå åĞÇ ÇáÃËÑ
    public void SetupTrack(UnitPool pool)
    {
        myPool = pool;
        // ÅáÛÇÁ Ãí ÇÓÊÏÚÇÁÇÊ ÓÇÈŞÉ (áÖãÇä ÇáÓáÇãÉ ÚäÏ ÅÚÇÏÉ ÇáÇÓÊÎÏÇã)
        CancelInvoke(nameof(ReturnToPool));
        // ÇÓÊÏÚÇÁ ÏÇáÉ ÇáÅÎİÇÁ ÈÚÏ ãÑæÑ ÇáæŞÊ ÇáãÍÏÏ
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    void ReturnToPool()
    {
        if (myPool != null)
        {
            myPool.Despawn(gameObject);
        }
    }
}