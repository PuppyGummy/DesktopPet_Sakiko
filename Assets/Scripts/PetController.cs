using System;
using System.Runtime.InteropServices;
using UnityEngine;
using SatorImaging.AppWindowUtility;

public class PetController : MonoBehaviour
{
    public float scaleSpeed = 0.2f;
    public float minScale = 0.5f;
    public float maxScale = 1.5f;
    private bool isDragging = false;
    private Vector3 dragOffset;
    void Start()
    {
        AppWindowUtility.Transparent = true;
        AppWindowUtility.AlwaysOnTop = true;

        minScale = transform.localScale.x * minScale;
        maxScale = transform.localScale.x * maxScale;
    }
    void Update()
    {
        Windows.ClickThroughPixel = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) == null;
        AppWindowUtility.ClickThrough = true;

        HandleScaling();

        HandleDragging();
    }
    void HandleScaling()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            float newScale = Mathf.Clamp(transform.localScale.x + scroll * scaleSpeed, minScale, maxScale);
            transform.localScale = new Vector3(newScale, newScale, 1);
        }
    }

    void HandleDragging()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            dragOffset = transform.position - mouseWorldPos;
        }

        if (Input.GetMouseButton(1) && isDragging)
        {
            transform.position = mouseWorldPos + dragOffset;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }
    }
}
