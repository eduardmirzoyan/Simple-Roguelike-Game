using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GameObject floatingTextPrefab;

    public static FloatingTextManager instance;
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

    public void SpawnText(string message, Color color, Vector3 position)
    {
        Instantiate(floatingTextPrefab, position, Quaternion.identity, transform).GetComponent<FloatingText>().Initialize(message, color);
    }
}
