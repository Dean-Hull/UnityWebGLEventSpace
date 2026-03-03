using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Transform))]
public class CameraController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static CameraController Instance { get; private set; }

    [Header("Camera Setup")]
    [SerializeField] private Transform cameraToControl;
    private Transform _cameraTransform;

    [Header("Default Angles and Distance")]
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

    private float _currentXAngle;
    private float _currentYAngle;
    private float _currentDistance;

    private int _numberOfPointers;
    private int _firstPointerId = -1;
    private Vector3 _firstPointerPosition;
    private Vector3 _lastPointerPosition;
    private Vector3 _lastFirstPointerPosition;
    private float _gestureStartingDistance;
    private RaycastHit _hitData;

    private bool _isMovingToNewPosition;

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

    void Start()
    {
        _cameraTransform = cameraToControl.transform;
        _currentXAngle = xAngle;
        _currentYAngle = yAngle;
        _currentDistance = distance;
    }

    void Update()
    {
        if (!_isMovingToNewPosition)
        {
            distance -= Input.GetAxis("Mouse ScrollWheel") * scrollZoomSensitivity;
            distance = Mathf.Clamp(distance, zoomMin, zoomMax);
        }
    }

    void LateUpdate()
    {
        _currentXAngle = Mathf.Lerp(_currentXAngle, xAngle, Time.deltaTime * rotationInertia);
        _currentYAngle = Mathf.Lerp(_currentYAngle, yAngle, Time.deltaTime * rotationInertia);
        _currentDistance = Mathf.Lerp(_currentDistance, distance, Time.deltaTime * zoomInertia);

        Quaternion rotation = Quaternion.Euler(_currentYAngle, _currentXAngle, 0);
        Vector3 position = defaultOrbitPoint.position + rotation * new Vector3(0, 0, -_currentDistance);
        _cameraTransform.SetPositionAndRotation(position, rotation);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _numberOfPointers++;

        if (_firstPointerId == -1)
        {
            _firstPointerId = eventData.pointerId;
            _gestureStartingDistance = Mathf.Max(distance, 5f);
            _firstPointerPosition = GetWorldPoint(eventData);
            _lastPointerPosition = _firstPointerPosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_numberOfPointers == 1 && Vector2.Distance(eventData.pressPosition, eventData.position) < EventSystem.current.pixelDragThreshold)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if(Physics.Raycast(ray, out _hitData, float.MaxValue))
            {
                _hitData.transform.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
            }

            _numberOfPointers--;

            if (_firstPointerId == eventData.pointerId)
            {
                _firstPointerId = -1;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_numberOfPointers == 1 && eventData.pointerId == _firstPointerId)
        {
            _firstPointerPosition = GetWorldPoint(eventData);
            Vector2 delta = _firstPointerPosition - _lastFirstPointerPosition;
            xAngle += delta.x * rotationSensitivity;
            yAngle -= delta.y * rotationSensitivity;
            _lastFirstPointerPosition = _firstPointerPosition;
        }
    }

    private Vector3 GetWorldPoint(PointerEventData eventData)
    {
        Vector3 pos = eventData.position;
        pos.z = _gestureStartingDistance;
        pos = Camera.main.ScreenToWorldPoint(pos);
        return _cameraTransform.InverseTransformPoint(pos);
    }

    public void SetCameraTarget(Transform target, float newDistance = -1f, float newXAngle = float.NaN, float newYAngle = float.NaN)
    {
        defaultOrbitPoint.position = target.position;

        if (!float.IsNaN(newXAngle)) xAngle = newXAngle;
        if (!float.IsNaN(newYAngle)) yAngle = newYAngle;
        if (newDistance > 0) distance = newDistance;
    }

    public void MoveCameraToPosition(float newXAngle, float newYAngle, float newDistance)
    {
        xAngle = newXAngle;
        yAngle = newYAngle;
        if (newDistance > 0) distance = newDistance;
    }

    public void Reset()
    {
        defaultOrbitPoint.position = Vector3.zero;
        xAngle = 0f;
        yAngle = 45f;
        distance = 10f;
    }
}
