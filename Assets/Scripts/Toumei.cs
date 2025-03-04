using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toumei : MonoBehaviour
{
    public Camera mainCamera;
    public Material transparentMaterial;
    public float transparencyDistance = 2.0f;

    private Material originalMaterial;
    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (objectRenderer == null || mainCamera == null || transparentMaterial == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);

        if (distance < transparencyDistance)
        {
            objectRenderer.material = transparentMaterial;
        }
        else
        {
            objectRenderer.material = originalMaterial;
        }
    }
}
