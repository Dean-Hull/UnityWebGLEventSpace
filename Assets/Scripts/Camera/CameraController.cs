using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TouchCount
{
    NONE = 0,
    ONE = 1,
    TWO = 2,
}

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
    public Transform defaultOrbitPoint;

    [Header("Rotation")]
    [SerializeField] private TouchCount rotationTouchCount = TouchCount.ONE;
    [SerializeField] private float rotationXSensitivity = 10f;
    [SerializeField] private float rotationYSensitivity = 10f;
    [SerializeField] private float rotationInertia = 5f;

    [Header("Panning")]
    [SerializeField] private TouchCount panningTouchCount = TouchCount.TWO;
    [SerializeField] private float panningXSensitivity = 1f;
    [SerializeField] private float panningYSensitivity = 1f;
    [SerializeField] private float panInertia = 5f;
    private Vector3 _panOffset = Vector3.zero;
    private Vector3 _currentPanOffset = Vector3.zero;
    private bool _isPanning;

    [Header("Zoom")]
    [SerializeField] private TouchCount zoomTouchCount = TouchCount.TWO;
    [SerializeField] private float zoomSensitivity = 10f;
    [SerializeField] private float zoomInertia = 5f;
    [SerializeField] private float zoomMin = 2f;
    [SerializeField] private float zoomMax = 20f;
    private bool _isZooming;

    private float _currentXAngle;
    private float _currentYAngle;
    private float _currentDistance;

    private int _numberOfPointers;
    private int _firstPointerId = -1;
    private Vector3 _firstPointerPosition;
    private Vector3 _lastFirstPointerPosition;
    private Vector2 _firstPointerDetla;
    private int _secondPointerId = -1;
    private Vector3 _secondPointerPosition;
    private Vector3 _lastSecondPointerPosition;
    private float _gestureStartingDistance;
    private float _lastPointerDistance;
    private float _currentPointerDistance;
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
        _currentPanOffset = _panOffset;
    }

    void Update()
    {
        if (!_isMovingToNewPosition)
        {
            distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        }
    }

    void LateUpdate()
    {
        _currentXAngle = Mathf.Lerp(_currentXAngle, xAngle, Time.deltaTime * rotationInertia);
        _currentYAngle = Mathf.Lerp(_currentYAngle, yAngle, Time.deltaTime * rotationInertia);
        _currentDistance = Mathf.Lerp(_currentDistance, distance, Time.deltaTime * zoomInertia);
        _currentPanOffset = Vector3.Lerp(_currentPanOffset, _panOffset, Time.deltaTime * panInertia);

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
            _lastFirstPointerPosition = _firstPointerPosition;
            return;
        }

        if (_secondPointerId == -1)
        {
            _secondPointerId = eventData.pointerId;
            _secondPointerPosition = GetWorldPoint(eventData);
            _lastSecondPointerPosition = _secondPointerPosition;
            _lastPointerDistance = Vector2.Distance(_firstPointerPosition, _secondPointerPosition);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_numberOfPointers == 1 && Vector2.Distance(eventData.pressPosition, eventData.position) < EventSystem.current.pixelDragThreshold)
        {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out _hitData, float.MaxValue))
            {
                _hitData.transform.SendMessage("OnClick", SendMessageOptions.RequireReceiver);
            }
        }

        _numberOfPointers--;

        if (_firstPointerId == eventData.pointerId)
        {
            _firstPointerId = -1;
        }

        if (_secondPointerId == eventData.pointerId)
        {
            _secondPointerId = -1;
        }

        if (_numberOfPointers == 0)
        {
            _isPanning = false;
            _isZooming = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isPanning && !_isZooming && _numberOfPointers == (int)rotationTouchCount)
        {
            if (eventData.pointerId == _firstPointerId)
            {
                _firstPointerPosition = GetWorldPoint(eventData);
                Vector2 delta = _firstPointerPosition - _lastFirstPointerPosition;
                xAngle += delta.x * rotationXSensitivity;
                yAngle -= delta.y * rotationYSensitivity;
                _lastFirstPointerPosition = _firstPointerPosition;
            }
        }

        if (!_isZooming && _numberOfPointers == (int)panningTouchCount)
        {
            if ((int)panningTouchCount > 1)
            {
                if (eventData.pointerId == _firstPointerId)
                {
                    _firstPointerDetla = eventData.delta;
                    _firstPointerPosition = GetWorldPoint(eventData);
                }
                else if (eventData.pointerId == _secondPointerId)
                {
                    if (eventData.delta != Vector2.zero && _firstPointerDetla != Vector2.zero && Vector2.Angle(eventData.delta, _firstPointerDetla) < 30f)
                    {
                        _isPanning = true;
                        _secondPointerPosition = GetWorldPoint(eventData);
                        PanCamera(_secondPointerPosition - _lastSecondPointerPosition);
                        _lastSecondPointerPosition = _secondPointerPosition;
                    }
                }
            }
            else
            {
                if (eventData.pointerId == _firstPointerId)
                {
                    _isPanning = true;
                    _firstPointerPosition = GetWorldPoint(eventData);
                    PanCamera(_firstPointerPosition - _lastFirstPointerPosition);
                    _lastFirstPointerPosition = _firstPointerPosition;
                }
            }
        }

        if (!_isPanning && _numberOfPointers == (int)zoomTouchCount)
        {
            if (eventData.pointerId == _firstPointerId)
            {
                _firstPointerPosition = GetWorldPoint(eventData);
                _lastPointerDistance = Vector2.Distance(_firstPointerPosition, _secondPointerPosition);
            }
            else if (eventData.pointerId == _secondPointerId)
            {
                if (eventData.delta != Vector2.zero && _firstPointerDetla != Vector2.zero && Vector2.Angle(eventData.delta, _firstPointerDetla) > 120f)
                {
                    _isZooming = true;
                    _secondPointerPosition = GetWorldPoint(eventData);
                    _currentPointerDistance = Vector3.Distance(_firstPointerPosition, _secondPointerPosition);
                    distance -= (_currentPointerDistance - _lastPointerDistance) * zoomSensitivity;
                    _lastPointerDistance = _currentPointerDistance;
                }
            }
        }
    }

    private void PanCamera(Vector2 delta)
    {
        if (_isMovingToNewPosition) return;

        Vector3 movement = -cameraToControl.right * delta.x * panningXSensitivity;
        movement -= (Quaternion.Euler(0f, _cameraTransform.eulerAngles.y, 0f) * Vector3.forward) * delta.y * panningYSensitivity;
        movement.y = 0f;
        _panOffset += movement;
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

    private IEnumerator DoMoveCameraToPosition(float newXAngle, float newYAngle, float newDistance)
    {
        _isMovingToNewPosition = true;
        xAngle = newXAngle;
        yAngle = newYAngle;
        distance = newDistance;
        yield return new WaitForEndOfFrame();
        _isMovingToNewPosition = false;
    }

    public void Reset()
    {
        defaultOrbitPoint.position = Vector3.zero;
        xAngle = 0f;
        yAngle = 45f;
        distance = 10f;
        _panOffset = Vector3.zero;
    }
}
