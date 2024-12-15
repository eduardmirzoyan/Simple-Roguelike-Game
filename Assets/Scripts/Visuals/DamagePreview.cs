using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePreview : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshPro damageText;

    public void Initialize(Vector2Int damageRange, Color color)
    {
        damageText.text = $"{damageRange.x}-{damageRange.y}";
        damageText.color = color;
    }
}
