using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;

public class MRRigHand : MonoBehaviour
{
    public UnityEvent OnPinchBegin;
    public UnityEvent OnPinchEnd;

    [SerializeField] private float pinchThreshold = 0.02f; // Adjust as needed

    public bool IsPinching { get; private set; } = false;

    public void UpdateHandPosition(XRHand hand)
    {
        transform.localPosition = hand.rootPose.position;
        transform.localRotation = hand.rootPose.rotation;
        gameObject.SetActive(true);
    }

    public void DetectPinchGesture(XRHand hand)
    {
        var handName = name;
        XRHandJoint thumbTip, indexTip;

        if (hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var thumbPose) &&
            hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var indexPose))
        {
            float distance = Vector3.Distance(thumbPose.position, indexPose.position);

            if (distance < pinchThreshold)
            {
                //Debug.Log($"{handName} is pinching.");
                if (!IsPinching)
                {
                    IsPinching = true;
                    OnPinchBegin.Invoke();
                }
            }
            else
            {
             //   Debug.Log($"{handName} is not pinching.");
                if (IsPinching)
                {
                    IsPinching = false;
                    OnPinchEnd.Invoke();
                }
               
            }
        }
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
