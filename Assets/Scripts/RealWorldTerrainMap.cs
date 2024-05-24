using InfinityCode.RealWorldTerrain;
using System.Collections.Generic;
using UnityEngine;

public class RealWorldTerrainMap : MonoBehaviour, IMap
{
    [SerializeField]
    private RealWorldTerrainContainer _terrain;

    [SerializeField]
    private FocusControl _focusControl;

    [SerializeField]
    private Transform _hmd;

    private List<MapPointer> mapPointers = new List<MapPointer>();
    private Dictionary<MapPointer, Vector3> previousPositions = new Dictionary<MapPointer, Vector3>();
    private float initialDistance = 0f;
    private float _mapZoom = 1f;
    private Vector3 targetPosition;
    private float smoothSpeed = 0.125f; // Adjust this value for more or less smoothing

    public Vector3 GetGlobalPos(float lat, float lon)
    {
        Vector3 outPos = Vector3.zero;
        bool validCoordinates = _terrain.GetWorldPosition((double)lat, (double)lon, out outPos);
        return outPos;
    }

    public Vector2 GetCoordinates(Vector3 worldPos)
    {
        Vector2 outCoord = Vector2.zero;
        _terrain.GetCoordinatesByWorldPosition(worldPos, out outCoord);
        return outCoord;
    }

    public float GetGlobalGroundPosition(Vector3 queryPoint)
    {
        return (float)_terrain.GetAltitudeByWorldPosition(queryPoint);
    }

    private void Awake()
    {
    }

    void Start()
    {
        _mapZoom = _focusControl.GetMapDepth();
        targetPosition = transform.position;
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
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
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
        Vector3 direction = (averagePosition - _hmd.position).normalized;

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

    public void OnRaycastHit(MapPointer mapPointer)
    {
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

        // Turn off the focus control when map is being clicked
        _focusControl.Toggle(false);
    }

    public void OnPointerClickEnd(MapPointer mapPointer)
    {
        mapPointers.Remove(mapPointer);
        previousPositions.Remove(mapPointer);
        if (mapPointers.Count < 2)
        {
            initialDistance = 0f; // Reset initial distance when less than 2 pointers are left
        }

        // Turn on the focus control when map clicking ends
        _focusControl.Toggle(true);
    }

    public List<MapPointer> GetMapPointers()
    {
        return new List<MapPointer>(mapPointers); // Return a copy to avoid external modifications
    }
}
