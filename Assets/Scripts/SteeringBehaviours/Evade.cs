using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : SteeringBehaviour
{
    [SerializeField] private Motor m_Target;

    private bool m_PercieveUnitsGlobally = true;

    public override Vector3 CalculateSteeringBehaviour()
    {
        if (m_PercieveUnitsGlobally)
            return CalculateSteeringBehaviourGloabally();

        return CalculateSteeringBehaviourFOV(m_Target);
    }

    /// <summary>
    /// Has global knowledge on targets.
    /// </summary>
    /// <param name="_target">Target to evade.</param>
    /// <returns></returns>
    private Vector3 CalculateSteeringBehaviourGloabally()
    {
        float dist = Vector3.Distance(m_Target.transform.position, transform.position);
        float timeAhead = dist / SteeringMotor.MaxSpeed;

        Vector3 futurePos = m_Target.transform.position + m_Target.Velocity * timeAhead;

        m_DesiredVelocity = (futurePos - transform.position).normalized * SteeringMotor.MaxSpeed;
        return -(m_DesiredVelocity - SteeringMotor.Velocity);
    }

    /// <summary>
    /// Perceives units based on the Field of View.
    /// </summary>
    /// <param name="_target">Target to compare in FOV.</param>
    /// <returns></returns>
    private Vector3 CalculateSteeringBehaviourFOV(Motor _target)
    {
        if (SteeringMotor.Fov.ObjectsInFov.Contains(_target.gameObject))
        {
            float dist = Vector3.Distance(_target.transform.position, transform.position);
            float timeAhead = dist / SteeringMotor.MaxSpeed;

            Vector3 futurePos = _target.transform.position + _target.Velocity * timeAhead;


            m_DesiredVelocity = (futurePos - transform.position).normalized * SteeringMotor.MaxSpeed;
            return -(m_DesiredVelocity - SteeringMotor.Velocity);
        }
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (m_Debug)
        {
            if (SteeringMotor == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, -m_DesiredVelocity);
        }
    }
}
