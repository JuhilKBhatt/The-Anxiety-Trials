using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    [SerializeField] private Tilemap pathTilemap;       // Assign your Tilemap here
    [SerializeField] private TileBase pathTile;         // Assign your ground/tile sprite

    [Header("Path Generation Settings")]
    [SerializeField] private Vector2Int startPos = new Vector2Int(0, 0);
    [SerializeField] private int pathLength = 50;
    [SerializeField, Range(0f, 1f)] private float turnChance = 0.3f; // How often the path curves
    [SerializeField] private bool generateOnStart = true;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    // Stores the world positions of the path tiles
    public List<Vector3> worldPathPoints { get; private set; } = new List<Vector3>();

    private void Start()
    {
        if (generateOnStart)
        {
            GeneratePath();
        }
    }

    [ContextMenu("Generate Path Now")]
    public void GeneratePath()
    {
        if (pathTilemap == null || pathTile == null)
        {
            Debug.LogError("⚠️ PathTilemap or PathTile not assigned!");
            return;
        }

        pathTilemap.ClearAllTiles();
        worldPathPoints.Clear();

        Vector2Int currentPos = startPos;
        Vector2Int direction = Vector2Int.right; // path starts going right

        for (int i = 0; i < pathLength; i++)
        {
            // Paint the tile
            pathTilemap.SetTile((Vector3Int)currentPos, pathTile);

            // Store world position (for player auto-follow)
            Vector3 worldPos = pathTilemap.CellToWorld((Vector3Int)currentPos) + pathTilemap.tileAnchor;
            worldPathPoints.Add(worldPos);

            // Randomly turn slightly
            float rand = Random.value;
            if (rand < turnChance / 2f)
            {
                direction = Vector2Int.up;
            }
            else if (rand < turnChance)
            {
                direction = Vector2Int.down;
            }
            else
            {
                direction = Vector2Int.right;
            }

            // Step forward
            currentPos += direction;
        }

        Debug.Log($"✅ Path generated with {pathLength} tiles.");
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || worldPathPoints == null) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < worldPathPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(worldPathPoints[i], worldPathPoints[i + 1]);
        }
    }
}