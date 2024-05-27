using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class JumpToMap : MonoBehaviour
{
    private IMap map;
   [SerializeField] private MapPointer mapPointer;
    [SerializeField] private Transform user;
    [SerializeField]MRSandboxController controller;
    [SerializeField]private FocusControl focusControl;
    [SerializeField] private ARCameraManager passthrough;
    // Start is called before the first frame update
    void Start()
    {
        map = GetComponent<IMap>();
    }

    public void PutUserOnMap()
    {
        user.transform.SetParent(transform.GetChild(0));
        user.transform.localScale = Vector3.one;
        user.transform.position = mapPointer.transform.position;
        controller.ToggleSandbox(false);
        focusControl.gameObject.SetActive(false);
        passthrough.enabled = false;
        passthrough.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
