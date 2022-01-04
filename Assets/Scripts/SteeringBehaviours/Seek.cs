using System;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour
{
    [SerializeField] private List<Transform> m_Targets;

    [SerializeField]
    private bool m_PercieveUnitsGlobally = true;

    public override Vector3 CalculateSteeringBehaviour()
    {
        if (m_PercieveUnitsGlobally)
            return CalculateSteeringGlobally(m_Targets);

        return CalculateSteeringBehaviourFOV(m_Targets);
    }

    public void SetTarget(Transform _target)
    {
        m_Targets.Clear();
        m_Targets.Add(_target);
    }

    public void SetTargets(List<Transform> _targets)
    {
        m_Targets.Clear();
        foreach (var target in _targets)
        {
            m_Targets.Add(target);
        }
    }
    /// <summary>
    /// Has global knowledge on targets.
    /// </summary>
    /// <param name="_target">Target to seek.</param>
    /// <returns></returns>
    private Vector3 CalculateSteeringGlobally(List<Transform> _target)
    {
        Vector3 sum = Vector3.zero;
        foreach (Transform target in m_Targets)
        {
            if (!target.gameObject.activeSelf)
                continue;

            m_DesiredVelocity = (target.position - transform.position).normalized * SteeringMotor.MaxSpeed;
            sum += m_DesiredVelocity - SteeringMotor.Velocity;
        }
        return sum;
    }

    /// <summary>
    /// Perceives units based on the Field of View.
    /// </summary>
    /// <param name="_target">Target to compare in FOV.</param>
    /// <returns></returns>
    private Vector3 CalculateSteeringBehaviourFOV(List<Transform> _target)
    {
        Vector3 sum = Vector3.zero;
        foreach (Transform target in m_Targets)
        {
            if (!target.gameObject.activeSelf || !(SteeringMotor.Fov.ObjectsInFov.Contains(target.gameObject)))
                continue;

            m_DesiredVelocity = (target.position - transform.position).normalized * SteeringMotor.MaxSpeed;
            sum += m_DesiredVelocity - SteeringMotor.Velocity;
        }
        return sum;
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
