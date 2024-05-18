using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class MRRigHandAnimator : XRHandSkeletonDriver
{
    protected override void OnRootPoseUpdated(Pose rootPose)
    {
        // base.OnRootPoseUpdated(rootPose);
        rootTransform.localPosition = Vector3.zero;
        rootTransform.localRotation = Quaternion.identity;  
       
    }
}
