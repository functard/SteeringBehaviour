using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Allignment : SteeringBehaviour
{
    public override Vector3 CalculateSteeringBehaviour()
    {
        if (SteeringMotor.Fov.ObjectsInFov.Count == 0)
            return Vector3.zero;

        m_DesiredVelocity = CalculateAverageDir() * SteeringMotor.MaxSpeed;
        return  m_DesiredVelocity - SteeringMotor.Velocity;
    }

    private Vector3 CalculateAverageDir()
    {
        Vector3 sum = Vector3.zero;
        foreach (GameObject other in SteeringMotor.Fov.ObjectsInFov)
        {
            sum += other.transform.forward;
        }
        return (sum / SteeringMotor.Fov.ObjectsInFov.Count).normalized;
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
