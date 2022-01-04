using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private List<Vector3> m_PathNodes;
    [SerializeField] private bool m_Grounded = true;

    [SerializeField] private float m_RandomRange = 20f;
    [SerializeField] private float m_xInterval = 15f;
    [SerializeField] private int m_PathCount = 22;

    [SerializeField] private bool m_Debug;

    private void Awake()
    {
        CreateRandomPath();
    }

    public void AddPath(Vector3 _path)
    {
        m_PathNodes.Add(_path);
    }

    public List<Vector3> GetPath()
    {
        return m_PathNodes;
    }

    public Vector3 GetNodeAt(int _i)
    {
        return m_PathNodes[_i];
    }

    public void Reverse()
    {
        m_PathNodes.Reverse();
    }

    public void CreateRandomPath()
    {
        m_PathNodes.Clear();
        for (int i = 0; i < m_PathCount; i++)
        {
            float y = m_Grounded == true ? 0 : Random.Range(0, m_RandomRange);

            AddPath(transform.position + new Vector3(i * m_xInterval, y, Random.Range(0, m_RandomRange) + i * m_xInterval));
        }
    }

    private void OnDrawGizmos()
    {
        if (m_Debug)
        {
            for (int i = 1; i < m_PathNodes.Count; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(m_PathNodes[i - 1], m_PathNodes[i]);
                Gizmos.DrawSphere(m_PathNodes[i], 2f);
                if (i == 1)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(m_PathNodes[0], 2f);
                }
                if (i == m_PathNodes.Count -1)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(m_PathNodes[i], 2f);
                }
            }
        }
    }
}
