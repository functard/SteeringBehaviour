using UnityEngine;

public class PathFollowing : SteeringBehaviour
{
    [SerializeField] private bool m_Loop;

    [SerializeField] private Path m_PathNodes;

    [SerializeField] private float m_ReachDistTreshold = 7.5f;

    [SerializeField] private bool m_ResetPathOnComplete = true;

    private bool m_IsCompleted = false;

    private int m_LapCount = 0;

    private int m_PathIndex = 0;

    // TODO : remove path creation logic from this class
    public override Vector3 CalculateSteeringBehaviour()
    {
        if (m_PathNodes.GetPath().Count == 0 || (m_IsCompleted && !m_Loop))
        {
            return Vector3.zero;
        }

        Vector3 currPathNode = m_PathNodes.GetNodeAt(m_PathIndex);

        float sqrDist = (currPathNode - transform.position).sqrMagnitude;
        //float dist = (currPathNode - transform.position).magnitude;
        if (sqrDist < m_ReachDistTreshold * m_ReachDistTreshold)
        {
            m_PathIndex++;
        }
        if (m_PathIndex >= m_PathNodes.GetPath().Count)
        {
            m_PathIndex = 0;
            m_LapCount++;

            if (m_ResetPathOnComplete)
                m_PathNodes.CreateRandomPath();

            if (m_LapCount % 2 == 1)
                m_PathNodes.GetPath().Reverse();
        }
        return Seek(m_PathNodes.GetNodeAt(m_PathIndex));
    }

    private Vector3 Seek(Vector3 _target)
    {
        m_DesiredVelocity = (_target - transform.position).normalized * SteeringMotor.MaxSpeed;
        return m_DesiredVelocity - SteeringMotor.Velocity;
    }

    private void OnDrawGizmos()
    {
        if (m_Debug)
        {
            if (SteeringMotor == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, m_DesiredVelocity);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, m_DesiredVelocity - SteeringMotor.Velocity);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, SteeringMotor.Velocity);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, m_ReachDistTreshold);
        }
    }
}
