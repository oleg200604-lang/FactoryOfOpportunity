    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem;

public class FactoryManagerScr : MonoBehaviour
{
    public static FactoryManagerScr Instance { get; private set; }

    [Header("Groups")]
    public List<ZoneGroup> groups = new List<ZoneGroup>();

    public bool isBulder;
    public int buildID;
    [Header("Debug")]
    [SerializeField] private bool autoFindCells = true;
    [SerializeField] private bool logCellRegistration = false;
    public bool isDeleting;
    private readonly Dictionary<Vector2Int, CellScr> cellMap = new Dictionary<Vector2Int, CellScr>();

    private CellScr firstSelected;
    private CellScr secondSelected;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "На сцені існує більше одного FactoryManagerScr!"
            );

            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        ClearRuntimeGroups();
        FindAllCells();

        Debug.Log($"FactoryManager: знайдено {cellMap.Count} клітинок.");
    }

    public void Tic()
    {
        foreach (ZoneGroup group in groups)
        {
            if (group != null)
            {
                group.Manufacturing();
            }
        }
    }
    private void ClearRuntimeGroups()
    {
        foreach (CellScr cell in FindObjectsByType<CellScr>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            cell.ownerGroup = null;
        }

        groups.Clear();
    }
    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        CellScr clicked = GetCellUnderMouse();

        if (clicked == null)
            return;
        if (isBulder == true)
        {
            HandleCellClick(clicked);
            return;
        }

        if (isDeleting == true)
        {
            HandleDeleteClick(clicked);
            return;
        }

        HandleNormalClick(clicked);
    }

    private void HandleNormalClick(CellScr clicked)
    {
        if (clicked.ownerGroup == null)
            return;

        ZoneGroup group = clicked.ownerGroup;

        if (group.building is Workshop)
        {
            ProductionZonePanelScr panel = FindFirstObjectByType<ProductionZonePanelScr>();

            if (panel == null)
            {
                Debug.LogWarning(
                    "ProductionZonePanel не знайдений на сцені."
                );

                return;
            }

            panel.Open(group);
        }
    }
    public void Bulds(int id)
    {
        isBulder = true;
        isDeleting = false;
        buildID = id;
    }
    public void DeleteMode()
    {
        print("Destroy");
        isDeleting = true;
        isBulder = false;
        ClearSelection();
    }
    private void HandleDeleteClick(CellScr clicked)
    {
        if (clicked.ownerGroup == null)
        {
            Debug.Log($"Клітинка ({clicked.x}, {clicked.y}) не належить жодній зоні.");
            isDeleting = false;
            return;
        }

        DeleteZone(clicked.ownerGroup);
        isDeleting = false;
    }
    public void FindAllCells()
    {
        cellMap.Clear();

        CellScr[] cells = FindObjectsByType<CellScr>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (CellScr cell in cells)
        {
            RegisterCell(cell);
        }
    }
    public void RegisterCell(CellScr cell)
    {
        if (cell == null)
            return;

        Vector2Int position = cell.GetPosition();

        if (cellMap.TryGetValue(position, out CellScr existing))
        {
            if (existing != cell)
            {
                Debug.LogError($"Конфлікт координат клітинок: " + $"({position.x}, {position.y}). " + $"Вже зареєстрована клітинка '{existing.name}', " + $"але намагається зареєструватися '{cell.name}'.");

                return;
            }

            return;
        }

        cellMap.Add(position, cell);

        if (logCellRegistration)
        {
            Debug.Log($"Зареєстровано CellScr '{cell.name}' " + $"на координатах ({cell.x}, {cell.y})");
        }
    }
    public void UnregisterCell(CellScr cell)
    {
        if (cell == null)
            return;

        Vector2Int position = cell.GetPosition();

        if (cellMap.TryGetValue(position, out CellScr registered))
        {
            if (registered == cell)
            {
                cellMap.Remove(position);
            }
        }
    }
    public bool TryGetCell(int x, int y, out CellScr cell)
    {
        return cellMap.TryGetValue(
            new Vector2Int(x, y),
            out cell
        );
    }
    private void HandleCellClick(CellScr clicked)
    {
        if (firstSelected == null)
        {
            firstSelected = clicked;

            firstSelected.Highlight(true);

            Debug.Log(
                $"Перша клітинка: ({clicked.x}, {clicked.y})"
            );

            return;
        }

        if (clicked == firstSelected)
        {
            firstSelected.Highlight(false);
            firstSelected = null;

            Debug.Log("Вибір скасовано.");

            return;
        }

        secondSelected = clicked;

        Debug.Log($"Друга клітинка: ({clicked.x}, {clicked.y})");

        ZoneGroup group = CreateGroup(firstSelected, secondSelected);

        if (group != null)
        {
            Debug.Log($"Створено групу '{group.groupName}' " + $"з {group.cells.Count} клітинок.");
        }


        ClearSelection();
    }


    private void ClearSelection()
    {
        if (firstSelected != null)
        {
            firstSelected.Highlight(false);
        }

        if (secondSelected != null)
        {
            secondSelected.Highlight(false);
        }

        firstSelected = null;
        secondSelected = null;
        isBulder = false;
    }

    private CellScr GetCellUnderMouse()
    {
        if (Camera.main == null)
        {
            Debug.LogError("FactoryManagerScr: Camera.main не знайдена.");

            return null;
        }


        Vector2 screenPosition =
            Mouse.current.position.ReadValue();


        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0f));


        Collider2D hit =
            Physics2D.OverlapPoint(worldPosition);


        if (hit == null)
            return null;


        CellScr cell = hit.GetComponent<CellScr>();

        if (cell != null)
            return cell;

        cell = hit.GetComponentInParent<CellScr>();

        if (cell != null)
            return cell;


        return null;
    }

    public void DeleteZone(ZoneGroup group)
    {
        if (group == null)
            return;

        if (!groups.Contains(group))
            return;

        foreach (CellScr cell in group.cells)
        {
            if (cell != null && cell.ownerGroup == group)
            {
                cell.ownerGroup = null;
                cell.UpdateColor();
            }
        }

        groups.Remove(group);

        Debug.Log($"Групу '{group.groupName}' видалено.");

    }
    public ZoneGroup CreateGroup(CellScr a,CellScr b, string groupName = "New Group", Building d = null)
    {
        if (a == null || b == null)
        {
            Debug.LogWarning( "Не можна створити групу: одна з клітинок null.");

            return null;
        }

        if (!cellMap.ContainsKey(a.GetPosition()))
        {
            Debug.LogWarning($"Клітинка '{a.name}' " + $"({a.x}, {a.y}) не зареєстрована.");

            RegisterCell(a);
        }


        if (!cellMap.ContainsKey(b.GetPosition()))
        {
            Debug.LogWarning($"Клітинка '{b.name}' " + $"({b.x}, {b.y}) не зареєстрована.");

            RegisterCell(b);
        }

        int minX = Mathf.Min(a.x, b.x);
        int maxX = Mathf.Max(a.x, b.x);

        int minY = Mathf.Min(a.y, b.y);
        int maxY = Mathf.Max(a.y, b.y);
        // Визначаємо тип будівлі
        Building building = null;

        switch (buildID)
        {
            case 1:
                building = new Workshop();
                break;

            case 2:
                building = new Research();
                break;

            case 3:
                building = new Storage();
                break;

            case 4:
                building = new Office();
                break;

            default:
                Debug.LogWarning($"Невідомий buildID: {buildID}");
                return null;
        }

        if (building is Workshop workshop)
        {
            workshop.gameManager = GameManagerScr.Instance;
        }
        if (building is Workshop || building is Research)
        {
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;

            if (width < 2 || height < 2)
            {
                Debug.LogWarning($"Не можна створити групу: " + $"мінімальний розмір для {building.GetType().Name} — 2x2 " + $"(отримано {width}x{height}).");
                return null;
            }
        }

        List<CellScr> cellsInRect = new List<CellScr>();

        List<CellScr> conflicting = new List<CellScr>();

        List<Vector2Int> missingCells =  new List<Vector2Int>();

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int position =
                    new Vector2Int(x, y);


                if (!cellMap.TryGetValue(
                    position,
                    out CellScr cell))
                {
                    missingCells.Add(position);
                    continue;
                }


                if (cell.ownerGroup != null)
                {
                    conflicting.Add(cell);
                }
                else
                {
                    cellsInRect.Add(cell);
                }
            }
        }


        if (missingCells.Count > 0)
        {
            Debug.LogWarning("Не можна створити групу: " + $"в прямокутнику відсутні " + $"{missingCells.Count} клітинок.\n" + "Відсутні: " + string.Join(", ",missingCells.Select(p => $"({p.x},{p.y})")));

            return null;
        }


        if (conflicting.Count > 0)
        {
            Debug.LogWarning("Не можна створити групу: " + $"{conflicting.Count} клітинок " + "вже належать іншим групам.\n" + "Клітинки: " + string.Join(", ", conflicting.Select( c => $"({c.x},{c.y})")));

            return null;
        }


        if (cellsInRect.Count == 0)
        {
            Debug.LogWarning("Не можна створити групу: " + "вибрана область не містить клітинок.");

            return null;
        }

        ZoneGroup group = new ZoneGroup(groupName, cellsInRect, building);

        foreach (CellScr cell in cellsInRect)
        {
            cell.ownerGroup = group;
            cell.UpdateColor();
        }


        groups.Add(group);


        Debug.Log($"Створено групу '{group.groupName}'"  + "з типом {building.GetType().Name} " + $"і {group.cells.Count} клітинок.");

        return group;
    }

    public bool DeleteGroup(ZoneGroup group)
        {
            if (group == null)
                return false;


            if (!groups.Contains(group))
                return false;


            foreach (CellScr cell in group.cells)
            {
                if (cell != null &&
                    cell.ownerGroup == group)
                {
                    cell.ownerGroup = null;
                }
            }


            groups.Remove(group);

            Debug.Log($"Групу '{group.groupName}' видалено.");

            return true;
        }


        [ContextMenu("Rebuild Cell Map")]
        private void RebuildCellMap()
        {
            FindAllCells();

            Debug.Log($"CellMap перебудовано. " + $"Клітинок: {cellMap.Count}");
        }


        [ContextMenu("Print Cell Map")]
        private void PrintCellMap()
        {
            Debug.Log($"=== CELL MAP ({cellMap.Count}) ===");

            foreach (var pair in cellMap)
            {
                Debug.Log($"({pair.Key.x}, {pair.Key.y}) " + $"-> {pair.Value.name}");
            }
        }
    }


[System.Serializable]
public class ZoneGroup
{
    public string groupName;

    public List<CellScr> cells = new List<CellScr>();

    public Building building;

    public ZoneGroup(string name, List<CellScr> cells, Building building)
    {
        groupName = name;
        this.cells = cells ?? new List<CellScr>();
        this.building = building;
    }

    public int GetCellCount()
    {
        return cells.Count;
    }

    public bool Contains(CellScr cell)
    {
        return cell != null && cells.Contains(cell);
    }

    public Color GetColor()
    {
        if (building is Workshop)
            return new Color(1f, 0.55f, 0.1f);

        if (building is Storage)
            return new Color(0.3f, 0.75f, 1f);

        if (building is Research)
            return new Color(0.55f, 1f, 0.3f);

        if (building is Office)
            return new Color(0.65f, 0.65f, 0.65f);

        return Color.white;
    }
    public void Manufacturing()
    {
        if (building is Workshop workshop)
        {
            workshop.ProductionCycle(cells.Count);
            Debug.Log(workshop.production);
        }


        if (building is Storage)
        {

        }


        if (building is Research)
        {

        }

        if (building is Office)
        {

        }
    }
}