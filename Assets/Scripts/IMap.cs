using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMap 
{
    public float GetGlobalGroundPosition(Vector3 queryPoint);
    public void OnRaycastHit(MapPointer mapPointer);
    void OnPointerClickBegin(MapPointer mapPointer);
    void OnPointerClickEnd(MapPointer mapPointer);
}
