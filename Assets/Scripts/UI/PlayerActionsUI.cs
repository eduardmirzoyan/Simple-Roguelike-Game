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

    public void Initialize(PlayerData playerData)
    {
        skipActionUI.Initialize(playerData);
        moveActionUI.Initialize(playerData);

        for (int i = 0; i < playerData.weapons.Count; i++)
        {
            weaponUIs[i].Initialize(playerData, i);
        }

        GameEvents.instance.onAttackSelect += OnAttackSelect;
        GameEvents.instance.onAttackCancel += OnAttackCancel;
        GameEvents.instance.onActionStart += OnActionStart;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onAttackSelect -= OnAttackSelect;
        GameEvents.instance.onAttackCancel -= OnAttackCancel;
        GameEvents.instance.onActionStart -= OnActionStart;
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
