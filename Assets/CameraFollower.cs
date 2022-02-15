using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private List<Motor> m_Units;

    private Camera m_Cam;

    [SerializeField] private Vector2 m_MinMaxCamDist = new Vector2(10, 100);

    private void Awake()
    {
        m_Cam = Camera.main;
    }
    private void Start()
    {
        m_Units = FieldOfView.AllSteeringUnits;
    }

    private void LateUpdate()
    {
        Vector3 sum = Vector3.zero;
        foreach (var item in m_Units)
        {
            sum += item.transform.position;
        }
        m_Cam.transform.position = (sum / m_Units.Count) + Vector3.up *
                                    Mathf.Clamp(Mathf.Sqrt(GreatestDist()), m_MinMaxCamDist.x, m_MinMaxCamDist.y);
    }
    private float GreatestDist()
    {
        float best = 0;

        for (int i = 0; i < m_Units.Count; i++)
        {
            for (int k = 0; k < m_Units.Count; k++)
            {
                float sqrDist = (m_Units[i].transform.position - m_Units[k].transform.position).sqrMagnitude;
                if (sqrDist > best)
                {
                    best = sqrDist;
                }
            }
        }
        return best;
    }
}
