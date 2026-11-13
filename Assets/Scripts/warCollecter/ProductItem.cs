using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductController : MonoBehaviour
{
    public TMP_Text amountText; // مثال: "2/20"
    public string productName; // مثال: "خشب" أو "حجر"...
    public Button useOneButton;
    public Button useAllButton;

    int currentAmount;
    int maxAmount;

    void Start()
    {
        ParseAmountText();
        UpdateUI();
    }

    void ParseAmountText()
    {
        string[] parts = amountText.text.Split('/');
        if (parts.Length == 2)
        {
            int.TryParse(parts[0], out currentAmount);
            int.TryParse(parts[1], out maxAmount);
        }
    }

    void UpdateAmountText()
    {
        amountText.text = currentAmount + "/" + maxAmount;
    }

    void UpdateUI()
    {
        // الزر يظل مفعل دائمًا
        useOneButton.interactable = true;
        useAllButton.interactable = true;
    }


    public void UseOne()
    {
        ParseAmountText();
        if (currentAmount > 0)
        {
            currentAmount--;
            CallUseFunction(1);
            UpdateAmountText();
        }
        else
        {
            //Debug.Log("❌ لا يوجد لديك هذا المنتج!");  ديبك
        }

        UpdateUI();
    }


    public void UseAll()
    {
        ParseAmountText();
        if (currentAmount > 0)
        {
            CallUseFunction(currentAmount);
            currentAmount = 0;
            UpdateAmountText();
        }
        else
        {
            //Debug.Log("❌ لا يوجد لديك هذا المنتج!"); ديبك
        }

        UpdateUI();
    }


    void CallUseFunction(int amount)
    {
        int valuePerUnit = 0;

        switch (productName)
        {
            case "Lumber pack":
                valuePerUnit = 10000;
                ResourceManager.AddWood(amount * valuePerUnit);
                break;

            case "huge Lumber":
                valuePerUnit = 50000;
                ResourceManager.AddWood(amount * valuePerUnit);
                break;

            case "stone pack":
                valuePerUnit = 10000;
                ResourceManager.AddStone(amount * valuePerUnit);
                break;
            case "huge stone":
                valuePerUnit = 50000;
                ResourceManager.AddStone(amount * valuePerUnit);
                break;
            case "iron pack":
                valuePerUnit = 10000;
                ResourceManager.AddIron(amount * valuePerUnit);
                break;
            case "huge iron":
                valuePerUnit = 50000;
                ResourceManager.AddIron(amount * valuePerUnit);
                break;
            case "might pack":
                valuePerUnit = 10000;
                ResourceManager.AddPower(amount * valuePerUnit);
                break;
            case "huge might":
                valuePerUnit = 50000;
                ResourceManager.AddPower(amount * valuePerUnit);
                break;
            case "nubian guard":
                Barracks.instnce.AddWarrior("Savge");
                Barracks.instnce.AddWarrior("Savge");
                Barracks.instnce.AddWarrior("Savge");
                Barracks.instnce.AddWarrior("Savge");
                break;
            case "bedouin brute":
                for(int i= 1; i <=20; i++)
                Barracks.instnce.AddWarrior("bedoiun");
                break;
            case "Gentle Healer":
                Barracks.instnce.AddWarrior("Helaer");
                break;
            case "Rock gaul":
                Barracks.instnce.AddWarrior("Gaul");
                Barracks.instnce.AddWarrior("Gaul");
                break;
            default:
                Debug.LogWarning("اسم المنتج غير معروف: " + productName);
                break;
        }
    }

}
