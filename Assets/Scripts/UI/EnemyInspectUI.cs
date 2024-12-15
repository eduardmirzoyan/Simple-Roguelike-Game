using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class EnemyInspectUI : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI stateLabel;
    [SerializeField] private ResourcebarUI healthbar;
    [SerializeField] private FocusBarUI focusBar;

    [Header("Debug")]
    [SerializeField, ReadOnly] private EnemyData enemyData;

    public static EnemyInspectUI instance;
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

        GameEvents.instance.OnEntityInspect += OnInspect;
        GameEvents.instance.OnEntityResourceChange += UpdateData;

        canvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEntityInspect -= OnInspect;
        GameEvents.instance.OnEntityResourceChange -= UpdateData;
    }

    private void OnInspect(EnemyData enemyData)
    {
        this.enemyData = enemyData;

        if (enemyData != null)
        {
            nameLabel.text = enemyData.name;

            if (enemyData.ai != null)
                stateLabel.text = $"Intent: <color=yellow>{enemyData.ai.intent}</color>";
            else
                stateLabel.text = "Inanimate";

            UpdateData(enemyData);

            focusBar.Uninitialize();
            focusBar.Initialize(enemyData);

            canvasGroup.alpha = 1f;
        }
        else
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void UpdateData(EntityData entityData)
    {
        if (enemyData != entityData)
            return;

        healthbar.UpdateValue(enemyData.currentHealth, enemyData.maxHealth);
        focusBar.UpdateFocus(entityData);
    }
}
