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
    public SchemeCommodity[] MachineTools;
    public int production;
    public void ProductionCycle(int productionPower)
    {
        production += productionPower;
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