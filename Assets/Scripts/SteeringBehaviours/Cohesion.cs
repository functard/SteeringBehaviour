using UnityEngine;

public class Cohesion : SteeringBehaviour
{
    public override Vector3 CalculateSteeringBehaviour()
    {
        if (SteeringMotor.Fov.ObjectsInFov.Count == 0)
            return Vector3.zero;

        Vector3 averagePos = Vector3.zero;
        foreach (GameObject other in SteeringMotor.Fov.ObjectsInFov)
        {
            averagePos += other.transform.position;
        }
        averagePos /= SteeringMotor.Fov.ObjectsInFov.Count;
        m_DesiredVelocity = (averagePos - transform.position).normalized * SteeringMotor.MaxSpeed;
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
            Gizmos.DrawLine(transform.position,m_DesiredVelocity - SteeringMotor.Velocity);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, SteeringMotor.Velocity);
        }
    }
}
