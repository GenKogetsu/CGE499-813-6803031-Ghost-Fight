using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private SpatialGrid grid;
    [SerializeField] private float interval = 3.5f;

    private float _timer;

    void Start()
    {
        if (grid == null) grid = FindObjectOfType<SpatialGrid>();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= interval) { _timer = 0f; Spawn(); }
    }

    void Spawn()
    {
        if (ghostPrefab == null || grid == null) return;

        Vector2? pos = grid.GetRandomFreeSpawnPos(exclusionRadius: 1);
        if (pos == null) return;

        Instantiate(ghostPrefab, pos.Value, Quaternion.identity);
    }
}
