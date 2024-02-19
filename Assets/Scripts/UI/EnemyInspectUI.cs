using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyInspectUI : MonoBehaviour
{
    [Header("Basic")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI stateLabel;
    [SerializeField] private ResourcebarUI guardbar;
    [SerializeField] private ResourcebarUI healthbar;

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

        GameEvents.instance.onEntityInspect += OnInspect;
        GameEvents.instance.onEntityResourceChange += UpdateData;

        canvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onEntityInspect -= OnInspect;
        GameEvents.instance.onEntityResourceChange -= UpdateData;
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
                stateLabel.text = enemyData.ai.state.ToString();
            }
            else
                stateLabel.gameObject.SetActive(false);

            guardbar.Initialize(enemyData.currentPosture, enemyData.maxPosture);
            healthbar.Initialize(enemyData.currentHealth, enemyData.maxHealth);

            UpdateStats();

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
            guardbar.UpdateValue(enemyData.currentPosture, enemyData.maxPosture);
            healthbar.UpdateValue(enemyData.currentHealth, enemyData.maxHealth);

            UpdateStats();
        }
    }

    private void UpdateStats()
    {
        if (enemyData.weapons.Count > 0)
        {
            statsBar.SetActive(true);
            var weapon = enemyData.weapons[0];
            damageLabel.text = weapon.damage + "";
            rangeLabel.text = weapon.range + "";
            cooldownLabel.text = weapon.cooldown - 1 + "";
        }
        else
        {
            statsBar.SetActive(false);
        }
    }
}
