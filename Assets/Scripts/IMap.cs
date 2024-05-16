using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMap 
{
    public float GetGlobalGroundPosition(Vector3 queryPoint);
}
