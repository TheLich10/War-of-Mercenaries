using UnityEngine;

public interface IDamageable
{
    // íãßäß ÊÚÏíá ÇáãÚÇãáÇÊ áÊäÇÓÈ áÚÈÊß¡ ãËáÇğ ÅÖÇİÉ ÇáÜ Transform ÅĞÇ ßäÊ ÊÍÊÇÌå
    void TakeDamage(float damage, Transform attacker = null);
}