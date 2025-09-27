using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ForestEnvironmentGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PathGenerator pathGenerator;  // Reference to your PathGenerator
    [SerializeField] private Tilemap groundTilemap;        // Tilemap for ground/grass/path
    [SerializeField] private Tilemap detailTilemap;        // Tilemap for trees/objects

    [Header("Tiles")]
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase pathTile;            // 👈 New: Tile for the path itself

    [Header("Decoration Settings")]
    [SerializeField] private List<TileBase> decorationTiles;
    [SerializeField, Range(0f, 1f)] private float decorationSpawnChance = 0.1f;

    [Header("World Bounds")]
    [SerializeField] private int worldWidth = 100;
    [SerializeField] private int worldHeight = 40;

    [SerializeField] private bool generateOnStart = true;

    private HashSet<Vector3Int> pathPositions = new HashSet<Vector3Int>();

    private void Start()
    {
        if (generateOnStart)
            GenerateEnvironment();
    }

    [ContextMenu("Generate Environment Now")]
    public void GenerateEnvironment()
    {
        if (pathGenerator == null || groundTilemap == null || grassTile == null || pathTile == null)
        {
            Debug.LogError("⚠️ Missing references on EnvironmentGenerator!");
            return;
        }

        groundTilemap.ClearAllTiles();
        detailTilemap?.ClearAllTiles();
        pathPositions.Clear();

        // ✅ Collect all path positions from PathGenerator
        foreach (Vector3 worldPos in pathGenerator.worldPathPoints)
        {
            Vector3Int cell = groundTilemap.WorldToCell(worldPos);
            pathPositions.Add(cell);
            pathPositions.Add(cell + Vector3Int.right);
            pathPositions.Add(cell + Vector3Int.left);
            pathPositions.Add(cell + Vector3Int.up);
            pathPositions.Add(cell + Vector3Int.down);
        }

        // 🌱 Paint environment (grass + decorations)
        for (int x = -worldWidth / 2; x < worldWidth / 2; x++)
        {
            for (int y = -worldHeight / 2; y < worldHeight / 2; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                // Paint base ground tile
                groundTilemap.SetTile(pos, grassTile);

                // Optional: spawn decoration if not path
                if (!pathPositions.Contains(pos) && decorationTiles.Count > 0 && Random.value < decorationSpawnChance)
                {
                    TileBase deco = decorationTiles[Random.Range(0, decorationTiles.Count)];
                    detailTilemap?.SetTile(pos, deco);
                }
            }
        }

        // 🪵 Finally: Paint the path itself
        foreach (var pos in pathPositions)
        {
            groundTilemap.SetTile(pos, pathTile);
        }

        Debug.Log("🌿 Environment generated and path painted!");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawCube(Vector3.zero, new Vector3(worldWidth, worldHeight, 0.1f));
    }
}