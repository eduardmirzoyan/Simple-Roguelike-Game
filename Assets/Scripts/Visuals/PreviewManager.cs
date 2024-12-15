using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PreviewManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap previewTilemap;
    [SerializeField] private Tilemap selectTilemap;
    [SerializeField] private RuleTile previewTile;
    [SerializeField] private Tile selectTile;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject damagePreviewPrefab;

    [Header("Settings")]
    [SerializeField] private int numLinePoints;

    [Header("Debug")]
    [SerializeField, ReadOnly] private EntityData entityData;
    [SerializeField, ReadOnly] private int weaponIndex;
    [SerializeField, ReadOnly] private Vector3Int previousPosition;
    [SerializeField, ReadOnly] private bool isPreviewing;

    private HashSet<Vector3Int> validPositions;
    private List<DamagePreview> previews;
    private const int LEFT_CLICK = 0, RIGHT_CLIK = 1;

    public static PreviewManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        validPositions = new();
        previews = new();
    }

    private void Update()
    {
        if (!isPreviewing)
            return;

        var currentPosition = PlayerMananger.instance.GetMousePosition();

        // === THIS LOGIC SHOULD BE MOVED TO PLAYER MANAGER ===
        if (Input.GetMouseButtonDown(LEFT_CLICK)) // Left click to confirm
        {
            if (currentPosition != Vector3Int.back)
            {
                // Attack
                GameManager.instance.EntityPerformAttack(entityData, weaponIndex, currentPosition);
            }
        }
        else if (Input.GetMouseButtonDown(RIGHT_CLIK)) // Right click to cancel
        {
            // Cancel
            GameManager.instance.EntityCancelAttack(entityData, weaponIndex);
        }
        // ===

        // If same position then do nothing
        if (currentPosition == previousPosition)
            return;

        // If in range of valid tiles (and not itself)
        if (validPositions.Contains(currentPosition) && currentPosition != entityData.Position)
        {
            var weapon = entityData.weapons[weaponIndex];
            DrawPreviewArea(currentPosition, entityData, weapon);
        }
        else // If not, then clear
        {
            ClearPreviewArea();
        }

        // Update previous
        previousPosition = currentPosition;
    }

    public void ShowPreview(EntityData entityData, int weaponIndex)
    {
        this.entityData = entityData;
        this.weaponIndex = weaponIndex;
        previousPosition = Vector3Int.back;

        // Range based on weapon
        int range = entityData.weapons[weaponIndex].range;
        var tiles = entityData.worldData.tiles;

        Vector3Int startPosition = entityData.Position;
        foreach (var position in entityData.vision.visiblePositions.Keys)
        {
            // If in range
            if (Pathfinder.ManhattanDistance(startPosition, position) <= range)
            {
                // If not a wall
                if (tiles[position.x, position.y].type != TileType.Wall)
                {
                    previewTilemap.SetTile(position, previewTile);
                    validPositions.Add(position);
                }
            }
        }

        isPreviewing = true;
    }

    public void CancelPreview()
    {
        entityData = null;
        weaponIndex = -1;
        previousPosition = Vector3Int.back;
        validPositions.Clear();
        previewTilemap.ClearAllTiles();
        ClearPreviewArea();
        isPreviewing = false;
    }

    #region Helpers

    private void DrawPreviewArea(Vector3Int position, EntityData entityData, WeaponData weaponData)
    {
        // Highlight affected area
        var targets = weaponData.CalculateArea(entityData, position);
        foreach (var target in targets)
            selectTilemap.SetTile(target, selectTile);

        // Draw to target
        lineRenderer.enabled = true;
        DrawLine(entityData.Position, position);

        // Preview damage at targets
        ShowDamagePreview(entityData, weaponData, targets);
    }

    private void ClearPreviewArea()
    {
        selectTilemap.ClearAllTiles();
        lineRenderer.enabled = false;
        ClearDamagePreview();
    }

    private void DrawLine(Vector3Int start, Vector3Int end)
    {
        Vector3 startWorld = selectTilemap.GetCellCenterWorld(start);
        Vector3 endWorld = selectTilemap.GetCellCenterWorld(end);

        lineRenderer.positionCount = numLinePoints;
        for (int i = 0; i < numLinePoints; i++)
        {
            Vector3 worldPosition = Vector3.Lerp(startWorld, endWorld, (float)i / numLinePoints);
            lineRenderer.SetPosition(i, worldPosition);
        }
    }

    private void ShowDamagePreview(EntityData entityData, WeaponData weaponData, Vector3Int[] positions)
    {
        var tiles = entityData.worldData.tiles;

        foreach (var position in positions)
        {
            var targetData = tiles[position.x, position.y].entityData;
            if (targetData != null)
            {
                var worldPositon = selectTilemap.GetCellCenterWorld(position);
                var trueRange = weaponData.GetTrueDamageRange(entityData);

                // Check if empowered
                Color color = trueRange.x > weaponData.damageRange.x ? Color.yellow : Color.white;

                var preview = Instantiate(damagePreviewPrefab, worldPositon, Quaternion.identity).GetComponent<DamagePreview>();
                preview.Initialize(trueRange, color);

                previews.Add(preview);
            }
        }
    }

    private void ClearDamagePreview()
    {
        foreach (var preview in previews)
        {
            Destroy(preview.gameObject);
        }
        previews.Clear();
    }

    #endregion
}
