using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Hands;

public class MRRig : MonoBehaviour
{
    private XRHandSubsystem handSubsystem;
    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    [SerializeField] private MRRigHand leftHand;
    [SerializeField] private MRRigHand rightHand;

  
    void Start()
    {
        var arCameraManager = GetComponentInChildren<ARCameraManager>();
        arCameraManager.requestedBackgroundRenderingMode = CameraBackgroundRenderingMode.Any; // Adjust as needed

        InitializeDevices();
    }

    void Update()
    {
        CheckDeviceValidity();

        bool leftHandTracked = false;
        bool rightHandTracked = false;

        if (handSubsystem != null)
        {
            leftHandTracked = handSubsystem.leftHand.isTracked;
            rightHandTracked = handSubsystem.rightHand.isTracked;
        }

        if (leftHandTracked)
        {
            leftHand.UpdateHandPosition(handSubsystem.leftHand);
            leftHand.DetectPinchGesture(handSubsystem.leftHand);
        }
        else
        {
            leftHand.gameObject.SetActive(false);
        }

        if (rightHandTracked)
        {
            rightHand.UpdateHandPosition(handSubsystem.rightHand);
            rightHand.DetectPinchGesture(handSubsystem.rightHand);
        }
        else
        {
            rightHand.gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        // Properly clean up VR resources
        Debug.Log("Exiting play mode, cleaning up...");
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    void CheckDeviceValidity()
    {
        if (!leftHandDevice.isValid || !rightHandDevice.isValid)
        {
            InitializeDevices();
        }
    }

    void InitializeDevices()
    {
        Debug.Log("Initializing devices...");

        var inputDevices = new List<InputDevice>();

        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, inputDevices);
        if (inputDevices.Count > 0)
        {
            leftHandDevice = inputDevices[0];
            Debug.Log("Left hand device initialized.");
        }

        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, inputDevices);
        if (inputDevices.Count > 0)
        {
            rightHandDevice = inputDevices[0];
            Debug.Log("Right hand device initialized.");
        }

        InitializeHandSubsystem();
    }

    void InitializeHandSubsystem()
    {
        Debug.Log("Initializing hand subsystem...");

        List<XRHandSubsystemDescriptor> descriptors = new List<XRHandSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors(descriptors);

        if (descriptors.Count > 0)
        {
            handSubsystem = descriptors[0].Create();
            handSubsystem.Start();
            Debug.Log("Hand subsystem started.");
        }
        else
        {
            Debug.LogWarning("No hand subsystem descriptors found.");
        }
    }



 
}
