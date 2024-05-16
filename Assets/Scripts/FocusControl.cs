using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class FocusControl : MonoBehaviour
{
    [SerializeField]
    private Transform m_FocusTarget;
    [SerializeField]
    private Transform m_DesiredPoint;
    [SerializeField]
    private Transform m_map;

    [SerializeField]
    private float m_mapDepth = 5;

    [SerializeField]
    private Terrain m_terrain;

    void Update()
    {
        //if (!Input.GetKeyDown(KeyCode.Space))
        //{
        //    return;
        //}
        var tempPos = m_DesiredPoint.position;
        tempPos.y = m_terrain.transform.position.y + m_terrain.SampleHeight(tempPos);// GetGroundPos(tempPos).y;
        m_DesiredPoint.position = tempPos;

       
        var camPos = SceneView.lastActiveSceneView.camera.transform.position;
        var dir = transform.position -camPos;

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        var desiredPos = transform.position + (dir.normalized * m_mapDepth);

        m_FocusTarget.transform.position = desiredPos;

     
        var offset = m_DesiredPoint.position - m_FocusTarget.position;
        m_map.position -= offset;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, dir.normalized, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(SceneView.lastActiveSceneView.camera.transform.position, -dir.normalized * hit.distance, Color.yellow);
            Debug.Log("Did Hit");

          
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }   
    }

    public Vector3 GetGroundPos(Vector3 atGlobalPos)
    {
        int layerMask = 1 << 8;
        RaycastHit hit;
        if (Physics.Raycast(atGlobalPos + new Vector3(0,19999,0), Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
           // Debug.DrawRay(SceneView.lastActiveSceneView.camera.transform.position, Vector3, Color.yellow);
            Debug.Log("Did Hit");

            return hit.point;   
        }
    
          //  Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        return atGlobalPos;
    }
}
