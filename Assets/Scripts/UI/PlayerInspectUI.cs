using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInspectUI : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private ResourcebarUI healthbar;
    [SerializeField] private ResourcebarUI focusbar;

    [Header("Debug")]
    [SerializeField, ReadOnly] private PlayerData playerData;

    public static PlayerInspectUI instance;
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
        GameEvents.instance.OnEntityResourceChange += UpdateData;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnPlayerEnter -= Initialize;
        GameEvents.instance.OnEntityResourceChange -= UpdateData;
    }

    private void Initialize(PlayerData playerData)
    {
        this.playerData = playerData;
        nameLabel.text = playerData.name;

        UpdateData(playerData);
    }

    private void UpdateData(EntityData entityData)
    {
        if (playerData == entityData)
        {
            healthbar.UpdateValue(playerData.currentHealth, playerData.maxHealth);
            focusbar.UpdateValue(playerData.currentFocus, playerData.maxFocus);
        }
    }
}
