using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Transform))]
public class CameraController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static CameraController Instance { get; private set; }

    [SerializeField] private Transform _cameraTransform;
    private Transform _transform;

    [Header("Defaults")]
    [SerializeField] private float xAngle = 0f;
    [SerializeField] private float yAngle = 45f;
    [SerializeField] private float distance = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotationSensitivity = 10f;
    [SerializeField] private float rotationInertia = 5f;

    [Header("Zoom")]
    [SerializeField] private float scrollZoomSensitivity = 10f;
    [SerializeField] private float zoomInertia = 5f;
    [SerializeField] private float zoomMin = 2f;
    [SerializeField] private float zoomMax = 20f;

    public Transform defaultOrbitPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        if (defaultOrbitPoint == null)
        {
            defaultOrbitPoint = new GameObject("Default Orbit Point").transform;
            defaultOrbitPoint.position = Vector3.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
