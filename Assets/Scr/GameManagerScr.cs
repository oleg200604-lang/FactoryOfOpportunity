using System.Collections.Generic;
using UnityEngine;

public class GameManagerScr : MonoBehaviour
{
    public static GameManagerScr Instance { get; private set; }

    public List<Scheme> schemes = new List<Scheme>();
    public List<Commodity> commodity = new List<Commodity>();

    public float timer, timerSpeed, timeDay;
    public int day, season, year;
    public FactoryManagerScr factory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("На сцені існує більше одного GameManagerScr!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (timer <= 0)
        {
            UpdateDates();
            UpdateTic();
        }
        else
        {
            timer -= Time.deltaTime * timerSpeed;
        }
    }
    private void UpdateDates()
    {
        if (day < 7)
        {
            day++;
        }
        else
        {
            day = 1;
            if (season < 4)
            {
                season++;
            }
            else
            {
                season = 1;
                year++;
            }
        }
        timer = timeDay;
    }
    public void UpdateTic()
    {
        factory.Tic();
    }

    // Реєструє одночасно новий товар і нову схему його виробництва.
    // Викликається з панелі створення схеми (SchemeCreationPanel<T>).
    public void RegisterSchemeAndCommodity(Commodity newCommodity, Scheme newScheme)
    {
        if (newCommodity == null || newScheme == null)
        {
            Debug.LogWarning("Не можна зареєструвати схему: товар або схема — null.");
            return;
        }

        commodity.Add(newCommodity);
        schemes.Add(newScheme);

        Debug.Log($"Створено товар '{newCommodity.name}' ({newCommodity.GetType().Name}) " +
                  $"і схему його виробництва.");
    }
}
public abstract class Commodity
{
    public string name;
    public int quality;
}
[System.Serializable]
public class Scheme
{
    public SchemeCommodity[] inputs;

    public float complexity;
    public int quality;
    public SchemeCommodity[] outputs;
}
[System.Serializable]
public class SchemeCommodity
{
    public Commodity commodity;
    public int amount;
}
[System.Serializable]
public class Raw : Commodity
{
    public RawTupe rawTupe;
    public int amount;

}
public enum RawTupe
{
    cotton, leather, synthetics
}
[System.Serializable]
public class Machinery : Commodity
{
    public MachineryType machineryType;
}
public enum MachineryType
{
    machine, sewingMachine, tools, cables
}

[System.Serializable]
public class Clothing : Commodity
{
    public ClothingType clothingType;
    public ClothingStyleType clothingStyleType;

    public void ClothingTypes(ClothingType clothingTypes)
    {
        clothingType = clothingTypes;
    }

    public void ClothingStyleTypes(ClothingStyleType clothingStyleTypes)
    {
        clothingStyleType = clothingStyleTypes;
    }
}

public enum ClothingType
{
    casual, formal, swimwear, winter
}
public enum ClothingStyleType
{
    classic, casual, sporty, boho, grunge, minimalism
}
[System.Serializable]
public class Electronics : Commodity
{
    public ElectronicsType electronicsType;
}

public enum ElectronicsType
{
    telephone, television, radio, computers, semiconductors, electronics
}

[System.Serializable]
public class Furniture : Commodity
{
    public FurnitureType electronicsType;
}
public enum FurnitureType
{
    tables, chairs, bed, cabinets
}