using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusBarUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject focusSocketPrefab;
    [SerializeField] private Sprite activeFocusIcon;
    [SerializeField] private Sprite inactiveFocusIcon;

    [Header("Debug")]
    [SerializeField, ReadOnly] private EntityData entityData;
    [SerializeField, ReadOnly] private List<Image> focusSockets;

    private void Start()
    {
        GameEvents.instance.OnEntityResourceChange += UpdateFocus;
    }

    private void OnDestroy()
    {
        GameEvents.instance.OnEntityResourceChange -= UpdateFocus;
    }

    public void Initialize(EntityData entityData)
    {
        this.entityData = entityData;

        focusSockets = new();
        for (int i = 0; i < entityData.maxFocus; i++)
        {
            var focusSocket = Instantiate(focusSocketPrefab, transform).GetComponentInChildren<Image>();
            focusSockets.Add(focusSocket);
        }

        UpdateFocus(entityData);
    }

    public void Uninitialize()
    {
        entityData = null;

        foreach (var focusSocket in focusSockets)
            Destroy(focusSocket.transform.parent.gameObject);

        focusSockets.Clear();
    }

    public void UpdateFocus(EntityData entityData)
    {
        if (this.entityData != entityData)
            return;

        // Lit up active icons
        for (int i = 0; i < entityData.maxFocus; i++)
        {
            if (i < entityData.currentFocus)
            {
                focusSockets[i].sprite = activeFocusIcon;
            }
            else
            {
                focusSockets[i].sprite = inactiveFocusIcon;
            }
        }
    }
}
