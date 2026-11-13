using UnityEngine;
using System.Collections.Generic;

public class BarracksManager : MonoBehaviour
{
    public static BarracksManager Instance;
    public event System.Action<float, float> OnCapacityChanged; // (current, max)

    private List<Barracks> allBarracks = new List<Barracks>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterBarracks(Barracks b)
    {
        if (!allBarracks.Contains(b))
            allBarracks.Add(b);
    }

    public void UnregisterBarracks(Barracks b)
    {
        if (allBarracks.Contains(b))
            allBarracks.Remove(b);
    }

    public void UpdateBarracksCapacity(Barracks b, float oldCapacity, float newCapacity)
    {
        //Debug.Log("Number of registered barracks: " + allBarracks.Count); ديبك
        foreach (var barrack in allBarracks)
        {
            //Debug.Log(barrack.buildingName + " - Max: " + barrack.maxCapacity + ", Current: " + barrack.currentCapacity);
        }

        b.maxCapacity = newCapacity;
        //Debug.Log($"Updated {b.buildingName}: New Capacity = {newCapacity} | Total: {GetTotalMaxCapacity()}"); ديك
        OnCapacityChanged?.Invoke(GetTotalCurrentCapacity(), GetTotalMaxCapacity());

    }
    public void makeEvent(){
        OnCapacityChanged?.Invoke(GetTotalCurrentCapacity(), GetTotalMaxCapacity());

    }

    public float GetTotalMaxCapacity()
    {
        float total = 0;
        foreach (Barracks b in allBarracks)
        {
            total += b.maxCapacity;
        }
        return total;
    }

    public float GetTotalCurrentCapacity()
    {
        float total = 0;
        foreach (Barracks b in allBarracks)
        {
            total += b.currentCapacity;
        }
        return total;
    }
}
