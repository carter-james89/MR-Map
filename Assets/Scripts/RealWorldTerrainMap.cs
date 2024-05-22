using InfinityCode.RealWorldTerrain;
using System.Collections.Generic;
using UnityEngine;

public class RealWorldTerrainMap : MonoBehaviour, IMap
{
    [SerializeField]
    private RealWorldTerrainContainer _terrain;

    [SerializeField]
    private FocusControl _focusControl;

    private List<MapPointer> mapPointers = new List<MapPointer>();
    private Dictionary<MapPointer, Vector2> previousCoordinates = new Dictionary<MapPointer, Vector2>();
    private float initialDistance = 0f;
    private float _mapZoom = 1f;

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
    }

    private void UpdateSinglePointer()
    {
        var pointer = mapPointers[0];
        var currentCoordinates = GetCoordinates(pointer.transform.position);

        if (previousCoordinates.TryGetValue(pointer, out var prevCoordinates))
        {
            var deltaCoordinates = prevCoordinates - currentCoordinates; // Inverted delta

            // Handle longitude wrap-around
            if (Mathf.Abs(deltaCoordinates.x) > 180)
            {
                if (deltaCoordinates.x > 0)
                {
                    deltaCoordinates.x -= 360;
                }
                else
                {
                    deltaCoordinates.x += 360;
                }
            }

            var focusCoordinates = _focusControl.GetFocusLatLong();
            focusCoordinates += deltaCoordinates;
            _focusControl.SetFocusCoordintes(focusCoordinates);

            // Debug logging for delta coordinates
            Debug.Log($"Previous Coordinates: {prevCoordinates}, Current Coordinates: {currentCoordinates}, Delta Coordinates: {deltaCoordinates}");
        }

        previousCoordinates[pointer] = currentCoordinates;
    }

    private void UpdateMultiplePointers()
    {
        // Handle zoom
        float currentDistance = Vector3.Distance(mapPointers[0].transform.position, mapPointers[1].transform.position);
        if (initialDistance == 0f)
        {
            initialDistance = currentDistance;
        }
        else
        {
            float deltaDistance = currentDistance - initialDistance;
            _mapZoom -= deltaDistance * 1f; // Adjust the multiplier as needed for zoom sensitivity
            _mapZoom = Mathf.Clamp(_mapZoom, 0.1f, 20f); // Clamp zoom level to a range
            _focusControl.SetFocusDepth(_mapZoom);
            initialDistance = currentDistance;
        }

        // Handle focus coordinates
        var currentCoordinatesPointer1 = GetCoordinates(mapPointers[0].transform.position);
        var currentCoordinatesPointer2 = GetCoordinates(mapPointers[1].transform.position);

        if (previousCoordinates.TryGetValue(mapPointers[0], out var prevCoordinates1) &&
            previousCoordinates.TryGetValue(mapPointers[1], out var prevCoordinates2))
        {
            var deltaCoordinates1 = prevCoordinates1 - currentCoordinatesPointer1; // Inverted delta
            var deltaCoordinates2 = prevCoordinates2 - currentCoordinatesPointer2; // Inverted delta

            // Handle longitude wrap-around
            if (Mathf.Abs(deltaCoordinates1.x) > 180)
            {
                if (deltaCoordinates1.x > 0)
                {
                    deltaCoordinates1.x -= 360;
                }
                else
                {
                    deltaCoordinates1.x += 360;
                }
            }
            if (Mathf.Abs(deltaCoordinates2.x) > 180)
            {
                if (deltaCoordinates2.x > 0)
                {
                    deltaCoordinates2.x -= 360;
                }
                else
                {
                    deltaCoordinates2.x += 360;
                }
            }

            var averageDeltaCoordinates = (deltaCoordinates1 + deltaCoordinates2) / 2;

            var focusCoordinates = _focusControl.GetFocusLatLong();
            focusCoordinates += averageDeltaCoordinates;
            _focusControl.SetFocusCoordintes(focusCoordinates);

            // Debug logging for delta coordinates
            Debug.Log($"Previous Coordinates 1: {prevCoordinates1}, Current Coordinates 1: {currentCoordinatesPointer1}, Delta Coordinates 1: {deltaCoordinates1}");
            Debug.Log($"Previous Coordinates 2: {prevCoordinates2}, Current Coordinates 2: {currentCoordinatesPointer2}, Delta Coordinates 2: {deltaCoordinates2}");
        }

        previousCoordinates[mapPointers[0]] = currentCoordinatesPointer1;
        previousCoordinates[mapPointers[1]] = currentCoordinatesPointer2;
    }

    public void OnRaycastHit(MapPointer mapPointer)
    {
    }

    public void OnPointerClickBegin(MapPointer mapPointer)
    {
        mapPointers.Add(mapPointer);
        previousCoordinates[mapPointer] = GetCoordinates(mapPointer.transform.position);

        if (mapPointers.Count == 1)
        {
            previousCoordinates[mapPointer] = GetCoordinates(mapPointer.transform.position);
        }
        else if (mapPointers.Count == 2)
        {
            initialDistance = Vector3.Distance(mapPointers[0].transform.position, mapPointers[1].transform.position);
        }
    }

    public void OnPointerClickEnd(MapPointer mapPointer)
    {
        mapPointers.Remove(mapPointer);
        previousCoordinates.Remove(mapPointer);
        if (mapPointers.Count < 2)
        {
            initialDistance = 0f; // Reset initial distance when less than 2 pointers are left
        }
    }

    public List<MapPointer> GetMapPointers()
    {
        return new List<MapPointer>(mapPointers); // Return a copy to avoid external modifications
    }
}
