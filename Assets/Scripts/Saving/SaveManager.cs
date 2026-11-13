using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private string saveFilePath;
    public static SaveManager Instance;

    private void Awake()
    {
        // نظام تثبيت الـ Singleton للتأكد من عدم تكراره
        if (Instance == null)
        {
            Instance = this;
            // تحديد مسار آمن للحفظ (يعمل على الويندوز والأندرويد والآيفون)
            saveFilePath = Application.persistentDataPath + "/citySaveData.json";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        if (GridBuildingSystem.current == null)
        {
            Debug.LogWarning("⚠️ لا يمكن الحفظ: GridBuildingSystem غير جاهز بعد!");
            return;
        }

        SaveFile newSave = new SaveFile();

        newSave.goldAmount = GoldCollector.GetGold();
        newSave.resourceData = ResourceManager.Instance != null ? ResourceManager.Instance.GetResourceData() : null;

        // حفظ العمال
        if (WorkerUI.Instance != null)
        {
            newSave.totalOwnedWorkers = WorkerUI.Instance.GetOwnedWorkers();
        }
        // حفظ بيانات المستوى
        if (PlayerLevel.instance != null)
        {
            newSave.playerLevelData = PlayerLevel.instance.GetLevelData();
        }

        // حفظ إعدادات الصوت
        if (AudioManager.Instance != null)
        {
            newSave.audioData.musicVolume = AudioManager.Instance.globalMusicVolume;
            newSave.audioData.sfxVolume = AudioManager.Instance.globalSFXVolume;
        }

        // حفظ بيانات المحاربين
        if (openNewWorior.instain != null)
        {
            newSave.warriorCurrentIndex = openNewWorior.instain.currentIndex;
            newSave.isWarriorUnlocking = openNewWorior.instain.isUnlocking;
            newSave.warriorUnlockEndTimeTicks = openNewWorior.instain.unlockEndTimeTicks;
            newSave.warriorUnlockTotalDuration = openNewWorior.instain.unlockTotalDuration;
        }

        // حفظ مستويات المحاربين
        if (WarriorLevelManager.Instance != null)
        {
            newSave.warriorLevels = WarriorLevelManager.Instance.GetWarriorLevelsForSave();
        }

        // حفظ بيانات الوحوش
        if (MonsterLevelManager.Instance != null)
        {
            newSave.savedMonsters = MonsterLevelManager.Instance.GetMonstersDataForSave();
        }

        // حفظ البطل المفعل حالياً
        if (SimpleButton.Instance != null)
        {
            newSave.activeHero = SimpleButton.Instance.currentActiveHero;
        }

        // حفظ بيانات نقاط الحرب (War Points)
        if (WarPointsManager.Instance != null)
        {
            newSave.warPoints = WarPointsManager.GetWarPoints();
            newSave.lastLevelReached = WarPointsManager.GetLastLevelReached();
        }

        // حفظ إعدادات الإطارات (FPS)
        if (SettingsManager.Instance != null)
        {
            newSave.targetFPS = SettingsManager.Instance.currentFPS;
        }

        // حفظ أعداد المحاربين في الثكنة
        if (Barracks.instnce != null)
        {
            newSave.savedWarriorCounts = Barracks.GetWarriorsForSaveGlobal();
        }

        // احفظ بيانات الروليت
        if (Roulette.intstin != null)
        {
            newSave.rouletteData = Roulette.intstin.GetSaveData();
        }

        // حفظ بيانات العملة الحمراء (Red Coin)
        newSave.redCoinAmount = RedCoinCollector.GetRedCoinAmount();

        // حفظ بيانات الزر المخفي
        if (HideButtonSimple.intstin != null)
        {
            newSave.isButtonHidden = HideButtonSimple.intstin.isButtonHidden;
            newSave.buttonShowTimeTicks = HideButtonSimple.intstin.buttonShowTimeTicks;
            newSave.isExtraObjectActive = HideButtonSimple.intstin.isExtraObjectActive;
            newSave.extraObjectHideTimeTicks = HideButtonSimple.intstin.extraObjectHideTimeTicks;
        }

        newSave.lastGlobalSaveTimeTicks = System.DateTime.Now.Ticks;

        // سحب كل بنلات التدريب في المشهد
        WarriorButtonHandler[] allPanels = UnityEngine.Object.FindObjectsByType<WarriorButtonHandler>(UnityEngine.FindObjectsSortMode.None);
        foreach (WarriorButtonHandler panel in allPanels)
        {
            if (!string.IsNullOrEmpty(panel.panelID))
            {
                newSave.savedTrainingPanels.Add(panel.GetSaveData());
            }
        }

        // سحب السكربت وحفظ بياناته
        MultiWarriorButtonHandler multiHandler = UnityEngine.Object.FindFirstObjectByType<MultiWarriorButtonHandler>();
        if (multiHandler != null)
        {
            newSave.multiTrainingData = multiHandler.GetSaveData();
        }

        // حفظ عدد المحاربين النشطين
        Dictionary<string, int> warriorCounts = new Dictionary<string, int>();
        Warrior[] allWarriors = UnityEngine.Object.FindObjectsByType<Warrior>(FindObjectsSortMode.None);

        foreach (Warrior w in allWarriors)
        {
            if (w != null && w.gameObject.activeInHierarchy)
            {
                if (!warriorCounts.ContainsKey(w.warriorName)) warriorCounts[w.warriorName] = 0;
                warriorCounts[w.warriorName]++;
            }
        }

        newSave.activeWarriorsCount.Clear();
        foreach (var entry in warriorCounts)
        {
            newSave.activeWarriorsCount.Add(new WarriorCountSave { warriorName = entry.Key, count = entry.Value });
        }

        // حفظ المباني
        if (GridBuildingSystem.current != null && GridBuildingSystem.current.activeBuildingsInScene != null)
        {
            foreach (GameObject buildingObj in GridBuildingSystem.current.activeBuildingsInScene)
            {
                if (buildingObj == null) continue;

                string tag = GridBuildingSystem.current.GetTagFromBuildingOrChildren(buildingObj);
                Vector3Int cellPos = GridBuildingSystem.current.gridlayout.WorldToCell(buildingObj.transform.position);

                int bLevel = 0;
                bool isUpgrading = false;
                long upgEndTime = 0;
                float upgTotalDur = 0f;

                BuildLvl lvlScript = buildingObj.GetComponent<BuildLvl>();
                if (lvlScript != null)
                {
                    bLevel = lvlScript.GetCurrentLevel();
                    isUpgrading = lvlScript.isUpgrading;
                    upgEndTime = lvlScript.upgradeEndTimeTicks;
                    upgTotalDur = lvlScript.upgradeTotalDuration;
                }

                building bScript = buildingObj.GetComponent<building>();
                bool isBuildingNew = (bScript != null) ? bScript.IsUnderConstruction : false;
                long buildEndTime = (bScript != null) ? bScript.buildEndTimeTicks : 0;
                float buildTotalDur = (bScript != null) ? bScript.buildTotalDuration : 0f;

                TrainingBuilding tBuilding = buildingObj.GetComponentInChildren<TrainingBuilding>(true);
                bool isTraining = false;
                string trainName = "";
                long trainEnd = 0;
                float trainTotal = 0f;

                if (tBuilding != null && UpgradeWarriorButton.Instance != null)
                {
                    isTraining = UpgradeWarriorButton.Instance.IsBuildingBusy(tBuilding);
                    if (isTraining)
                    {
                        trainName = UpgradeWarriorButton.Instance.GetUpgradingWarrior(tBuilding);
                        trainEnd = UpgradeWarriorButton.Instance.GetEndTime(tBuilding);
                        trainTotal = UpgradeWarriorButton.Instance.GetTotalDuration(tBuilding);
                    }
                }

                int savedResAmount = 0;
                float savedResTimer = 0f;

                buildStoneStroge stoneScript = buildingObj.GetComponentInChildren<buildStoneStroge>(true);
                buildWoodStroge woodScript = buildingObj.GetComponentInChildren<buildWoodStroge>(true);
                buildIronStorge ironScript = buildingObj.GetComponentInChildren<buildIronStorge>(true);
                buildPowerStroge powerScript = buildingObj.GetComponentInChildren<buildPowerStroge>(true);
                BuildingGoldStorage goldScript = buildingObj.GetComponentInChildren<BuildingGoldStorage>(true);

                if (stoneScript != null)
                {
                    savedResAmount = stoneScript.currentStone;
                    savedResTimer = stoneScript.timer;
                }
                else if (woodScript != null)
                {
                    savedResAmount = woodScript.currentwood;
                    savedResTimer = woodScript.timer;
                }
                else if (ironScript != null)
                {
                    savedResAmount = ironScript.currentIron;
                    savedResTimer = ironScript.timer;
                }
                else if (powerScript != null)
                {
                    savedResAmount = powerScript.currentPower;
                    savedResTimer = powerScript.timer;
                }
                else if (goldScript != null)
                {
                    savedResAmount = goldScript.currentGold;
                    savedResTimer = goldScript.timer;
                }

                // 🟢 استخراج مستوى السور من المبنى الدفاعي إن وجد
                // تأكد أنك تقرأ القيمة من سكربت المبنى الأب لأنه المصدر الأساسي في دالة Start
                int savedSorLevel = 0;
                if (bScript != null)
                {
                    savedSorLevel = bScript.sorlevl;
                }

                if (BuildingLimitSystem.instaine != null)
                {
                    newSave.buildingLimitData = BuildingLimitSystem.instaine.GetSaveData();
                }

                BuildingSaveData bData = new BuildingSaveData
                {
                    buildingTag = tag,
                    x = cellPos.x,
                    y = cellPos.y,
                    z = cellPos.z,
                    level = bLevel,
                    isBuildingNew = isBuildingNew,
                    buildEndTimeTicks = buildEndTime,
                    buildTotalDuration = buildTotalDur,
                    isUpgrading = isUpgrading,
                    upgradeEndTimeTicks = upgEndTime,
                    upgradeTotalDuration = upgTotalDur,
                    isTrainingWarrior = isTraining,
                    trainingWarriorName = trainName,
                    trainingWarriorEndTimeTicks = trainEnd,
                    trainingWarriorTotalDuration = trainTotal,
                    storedResourceAmount = savedResAmount,
                    resourceTimer = savedResTimer,
                    sorLevel = savedSorLevel // 🟢 حفظ مستوى السور هنا
                };

                newSave.savedBuildings.Add(bData);
            }
        }

        string json = JsonUtility.ToJson(newSave, true);
        File.WriteAllText(saveFilePath, json);
        //Debug.Log("✅ تم حفظ اللعبة بنجاح في: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        SaveFile loadedSave = JsonUtility.FromJson<SaveFile>(json);

        if (loadedSave == null) return;

        GoldCollector.SetGold(loadedSave.goldAmount);

        // 🔴 تم إزالة الـ Spawn المباشر العشوائي من هنا لأنه مكرر ويسبب كراش!
        // نكتفي فقط بالـ Coroutine الآمن الذي يعمل بتأخير زمني لضمان تحميل البيئة أولاً.
        StartCoroutine(DelayedLoadWarriors(loadedSave));

        if (loadedSave.resourceData != null && ResourceManager.Instance != null)
        {
            ResourceManager.Instance.LoadResources(loadedSave.resourceData);
        }

        if (AudioManager.Instance != null && loadedSave.audioData != null)
        {
            AudioManager.Instance.LoadData(loadedSave.audioData.musicVolume, loadedSave.audioData.sfxVolume);
            if (VolumeController.Instance != null)
            {
                VolumeController.Instance.RefreshUI();
            }
        }

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.LoadData(loadedSave.targetFPS);
            if (SwitchFPSbtn.Instance != null)
            {
                SwitchFPSbtn.Instance.RefreshUI();
            }
        }

        if (PlayerLevel.instance != null && loadedSave.playerLevelData != null)
        {
            PlayerLevel.instance.LoadLevelData(loadedSave.playerLevelData);
        }

        if (RedCoinCollector.instance != null)
        {
            RedCoinCollector.instance.LoadData(loadedSave.redCoinAmount);
        }

        if (HideButtonSimple.intstin != null)
        {
            HideButtonSimple.intstin.LoadData(
                loadedSave.isButtonHidden,
                loadedSave.buttonShowTimeTicks,
                loadedSave.isExtraObjectActive,
                loadedSave.extraObjectHideTimeTicks
            );
        }

        if (BuildingLimitSystem.instaine != null && loadedSave.buildingLimitData != null)
        {
            BuildingLimitSystem.instaine.LoadData(loadedSave.buildingLimitData);
        }

        if (WarPointsManager.Instance != null)
        {
            WarPointsManager.Instance.LoadData(loadedSave.warPoints, loadedSave.lastLevelReached);
        }

        if (WorkerUI.Instance != null)
        {
            WorkerUI.Instance.LoadOwnedWorkers(loadedSave.totalOwnedWorkers);
        }

        if (SimpleButton.Instance != null && !string.IsNullOrEmpty(loadedSave.activeHero))
        {
            SimpleButton.Instance.LoadSavedHero(loadedSave.activeHero);
        }

        if (openNewWorior.instain != null)
        {
            openNewWorior.instain.LoadWarriorData(
                loadedSave.warriorCurrentIndex,
                loadedSave.isWarriorUnlocking,
                loadedSave.warriorUnlockEndTimeTicks,
                loadedSave.warriorUnlockTotalDuration
            );
        }

        if (WarriorLevelManager.Instance != null && loadedSave.warriorLevels != null)
        {
            WarriorLevelManager.Instance.LoadWarriorLevels(loadedSave.warriorLevels);
        }

        if (MonsterLevelManager.Instance != null && loadedSave.savedMonsters != null)
        {
            MonsterLevelManager.Instance.LoadMonstersData(loadedSave.savedMonsters);
        }

        // تنظيف الخريطة الحالية تماماً قبل التحميل
        if (GridBuildingSystem.current != null && GridBuildingSystem.current.activeBuildingsInScene != null)
        {
            foreach (GameObject buildingObj in GridBuildingSystem.current.activeBuildingsInScene)
            {
                if (buildingObj != null)
                {
                    building bScript = buildingObj.GetComponent<building>();
                    if (bScript != null)
                    {
                        GridBuildingSystem.current.FreeArea(bScript.area);
                    }
                    Destroy(buildingObj);
                }
            }
            GridBuildingSystem.current.activeBuildingsInScene.Clear();
        }

        // إعادة بناء المدينة من الملف المحفوظ
        if (GridBuildingSystem.current != null && loadedSave.savedBuildings != null)
        {
            foreach (BuildingSaveData bData in loadedSave.savedBuildings)
            {
                GridBuildingSystem.current.SpawnBuildingFromSave(bData);
            }
        }

        StartCoroutine(DelayedLoadBuildingsData(loadedSave));

        if (loadedSave.savedWarriorCounts != null)
        {
            Barracks.LoadGlobalWarriors(loadedSave.savedWarriorCounts);
        }

        WarriorButtonHandler[] allPanels = UnityEngine.Object.FindObjectsByType<WarriorButtonHandler>(UnityEngine.FindObjectsSortMode.None);
        foreach (WarriorButtonHandler panel in allPanels)
        {
            TrainingPanelSaveData panelData = loadedSave.savedTrainingPanels.Find(p => p.panelID == panel.panelID);
            if (panelData != null)
            {
                panel.LoadAndCatchUp(panelData, loadedSave.lastGlobalSaveTimeTicks);
            }
        }

        MultiWarriorButtonHandler multiHandler = UnityEngine.Object.FindFirstObjectByType<MultiWarriorButtonHandler>();
        if (multiHandler != null && loadedSave.multiTrainingData != null && loadedSave.multiTrainingData.slotIsBusy != null)
        {
            multiHandler.LoadAndCatchUp(loadedSave.multiTrainingData, loadedSave.lastGlobalSaveTimeTicks);
        }

        if (Roulette.intstin != null && loadedSave.rouletteData != null)
        {
            Roulette.intstin.LoadData(loadedSave.rouletteData);
        }
    }

    private IEnumerator DelayedLoadWarriors(SaveFile loadedSave)
    {
        // 0.5 ثانية تأخير لضمان جاهزية الـ Pools في المشهد بالكامل
        yield return new WaitForSeconds(0.5f);

        if (PoolManager.Instance == null)
        {
            Debug.LogWarning("⚠️ PoolManager.Instance غير موجود في المشهد بعد!");
            yield break;
        }

        UnitPool[] allPools = Object.FindObjectsByType<UnitPool>(FindObjectsSortMode.None);

        foreach (WarriorCountSave data in loadedSave.activeWarriorsCount)
        {
            UnitPool targetPool = null;

            foreach (UnitPool pool in allPools)
            {
                if (pool != null && pool.prefab != null && pool.prefab.name == data.warriorName)
                {
                    targetPool = pool;
                    break;
                }
            }

            if (targetPool != null)
            {
                for (int i = 0; i < data.count; i++)
                {
                    Vector3 spawnPos = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
                    GameObject obj = targetPool.Spawn(spawnPos);

                    if (obj != null)
                    {
                        obj.SetActive(true);
                        Warrior w = obj.GetComponent<Warrior>();
                        if (w != null)
                        {
                            w.warriorName = data.warriorName;
                        }
                    }
                }
            }
            else
            {
                //Debug.LogWarning("⚠️ لم يتم العثور على Pool لجندي باسم: " + data.warriorName);
            }
        }
    }

    private IEnumerator DelayedLoadBuildingsData(SaveFile loadedSave)
    {
        yield return new WaitForSeconds(0.2f);

        if (GridBuildingSystem.current == null || GridBuildingSystem.current.activeBuildingsInScene == null) yield break;

        foreach (GameObject buildingObj in GridBuildingSystem.current.activeBuildingsInScene)
        {
            if (buildingObj == null) continue;

            Vector3Int cellPos = GridBuildingSystem.current.gridlayout.WorldToCell(buildingObj.transform.position);
            BuildingSaveData bData = loadedSave.savedBuildings.Find(b => b.x == cellPos.x && b.y == cellPos.y && b.z == cellPos.z);

            if (bData != null)
            {
                buildStoneStroge stoneScript = buildingObj.GetComponentInChildren<buildStoneStroge>(true);
                if (stoneScript != null)
                {
                    stoneScript.LoadAndCatchUp(bData.storedResourceAmount, bData.resourceTimer, loadedSave.lastGlobalSaveTimeTicks);
                }

                buildWoodStroge woodScript = buildingObj.GetComponentInChildren<buildWoodStroge>(true);
                if (woodScript != null)
                {
                    woodScript.LoadAndCatchUp(bData.storedResourceAmount, bData.resourceTimer, loadedSave.lastGlobalSaveTimeTicks);
                }

                buildIronStorge ironScript = buildingObj.GetComponentInChildren<buildIronStorge>(true);
                if (ironScript != null)
                {
                    ironScript.LoadAndCatchUp(bData.storedResourceAmount, bData.resourceTimer, loadedSave.lastGlobalSaveTimeTicks);
                }

                buildPowerStroge powerScript = buildingObj.GetComponentInChildren<buildPowerStroge>(true);
                if (powerScript != null)
                {
                    powerScript.LoadAndCatchUp(bData.storedResourceAmount, bData.resourceTimer, loadedSave.lastGlobalSaveTimeTicks);
                }

                BuildingGoldStorage goldScript = buildingObj.GetComponentInChildren<BuildingGoldStorage>(true);
                if (goldScript != null)
                {
                    goldScript.LoadAndCatchUp(bData.storedResourceAmount, bData.resourceTimer, loadedSave.lastGlobalSaveTimeTicks);
                }

                DefensiveBuilding defBuilding = buildingObj.GetComponentInChildren<DefensiveBuilding>(true);
                if (defBuilding != null)
                {
                    defBuilding.LoadSorLevel(bData.sorLevel);
                }

                building mainBuildingScript = buildingObj.GetComponent<building>();
                if (mainBuildingScript != null)
                {
                    mainBuildingScript.LoadSorLevel(bData.sorLevel);
                }
            }
        }
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.UpdateUI();
        }
    }

    public void ResetGame()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        if (GridBuildingSystem.current != null && GridBuildingSystem.current.activeBuildingsInScene != null)
        {
            foreach (GameObject buildingObj in GridBuildingSystem.current.activeBuildingsInScene)
            {
                if (buildingObj != null)
                {
                    building bScript = buildingObj.GetComponent<building>();
                    if (bScript != null)
                    {
                        GridBuildingSystem.current.FreeArea(bScript.area);
                    }
                    Destroy(buildingObj);
                }
            }
            GridBuildingSystem.current.activeBuildingsInScene.Clear();
        }

        Debug.Log("🔄 تم إعادة تعيين اللعبة بنجاح!");
    }

    public void SaveResourcesAndBarracksOnly()
    {
        SaveFile currentSave = new SaveFile();

        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            currentSave = JsonUtility.FromJson<SaveFile>(json);
        }

        currentSave.goldAmount = GoldCollector.GetGold();

        if (ResourceManager.Instance != null)
        {
            currentSave.resourceData = ResourceManager.GetGlobalResourceData();
        }

        if (Barracks.instnce != null)
        {
            currentSave.savedWarriorCounts = Barracks.GetWarriorsForSaveGlobal();
        }
        //Debug.Log("🔄 تم إعادة تعيين اللعبة بنجاح!");


        currentSave.activeWarriorsCount.Clear();
        if (currentSave.savedWarriorCounts != null)
        {
            foreach (var w in currentSave.savedWarriorCounts)
            {
                currentSave.activeWarriorsCount.Add(new WarriorCountSave
                {
                    warriorName = w.warriorName,
                    count = w.count
                });
            }
        }

        currentSave.lastGlobalSaveTimeTicks = System.DateTime.Now.Ticks;

        string updatedJson = JsonUtility.ToJson(currentSave, true);
        File.WriteAllText(saveFilePath, updatedJson);
    }

    public void LoadAudioAndFPSOnly()
    {
        if (!File.Exists(saveFilePath)) return;

        string json = File.ReadAllText(saveFilePath);
        SaveFile loadedSave = JsonUtility.FromJson<SaveFile>(json);

        if (loadedSave == null) return;

        if (loadedSave.savedWarriorCounts != null)
        {
            Barracks.LoadGlobalWarriors(loadedSave.savedWarriorCounts);
        }

        if (AudioManager.Instance != null && loadedSave.audioData != null)
        {
            AudioManager.Instance.LoadData(loadedSave.audioData.musicVolume, loadedSave.audioData.sfxVolume);
            if (VolumeController.Instance != null)
            {
                VolumeController.Instance.RefreshUI();
            }
        }

        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.LoadData(loadedSave.targetFPS);
            if (SwitchFPSbtn.Instance != null)
            {
                SwitchFPSbtn.Instance.RefreshUI();
            }
        }
    }

    private void HandleAutoSave()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "SampleScene")
        {
            SaveGame();
        }
        else if (currentScene.Contains("AttackScenes"))
        {
            SaveResourcesAndBarracksOnly();
        }
    }

    //private void OnApplicationQuit()
    //{
    //    HandleAutoSave();
    //}

    //private void OnApplicationPause(bool pauseStatus)
    //{
    //    if (pauseStatus)
    //    {
    //        HandleAutoSave();
    //    }
    //}
}