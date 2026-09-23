using UnityEngine;

public class GameManagerScr : MonoBehaviour
{
    public Scheme[] schemes;
    public Commodity[] commodity;
    public float timer, timerSpeed, timeDay;
    public int day, season, year;
    public FactoryManagerScr factory;
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
            day ++;
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

    public int complexity;

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