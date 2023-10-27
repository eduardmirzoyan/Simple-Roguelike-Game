using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMananger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap tilemap;

    [Header("Data")]
    [SerializeField, ReadOnly] private bool allowInput;
    [SerializeField, ReadOnly] private PlayerData playerData;
    [SerializeField, ReadOnly] private Vector3Int currCellPosition;

    public static KeyCode[] weaponKeyCodeMap = new KeyCode[] { KeyCode.F, KeyCode.G, KeyCode.H };


    public static PlayerMananger instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void Initialize(PlayerData playerData)
    {
        this.playerData = playerData;
        allowInput = false;
        currCellPosition = Vector3Int.back;
    }

    public Vector3Int GetMousePosition()
    {
        return currCellPosition;
    }

    private void Update()
    {
        if (playerData == null)
            return;

        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mouseCellPosition = tilemap.WorldToCell(mouseWorldPosition);

        if (mouseCellPosition != currCellPosition)
        {
            if (WorldGenerator.OutOfBounds(mouseCellPosition, playerData.worldData.tiles))
                return;

            var tileData = playerData.worldData.tiles[mouseCellPosition.x, mouseCellPosition.y];

            EnemyInspectManager.instance.InspectTile(tileData);

            currCellPosition = mouseCellPosition;
        }

        if (!allowInput)
            return;

        // ~~~~~ Mouse input ~~~~~

        if (Input.GetMouseButtonDown(0)) // Left click
        {
            // TODO?
        }
        else if (Input.GetMouseButtonDown(1)) // Right click
        {
            // TODO?
        }

        // ~~~~~ Keyboard input ~~~~~

        HandleSkipInput();

        HandleMoveInput();

        HandleAttackInput();
    }

    private void HandleSkipInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.SkipTurn(playerData);
        }
    }

    private void HandleMoveInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameManager.instance.MoveEntity(playerData, Vector3Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            GameManager.instance.MoveEntity(playerData, Vector3Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.instance.MoveEntity(playerData, Vector3Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            GameManager.instance.MoveEntity(playerData, Vector3Int.right);
        }
    }

    private void HandleAttackInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Shift + F
            if (Input.GetKeyDown(weaponKeyCodeMap[0]))
            {
                GameManager.instance.EntityChangeWeapon(playerData, 0);
            }
            // Shift + G
            else if (Input.GetKeyDown(weaponKeyCodeMap[1]))
            {
                GameManager.instance.EntityChangeWeapon(playerData, 1);
            }
        }
        else
        {
            if (Input.GetKeyDown(weaponKeyCodeMap[0]))
            {
                GameManager.instance.EntitySelectAttack(playerData, 0);
            }
            else if (Input.GetKeyDown(weaponKeyCodeMap[1]))
            {
                GameManager.instance.EntitySelectAttack(playerData, 1);
            }
        }
    }

    public void AllowInput()
    {
        allowInput = true;
    }

    public void PreventInput()
    {
        allowInput = false;
        currCellPosition = Vector3Int.back;
    }
}
