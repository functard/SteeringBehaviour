using UnityEngine;

public class Tentacle : MonoBehaviour
{
    [SerializeField]
    private int m_Resolution = 10;

    [SerializeField]
    private float m_SegmentSpeed = 0.2f;

    [SerializeField]
    private float m_SegmentDistance = 2f;

    private LineRenderer m_LineRenderer;
    private Vector3[] m_SegmentPositions;
    private Vector3[] m_SegmentVelocity;

    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        m_SegmentPositions = new Vector3[m_Resolution];
        m_SegmentVelocity = new Vector3[m_Resolution];
        m_LineRenderer.positionCount = m_Resolution;
    }

    private void Update()
    {
        m_SegmentPositions[0] = transform.position;
        for (int i = 1; i < m_Resolution; i++)
        {
            Vector3 targetPos = m_SegmentPositions[i - 1] + (m_SegmentPositions[i] - m_SegmentPositions[i - 1]).normalized * m_SegmentDistance;
            m_SegmentPositions[i] = Vector3.SmoothDamp(m_SegmentPositions[i], targetPos,  ref m_SegmentVelocity[i], m_SegmentSpeed);
        }
        m_LineRenderer.SetPositions(m_SegmentPositions);
    }
}
