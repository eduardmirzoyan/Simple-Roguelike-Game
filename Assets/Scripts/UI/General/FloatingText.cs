using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private TextMeshPro messageLabel;

    [Header("Settings")]
    [SerializeField] private float duration;
    [SerializeField] private Vector2 power;

    public void Initialize(string message, Color color)
    {
        messageLabel.text = message;
        messageLabel.color = color;

        StartCoroutine(Animate(duration, power));
    }

    private IEnumerator Animate(float duration, Vector2 power)
    {
        Color startColor = messageLabel.color;
        Color endColor = Color.clear;

        // Randomly invert x
        if (Random.Range(0, 2) == 0) power.x = -power.x;

        rigidbody2d.AddForce(power, ForceMode2D.Impulse);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            messageLabel.color = Color.Lerp(startColor, endColor, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
        messageLabel.color = Color.clear;

        Destroy(gameObject);
    }
}
