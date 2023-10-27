using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInspectUI : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private ResourcebarUI guardbar;
    [SerializeField] private ResourcebarUI healthbar;

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

    private void OnDestroy()
    {
        GameEvents.instance.onEntityResourceChange -= UpdateData;
    }

    public void Initialize(PlayerData playerData)
    {
        this.playerData = playerData;
        nameLabel.text = playerData.name;

        guardbar.Initialize(playerData.currentPosture, playerData.maxPosture);
        healthbar.Initialize(playerData.currentHealth, playerData.maxHealth);

        GameEvents.instance.onEntityResourceChange += UpdateData;
    }

    private void UpdateData(EntityData entityData)
    {
        if (playerData == entityData)
        {
            guardbar.UpdateValue(playerData.currentPosture, playerData.maxPosture);
            healthbar.UpdateValue(playerData.currentHealth, playerData.maxHealth);
        }
    }
}
