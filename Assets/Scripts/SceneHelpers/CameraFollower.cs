using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private Camera m_Cam;
    private void Awake()
    {
        m_Cam = Camera.main;
    }

    void Update()
    {
        transform.position = m_Cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_Cam.transform.position.y));
            
    }
}
