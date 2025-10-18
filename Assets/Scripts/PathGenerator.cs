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
    [SerializeField] private List<TileBase> detailTiles;

    [Header("Path Generation Settings")]
    [SerializeField] private Vector2Int startPos = Vector2Int.zero;
    [SerializeField] private int pathLength = 50;
    [SerializeField, Range(0f, 1f)] private float turnChance = 0.2f;
    [SerializeField, Range(0f, 1f)] private float detailChance = 0.15f;
    [SerializeField] private bool generateOnStart = true;

    [Header("Path Appearance")]
    [SerializeField] private int pathWidth = 2;
    [SerializeField] private int minTilesBetweenTurns = 3; // new

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
        Vector2Int direction = Vector2Int.right; // initial direction
        int tilesUntilNextTurn = minTilesBetweenTurns;

        for (int i = 0; i < pathLength; i++)
        {
            // Select main tile
            TileBase mainTile = pathTile;
            if (i == 0 && startTile != null) mainTile = startTile;
            else if (i == pathLength - 1 && endTile != null) mainTile = endTile;

            // Paint path width
            Vector2Int perpendicular = new Vector2Int(-direction.y, direction.x);
            for (int w = -pathWidth / 2; w < (pathWidth + 1) / 2; w++)
            {
                Vector2Int tilePos = currentPos + perpendicular * w;
                pathTilemap.SetTile((Vector3Int)tilePos, mainTile);

                // Possibly add decorative detail
                if (detailTiles.Count > 0 && Random.value < detailChance)
                {
                    TileBase randomDetail = detailTiles[Random.Range(0, detailTiles.Count)];
                    pathTilemap.SetTile((Vector3Int)tilePos, randomDetail);
                }

                // Save center for auto-follow
                if (w == 0)
                {
                    Vector3 worldPos = pathTilemap.CellToWorld((Vector3Int)tilePos) + pathTilemap.tileAnchor;
                    worldPathPoints.Add(worldPos);
                }
            }

            // Handle turning only if enough tiles have passed
            if (tilesUntilNextTurn <= 0 && Random.value < turnChance)
            {
                // Rotate left or right 90°
                direction = Random.value < 0.5f ? new Vector2Int(-direction.y, direction.x) : new Vector2Int(direction.y, -direction.x);
                tilesUntilNextTurn = minTilesBetweenTurns; // reset counter
            }
            else
            {
                tilesUntilNextTurn--;
            }

            currentPos += direction;
        }

        Debug.Log($"✅ Smooth Path generated ({pathLength} tiles, width {pathWidth}).");
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