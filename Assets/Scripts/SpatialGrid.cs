using UnityEngine;

public class SpatialGrid : MonoBehaviour
{
    [SerializeField] private RoomSetup _room;
    [SerializeField] public  float     cellSize   = 2f;
    [SerializeField] private float     wallMargin = 0.5f;

    public int     GridWidth  { get; private set; }
    public int     GridHeight { get; private set; }
    public Vector2 Offset     { get; private set; }

    private Transform _player;

    void Awake()
    {
        if (_room == null) _room = FindObjectOfType<RoomSetup>();
        BuildFromRoom();
    }

    void Start() => _player = GameObject.FindWithTag("Player")?.transform;

    void BuildFromRoom()
    {
        if (_room == null)
        {
            GridWidth  = 9;
            GridHeight = 5;
            Offset     = new Vector2(-9f, -5f);
            return;
        }

        float hw = _room.HalfW - wallMargin;
        float hh = _room.HalfH - wallMargin;

        GridWidth  = Mathf.Max(1, Mathf.FloorToInt(hw * 2f / cellSize));
        GridHeight = Mathf.Max(1, Mathf.FloorToInt(hh * 2f / cellSize));

        Offset = new Vector2(
            -(GridWidth  * cellSize) * 0.5f,
            -(GridHeight * cellSize) * 0.5f);
    }

    public Vector2Int WorldToCell(Vector2 world)
    {
        return new Vector2Int(
            Mathf.FloorToInt((world.x - Offset.x) / cellSize),
            Mathf.FloorToInt((world.y - Offset.y) / cellSize));
    }

    public Vector2 CellCenter(Vector2Int cell)
    {
        return new Vector2(
            cell.x * cellSize + cellSize * 0.5f + Offset.x,
            cell.y * cellSize + cellSize * 0.5f + Offset.y);
    }

    public bool IsCellNearPlayer(Vector2Int cell, int radius = 1)
    {
        if (_player == null) return false;
        Vector2Int pc = WorldToCell(_player.position);
        return Mathf.Abs(cell.x - pc.x) <= radius &&
               Mathf.Abs(cell.y - pc.y) <= radius;
    }

    public Vector2? GetRandomFreeSpawnPos(int exclusionRadius = 1)
    {
        for (int i = 0; i < 30; i++)
        {
            int x    = Random.Range(0, GridWidth);
            int y    = Random.Range(0, GridHeight);
            var cell = new Vector2Int(x, y);
            if (!IsCellNearPlayer(cell, exclusionRadius))
                return CellCenter(cell);
        }
        return null;
    }

    void Update()
    {
        if (_player == null) return;
        Vector2Int pc = WorldToCell(_player.position);

        for (int x = 0; x <= GridWidth; x++)
        {
            float wx = x * cellSize + Offset.x;
            Debug.DrawLine(
                new Vector3(wx, Offset.y, 0),
                new Vector3(wx, GridHeight * cellSize + Offset.y, 0),
                new Color(0.4f, 0.9f, 0.4f, 0.5f));
        }
        for (int y = 0; y <= GridHeight; y++)
        {
            float wy = y * cellSize + Offset.y;
            Debug.DrawLine(
                new Vector3(Offset.x, wy, 0),
                new Vector3(GridWidth * cellSize + Offset.x, wy, 0),
                new Color(0.4f, 0.9f, 0.4f, 0.5f));
        }

        for (int dx = -1; dx <= 1; dx++)
        for (int dy = -1; dy <= 1; dy++)
            DrawCellOutline(new Vector2Int(pc.x + dx, pc.y + dy), Color.red);
    }

    void DrawCellOutline(Vector2Int cell, Color color)
    {
        if (cell.x < 0 || cell.x >= GridWidth || cell.y < 0 || cell.y >= GridHeight) return;
        float x0 = cell.x * cellSize + Offset.x, y0 = cell.y * cellSize + Offset.y;
        float x1 = x0 + cellSize,                y1 = y0 + cellSize;
        Debug.DrawLine(new Vector3(x0, y0, 0), new Vector3(x1, y0, 0), color);
        Debug.DrawLine(new Vector3(x1, y0, 0), new Vector3(x1, y1, 0), color);
        Debug.DrawLine(new Vector3(x1, y1, 0), new Vector3(x0, y1, 0), color);
        Debug.DrawLine(new Vector3(x0, y1, 0), new Vector3(x0, y0, 0), color);
    }
}
