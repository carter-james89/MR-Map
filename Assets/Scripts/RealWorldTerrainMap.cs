using InfinityCode.RealWorldTerrain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealWorldTerrainMap : MonoBehaviour, IMap
{
    [SerializeField]
    private RealWorldTerrainContainer _terrain;

    public Vector3 GetGlobalPos(float lat, float lon)
    {
        Vector3 outPos = Vector3.zero;
        bool validCoordinates = _terrain.GetWorldPosition((double)lat, (double)lon, out outPos);
      //  Debug.Log(validCoordinates);
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
        //_terrain.
        return (float)_terrain.GetAltitudeByWorldPosition(queryPoint);
    }

    private void Awake()
    {
       // _terrain = GetComponent<RealWorldTerrainContainer>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
