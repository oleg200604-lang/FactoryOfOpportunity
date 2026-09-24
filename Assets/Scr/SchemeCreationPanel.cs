using UnityEngine;
using UnityEngine.UI;

// Тип товару, який зараз налаштовується на панелі.
// Додаєш новий тип товару -> додаєш варіант сюди.
public enum CommodityTypeSelector
{
    None,
    Clothing,
    Electronics,
    Furniture,
    Machinery,
    Raw
}

public class SchemeCreationPanel : MonoBehaviour
{
    [Header("Base commodity fields")]
    public InputField commodityNameField;
    public InputField qualityField;

    [Header("Recipe")]
    public Commodity[] inputs;
    public int complexity;
    public int outputAmount = 1;

    [Header("Current selection (виставляється кнопками)")]
    public CommodityTypeSelector commodityType;

    private ClothingType clothingType;
    private ClothingStyleType clothingStyleType;
    private ElectronicsType electronicsType;
    private FurnitureType furnitureType;
    private MachineryType machineryType;
    private RawTupe rawType;

    public void SelectCommodityType(int id)
    {
        switch (id)
        {
            case 1: commodityType = CommodityTypeSelector.Clothing; break;
            case 2: commodityType = CommodityTypeSelector.Electronics; break;
            case 3: commodityType = CommodityTypeSelector.Furniture; break;
            case 4: commodityType = CommodityTypeSelector.Machinery; break;
            case 5: commodityType = CommodityTypeSelector.Raw; break;

            default:
                Debug.LogWarning($"Невідомий CommodityTypeSelector id: {id}");
                break;
        }
    }

    public void SelectClothingType(int id)
    {
        switch (id)
        {
            case 1: clothingType = ClothingType.casual; break;
            case 2: clothingType = ClothingType.formal; break;
            case 3: clothingType = ClothingType.swimwear; break;
            case 4: clothingType = ClothingType.winter; break;
            default: Debug.LogWarning($"Невідомий ClothingType id: {id}"); break;
        }
    }

    public void SelectClothingStyleType(int id)
    {
        switch (id)
        {
            case 1: clothingStyleType = ClothingStyleType.classic; break;
            case 2: clothingStyleType = ClothingStyleType.casual; break;
            case 3: clothingStyleType = ClothingStyleType.sporty; break;
            case 4: clothingStyleType = ClothingStyleType.boho; break;
            case 5: clothingStyleType = ClothingStyleType.grunge; break;
            case 6: clothingStyleType = ClothingStyleType.minimalism; break;
            default: Debug.LogWarning($"Невідомий ClothingStyleType id: {id}"); break;
        }
    }

    public void SelectElectronicsType(int id)
    {
        switch (id)
        {
            case 1: electronicsType = ElectronicsType.telephone; break;
            case 2: electronicsType = ElectronicsType.television; break;
            case 3: electronicsType = ElectronicsType.radio; break;
            case 4: electronicsType = ElectronicsType.computers; break;
            case 5: electronicsType = ElectronicsType.semiconductors; break;
            case 6: electronicsType = ElectronicsType.electronics; break;
            default: Debug.LogWarning($"Невідомий ElectronicsType id: {id}"); break;
        }
    }

    public void SelectFurnitureType(int id)
    {
        switch (id)
        {
            case 1: furnitureType = FurnitureType.tables; break;
            case 2: furnitureType = FurnitureType.chairs; break;
            case 3: furnitureType = FurnitureType.bed; break;
            case 4: furnitureType = FurnitureType.cabinets; break;
            default: Debug.LogWarning($"Невідомий FurnitureType id: {id}"); break;
        }
    }

    public void SelectMachineryType(int id)
    {
        switch (id)
        {
            case 1: machineryType = MachineryType.machine; break;
            case 2: machineryType = MachineryType.sewingMachine; break;
            case 3: machineryType = MachineryType.tools; break;
            case 4: machineryType = MachineryType.cables; break;
            default: Debug.LogWarning($"Невідомий MachineryType id: {id}"); break;
        }
    }

    public void SelectRawType(int id)
    {
        switch (id)
        {
            case 1: rawType = RawTupe.cotton; break;
            case 2: rawType = RawTupe.leather; break;
            case 3: rawType = RawTupe.synthetics; break;
            default: Debug.LogWarning($"Невідомий RawTupe id: {id}"); break;
        }
    }
    public void CreateButton()
    {
        if (string.IsNullOrWhiteSpace(commodityNameField.text))
        {
            Debug.LogWarning("Не можна створити схему: не вказано назву товару.");
            return;
        }

        Commodity newCommodity = BuildCommodity();

        if (newCommodity == null)
        {
            Debug.LogWarning("Не можна створити схему: не обрано тип товару.");
            return;
        }

        newCommodity.name = commodityNameField.text;
        newCommodity.quality = ParseQuality();

        Scheme newScheme = new Scheme
        {
            inputs = inputs,
            complexity = complexity,
            outputs = new Commodity[]
            {
                //new Commodity { commodity = newCommodity, amount = outputAmount }
            }
        };

        GameManagerScr.Instance.RegisterSchemeAndCommodity(newCommodity, newScheme);
    }
    private Commodity BuildCommodity()
    {
        switch (commodityType)
        {
            case CommodityTypeSelector.Clothing:
                return new Clothing { clothingType = clothingType, clothingStyleType = clothingStyleType };

            case CommodityTypeSelector.Electronics:
                return new Electronics { electronicsType = electronicsType };

            case CommodityTypeSelector.Furniture:
                return new Furniture { electronicsType = furnitureType };

            case CommodityTypeSelector.Machinery:
                return new Machinery { machineryType = machineryType };

            case CommodityTypeSelector.Raw:
                return new Raw { rawTupe = rawType };

            default:
                return null;
        }
    }

    private int ParseQuality()
    {
        return int.TryParse(qualityField.text, out int q) ? q : 0;
    }
}