using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkipActionUI skipActionUI;
    [SerializeField] private MoveActionUI moveActionUI;
    [SerializeField] private WeaponActionUI[] weaponUIs;
    [SerializeField] private CanvasGroup instructionsGroup;

    public static PlayerActionsUI instance;
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

    private void Start()
    {
        GameEvents.instance.OnPlayerEnter += Initialize;
        GameEvents.instance.OnAttackSelect += OnAttackSelect;
        GameEvents.instance.OnAttackCancel += OnAttackCancel;
        GameEvents.instance.OnActionStart += OnActionStart;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnPlayerEnter -= Initialize;
        GameEvents.instance.OnAttackSelect -= OnAttackSelect;
        GameEvents.instance.OnAttackCancel -= OnAttackCancel;
        GameEvents.instance.OnActionStart -= OnActionStart;
    }

    private void Initialize(PlayerData playerData)
    {
        skipActionUI.Initialize(playerData);
        moveActionUI.Initialize(playerData);

        for (int i = 0; i < playerData.weapons.Count; i++)
        {
            weaponUIs[i].Initialize(playerData, i);
        }
    }

    private void OnAttackSelect(WeaponData data)
    {
        instructionsGroup.alpha = 1f;
    }

    private void OnAttackCancel(WeaponData data)
    {
        instructionsGroup.alpha = 0f;
    }

    private void OnActionStart()
    {
        instructionsGroup.alpha = 0f;
    }
}
