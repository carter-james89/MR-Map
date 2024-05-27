using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;

public class MRRigHand : MonoBehaviour
{
    public UnityEvent OnPinchBegin;
    public UnityEvent OnPinchEnd;
    public UnityEvent OnGripBegin;
    public UnityEvent OnGripEnd;

    public Transform _wristRoot;
    [SerializeField] private float pinchThreshold = 0.02f; // Adjust as needed
    [SerializeField] private float gripDotThreshold = 0.9f; // Adjust as needed

    public HandPose gripPose;

    public bool IsPinching { get; private set; } = false;
    public bool IsGripping { get; private set; } = false;

    public void UpdateHandPosition(XRHand hand)
    {
        transform.localPosition = hand.rootPose.position;
        transform.localRotation = hand.rootPose.rotation;
        gameObject.SetActive(true);
    }

    public void DetectPinchGesture(XRHand hand)
    {
        var handName = name;

        if (hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out var thumbPose) &&
            hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var indexPose))
        {
            float distance = Vector3.Distance(thumbPose.position, indexPose.position);

            if (distance < pinchThreshold)
            {
                if (!IsPinching)
                {
                    IsPinching = true;
                    OnPinchBegin.Invoke();
                }
            }
            else
            {
                if (IsPinching)
                {
                    IsPinching = false;
                    OnPinchEnd.Invoke();
                }
            }
        }
    }

    public void DetectGripPose()
    {
        if (_wristRoot == null || gripPose == null || gripPose.jointRotations.Count == 0)
        {
            return;
        }

        float totalDot = 0f;
        int jointCount = 0;

        // Get all the transforms under _wristRoot
        var jointTransforms = _wristRoot.GetComponentsInChildren<Transform>();

        // Compare each joint's rotation with the corresponding rotation in the grip pose
        for (int i = 0; i < jointTransforms.Length && i < gripPose.jointRotations.Count; i++)
        {
            Vector3 currentForward = jointTransforms[i].localRotation * Vector3.forward;
            Vector3 poseForward = gripPose.jointRotations[i] * Vector3.forward;
            totalDot += Vector3.Dot(currentForward, poseForward);
            jointCount++;
        }

        if (jointCount > 0)
        {
            float averageDot = totalDot / jointCount;
            Debug.Log(averageDot);

            if (averageDot > gripDotThreshold)
            {
                if (!IsGripping)
                {
                    GetComponent<AudioSource>().Play();
                    IsGripping = true;
                    OnGripBegin.Invoke();
                }
            }
            else
            {
                if (IsGripping)
                {
                    IsGripping = false;
                    OnGripEnd.Invoke();
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
        // Example of calling both detection methods
        // if (TryGetComponent(out XRHand hand))
        //{
        //    DetectPinchGesture(hand);
        //    DetectGripPose(hand);
        //}
    }
}
