using UnityEngine;
using System.Collections.Generic;

public class UpgradeTarget : MonoBehaviour
{
    // ŞÇÆãÉ ÚÇãÉ ÊÎÒä ßá ÇáÊØæíÑÇÊ ÇáäÔØÉ İí ÇáãÔåÏ
    public static List<UpgradeTarget> AvailableUpgrades = new List<UpgradeTarget>();

    // ãÊÛíÑ íÍÏÏ ãÇ ÅĞÇ ßÇä åĞÇ ÇáãÈäì ãÍÌæÒÇğ ãä ŞÈá ÚÇãá Ãã áÇ
    [HideInInspector]
    public bool isClaimed = false;

    void OnEnable()
    {
        if (!AvailableUpgrades.Contains(this))
        {
            AvailableUpgrades.Add(this);
        }
        isClaimed = false; // ÅÚÇÏÉ ÇáÊÚííä ÚäÏ ÊİÚíá ÇáãÈäì ãä ÌÏíÏ
    }

    void OnDisable()
    {
        if (AvailableUpgrades.Contains(this))
        {
            AvailableUpgrades.Remove(this);
        }
        isClaimed = false; // ÊÍÑíÑ ÇáÍÌÒ ÊáŞÇÆíÇğ ÚäÏ ÇÎÊİÇÁ ÇáãÈäì
    }
}