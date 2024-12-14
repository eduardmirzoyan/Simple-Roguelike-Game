using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField, ReadOnly] private CinemachineVirtualCamera virtualCamera;
    [SerializeField, ReadOnly] private CinemachineConfiner2D confiner;

    public static CameraManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    public void Initialize(Transform target)
    {
        MapBoundaryManager.instance.GenerateBoundary();

        // Set follow target to player
        virtualCamera.m_Follow = target;

        // Set boundary based on tilemap
        confiner.m_BoundingShape2D = MapBoundaryManager.instance.GetBoundingCollider();
    }
}
