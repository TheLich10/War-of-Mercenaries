using UnityEngine;

public class BuildingID : MonoBehaviour
{
    // åĞÇ ÇáãÊÛíÑ Óíßæä ÇáÜ ID ÇáİÑíÏ áßá äÓÎÉ
    public int buildingID;

    // ÚÏÇÏ ÚÇã áßá ãÈäì ãä åĞÇ ÇáäæÚ
    private static int nextID = 1;

    void Awake()
    {
        // ßá äÓÎÉ ÌÏíÏÉ ÊÃÎĞ ID ÌÏíÏ
        buildingID = nextID;
        nextID++;

    }
}
