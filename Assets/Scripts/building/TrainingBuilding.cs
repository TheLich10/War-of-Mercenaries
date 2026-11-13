using UnityEngine;

public class TrainingBuilding : MonoBehaviour
{
    public bool isBusy = false;
    public int level = 1; // مستوى هذا المبنى
    void OnMouseDown() // أو أي حدث اختيار تستخدمه
    {
        OpenUpgradePanel();
    }

    void OpenUpgradePanel()
    {
        if (UpgradeWarriorButton.Instance == null)
        {
            Debug.LogError("Upgrade Panel غير موجود!");
            return;
        }

        // 👇 الربط هنا (وقت التشغيل)
        UpgradeWarriorButton.Instance.currentBuilding = this;

        UpgradeWarriorButton.Instance.gameObject.SetActive(true);
    }
}
