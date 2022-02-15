using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomManager : MonoBehaviour
{
    private List<Motor> m_Units;

    private Camera m_Cam;

    private void Awake()
    {
        m_Cam = Camera.main;
    }
    private void Start()
    {
        Bounds b;
        m_Units = FieldOfView.AllSteeringUnits;   
    }

    private void LateUpdate()
    {
        Vector3 sum = Vector3.zero;
        foreach (var item in m_Units)
        {
            sum += item.transform.position;
        }
        m_Cam.transform.position = (sum / m_Units.Count) + Vector3.up * 55;
    }
}
