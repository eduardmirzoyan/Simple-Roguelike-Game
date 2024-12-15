using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;

    [Header("References")]
    [SerializeField] private WorldGenerator worldGenerator;
    [SerializeField] private GameSpeedData gameSpeedData;

    [Header("Debug")]
    [SerializeField, ReadOnly] private WorldData worldData;
    [SerializeField, ReadOnly] private int roundNumber;
    [SerializeField, ReadOnly] private List<EntityData> turnQueue;
    [SerializeField, ReadOnly] private EntityData currentEntity;
    [SerializeField, ReadOnly] private WeaponData selectedWeapon;

    [Header("Logging")]
    [SerializeField] private bool logGameStates;
    [SerializeField] private bool logEntityActions;

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

    #region Game Loop

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

        FogOfWarRenderer.instance.Initialize(worldData.Size);
        FogOfWarRenderer.instance.UpdateFog(worldData.player.vision);

        PlayerMananger.instance.Initialize(worldData.player);

        GameEvents.instance.TriggerOnEnterWorld(worldData);
        GameEvents.instance.TriggerOnPlayerEnter(worldData.player);

        yield return new WaitForEndOfFrame();
        TransitionManager.instance.OpenScene();

        if (logGameStates) print("Starting Level");

        yield return StartRound();
    }

    private IEnumerator StartRound()
    {
        roundNumber++;

        if (logGameStates) print($"Round {roundNumber} Start");

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
        if (logGameStates) print($"Turn Start: {entityData.name}");

        GameEvents.instance.TriggerOnTurnStart(entityData);

        if (entityData is PlayerData)
        {
            // Allow player to make choices
            PlayerMananger.instance.AllowInput();
        }
        else if (entityData is EnemyData)
        {
            yield return HandleEnemyTurn(entityData as EnemyData);
        }
        else throw new System.Exception("UNHANDLED ENTITY");

    }

    private IEnumerator HandleEnemyTurn(EnemyData enemyData)
    {
        if (enemyData.ai == null)
            throw new System.Exception("Enemy AI not found.");

        // Perform action based on AI
        yield return HandleEnemyAction(enemyData, enemyData.ai);

        yield return HandleEnemyIntent(enemyData);
    }

    private IEnumerator HandleEnemyAction(EnemyData enemyData, AI ai)
    {
        Vector3Int enemyPosition = enemyData.Position;
        Vector3Int playerPosition = worldData.player.Position;

        switch (ai.intent)
        {
            // If sleeping, alert or focusing
            case Intent.Dormant:
            case Intent.Alert:
            case Intent.Focus:

                // Skip turn
                yield return SkipOverTime(enemyData);
                break;

            // Chase player until death do us part
            case Intent.Move:

                // path[0] - current position
                // path[1] - next position to move to
                var path = Pathfinder.FindPath(enemyPosition, playerPosition, worldData);

                // If we have a valid path
                if (path.Count > 2)
                {
                    Vector3Int nextPosition = path[1];
                    Vector3Int optimalDirection = nextPosition - enemyPosition;

                    Vector3Int newPosition = enemyPosition + optimalDirection;
                    TileData newTile = worldData.GetTile(newPosition);

                    // Move
                    yield return MoveToTileOverTime(enemyData, newTile);
                }
                else
                {
                    // Skip turn
                    yield return SkipOverTime(enemyData);
                }

                break;
            case Intent.Attack:

                // Attack
                var weapon = enemyData.Weapon;
                var tileData = worldData.GetTile(ai.attackPosition);
                var targets = weapon.CalculateArea(enemyData, ai.attackPosition);

                yield return AttackPositionsOverTime(enemyData, weapon, tileData, targets);

                enemyData.ai.threatenedPositions.Clear();
                IntentManager.instance.Clear(targets);

                break;
            default:
                throw new System.Exception("Unhandled Enemy state: " + ai.intent);
        }
    }

    private IEnumerator HandleEnemyIntent(EnemyData enemyData)
    {
        // Update next intents
        enemyData.ai.CalculateIntent(enemyData, worldData);
        GameEvents.instance.TriggerOnEnemyIntent(enemyData);

        // If an attack is coming
        if (enemyData.ai.intent == Intent.Attack)
        {
            // Project attack area
            var weapon = enemyData.Weapon;
            var targets = weapon.CalculateArea(enemyData, enemyData.ai.attackPosition);

            enemyData.ai.threatenedPositions = targets.ToList();
            IntentManager.instance.Target(targets);
        }

        yield return null;
    }

    private IEnumerator EndTurn()
    {
        if (logGameStates) print($"Turn End: {currentEntity.name}");

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
        if (logGameStates) print($"Round {roundNumber} End");

        // Start new round
        yield return StartRound();
    }

    private IEnumerator EndLevel()
    {
        if (logGameStates) print($"Ending Level");

        // Load new randomly generated level
        TransitionManager.instance.ReloadScene();

        yield return null;
    }

    #endregion

    #region Actions

    public void SkipTurn(EntityData entityData)
    {
        if (selectedWeapon != null)
            return;

        if (logGameStates) print($"{entityData.name} skipped their turn.");

        // Trigger event
        GameEvents.instance.TriggerOnActionStart();

        if (currentEntity is PlayerData)
        {
            PlayerMananger.instance.PreventInput();

            StartCoroutine(SkipOverTime(entityData));
        }
        else
        {
            StartCoroutine(EndTurn());
        }
    }

    private IEnumerator SkipOverTime(EntityData entityData)
    {
        // Wait a bit before ending turn
        float waitDuration = entityData is EnemyData ? 0f : gameSpeedData.waitDuration;
        yield return entityData.entityRenderer.WaitOverTime(waitDuration);

        // Gain focus
        EntityGainFocus(entityData);

        yield return EndTurn();
    }

    public bool IsValidMove(EntityData entityData, Vector3Int direction)
    {
        if (direction.magnitude > 1f)
            throw new System.Exception($"MAGNITUDE IS TOO BIG: {direction}");

        if (direction == Vector3Int.zero)
            return false;

        // Calculate new position
        Vector3Int newPosition = entityData.Position + direction;

        // Check bounds
        if (WorldGenerator.OutOfBounds(newPosition, worldData.tiles))
            return false;

        TileData newTile = worldData.GetTile(newPosition);

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

        if (logEntityActions) print($"{entityData.name} moved towards {direction}.");

        if (entityData is PlayerData)
        {
            PlayerMananger.instance.PreventInput();
        }

        // Trigger event
        GameEvents.instance.TriggerOnActionStart();

        Vector3Int newPosition = entityData.Position + direction;
        TileData newTile = worldData.GetTile(newPosition);

        StartCoroutine(MoveToTileOverTime(entityData, newTile));
    }

    private IEnumerator MoveToTileOverTime(EntityData entityData, TileData destinationTile)
    {
        // Handle visuals
        Vector3Int direction = destinationTile.position - entityData.Position;
        yield return entityData.entityRenderer.MoveOverTime(direction, gameSpeedData.moveDuration);

        // Set new location for entity
        entityData.tileData.entityData = null;
        destinationTile.entityData = entityData;
        entityData.tileData = destinationTile;

        // Calculate new vision for that location
        entityData.vision.Refresh();

        // Handle special case for player
        if (entityData is PlayerData)
        {
            // Update vision
            FogOfWarRenderer.instance.UpdateFog(entityData.vision);
        }

        // Trigger event
        GameEvents.instance.TriggerOnTileEnter(entityData, destinationTile);

        // Expend focus
        if (entityData.currentFocus > 0)
        {
            entityData.currentFocus--;
            GameEvents.instance.TriggerOnEntityResourceChange(entityData);
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
        if (logEntityActions) print($"{entityData.name} cancels the attack.");

        selectedWeapon = null;

        PreviewManager.instance.CancelPreview();

        GameEvents.instance.TriggerOnAttackCancel(entityData.weapons[weaponIndex]);
    }

    public void EntityPerformAttack(EntityData entityData, int weaponIndex, Vector3Int position)
    {
        if (logEntityActions) print($"{entityData.name} attacks the location: {position}.");

        selectedWeapon = null;

        if (entityData is PlayerData)
        {
            PreviewManager.instance.CancelPreview();
            PlayerMananger.instance.PreventInput();
        }

        // Trigger event
        GameEvents.instance.TriggerOnActionStart();

        var weapon = entityData.weapons[weaponIndex];
        var targets = weapon.CalculateArea(entityData, position);
        TileData tileData = worldData.GetTile(position);
        StartCoroutine(AttackPositionsOverTime(entityData, weapon, tileData, targets));
    }

    #endregion

    #region Helpers

    private IEnumerator AttackPositionsOverTime(EntityData entityData, WeaponData weaponData, TileData targetTile, Vector3Int[] positions)
    {
        // Trigger related event
        GameEvents.instance.TriggerOnEntityAttackTile(entityData, weaponData, targetTile);

        // Deal damage with weapon
        foreach (var position in positions)
        {
            TileData tile = worldData.GetTile(position);
            EntityData targetData = tile.entityData;
            if (targetData != null)
            {
                // If enemy is attacking enemy, then skip
                if (entityData is EnemyData && targetData is EnemyData)
                    continue;

                // Deal damage
                int damage = weaponData.CalculateDamage(entityData);
                EntityTakeDamage(targetData, damage, entityData);
            }
        }

        // Handle visuals
        yield return entityData.entityRenderer.MeleeOverTime(targetTile, gameSpeedData.attackDuration);

        // Use with weapon
        weaponData.Use();

        // Expend ALL focus
        if (entityData.currentFocus > 0)
        {
            entityData.currentFocus = 0;
            GameEvents.instance.TriggerOnEntityResourceChange(entityData);
        }

        // End turn after
        yield return EndTurn();
    }

    public void EntityTakeDamage(EntityData entityData, int amount, EntityData source)
    {
        FloatingTextManager.instance.SpawnText($"{amount}", Color.red, entityData.entityRenderer.transform.position);

        entityData.currentHealth = Mathf.Max(entityData.currentHealth - amount, 0);
        if (entityData.currentHealth == 0)
        {
            // Despawn
            EntityDespawn(entityData);
            return;
        }

        // Trigger event
        GameEvents.instance.TriggerOnEntityResourceChange(entityData);

        print($"{entityData.name} took {amount} damage.");

        entityData.entityRenderer.TakeHit(source.Position);
    }

    private void EntityDespawn(EntityData entityData)
    {
        print($"{entityData.name} was killed.");

        // Destroy game-object
        entityData.entityRenderer.Die();

        // Remove from tile
        TileData tileData = entityData.tileData;
        tileData.entityData = null;

        // Logic for when player is killed
        if (entityData is PlayerData)
        {
            // Open game over UI
            GameEvents.instance.TriggerOnGameOver(0);
        }
        else if (entityData is EnemyData)
        {
            var enemy = entityData as EnemyData;
            if (worldData.enemies.Remove(enemy)) { }
            else if (worldData.neutrals.Remove(enemy)) { }

            // Deselect targets if possible
            if (enemy.ai != null)
            {
                IntentManager.instance.Clear(enemy.ai.threatenedPositions.ToArray());
            }

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

    private void EntityGainFocus(EntityData entityData)
    {
        if (entityData.currentFocus < entityData.maxFocus)
        {
            print($"{entityData.name} gain focus!");
            entityData.currentFocus++;

            // Trigger event
            GameEvents.instance.TriggerOnEntityResourceChange(entityData);
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
                oldWeapon.Unintialize();

                // Create visuals
                WorldRenderer.instance.SpawnWeapon(oldWeapon, tileData.position);

                // Trigger event
                GameEvents.instance.TriggerOnWeaponDrop(weaponIndex, oldWeapon, tileData);
            }
            else
            {
                // Remove from ground
                tileData.weapon = null;
            }

            // Pick up new one
            entityData.weapons[weaponIndex] = newWeapon;
            newWeapon.Initialize(entityData);

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
                WorldRenderer.instance.SpawnWeapon(weapon, tileData.position);

                // Trigger event
                GameEvents.instance.TriggerOnWeaponDrop(weaponIndex, weapon, tileData);
            }
        }
    }

    #endregion

    private void GenerateTurnOrder()
    {
        // First add player
        turnQueue.Add(worldData.player);

        // Then add enemies
        foreach (EnemyData enemy in worldData.enemies)
        {
            if (enemy.ai != null)
            {
                turnQueue.Add(enemy);
            }
        }

        if (turnQueue.Count == 0)
            throw new System.Exception("No entities left in the map.");
    }
}
