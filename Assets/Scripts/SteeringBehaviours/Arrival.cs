using UnityEngine;

public class Arrival : SteeringBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private float m_StoppingDistance = 10;

    public override Vector3 CalculateSteeringBehaviour()
    {
        float dist = (transform.position - m_Target.position).sqrMagnitude;

        float stoppingForce;
        if (dist  < m_StoppingDistance * m_StoppingDistance)
            stoppingForce = dist / m_StoppingDistance * m_StoppingDistance;
        else
            stoppingForce = 1f;
        ////float sqrDist = (transform.position - m_target.position).sqrMagnitude;

        //float stoppingForce;

        //if (sqrDist * sqrDist < stoppingDistance)
        //    stoppingForce = sqrDist * sqrDist / stoppingDistance;
        //else
        //    stoppingForce = 1f;

        //float stoppingForce = sqrDist * sqrDist / stoppingDistance;

        m_DesiredVelocity = (m_Target.position - transform.position).normalized * SteeringMotor.MaxSpeed * stoppingForce;
        return m_DesiredVelocity - SteeringMotor.Velocity;
    }

    public void SetTarget(Transform _target)
    {
        m_Target = _target;
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
        }
    }
}
