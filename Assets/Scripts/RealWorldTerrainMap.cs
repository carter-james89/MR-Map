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
        initialDistance = _focusControl.GetMapDepth();
    }

    void Update()
    {
        if (mapPointers.Count > 1)
        {
            // Calculate the delta distance between the first two pointers
            float currentDistance = Vector3.Distance(mapPointers[0].transform.position, mapPointers[1].transform.position);
            if (initialDistance == 0f)
            {
                initialDistance = currentDistance;
            }
            else
            {
                float deltaDistance = currentDistance - initialDistance;
                _mapZoom += deltaDistance * 1f; // Adjust the multiplier as needed for zoom sensitivity
                _mapZoom = Mathf.Clamp(_mapZoom, 0.1f, 20f); // Clamp zoom level to a range
                _focusControl.SetFocusDepth(_mapZoom);
                initialDistance = currentDistance;
            }
        }
        else
        {
            initialDistance = 0f; // Reset initial distance when there are less than 2 pointers
        }
    }

    public void OnRaycastHit(MapPointer mapPointer)
    {
    }

    public void OnPointerClickBegin(MapPointer mapPointer)
    {
        mapPointers.Add(mapPointer);

        if (mapPointers.Count == 1)
        {
            _focusControl.SetFocusCoordintes(GetCoordinates(mapPointer.transform.position));
        }
        else if (mapPointers.Count > 1)
        {
            Vector3 averagePosition = Vector3.zero;
            foreach (var pointer in mapPointers)
            {
                averagePosition += pointer.transform.position;
            }
            averagePosition /= mapPointers.Count;
            _focusControl.SetFocusCoordintes(GetCoordinates(averagePosition));
        }
    }

    public void OnPointerClickEnd(MapPointer mapPointer)
    {
        mapPointers.Remove(mapPointer);
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
