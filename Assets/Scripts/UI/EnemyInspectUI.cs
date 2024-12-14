using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyInspectUI : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI stateLabel;
    [SerializeField] private ResourcebarUI healthbar;
    [SerializeField] private ResourcebarUI focusbar;

    [Header("Stats")]
    [SerializeField] private GameObject statsBar;
    [SerializeField] private TextMeshProUGUI damageLabel;
    [SerializeField] private TextMeshProUGUI rangeLabel;
    [SerializeField] private TextMeshProUGUI cooldownLabel;

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
            {
                stateLabel.gameObject.SetActive(true);
                stateLabel.text = enemyData.ai.intent.ToString();
            }
            else
                stateLabel.gameObject.SetActive(false);

            UpdateData(enemyData);

            canvasGroup.alpha = 1f;
        }
        else
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void UpdateData(EntityData entityData)
    {
        if (enemyData == entityData)
        {
            healthbar.UpdateValue(enemyData.currentHealth, enemyData.maxHealth);
            focusbar.UpdateValue(enemyData.currentFocus, enemyData.maxFocus);

            UpdateStats(enemyData);
        }
    }

    private void UpdateStats(EnemyData enemyData)
    {
        if (enemyData.weapons.Count > 0)
        {
            statsBar.SetActive(true);
            var weapon = enemyData.Weapon;
            damageLabel.text = $"{weapon.damage}";
            rangeLabel.text = $"{weapon.range}";
            cooldownLabel.text = $"{weapon.cooldown - 1}";
        }
        else
        {
            statsBar.SetActive(false);
        }
    }
}
