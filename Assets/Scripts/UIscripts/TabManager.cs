using UnityEngine;
using UnityEngine.UI;

public class TabHighlightOnly : MonoBehaviour
{
    [System.Serializable]
    public class TabElements
    {
        public Button tabButton;
        public GameObject tabPanel;
        [HideInInspector] public Sprite normalSprite; // áÍİÙ ÇáÕæÑÉ ÇáÃÕáíÉ
    }

    [Header("ÅÚÏÇÏÇÊ ÇáÃÒÑÇÑ æÇááæÍÇÊ")]
    public TabElements[] tabs;

    [Header("ÕæÑÉ ÇáÊÍÏíÏ ÇáÌãÇåíÉ")]
    public Sprite selectedSprite;

    void Start()
    {
        // ÍİÙ ÇáÕæÑ ÇáÃÕáíÉ ááÃÒÑÇÑ ÚäÏ ÈÏÇíÉ ÇááÚÈÉ
        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].tabButton != null)
            {
                tabs[i].normalSprite = tabs[i].tabButton.GetComponent<Image>().sprite;
            }
        }
    }

    void Update()
    {
        // äÓÊÏÚí ÏÇáÉ ÇáãÒÇãäÉ İí ßá ÅØÇÑ áãÑÇŞÈÉ ÇááæÍÇÊ ÇáãİÊæÍÉ
        SyncButtonImages();
    }

    private void SyncButtonImages()
    {
        bool hasSelectedOne = false; // ãÊÛíÑ áÖãÇä ÊÍÏíÏ ÒÑ æÇÍÏ İŞØ

        for (int i = 0; i < tabs.Length; i++)
        {
            if (tabs[i].tabPanel != null && tabs[i].tabButton != null)
            {
                Image btnImage = tabs[i].tabButton.GetComponent<Image>();
                bool isPanelOpen = tabs[i].tabPanel.activeSelf;

                // ÇáÔÑØ: ÅĞÇ ßÇä ÇáÈäá ãİÊæÍÇğ¡ æáã äŞã ÈÊÍÏíÏ Ãí ÒÑ ÂÎÑ ŞÈáå
                if (isPanelOpen && !hasSelectedOne)
                {
                    btnImage.sprite = selectedSprite; // æÖÚ ÕæÑÉ ÇáÊÍÏíÏ
                    hasSelectedOne = true; // äÄßÏ Ãäå Êã ÊÍÏíÏ ÒÑ¡ áãäÚ ÊÍÏíÏ Ãí ÒÑ ÂÎÑ
                }
                else
                {
                    btnImage.sprite = tabs[i].normalSprite; // ÅÑÌÇÚ ÇáÕæÑÉ ÇáÃÕáíÉ
                }
            }
        }
    }
}