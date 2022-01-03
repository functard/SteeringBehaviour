using System.Collections;
using UnityEngine;

public class Flee : SteeringBehaviour
{
    // targets to flee from
    private Transform[] m_Targets;

    private bool m_PercieveUnitsGlobally = true;

    public override Vector3 CalculateSteeringBehaviour()
    {
        if (m_PercieveUnitsGlobally)
            return CalculateSteeringBehaviourGlobal(m_Targets);

        return CalculateSteeringBehaviourFOV(m_Targets);
    }

    /// <summary>
    /// Flee from a point for a duration.
    /// </summary>
    /// <param name="_point">Point to flee from</param>
    /// <param name="_weigth">Force coeficent</param>
    /// <param name="_timer">Time in seconds</param>
    /// <returns></returns>
    public IEnumerator FleeFrom(Vector3 _point, float _weigth, float _timer = 1.25f)
    {
        StopCoroutine(FleeFrom(_point, _weigth));
        float time = 0f;
        while (_timer > time)
        {
            SteeringMotor.MaxSteeringForce = _weigth;
            m_DesiredVelocity = (_point - transform.position).normalized * SteeringMotor.MaxSpeed;
            SteeringMotor.AccumulateForce(-(m_DesiredVelocity - SteeringMotor.Velocity) * _weigth);
            //SteeringMotor.SteringForceAccumulation += -(m_DesiredVelocity - SteeringMotor.Velocity) * _weigth;
            time += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Has global knowledge on targets.
    /// </summary>
    /// <param name="_targets">Targets to flee from.</param>
    /// <returns>Steering force</returns>
    private Vector3 CalculateSteeringBehaviourGlobal(Transform[] _targets)
    {
        Vector3 sum = Vector3.zero;
        foreach (Transform target in _targets)
        {
            if (!target.gameObject.activeSelf)
                continue;

            m_DesiredVelocity = (target.position - transform.position).normalized * SteeringMotor.MaxSpeed;

            // calculate average position
            sum += -(m_DesiredVelocity - SteeringMotor.Velocity);
        }
        return sum;
    }

    /// <summary>
    /// Perceives units based on the Field of View.
    /// </summary>
    /// <param name="_targets">Targets to compare in FOV.</param>
    /// <returns>Steering force</returns>
    private Vector3 CalculateSteeringBehaviourFOV(Transform[] _targets)
    {
        Vector3 sum = Vector3.zero;
        foreach (Transform target in _targets)
        {
            if (!target.gameObject.activeSelf || !(SteeringMotor.Fov.ObjectsInFov.Contains(target.gameObject)))
                continue;

            m_DesiredVelocity = (target.position - transform.position).normalized * SteeringMotor.MaxSpeed;
            sum += -(m_DesiredVelocity - SteeringMotor.Velocity);
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
            Gizmos.DrawRay(transform.position, -m_DesiredVelocity);
        }
    }
}
