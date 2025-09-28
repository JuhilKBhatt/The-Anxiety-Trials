using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TallDecoration
{
    public TileBase bottomTile;
    public TileBase topTile;
}

public class ForestEnvironmentGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PathGenerator pathGenerator;  
    [SerializeField] private Tilemap groundTilemap;        
    [SerializeField] private Tilemap detailTilemap;        

    [Header("Tiles")]
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase pathTile;

    [Header("Decoration Settings - Single Tile")]
    [SerializeField] private List<TileBase> decorationTiles;
    [SerializeField, Range(0f, 1f)] private float decorationSpawnChance = 0.1f;

    [Header("Decoration Settings - Tall Objects (2-Tile)")]
    [SerializeField] private List<TallDecoration> tallDecorations;
    [SerializeField, Range(0f, 1f)] private float tallDecorationSpawnChance = 0.05f;

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

                // Skip decoration if it's a path tile
                if (pathPositions.Contains(pos)) continue;

                // 🌳 Try spawning a tall decoration
                if (tallDecorations.Count > 0 && Random.value < tallDecorationSpawnChance)
                {
                    Vector3Int topPos = pos + Vector3Int.up;

                    // ✅ Check that both bottom and top are empty and not part of the path
                    if (!pathPositions.Contains(topPos) &&
                        detailTilemap.GetTile(pos) == null &&
                        detailTilemap.GetTile(topPos) == null)
                    {
                        TallDecoration tall = tallDecorations[Random.Range(0, tallDecorations.Count)];
                        if (tall.bottomTile != null)
                            detailTilemap.SetTile(pos, tall.bottomTile);
                        if (tall.topTile != null)
                            detailTilemap.SetTile(topPos, tall.topTile);
                    }
                }
                // 🌼 Otherwise try spawning a single-tile decoration
                else if (decorationTiles.Count > 0 && Random.value < decorationSpawnChance)
                {
                    // ✅ Only place if the tile is free
                    if (detailTilemap.GetTile(pos) == null)
                    {
                        TileBase deco = decorationTiles[Random.Range(0, decorationTiles.Count)];
                        detailTilemap.SetTile(pos, deco);
                    }
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