using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FocusControl : MonoBehaviour
{
    [SerializeField]
    private Transform m_DesiredPoint;

    [SerializeField]
    private Transform _startPoint;

    [SerializeField]
    private float m_mapDepth = 5;

    [SerializeField]
    private Camera runtimeCamera;

    private Vector2 _focusLatLong;

    private float _focusSpeed = .1f;

    private bool _running = true;

    [SerializeField] private RealWorldTerrainMap _terrain;

    private List<MapPointer> mapPointers = new List<MapPointer>();
    private Dictionary<MapPointer, Vector3> previousPositions = new Dictionary<MapPointer, Vector3>();
    private float initialDistance = 0f;
    private float _mapZoom = 1f;
    private Vector3 targetPosition;
    private float smoothSpeed = 0.125f;

    private void Awake()
    {
        _terrain.OnMapClickBegin.AddListener(OnPointerClickBegin);
        _terrain.OnMapClickEnd.AddListener(OnPointerClickEnd);

        _focusLatLong = _terrain.GetCoordinates(_startPoint.position);
        targetPosition = _terrain.transform.position;
        Debug.Log("Set start coordinates to " + _focusLatLong);
    }

    void Update()
    {
        if (mapPointers.Count == 1)
        {
            UpdateSinglePointer();
        }
        else if (mapPointers.Count > 1)
        {
            UpdateMultiplePointers();
        }

        // Smoothly interpolate to the target position
        _terrain.transform.position = Vector3.Lerp(_terrain.transform.position, targetPosition, smoothSpeed);

        var desiredFocusPoint = _terrain.GetGlobalPos(_focusLatLong.x, _focusLatLong.y);
        m_DesiredPoint.position = desiredFocusPoint;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var camPos = SceneView.lastActiveSceneView.camera.transform.position;
            UpdateCameraPosition(camPos, desiredFocusPoint);
        }
#endif

        if (!_running)
        {
            return;
        }
        if (Application.isPlaying)
        {
            var camPos = runtimeCamera.transform.position;
            UpdateCameraPosition(camPos, desiredFocusPoint);
        }
    }

    public void OnPointerClickBegin(MapPointer mapPointer)
    {
        mapPointers.Add(mapPointer);
        previousPositions[mapPointer] = mapPointer.transform.position;

        if (mapPointers.Count == 1)
        {
            previousPositions[mapPointer] = mapPointer.transform.position;
        }
        else if (mapPointers.Count == 2)
        {
            initialDistance = Vector3.Distance(mapPointers[0].transform.position, mapPointers[1].transform.position);
        }

        Toggle(false);
    }

    public void OnPointerClickEnd(MapPointer mapPointer)
    {
        mapPointers.Remove(mapPointer);
        previousPositions.Remove(mapPointer);
        if (mapPointers.Count < 2)
        {
            initialDistance = 0f; // Reset initial distance when less than 2 pointers are left
        }
        if(mapPointers.Count == 0)
        {
            Toggle(true);
        }
    
    }

    private void UpdateSinglePointer()
    {
        var pointer = mapPointers[0];
        var currentPosition = pointer.transform.position;

        if (previousPositions.TryGetValue(pointer, out var prevPosition))
        {
            var deltaPosition = currentPosition - prevPosition;

            // Apply delta position to the target position's x and z positions
            targetPosition.x += deltaPosition.x;
            targetPosition.z += deltaPosition.z;

            // Debug logging for delta positions
            Debug.Log($"Previous Position: {prevPosition}, Current Position: {currentPosition}, Delta Position: {deltaPosition}");
        }

        previousPositions[pointer] = currentPosition;
    }

    private void UpdateMultiplePointers()
    {
        // Calculate average position of pointers
        Vector3 averagePosition = Vector3.zero;
        foreach (var pointer in mapPointers)
        {
            averagePosition += pointer.transform.position;
        }
        averagePosition /= mapPointers.Count;

        // Calculate direction from _hmd to average position
        Vector3 direction = (averagePosition - runtimeCamera.transform.position).normalized;

        // Handle zoom
        float currentDistance = Vector3.Distance(mapPointers[0].transform.position, mapPointers[1].transform.position);
        if (initialDistance == 0f)
        {
            initialDistance = currentDistance;
        }
        else
        {
            float deltaDistance = initialDistance - currentDistance; // Invert zoom direction

            // Ensure zoom distance doesn't go negative
            _mapZoom = Mathf.Max(0f, _mapZoom - deltaDistance * 1f); // Adjust the multiplier as needed for zoom sensitivity

            // Apply movement along the direction
            targetPosition += direction * deltaDistance;

            initialDistance = currentDistance;
        }

        // Handle focus coordinates
        var currentPositionPointer1 = mapPointers[0].transform.position;
        var currentPositionPointer2 = mapPointers[1].transform.position;

        if (previousPositions.TryGetValue(mapPointers[0], out var prevPosition1) &&
            previousPositions.TryGetValue(mapPointers[1], out var prevPosition2))
        {
            var deltaPosition1 = currentPositionPointer1 - prevPosition1;
            var deltaPosition2 = currentPositionPointer2 - prevPosition2;

            var averageDeltaPosition = (deltaPosition1 + deltaPosition2) / 2;

            // Apply average delta position to the target position's x and z positions
            targetPosition.x += averageDeltaPosition.x;
            targetPosition.z += averageDeltaPosition.z;

            // Debug logging for delta positions
            Debug.Log($"Previous Position 1: {prevPosition1}, Current Position 1: {currentPositionPointer1}, Delta Position 1: {deltaPosition1}");
            Debug.Log($"Previous Position 2: {prevPosition2}, Current Position 2: {currentPositionPointer2}, Delta Position 2: {deltaPosition2}");
        }

        previousPositions[mapPointers[0]] = currentPositionPointer1;
        previousPositions[mapPointers[1]] = currentPositionPointer2;
    }

    public void SetFocusCoordintes(Vector2 vector2)
    {
        _focusLatLong = vector2;
    }

    private void UpdateCameraPosition(Vector3 camPos, Vector3 desiredFocusPoint)
    {
        var dir = transform.position - camPos;
        var desiredPos = transform.position + (dir.normalized * m_mapDepth);
        var offset = desiredFocusPoint - desiredPos;
        desiredPos = _terrain.transform.position - offset;
        _terrain.transform.position = Vector3.Lerp(_terrain.transform.position, desiredPos, Time.deltaTime * _focusSpeed);
    }

    private void CalculateFocusCoordinates()
    {
        var dir = transform.position - runtimeCamera.transform.position;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100);

        foreach (RaycastHit hit in hits)
        {
            var mapComponent = hit.collider.GetComponentInParent<IMap>();
            if (mapComponent != null)
            {
                SetFocusDepth(Vector3.Distance(transform.position, hit.point));
                SetFocusCoordintes(_terrain.GetCoordinates(hit.point));
            }
        }
    }

    public void SetFocusDepth(float mapZoom)
    {
        m_mapDepth = mapZoom;
    }

    public void Toggle(bool on)
    {
        _running = on;
        if (on)
        {
            CalculateFocusCoordinates();
        }
    }
}
