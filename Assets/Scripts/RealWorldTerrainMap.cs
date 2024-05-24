using InfinityCode.RealWorldTerrain;
using UnityEngine;
using UnityEngine.Events;

public class RealWorldTerrainMap : MonoBehaviour, IMap
{
    [SerializeField]
    private RealWorldTerrainContainer _terrain;

    public UnityEvent<MapPointer> OnMapClickBegin = new UnityEvent<MapPointer>();
    public UnityEvent<MapPointer> OnMapClickEnd = new UnityEvent<MapPointer>();

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
    }

    void Update()
    {
    }

    public void OnRaycastHit(MapPointer mapPointer)
    {
    }

    public void OnPointerClickBegin(MapPointer mapPointer)
    {
        OnMapClickBegin.Invoke(mapPointer);
    }

    public void OnPointerClickEnd(MapPointer mapPointer)
    {
       OnMapClickEnd.Invoke(mapPointer);
    }
}
