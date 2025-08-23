using System.Collections.Generic;
using UnityEngine;
using BooterAndBigARM.World;
using BooterAndBigARM.World.Decor;

public class ChunkManager : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;          // ← assign your player here
    public GameObject chunkPrefab;    // ← prefab with MeshFilter + MeshRenderer + Chunk
    public WorldConfig config;        // ← your WorldConfig asset

    [Header("Debug")]
    [SerializeField] bool showHUD = true;

    // runtime
    private readonly Dictionary<Vector2Int, Chunk> _loaded = new Dictionary<Vector2Int, Chunk>();
    private readonly List<Vector2Int> _scratch = new List<Vector2Int>();  // temp set for desired coords
    private readonly List<Vector2Int> _toRemove = new List<Vector2Int>(); // temp list for unloads

    // cached helpers
    private int ChunkSize    => config ? config.scale.chunkSize : 16;
    private float TileSize   => config ? config.scale.tileSize : 1f;  // if your Scale has tileSize
    private float ChunkWorld => ChunkSize * TileSize;
    private int ViewRadius   => Mathf.Max(1, config ? config.streaming.viewRadius : 1);

    private void Update()
    {
        if (!player || !chunkPrefab || !config)
            return;

        // 1) Which chunk is the player in?  (XZ ground; Y is up)
        int pcx = Mathf.FloorToInt(player.position.x / ChunkWorld);
        int pcz = Mathf.FloorToInt(player.position.z / ChunkWorld);
        var center = new Vector2Int(pcx, pcz);

        // 2) Build the exact square set of coords we want this frame
        _scratch.Clear();
        for (int dz = -ViewRadius; dz <= ViewRadius; dz++)
        for (int dx = -ViewRadius; dx <= ViewRadius; dx++)
            _scratch.Add(new Vector2Int(pcx + dx, pcz + dz));

        // 3) UNLOAD: remove anything not in desired set (square check: OR, not Manhattan)
        _toRemove.Clear();
        foreach (var kv in _loaded)
        {
            var c = kv.Key;
            int dx = Mathf.Abs(c.x - pcx);
            int dz = Mathf.Abs(c.y - pcz);
            if (dx > ViewRadius || dz > ViewRadius)  // strict square ring
                _toRemove.Add(c);
        }
        foreach (var coord in _toRemove)
        {
            Destroy(_loaded[coord].gameObject);
            _loaded.Remove(coord);
            if (showHUD) { /* Debug.Log($"Unloaded chunk at: {coord}"); */ }
        }

        // 4) LOAD: spawn any coord in desired set that we don't have yet (closest first)
        _scratch.Sort((a, b) =>
        {
            var da = (a - center).sqrMagnitude;
            var db = (b - center).sqrMagnitude;
            return da.CompareTo(db);
        });

        foreach (var coord in _scratch)
        {
            if (_loaded.ContainsKey(coord)) continue;
            SpawnChunk(coord);
        }

        // 5) Final hard clamp (paranoia): delete anything outside radius if it slipped in
        HardClamp(center);
    }

    void HardClamp(Vector2Int center)
    {
        var toRemove = new List<Vector2Int>();
        foreach (var kv in _loaded)
        {
            var c = kv.Key;
            int dx = Mathf.Abs(c.x - center.x);
            int dz = Mathf.Abs(c.y - center.y);
            if (dx > ViewRadius || dz > ViewRadius)
                toRemove.Add(c);
        }
        foreach (var coord in toRemove)
        {
            Destroy(_loaded[coord].gameObject);
            _loaded.Remove(coord);
            if (showHUD) { /* Debug.Log($"Hard clamped chunk at: {coord}"); */ }
        }
    }

    private void SpawnChunk(Vector2Int coord)
    {
        if (!chunkPrefab) { /* Debug.LogError("Chunk prefab is not assigned."); */ return; }

        var go = Instantiate(chunkPrefab, Vector3.zero, Quaternion.identity, transform);

        var chunk = go.GetComponent<Chunk>();
        if (!chunk)
        {
            /* Debug.LogError("Chunk prefab must have a Chunk component."); */
            Destroy(go);
            return;
        }

        // Optional: assign terrain material from config
        var mr = go.GetComponent<MeshRenderer>();
        if (mr && config.rendering.terrainMaterial)
            mr.sharedMaterial = config.rendering.terrainMaterial;

        // Create a flat ChunkData matching your current Chunk/ChunkData API
        // NOTE: using your constructor ChunkData(coord, size) and tileHeights[,] like you showed.
        var chunkData = new ChunkData(coord, ChunkSize);
        for (int x = 0; x < chunkData.size; x++)
            for (int z = 0; z < chunkData.size; z++)
                chunkData.tileHeights[x, z] = 0;

        // Initialize places the mesh at (coord.x * size * tileSize, 0, coord.y * size * tileSize)
        // Your current Initialize multiplies only by size; we pass tileSize via Chunk to keep math consistent at 1f.
        chunk.Initialize(coord, chunkData);

        // Debug.Log("ChunkManager: Calling decorators...");
        var decorators = GetComponents(typeof(MonoBehaviour));
        foreach (var deco in decorators)
        {
            if (deco is IChunkDecorator decorator)
            {
                // Debug.Log($"ChunkManager: Calling Decorate on {deco.GetType().Name}");
                decorator.Decorate(chunk);
            }
        }
        // Debug.Log("ChunkManager: Finished calling decorators.");

        _loaded[coord] = chunk;
        if (showHUD) { /* Debug.Log($"Loaded chunk at: {coord}"); */ }
    }
}