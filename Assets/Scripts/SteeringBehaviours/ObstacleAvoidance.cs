using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : SteeringBehaviour
{
    [SerializeField]
    private Transform m_RayCastLocation;

    [SerializeField]
    private float m_ObstaclePerceptionRange;

    [SerializeField]
    private LayerMask m_ObstacleLayer;

    [SerializeField]
    private int m_RayCount;

    private Vector3 m_PrevBestDir;

    private void Start()
    {
        m_PrevBestDir = transform.forward;
    }
    public override Vector3 CalculateSteeringBehaviour()
    {
        if (!SteeringMotor.Is3D)
        {
            if (IsHeadingObstacle())
            {
                // scale inversely proportional to the distance
                m_DesiredVelocity = CalculateBestDirGrounded().normalized / CalculateBestDirGrounded().magnitude * SteeringMotor.MaxSpeed;
                return m_DesiredVelocity - SteeringMotor.Velocity;
            }
            return Vector3.zero;
        }
        else
        {
            if (IsHeadingObstacle())
            {
                // scale inversely proportional to the distance
                m_DesiredVelocity = CalculateBestDirFlying().normalized / CalculateBestDirGrounded().magnitude * SteeringMotor.MaxSpeed;
                return m_DesiredVelocity - SteeringMotor.Velocity;
            }
            return Vector3.zero;
        }
    }

    private bool IsHeadingObstacle()
    {
        Ray ray = new Ray(m_RayCastLocation.position, SteeringMotor.Velocity.normalized);
        if (Physics.SphereCast(ray, 1, m_ObstaclePerceptionRange, m_ObstacleLayer))
            return true;

        return false;
    }

    /// <summary>
    /// Calculates optimal direction 
    /// </summary>
    /// <returns>best dir scaled by the magnitude when heading to obstacle</returns>
    private Vector3 CalculateBestDirGrounded()
    {
        if (!Physics.Raycast(m_RayCastLocation.position, m_PrevBestDir, m_ObstaclePerceptionRange, m_ObstacleLayer))
        {
            return m_PrevBestDir;
        }

        List<RaycastHit> obstacles = new List<RaycastHit>();
        for (int i = 0; i < m_RayCount; i++)
        {
            float t = (float)i / (m_RayCount - 1);

            float viewAngle = SteeringMotor.Fov.FovType == FieldOfView.EFOVType.ANGULAR ? SteeringMotor.Fov.ViewAngle : 360;

            // angle in degree
            float angle = t * viewAngle - viewAngle / 2 + transform.eulerAngles.y;

            // angle in radians
            angle *= Mathf.Deg2Rad;

            float x = Mathf.Sin(angle) * Mathf.Rad2Deg;
            float z = Mathf.Cos(angle) * Mathf.Rad2Deg;
            Vector3 dir = new Vector3(x, 0, z);

            Ray ray = new Ray(m_RayCastLocation.position, dir.normalized);
            RaycastHit hit;

            // if ray hits nothing
            if (!Physics.Raycast(ray, out hit, m_ObstaclePerceptionRange, m_ObstacleLayer))
            {
                m_PrevBestDir = dir.normalized;
                return m_PrevBestDir;
            }
            obstacles.Add(hit);
        }

        var best = obstacles[0];
        // find the ray with the longest distance
        for (int i = 1; i < obstacles.Count; i++)
        {
            if ((obstacles[i].point - transform.position).sqrMagnitude + 1f > (best.point - transform.position).sqrMagnitude)
            {
                best = obstacles[i];
            }
        }
        Debug.DrawRay(m_RayCastLocation.position, (best.point - m_RayCastLocation.position).normalized * m_ObstaclePerceptionRange, Color.red);
        return (best.point - m_RayCastLocation.position);
    }

    private Vector3 CalculateBestDirFlying()
    {
        List<RaycastHit> obstacles = new List<RaycastHit>();

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < m_RayCount; i++)
        {
            float t = (float)i / (m_RayCount - 1);
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;

            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);

            Vector3 dir = new Vector3(x, y, z) * m_ObstaclePerceptionRange;

            float viewAngle = SteeringMotor.Fov.FovType == FieldOfView.EFOVType.ANGULAR ? SteeringMotor.Fov.ViewAngle : 360;

            // TO DO : optimize angle check
            if (Vector3.Angle(dir, transform.forward) < viewAngle / 2)
            {
                Ray ray = new Ray(m_RayCastLocation.position, dir.normalized);
                RaycastHit hit;


                if (!Physics.Raycast(m_RayCastLocation.position, m_PrevBestDir, out hit, m_ObstaclePerceptionRange, m_ObstacleLayer))
                {
                    Debug.DrawRay(m_RayCastLocation.position, dir.normalized * m_ObstaclePerceptionRange, Color.red);
                    return m_PrevBestDir.normalized;
                }

                // if ray hits nothing
                if (!Physics.Raycast(ray, out hit, m_ObstaclePerceptionRange, m_ObstacleLayer))
                {
                    Debug.DrawRay(m_RayCastLocation.position, dir.normalized * m_ObstaclePerceptionRange);
                    m_PrevBestDir = dir.normalized;
                    return dir.normalized;
                }

                obstacles.Add(hit);
            }
        }
        Vector3 best = m_PrevBestDir * m_ObstaclePerceptionRange;
        // find the ray with the longest distance
        for (int i = 1; i < obstacles.Count; i++)
        {
            //if ((obstacles[i].point - transform.position).sqrMagnitude + 0.1f > (best.point - transform.position).sqrMagnitude)
            //{
            //    best = obstacles[i];
            //}

            if ((obstacles[i].point - transform.position).sqrMagnitude + 1f > (m_PrevBestDir * m_ObstaclePerceptionRange - transform.position).sqrMagnitude)
            {
                best = obstacles[i].point;
            }
        }
        return (best - m_RayCastLocation.position);
    }

    private void OnDrawGizmos()
    {
        if (m_Debug && SteeringMotor != null)
        {
            for (int i = 0; i < m_RayCount; i++)
            {
                float t = (float)i / (m_RayCount - 1);

                // angle in degree
                float angle = t * SteeringMotor.Fov.ViewAngle - SteeringMotor.Fov.ViewAngle / 2 + transform.eulerAngles.y;

                // angle in radians
                angle *= Mathf.Deg2Rad;

                float x = Mathf.Sin(angle) * Mathf.Rad2Deg;
                float z = Mathf.Cos(angle) * Mathf.Rad2Deg;
                Vector3 dir = new Vector3(x, 0, z);
                Ray ray = new Ray(m_RayCastLocation.position, dir);
                RaycastHit hit;
                Gizmos.color = Color.green;
                Gizmos.DrawRay(m_RayCastLocation.position, dir.normalized * m_ObstaclePerceptionRange);
                if (!Physics.Raycast(ray, out hit, m_ObstaclePerceptionRange, m_ObstacleLayer))
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(m_RayCastLocation.position, dir.normalized * m_ObstaclePerceptionRange);
                }
            }
        }
    }
}
