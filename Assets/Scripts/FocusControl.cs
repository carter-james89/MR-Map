using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class FocusControl : MonoBehaviour
{
    [SerializeField]
    private Transform m_DesiredPoint;

    [SerializeField]
    private Transform _startPoint;

    [SerializeField]
    private float m_mapDepth = 5;

    [SerializeField]
    private Camera runtimeCamera; // Serialized camera field for runtime

    private struct LatLong
    {
        public float Lat;
        public float Long;
    }
    private Vector2 _focusLatLong;

    // [SerializeField]
    // private Terrain m_terrain;

    [SerializeField]
    private RealWorldTerrainMap _terrain;
    [SerializeField]
    private float _focusSpeed = 1;

    private void OnEnable()
    {
        _focusLatLong = _terrain.GetCoordinates(_startPoint.position);
        Debug.Log("Set start coordinates to " + _focusLatLong);
    }

    void Update()
    {
        var desiredFocusPoint = _terrain.GetGlobalPos(_focusLatLong.x, _focusLatLong.y);
        m_DesiredPoint.position = desiredFocusPoint;

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            var camPos = SceneView.lastActiveSceneView.camera.transform.position;
            UpdateCameraPosition(camPos, desiredFocusPoint);
        }
#endif

        if (Application.isPlaying)
        {
            var camPos = runtimeCamera.transform.position;
            UpdateCameraPosition(camPos, desiredFocusPoint);
        }
    }

    private void UpdateCameraPosition(Vector3 camPos, Vector3 desiredFocusPoint)
    {
        var dir = transform.position - camPos;
        var desiredPos = transform.position + (dir.normalized * m_mapDepth);
        var offset = desiredFocusPoint - desiredPos;
        desiredPos = _terrain.transform.position - offset;
        _terrain.transform.position = Vector3.Lerp(_terrain.transform.position, desiredPos, Time.deltaTime * _focusSpeed);
    }
}
