using UnityEngine;
using UnityEngine.InputSystem;
public class MapCameraController : MonoBehaviour
{
    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 1.2f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 16f;

    [Header("Bounds")]
    [SerializeField] private float boundsPadding = 3f;

    private Camera cam;
    private Vector3 dragOriginWorld;
    private bool isDragging;

    private bool hasBounds;
    private float minX, maxX, minY, maxY;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX - boundsPadding;
        this.maxX = maxX + boundsPadding;
        this.minY = minY - boundsPadding;
        this.maxY = maxY + boundsPadding;
        hasBounds = true;

        float mapHeight = maxY - minY;
        float desiredSize = Mathf.Clamp(mapHeight * 0.35f, minZoom, maxZoom);
        cam.orthographicSize = desiredSize;
    }

    private void Update()
    {
        HandleZoom();
        HandlePan();
        if (hasBounds) ClampPosition();
    }

    private void HandleZoom()
    {
        if (Mouse.current == null) return;

        float scroll = Mouse.current.scroll.ReadValue().y * 0.01f;
        if (Mathf.Approximately(scroll, 0f)) return;

        cam.orthographicSize = Mathf.Clamp(
            cam.orthographicSize - scroll * zoomSpeed,
            minZoom,
            maxZoom
        );
    }

    private void HandlePan()
    {
        if (Mouse.current == null) return;

        bool startDrag = Mouse.current.rightButton.wasPressedThisFrame
                      || Mouse.current.middleButton.wasPressedThisFrame;

        bool holdDrag  = Mouse.current.rightButton.isPressed
                      || Mouse.current.middleButton.isPressed;

        bool endDrag   = Mouse.current.rightButton.wasReleasedThisFrame
                      || Mouse.current.middleButton.wasReleasedThisFrame;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        if (startDrag)
        {
            dragOriginWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
            isDragging = true;
        }

        if (holdDrag && isDragging)
        {
            Vector3 currentWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
            Vector3 delta = dragOriginWorld - currentWorld;
            transform.position += delta;

            dragOriginWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
        }

        if (endDrag) isDragging = false;
    }

    private void ClampPosition()
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float clampedX = Mathf.Clamp(transform.position.x, minX + halfW, maxX - halfW);
        float clampedY = Mathf.Clamp(transform.position.y, minY + halfH, maxY - halfH);

        if (minX + halfW > maxX - halfW) clampedX = (minX + maxX) / 2f;
        if (minY + halfH > maxY - halfH) clampedY = (minY + maxY) / 2f;

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
