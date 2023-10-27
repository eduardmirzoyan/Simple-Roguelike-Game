using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;

    [Header("References")]
    [SerializeField] private WorldGenerator worldGenerator;

    [Header("Debug")]
    [SerializeField, ReadOnly] private WorldData worldData;
    [SerializeField, ReadOnly] private int roundNumber;
    [SerializeField, ReadOnly] private List<EntityData> turnQueue;
    [SerializeField, ReadOnly] private EntityData currentEntity;
    [SerializeField, ReadOnly] private WeaponData selectedWeapon;

    public static GameManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        turnQueue = new List<EntityData>();
    }

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);

        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        // Generate world
        var player = DataManager.instance.GetPlayer();

        worldData = worldGenerator.Generate(player);

        WorldRenderer.instance.GenerateWorld(worldData);
        CameraManager.instance.Initialize(worldData.player.entityRenderer.transform);

        worldData.player.vision.Refresh();
        foreach (var enemy in worldData.enemies)
            enemy.vision.Refresh();

        FogOfWarRenderer.instance.Initialize(worldData.tiles.GetLength(0));
        FogOfWarRenderer.instance.UpdateFog(worldData.player.vision);

        PlayerMananger.instance.Initialize(worldData.player);

        PlayerActionsUI.instance.Initialize(worldData.player);

        PlayerInspectUI.instance.Initialize(worldData.player);

        TransitionManager.instance.Initialize(worldData.player.entityRenderer.transform);
        yield return new WaitForSeconds(0.5f);
        TransitionManager.instance.OpenScene();

        print($"Starting Level");

        yield return StartRound();
    }

    private IEnumerator StartRound()
    {
        roundNumber++;

        // print($"Round {roundNumber} Start");

        // Generate queue
        GenerateTurnOrder();

        if (turnQueue.Count > 0)
        {
            // Pop from front
            currentEntity = turnQueue[0];
            turnQueue.RemoveAt(0);

            yield return StartTurn(currentEntity);
        }
        else
        {
            // All entities have taken their turn

            // End Round
            yield return EndRound();
        }
    }

    private IEnumerator StartTurn(EntityData entityData)
    {
        //print($"Turn Start: {currentTurnEntity.name}");

        EntityRecoverPosture(entityData);

        GameEvents.instance.TriggerOnTurnStart(entityData);

        if (entityData is PlayerData)
        {
            // Allow player to make choices
            PlayerMananger.instance.AllowInput();
        }
        else if (entityData is EnemyData)
        {
            var enemy = entityData as EnemyData;
            if (enemy.ai != null)
            {
                // Perform action based on AI
                enemy.ai.PerformTurn(entityData as EnemyData, worldData);
            }
            else
            {
                // Skip turn instead
                yield return EndTurn();
            }
        }
        else
        {
            print("UNHANDLED ENTITY");
        }

    }

    private IEnumerator EndTurn()
    {
        //print($"Turn End: {currentTurnEntity.name}");

        EntityReduceCooldowns(currentEntity);

        //PlayerActionsUI.instance.DisableActions();

        GameEvents.instance.TriggerOnTurnEnd(currentEntity);

        // Start Next turn
        if (turnQueue.Count > 0)
        {
            // Pop from front
            currentEntity = turnQueue[0];
            turnQueue.RemoveAt(0);

            yield return StartTurn(currentEntity);
        }
        else
        {
            // All entities have taken their turn
            yield return null;

            // End Round
            yield return EndRound();
        }
    }

    private IEnumerator EndRound()
    {
        // print($"Round {roundNumber} End");

        // Start new round
        yield return StartRound();
    }

    private IEnumerator EndLevel()
    {
        print($"Ending Level");

        // Load new randomly generated level
        TransitionManager.instance.ReloadScene();

        yield return null;
    }

    public void SkipTurn(EntityData entityData)
    {
        if (selectedWeapon != null)
            return;

        print($"{entityData.name} skipped their turn.");

        if (currentEntity is PlayerData)
        {
            PlayerMananger.instance.PreventInput();
        }

        // Trigger event
        GameEvents.instance.TriggerOnActionStart();

        StartCoroutine(EndTurn());
    }

    public bool IsValidMove(EntityData entityData, Vector3Int direction)
    {
        if (direction.magnitude > 1f)
            throw new System.Exception($"MAGNITUDE IS TOO BIG: {direction}");

        if (direction == Vector3Int.zero)
            return false;

        // Calculate new position
        Vector3Int newPosition = entityData.tileData.position + direction;

        // Check bounds
        if (WorldGenerator.OutOfBounds(newPosition, worldData.tiles))
            return false;

        TileData newTile = worldData.tiles[newPosition.x, newPosition.y];

        // Check location
        if (newTile.type == TileType.Wall || newTile.entityData != null)
            return false;

        return true;
    }

    public void MoveEntity(EntityData entityData, Vector3Int direction)
    {
        if (selectedWeapon != null)
            return;

        if (!IsValidMove(entityData, direction))
            return;

        // print($"{entityData.name} moved towards {direction}.");

        if (entityData is PlayerData)
        {
            PlayerMananger.instance.PreventInput();
            // PlayerActionsUI.instance.DisableActions();
        }

        // Trigger event
        GameEvents.instance.TriggerOnActionStart();

        Vector3Int newPosition = entityData.tileData.position + direction;
        TileData newTile = worldData.tiles[newPosition.x, newPosition.y];

        StartCoroutine(MoveToTileOverTime(entityData, newTile, direction));
    }

    private IEnumerator MoveToTileOverTime(EntityData entityData, TileData destinationTile, Vector3Int direction)
    {
        // Handle visuals
        yield return entityData.entityRenderer.MoveOverTime(direction);

        // Set new location for entity
        entityData.tileData.entityData = null;
        destinationTile.entityData = entityData;
        entityData.tileData = destinationTile;

        // Calculate new vision for that location
        entityData.vision.Refresh();

        // if player
        if (entityData is PlayerData)
        {
            // Update vision
            FogOfWarRenderer.instance.UpdateFog(entityData.vision);

            // Display any weapons on the tile
            if (destinationTile.weapon != null)
                WeaponDropUI.instance.Show(destinationTile.weapon);
            else
                WeaponDropUI.instance.Hide();
        }

        // If landed on exit, then leave level
        if (destinationTile.type == TileType.Exit)
            yield return EndLevel();
        // Else end turn
        else
            yield return EndTurn();
    }

    public void EntitySelectAttack(EntityData entityData, int weaponIndex)
    {
        var weapon = entityData.weapons[weaponIndex];
        if (weapon != null)
        {
            // If weapon on cooldown
            if (weapon.cooldownTimer > 0)
            {
                print($"Weapon on cooldown.");
                return;
            }

            // If weapon already selected
            if (selectedWeapon != null)
            {
                // if same weapon
                if (weapon == selectedWeapon)
                {
                    // Deselect
                    EntityCancelAttack(entityData, weaponIndex);

                    selectedWeapon = null;
                }
                else // If different weapons
                {
                    // Deselect
                    EntityCancelAttack(entityData, weaponIndex);

                    // Select new
                    PreviewManager.instance.ShowPreview(entityData, weaponIndex);

                    // Trigger events
                    GameEvents.instance.TriggerOnAttackSelect(entityData.weapons[weaponIndex]);

                    selectedWeapon = weapon;
                }
            }
            else
            {
                // Select new
                PreviewManager.instance.ShowPreview(entityData, weaponIndex);

                // Trigger events
                GameEvents.instance.TriggerOnAttackSelect(entityData.weapons[weaponIndex]);

                selectedWeapon = weapon;
            }
        }
    }

    public void EntityCancelAttack(EntityData entityData, int weaponIndex)
    {
        print($"{entityData.name} cancels the attack.");

        selectedWeapon = null;

        PreviewManager.instance.CancelPreview();

        GameEvents.instance.TriggerOnAttackCancel(entityData.weapons[weaponIndex]);
    }

    public void EntityPerformAttack(EntityData entityData, int weaponIndex, Vector3Int position)
    {
        print($"{entityData.name} attacks the location: {position}.");

        selectedWeapon = null;

        if (entityData is PlayerData)
        {
            PreviewManager.instance.CancelPreview();
            PlayerMananger.instance.PreventInput();
        }

        // Trigger event
        GameEvents.instance.TriggerOnActionStart();

        TileData tileData = worldData.tiles[position.x, position.y];

        StartCoroutine(AttackTileOverTime(entityData, weaponIndex, tileData));
    }

    private IEnumerator AttackTileOverTime(EntityData entityData, int weaponIndex, TileData tileData)
    {
        entityData.canRecoverPosture = false;

        var weapon = entityData.weapons[weaponIndex];

        // Attack with weapon
        weapon.Attack(entityData, tileData);

        // Handle visuals
        yield return entityData.entityRenderer.MeleeOverTime(tileData);

        // Set cooldown timer
        weapon.cooldownTimer = weapon.cooldown;

        // End turn after
        yield return EndTurn();
    }

    public void EntityTakeDamage(EntityData entityData, int amount, EntityData source)
    {
        if (entityData.currentPosture >= amount)
        {
            entityData.currentPosture -= amount;

            print($"{entityData.name} took {amount} posture damage.");
        }
        else
        {
            int leftOver = amount - entityData.currentPosture;
            entityData.currentPosture = 0;

            entityData.currentHealth = Mathf.Max(entityData.currentHealth - leftOver, 0);
            if (entityData.currentHealth == 0)
            {
                // Despawn
                EntityDespawn(entityData);

                return;
            }

            print($"{entityData.name} took {amount - leftOver} posture damage and {leftOver} health damage.");
        }

        entityData.entityRenderer.TakeHit(source.tileData.position);

        // Cannot recover on their coming turn
        entityData.canRecoverPosture = false;

        // Trigger event
        GameEvents.instance.TriggerOnEntityResourceChange(entityData);
    }

    private void EntityDespawn(EntityData entityData)
    {
        print($"{entityData.name} was killed.");

        // Destroy game-object
        entityData.entityRenderer.Die();

        // Remove from tile
        TileData tileData = entityData.tileData;
        tileData.entityData = null;

        if (entityData is PlayerData)
        {
            // If player was killed, reload level
            TransitionManager.instance.ReloadScene();
        }
        else if (entityData is EnemyData)
        {
            var enemy = entityData as EnemyData;
            if (worldData.enemies.Remove(enemy)) { }
            else if (worldData.neutrals.Remove(enemy)) { }

            // Spawn if available
            var loot = enemy.weaponLoot;
            if (loot != null)
            {
                tileData.weapon = loot;
                WorldRenderer.instance.SpawnWeapon(loot, tileData.position);
            }
        }

        // Remove from turn queue
        turnQueue.Remove(entityData);
    }

    private void EntityRecoverPosture(EntityData entityData)
    {
        // Recover if possible
        if (entityData.canRecoverPosture)
        {
            if (entityData.currentPosture < entityData.maxPosture)
            {
                print($"{entityData.name} recovered some posture!");
                entityData.currentPosture++;

                // Trigger event
                GameEvents.instance.TriggerOnEntityResourceChange(entityData);
            }
        }

        // Set true for this round
        entityData.canRecoverPosture = true;
    }

    private void EntityReduceCooldowns(EntityData entityData)
    {
        foreach (var weapon in entityData.weapons)
        {
            if (weapon == null)
                continue;

            if (weapon.cooldownTimer > 0)
            {
                weapon.cooldownTimer--;
            }
        }
    }

    public void EntityChangeWeapon(EntityData entityData, int weaponIndex)
    {
        TileData tileData = entityData.tileData;

        // If there is a weapon on ground
        if (tileData.weapon != null)
        {
            var newWeapon = tileData.weapon;

            // If player already has a weapon
            if (entityData.weapons[weaponIndex] != null)
            {
                var oldWeapon = entityData.weapons[weaponIndex];

                // Drop old weapon
                tileData.weapon = oldWeapon;

                // Create visual
                WorldRenderer.instance.SpawnWeapon(oldWeapon, tileData.position);

                WeaponDropUI.instance.Show(oldWeapon);

                // Trigger event
                GameEvents.instance.TriggerOnWeaponDrop(weaponIndex, oldWeapon, tileData);
            }
            else
            {
                // Remove from ground
                tileData.weapon = null;

                WeaponDropUI.instance.Hide();
            }

            // Pick up new one
            entityData.weapons[weaponIndex] = newWeapon;

            // Trigger event
            GameEvents.instance.TriggerOnWeaponPickup(weaponIndex, newWeapon, tileData);
        }
        else
        {
            // Drop current weapon
            var weapon = entityData.weapons[weaponIndex];
            if (weapon != null)
            {
                tileData.weapon = weapon;
                entityData.weapons[weaponIndex] = null;

                // Update visuals
                WeaponDropUI.instance.Show(weapon);
                WorldRenderer.instance.SpawnWeapon(weapon, tileData.position);

                // Trigger event
                GameEvents.instance.TriggerOnWeaponDrop(weaponIndex, weapon, tileData);
            }
        }
    }


    // ~~~~~~~~~~~~~~HELPER FUNCTIONS HERE ~~~~~~~~~~~~~~~~~~


    private void GenerateTurnOrder()
    {
        // First add player
        turnQueue.Add(worldData.player);
        // Then Add enemies 
        turnQueue.AddRange(worldData.enemies);
    }
}
