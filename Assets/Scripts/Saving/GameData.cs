using System.Collections.Generic;

// هذه الفئة تمثل مبنى واحد فقط
[System.Serializable]
public class BuildingSaveData
{
    public string buildingTag;
    public int x;
    public int y;
    public int z;
    public int level; // 🟢 هذا هو السطر الجديد الذي سيحفظ المستوى
    // 🟢 بيانات البناء الجديد
    public bool isBuildingNew;
    public long buildEndTimeTicks;
    public float buildTotalDuration;

    // 🟢 بيانات الترقية (التطوير)
    public bool isUpgrading;
    public long upgradeEndTimeTicks;
    public float upgradeTotalDuration;

    // 🟢 السطور الجديدة: بيانات تطوير المحاربين داخل هذا المبنى
    public bool isTrainingWarrior;
    public string trainingWarriorName;
    public long trainingWarriorEndTimeTicks;
    public float trainingWarriorTotalDuration;

    // 🟢 بيانات الإنتاج للموارد (الحجر، الخشب، إلخ)
    public int storedResourceAmount;
    public float resourceTimer;

    public int sorLevel;
}

[System.Serializable]
public class WarriorLevelData
{
    public string name;
    public int level;
}

[System.Serializable]
public class MonsterSaveData
{
    public string monsterName;
    public int level;
    public int currentClicks; // عدد الأسهم/النقرات الحالية
    public bool isWaiting; // هل الوحش قيد التطوير؟
    public long upgradeEndTimeTicks; // وقت انتهاء التطوير (لحفظه أثناء إغلاق اللعبة)
}

[System.Serializable]
public class WarriorCountData
{
    public string warriorName;
    public int count;
}

[System.Serializable]
public class TrainingPanelSaveData
{
    public string panelID; // معرف البنل
    public string[] storageWarriorNames;
    public int[] storageCounts;
    public int[] readyCounts;

    public bool isTraining;
    public string currentTrainingWarrior;
    public int remainingTimeInSeconds; // الوقت المتبقي للجندي الحالي
}

[System.Serializable]
public class MultiTrainingSaveData
{
    public string[] storageWarriorNames;
    public int[] storageCounts;
    public int[] readyCounts;

    // حالة الخانات الخمسة
    public bool[] slotIsBusy;
    public string[] slotWarriorNames;
    public int[] slotRemainingTimes;
}

[System.Serializable]
public class WarriorCountSave
{
    public string warriorName;
    public int count;
}

[System.Serializable]
public class RouletteSaveData
{
    public List<int> unlockedRewardIDs = new List<int>(); // قائمة أرقام الجوائز المفتوحة
    public bool isBigRewardUnlocked;
}

[System.Serializable]
public class ResourceSaveData
{
    public int iron;
    public int wood;
    public int stone;
    public int power;
}

[System.Serializable]
public class PlayerLevelSaveData
{
    public int level;
    public int currentXP;
    public int baseXP;
    public int xpToLevelUp;
}

[System.Serializable]
public class AudioSaveData
{
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
}
// 🟢 داخل كلاس SaveFile الأساسي، أضف هذا المتغير:

[System.Serializable]
public class BuildingLimitItemSave
{
    public string buildingName;
    public int currentCount;
}

[System.Serializable]
public class BuildingLimitSaveData
{
    public int cityHallLevel = 1;
    public List<BuildingLimitItemSave> buildingLimits = new List<BuildingLimitItemSave>();
}

// داخل كلاس SaveFile الأساسي:

// 🟢 أضف هذه القائمة داخل كلاس SaveFile الأساسي:
// هذه الفئة تمثل ملف الحفظ بالكامل (الذي سيتم تحويله إلى JSON)
[System.Serializable]
public class SaveFile
{
    public int totalOwnedWorkers = 1;

    // 🟢 هل أضفت هذه الأسطر الأربعة هنا؟ إذا لم تفعل، لن يتم حفظ الوقت أبداً!
    public int warriorCurrentIndex = 0;
    public bool isWarriorUnlocking = false;
    public long warriorUnlockEndTimeTicks = 0;
    public float warriorUnlockTotalDuration = 0f;
    public int goldAmount; // 🟢 أضف هذا المتغير
    // 🟢 بيانات حفظ سكربت إخفاء الزر
    public bool isButtonHidden;
    public long buttonShowTimeTicks;
    public bool isExtraObjectActive;
    public long extraObjectHideTimeTicks;

    // 🟢 بيانات حفظ نقاط الحرب
    public int warPoints = 0;
    public int lastLevelReached = 0;

    // 🟢 بيانات حفظ العملة الحمراء
    public int redCoinAmount = 0;
    // 🟢 بيانات إعدادات الإطارات
    public int targetFPS = 60;

    public string activeHero = "None"; // 🟢 إضافة هذا السطر الجديد

    public List<BuildingSaveData> savedBuildings = new List<BuildingSaveData>();
    public List<WarriorLevelData> warriorLevels = new List<WarriorLevelData>(); // 🟢 الجديد
    public List<MonsterSaveData> savedMonsters = new List<MonsterSaveData>();
    public List<WarriorCountData> savedWarriorCounts = new List<WarriorCountData>();

    public long lastGlobalSaveTimeTicks = 0; // لحساب الوقت الذي كانت فيه اللعبة مغلقة
    public List<TrainingPanelSaveData> savedTrainingPanels = new List<TrainingPanelSaveData>();
    public MultiTrainingSaveData multiTrainingData = new MultiTrainingSaveData();
    public List<WarriorCountSave> activeWarriorsCount = new List<WarriorCountSave>();
    public RouletteSaveData rouletteData = new RouletteSaveData();
    public ResourceSaveData resourceData = new ResourceSaveData();
    public PlayerLevelSaveData playerLevelData = new PlayerLevelSaveData();
    // 🟢 بيانات حفظ الصوت
    public AudioSaveData audioData = new AudioSaveData();
    public BuildingLimitSaveData buildingLimitData = new BuildingLimitSaveData();



}