using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiAllBars : MonoBehaviour
{
    public static UiAllBars Instance;

    [Header("Barrack")]
    public Image barrackBarFill;
    public TMP_Text BarrackText;

    [Header("Excution")]
    public Image ExeBarFill;
    public TMP_Text ExeutionText;


    [Header("Resources")]
    public Image ResourBarFill;
    public TMP_Text ResouresText;

    [Header("Wood")]
    public Image woodFill;
    public TMP_Text woodText;

    [Header("Iron")]
    public Image ironFill;
    public TMP_Text ironText;

    [Header("Stone")]
    public Image stoneFill;
    public TMP_Text stoneText;

    [Header("Might")]
    public Image MightFill;
    public TMP_Text MightText;

    [Header("City Gold")]
    public Image CityFill;
    public TMP_Text CityText;

    [Header("Watch Post")]
    public Image WatchFill;
    public TMP_Text WatchText;


    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // Barracks
        if (BarracksManager.Instance != null)
        {
            BarracksManager.Instance.OnCapacityChanged += UpdateBarrackUI;
            BarracksManager.Instance.OnCapacityChanged += UpdateExctionUI;

            UpdateBarrackUI(BarracksManager.Instance.GetTotalCurrentCapacity(), BarracksManager.Instance.GetTotalMaxCapacity());
            UpdateExctionUI(BarracksManager.Instance.GetTotalCurrentCapacity(), BarracksManager.Instance.GetTotalMaxCapacity());

        }

        // Resources
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourcesChanged += UpdateResourcesUI;
            UpdateResourcesUI(ResourceManager.Instance.GetTotalResourcesAmount(), ResourceManager.Instance.GetTotalResourcesCapacity());
        }

    }
    public void SetGoldStorage(BuildingGoldStorage goldStorage)
    {
        // ÅáÛÇÁ ÇáÇÔÊÑÇß ãä ÇáÍÏË ÇáÞÏíã ÅÐÇ ßÇä ãæÌæÏ
        if (currentGoldStorage != null)
            currentGoldStorage.OnGoldChanged -= UpdateCityGoldUI;

        currentGoldStorage = goldStorage;

        if (currentGoldStorage != null)
        {
            currentGoldStorage.OnGoldChanged += UpdateCityGoldUI;
            UpdateCityGoldUI(currentGoldStorage.currentGold, currentGoldStorage.maxGold);
        }
    }

    private BuildingGoldStorage currentGoldStorage = null;


    public void SetWoodStorage(buildWoodStroge woodStorage)
    {
        // ÅáÛÇÁ ÇáÇÔÊÑÇß ãä ÍÏË ÇáÞÏíã (áæ ãæÌæÏ)
        if (currentWoodStorage != null)
            currentWoodStorage.OnWoodChanged -= UpdateWoodUI;

        currentWoodStorage = woodStorage;

        if (currentWoodStorage != null)
        {
            currentWoodStorage.OnWoodChanged += UpdateWoodUI;
            UpdateWoodUI(currentWoodStorage.currentwood, currentWoodStorage.maxwood);
        }
    }
    private buildWoodStroge currentWoodStorage = null;

    private buildPowerStroge currentPowerStorage = null;
    public void SetPowerStorage(buildPowerStroge powerStorage)
    {
        // Ýß ÇáÇÔÊÑÇß ãä ÇáÍÏË ÇáÞÏíã áæ ãæÌæÏ
        if (currentPowerStorage != null)
            currentPowerStorage.OnPowerStorageChanged -= UpdateMightUI;

        currentPowerStorage = powerStorage;

        if (currentPowerStorage != null)
        {
            currentPowerStorage.OnPowerStorageChanged += UpdateMightUI;
            UpdateMightUI(currentPowerStorage.currentPower, currentPowerStorage.maxPower);
        }
    }

    private buildIronStorge currentIronStorage = null;

    public void SetIronStorage(buildIronStorge ironStorage)
    {
        if (currentIronStorage != null)
            currentIronStorage.OnIronChanged -= UpdateIronUI;

        currentIronStorage = ironStorage;

        if (currentIronStorage != null)
        {
            currentIronStorage.OnIronChanged += UpdateIronUI;
            UpdateIronUI(currentIronStorage.currentIron, currentIronStorage.maxIron);
        }
    }

    private buildStoneStroge currentStoneStorage = null;

    public void SetStoneStorage(buildStoneStroge stoneStorage)
    {
        if (currentStoneStorage != null)
            currentStoneStorage.OnStoneChanged -= UpdateStoneUI;

        currentStoneStorage = stoneStorage;

        if (currentStoneStorage != null)
        {
            currentStoneStorage.OnStoneChanged += UpdateStoneUI;
            UpdateStoneUI(currentStoneStorage.currentStone, currentStoneStorage.maxStone);
        }
    }

    private BarracksWatchPost currentWatchPost;

    public void SetWatchPost(BarracksWatchPost watchPost)
    {
        // Ýß ÇáÇÔÊÑÇß ãä ÇáÈÑÌ ÇáÓÇÈÞ
        if (currentWatchPost != null)
            currentWatchPost.OnWatchChanged -= UpdateWatchPostUI;

        currentWatchPost = watchPost;

        // ÇáÇÔÊÑÇß ÈÇáÈÑÌ ÇáÌÏíÏ
        if (currentWatchPost != null)
        {
            currentWatchPost.OnWatchChanged += UpdateWatchPostUI;

            // ÊÍÏíË ÝæÑí
            UpdateWatchPostUI(
                currentWatchPost.currentCapacity,
                currentWatchPost.maxCapacity
            );
        }
    }



    private void UpdateBarrackUI(float current, float max)
    {
        if (barrackBarFill) barrackBarFill.fillAmount = max > 0 ? current / max : 0;
        if (BarrackText) BarrackText.text = current + " / " + max;
    }

    private void UpdateExctionUI(float current, float max)
    {
        if (ExeBarFill) ExeBarFill.fillAmount = max > 0 ? current / max : 0;
        if (ExeutionText) ExeutionText.text = current + " / " + max;
    }


    private void UpdateResourcesUI(int current, int max)
    {
        if (ResourBarFill) ResourBarFill.fillAmount = max > 0 ? (float)current / max : 0f;
        if (ResouresText) ResouresText.text = current.ToString();
    }

    private void UpdateWoodUI(int current, int max)
    {
        if (woodFill) woodFill.fillAmount = max > 0 ? (float)current / max : 0f;
        if (woodText) woodText.text = current + " / " + max;
    }

    private void UpdateIronUI(int current, int max)
    {
        if (ironFill) ironFill.fillAmount = max > 0 ? (float)current / max : 0f;
        if (ironText) ironText.text = current + " / " + max;
    }

    private void UpdateStoneUI(int current, int max)
    {
        if (stoneFill) stoneFill.fillAmount = max > 0 ? (float)current / max : 0f;
        if (stoneText) stoneText.text = current + " / " + max;
    }

    private void UpdateMightUI(int current, int max)
    {
        if (MightFill) MightFill.fillAmount = max > 0 ? (float)current / max : 0f;
        if (MightText) MightText.text = current + " / " + max;
    }


    private void UpdateCityGoldUI(int current, int max)
    {
        if (CityFill) CityFill.fillAmount = max > 0 ? (float)current / max : 0f;
        if (CityText) CityText.text = current + " / " + max;
    }

    private void UpdateWatchPostUI(float current, float max)
    {
        if (WatchFill) WatchFill.fillAmount = max > 0 ? current / max : 0;
        if (WatchText) WatchText.text = current + " / " + max;
    }
}
