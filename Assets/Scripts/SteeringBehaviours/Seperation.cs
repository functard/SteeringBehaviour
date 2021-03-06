using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seperation : SteeringBehaviour
{
    public override Vector3 CalculateSteeringBehaviour()
    {
        if (SteeringMotor.Fov.ObjectsInFov.Count == 0)
            return Vector3.zero;

        Vector3 averagePosDiff = Vector3.zero;
        foreach (GameObject other in SteeringMotor.Fov.ObjectsInFov)
        {
            if (other == this)
                Debug.Log("dasdsa");    
            Vector3 diff = transform.position - other.transform.position;
            //Debug.Log(diff.magnitude);

            averagePosDiff += diff.normalized; // scale it inversely proportional to the distance sqr
        }
        averagePosDiff /= SteeringMotor.Fov.ObjectsInFov.Count;

        m_DesiredVelocity = averagePosDiff.normalized * SteeringMotor.MaxSpeed;
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
            Gizmos.DrawLine(transform.position, transform.position + SteeringMotor.Velocity);
        }
    }
}
