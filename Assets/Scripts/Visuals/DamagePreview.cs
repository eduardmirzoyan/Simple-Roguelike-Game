using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePreview : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshPro damageText;

    public void Initialize(int damage, Color color)
    {
        damageText.text = damage.ToString();
        damageText.color = color;
    }
}
