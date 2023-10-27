using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapBoundaryManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap tilemap;

    [Header("Settings")]
    [SerializeField] private Vector2Int boundaryOffset = Vector2Int.one;

    [Header("Debugging")]
    [SerializeField, ReadOnly] private PolygonCollider2D boundaryCollider;

    public static MapBoundaryManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void GenerateBoundary()
    {
        // Create a child to hold boundary
        var boundary = new GameObject("Boundary");
        boundary.transform.parent = transform;
        boundaryCollider = boundary.AddComponent<PolygonCollider2D>();

        // Resize collider based on tilemap
        tilemap.CompressBounds();
        Bounds bounds = tilemap.localBounds;

        // Define the points of the collider's new shape based on the Tilemap's bounds.
        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(bounds.min.x + boundaryOffset.x, bounds.min.y + boundaryOffset.y);
        points[1] = new Vector2(bounds.min.x + boundaryOffset.x, bounds.max.y - boundaryOffset.y);
        points[2] = new Vector2(bounds.max.x - boundaryOffset.x, bounds.max.y - boundaryOffset.y);
        points[3] = new Vector2(bounds.max.x - boundaryOffset.x, bounds.min.y + boundaryOffset.y);

        // Set the points for the Polygon Collider 2D.
        boundaryCollider.SetPath(0, points);
    }

    public Collider2D GetBoundingCollider()
    {
        return boundaryCollider;
    }
}
