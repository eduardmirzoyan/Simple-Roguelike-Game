using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image intentImage;
    [SerializeField] private Transform focusOrbHolder;

    [Header("Settings")]
    [SerializeField] private GameObject focusOrbPrefab;
    [SerializeField] private Sprite activeFocusIcon;
    [SerializeField] private Sprite inactiveFocusIcon;
    [SerializeField] private Sprite[] intentIcons;

    private List<Image> focusIcons;
    private EntityData entityData;

    private void Start()
    {
        GameEvents.instance.OnEntityResourceChange += UpdateFocus;
        GameEvents.instance.OnEnemyIntent += UpdateIntent;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEntityResourceChange -= UpdateFocus;
        GameEvents.instance.OnEnemyIntent -= UpdateIntent;
    }

    public void Initialize(EntityData entityData)
    {
        this.entityData = entityData;

        focusIcons = new();
        for (int i = 0; i < entityData.maxFocus; i++)
        {
            var focusIcon = Instantiate(focusOrbPrefab, focusOrbHolder).GetComponent<Image>();
            focusIcons.Add(focusIcon);
        }

        UpdateFocus(entityData);
        UpdateIntent(null);
    }

    private void UpdateIntent(EnemyData enemyData)
    {
        if (this.entityData != enemyData)
            return;

        // If entity is not an enemy
        if (entityData is not EnemyData)
        {
            intentImage.enabled = false;
            return;
        }

        // Set sprite based on enum state
        intentImage.enabled = true;
        intentImage.sprite = intentIcons[(int)enemyData.ai.intent];
    }

    private void UpdateFocus(EntityData entityData)
    {
        if (this.entityData != entityData)
            return;

        // Lit up active icons
        for (int i = 0; i < entityData.maxFocus; i++)
        {
            if (i < entityData.currentFocus)
            {
                focusIcons[i].sprite = activeFocusIcon;
                focusIcons[i].color = Color.white;
            }
            else
            {
                focusIcons[i].sprite = inactiveFocusIcon;
                focusIcons[i].color = new(1, 1, 1, 0.5f);
            }
        }
    }
}
