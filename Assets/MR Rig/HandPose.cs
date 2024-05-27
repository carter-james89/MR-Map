using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandPose", menuName = "ScriptableObjects/HandPose", order = 1)]
public class HandPose : ScriptableObject
{
    public List<Quaternion> jointRotations = new List<Quaternion>();
    public string title;
}
