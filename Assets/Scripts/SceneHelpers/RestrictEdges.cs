using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictEdges : MonoBehaviour
{
    [SerializeField]
    private Vector2 m_Edges = new Vector2(36, 21);

    [SerializeField]
    private bool m_AdjustToCameraBounds = true;

    private void Start()
    {
        if (m_AdjustToCameraBounds)
        {
            Vector2 right = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

            m_Edges.x = right.x;
            m_Edges.y = Camera.main.orthographicSize;
        }
    }
    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * 8;
        float x = transform.position.x;
        float z = transform.position.z;

        if (x < -m_Edges.x)
            transform.position = new Vector3(m_Edges.x, 0, z);
        if (x > m_Edges.x)
            transform.position = new Vector3(-m_Edges.x, 0, z);
        if (z < -m_Edges.y)
            transform.position = new Vector3(x, 0, m_Edges.y);
        if (z > m_Edges.y)
            transform.position = new Vector3(x, 0, -m_Edges.y);
    }
}
