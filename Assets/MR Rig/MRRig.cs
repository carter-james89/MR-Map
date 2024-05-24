using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class MRRig : MonoBehaviour
{
    private XRHandSubsystem handSubsystem;
    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    [SerializeField] private MRRigHand leftHand;
    [SerializeField] private MRRigHand rightHand;

    void Start()
    {
        if (!XRSettings.isDeviceActive)
        {
            Debug.LogError("No XR device is connected. Please connect your HMD.");
            return;
        }

        try
        {
            var arCameraManager = GetComponentInChildren<ARCameraManager>();
            arCameraManager.requestedBackgroundRenderingMode = CameraBackgroundRenderingMode.Any; // Adjust as needed

            InitializeDevices();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to initialize AR Camera Manager or devices: " + ex.Message);
        }
    }

    void Update()
    {
        if (!XRSettings.isDeviceActive)
        {
            return;
        }

        try
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
        catch (System.Exception ex)
        {
            Debug.LogError("Error in Update method: " + ex.Message);
        }
    }

    void OnDisable()
    {
        try
        {
            Debug.Log("Exiting play mode, cleaning up...");
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error in OnDisable method: " + ex.Message);
        }
    }

    void CheckDeviceValidity()
    {
        if (!XRSettings.isDeviceActive)
        {
            return;
        }

        try
        {
            if (!leftHandDevice.isValid || !rightHandDevice.isValid)
            {
                InitializeDevices();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error in CheckDeviceValidity method: " + ex.Message);
        }
    }

    void InitializeDevices()
    {
        if (!XRSettings.isDeviceActive)
        {
            return;
        }

        try
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
        catch (System.Exception ex)
        {
            Debug.LogError("Error in InitializeDevices method: " + ex.Message);
        }
    }

    void InitializeHandSubsystem()
    {
        if (!XRSettings.isDeviceActive)
        {
            return;
        }
        if(handSubsystem != null)
        {
            return;
        }
        try
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
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to start hand subsystem: " + ex.Message);
        }
    }
}
