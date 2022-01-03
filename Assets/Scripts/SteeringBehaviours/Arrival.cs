using UnityEngine;

public class Arrival : SteeringBehaviour
{
    [SerializeField] private Transform m_Target;
    [SerializeField] private float m_StoppingDistance = 10;

    public override Vector3 CalculateSteeringBehaviour()
    {
        float dist = (transform.position - m_Target.position).sqrMagnitude;

        float stoppingForce;
        if (dist * dist < m_StoppingDistance)
            stoppingForce = dist * dist / m_StoppingDistance;
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
}
