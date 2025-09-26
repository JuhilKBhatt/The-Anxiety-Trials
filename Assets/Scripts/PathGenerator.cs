using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    [SerializeField] private Tilemap pathTilemap;

    [Header("Path Tiles")]
    [SerializeField] private TileBase startTile;
    [SerializeField] private TileBase endTile;
    [SerializeField] private TileBase pathTile;
    [SerializeField] private List<TileBase> detailTiles; // Decorative detail tiles

    [Header("Path Generation Settings")]
    [SerializeField] private Vector2Int startPos = new Vector2Int(0, 0);
    [SerializeField] private int pathLength = 50;
    [SerializeField, Range(0f, 1f)] private float turnChance = 0.3f;
    [SerializeField, Range(0f, 1f)] private float detailChance = 0.15f;
    [SerializeField] private bool generateOnStart = true;

    [Header("Path Appearance")]
    [SerializeField] private int pathWidth = 2; // how wide the path is

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    public List<Vector3> worldPathPoints { get; private set; } = new List<Vector3>();

    private void Start()
    {
        if (generateOnStart)
            GeneratePath();
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
        Vector2Int direction = Vector2Int.right;

        for (int i = 0; i < pathLength; i++)
        {
            // Choose the main tile based on position
            TileBase mainTile = pathTile;
            if (i == 0 && startTile != null) mainTile = startTile;
            else if (i == pathLength - 1 && endTile != null) mainTile = endTile;

            // Paint path width
            Vector2Int perpendicular = new Vector2Int(-direction.y, direction.x);
            for (int w = -pathWidth / 2; w < (pathWidth + 1) / 2; w++)
            {
                Vector2Int tilePos = currentPos + perpendicular * w;
                pathTilemap.SetTile((Vector3Int)tilePos, mainTile);

                // Maybe add detail tile
                if (detailTiles.Count > 0 && Random.value < detailChance)
                {
                    TileBase randomDetail = detailTiles[Random.Range(0, detailTiles.Count)];
                    pathTilemap.SetTile((Vector3Int)tilePos, randomDetail);
                }

                // Save center path point (for auto-follow) - only center
                if (w == 0)
                {
                    Vector3 worldPos = pathTilemap.CellToWorld((Vector3Int)tilePos) + pathTilemap.tileAnchor;
                    worldPathPoints.Add(worldPos);
                }
            }

            // Turn occasionally
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

            currentPos += direction;
        }

        Debug.Log($"✅ Path generated ({pathLength} tiles, width {pathWidth}).");
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