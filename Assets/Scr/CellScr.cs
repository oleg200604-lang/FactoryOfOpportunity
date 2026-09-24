using Unity.VisualScripting;
using UnityEngine;

public class CellScr : MonoBehaviour
{
    [Header("Grid Coordinates")]
    public int x;
    public int y;

    [Header("Group")]
    public ZoneGroup ownerGroup;

    [Header("Visual")]
    public SpriteRenderer sprite;
    private SpriteRenderer spriteRenderer;
    public Vector2Int GetPosition()
    {
        return new Vector2Int(x, y);
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateColor();
    }

    public void Highlight(bool state)
    {
        // Поки що порожньо.
        // Тут пізніше можна зробити окреме підсвічування вибраної клітинки.
    }

    public void UpdateColor()
    {
        if (spriteRenderer == null)
            return;

        if (ownerGroup == null)
        {
            spriteRenderer.color = Color.white;
            return;
        }

        spriteRenderer.color = ownerGroup.GetColor();
    }
}


public interface Building
{

}

public class Workshop : Building
{
    public GameManagerScr gameManager;

    // Індекс рецепту в GameManagerScr.schemes
    public int schemeIndex = -1;

    // Поточний прогрес виробництва
    public float production;

    public bool HasScheme()
    {
        return gameManager != null &&
               schemeIndex >= 0 &&
               schemeIndex < gameManager.schemes.Count;
    }

    public Scheme GetScheme()
    {
        if (!HasScheme())
            return null;

        return gameManager.schemes[schemeIndex];
    }

    public void SetScheme(int index)
    {
        if (gameManager == null)
        {
            Debug.LogWarning("Workshop: GameManagerScr не встановлений.");
            return;
        }

        if (index < 0 || index >= gameManager.schemes.Count)
        {
            Debug.LogWarning($"Workshop: неправильний індекс схеми {index}.");
            return;
        }

        schemeIndex = index;
        production = 0;

        Debug.Log($"Workshop отримав схему №{index}: " +
                  $"complexity = {gameManager.schemes[index].complexity}");
    }

    public void ProductionCycle(int productionPower)
    {
        if (!HasScheme())
            return;

        Scheme scheme = GetScheme();

        production += productionPower;

        while (production >= scheme.complexity)
        {
            ProductionCreation(scheme);
            production -= scheme.complexity;
        }
    }

    private void ProductionCreation(Scheme scheme)
    {
        if (scheme.outputs == null || scheme.outputs.Length == 0)
        {
            Debug.LogWarning("У схеми немає output.");
            return;
        }

        Commodity output = scheme.outputs[0];

        if (output == null)
            return;

        // Поки що додаємо вироблений товар
        // до глобального списку commodity.
        Commodity existing = null;

        for (int i = 0; i < gameManager.commodity.Count; i++)
        {
            if (IsSameCommodity(gameManager.commodity[i], output))
            {
                existing = gameManager.commodity[i];
                break;
            }
        }

        if (existing != null)
        {
            existing.amount += output.amount;
        }
        else
        {
            gameManager.commodity.Add(output);
        }

        Debug.Log($"Вироблено: {output.name} x{output.amount}");
    }

    private bool IsSameCommodity(Commodity a, Commodity b)
    {
        if (a == null || b == null)
            return false;

        if (a.GetType() != b.GetType())
            return false;

        return a.name == b.name;
    }
}

public class Research : Building
{
    public int production;
    public void ProductionCycle(int productionPower)
    {
        production += productionPower;
        
    }
}

public class Storage : Building
{

}

public class Office : Building
{

}