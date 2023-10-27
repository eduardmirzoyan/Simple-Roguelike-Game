using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    [Header("Settings")]
    [SerializeField] private int numLinePoints;

    [Header("Debug")]
    [SerializeField, ReadOnly] private EntityData entityData;
    [SerializeField, ReadOnly] private int weaponIndex;
    [SerializeField, ReadOnly] private Vector3Int previousPosition;
    [SerializeField, ReadOnly] private bool isPreviewing;

    private Dictionary<Vector3Int, int> validPositions;

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

        validPositions = new Dictionary<Vector3Int, int>();
    }

    private void Update()
    {
        if (!isPreviewing)
            return;

        var mouseCellPosition = PlayerMananger.instance.GetMousePosition();

        // If in range of valid tiles
        if (validPositions.ContainsKey(mouseCellPosition))
        {
            // Different position
            if (mouseCellPosition != previousPosition)
            {
                selectTilemap.SetTile(previousPosition, null);
                selectTilemap.SetTile(mouseCellPosition, selectTile);
                previousPosition = mouseCellPosition;

                DrawLine(entityData.tileData.position, mouseCellPosition);

                // Preview damage
                PreviewDamage(mouseCellPosition);
            }
        }
        else // If not in range
        {
            mouseCellPosition = Vector3Int.back;

            if (mouseCellPosition != previousPosition)
            {
                DamagePreviewUI.instance.Hide();
                lineRenderer.enabled = false;
                selectTilemap.SetTile(previousPosition, null);
                previousPosition = mouseCellPosition;
            }
        }

        if (Input.GetMouseButtonDown(0)) // Left click to confirm
        {
            if (previousPosition != Vector3Int.back)
            {
                // Attack
                GameManager.instance.EntityPerformAttack(entityData, weaponIndex, previousPosition);
            }

        }
        else if (Input.GetMouseButtonDown(1)) // Right click to cancel
        {
            // Cancel
            GameManager.instance.EntityCancelAttack(entityData, weaponIndex);
        }
    }

    private void PreviewDamage(Vector3Int position)
    {
        var tiles = entityData.worldData.tiles;

        if (tiles[position.x, position.y].entityData != null)
        {
            var weapon = entityData.weapons[weaponIndex];

            // Change color
            if (weapon.bonusDamage > 0)
            {
                // TODO?
            }
            else
            {
                // TODO?
            }

            var worldPositon = selectTilemap.GetCellCenterWorld(position);
            DamagePreviewUI.instance.Show(weapon.totalDamge, worldPositon);
        }
        else
        {
            DamagePreviewUI.instance.Hide();
        }

    }

    public void ShowPreview(EntityData entityData, int weaponIndex)
    {
        this.entityData = entityData;
        this.weaponIndex = weaponIndex;

        // Range based on weapon
        int range = entityData.weapons[weaponIndex].range;

        var tiles = entityData.worldData.tiles;

        Vector3Int startPosition = entityData.tileData.position;
        foreach (var position in entityData.vision.visiblePositions.Keys)
        {
            // If in range
            if (Pathfinder.ManhattanDistance(startPosition, position) <= range)
            {
                // If not a wall
                if (tiles[position.x, position.y].type != TileType.Wall)
                {
                    previewTilemap.SetTile(position, previewTile);
                    validPositions[position] = 1;
                }
            }
        }

        isPreviewing = true;
    }

    public void CancelPreview()
    {
        ResetPreview();
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

        lineRenderer.enabled = true;
    }

    private void ResetPreview()
    {
        DamagePreviewUI.instance.Hide();
        entityData = null;
        weaponIndex = -1;
        previousPosition = Vector3Int.back;
        lineRenderer.enabled = false;
        validPositions.Clear();
        previewTilemap.ClearAllTiles();
        selectTilemap.ClearAllTiles();
        isPreviewing = false;
    }
}
