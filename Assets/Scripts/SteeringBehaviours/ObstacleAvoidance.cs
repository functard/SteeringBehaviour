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

    private Vector3[] m_ObstacleDirs;

    private Vector3 obsDir;
    private const float ANGLE_INCREMENT = 1.61803f * Mathf.PI * 2;

    private void Start()
    {
        m_PrevBestDir = transform.forward;
        InitObstacleDirs();
    }


    public override Vector3 CalculateSteeringBehaviour()
    {
        if (!SteeringMotor.Is3D)
        {
            if (IsHeadingObstacle())
            {
                // scale inversely proportional to the distance
                Vector3 bestDir = CalculateBestDirGrounded();
                bestDir = TestGrounded();
                m_DesiredVelocity = bestDir.normalized / obsDir.sqrMagnitude /*/ bestDir.magnitude*/ * SteeringMotor.MaxSpeed;
                return m_DesiredVelocity - SteeringMotor.Velocity;
            }
            return Vector3.zero;
        }
        else
        {
            if (IsHeadingObstacle())
            {
                // scale inversely proportional to the distance
                Vector3 bestDir = CalculateBestDirFlying();
                m_DesiredVelocity = bestDir.normalized / bestDir.magnitude * SteeringMotor.MaxSpeed;
                return m_DesiredVelocity - SteeringMotor.Velocity;
            }
            return Vector3.zero;
        }
    }
    private void InitObstacleDirs()
    {
        m_ObstacleDirs = new Vector3[m_RayCount];
        if (!SteeringMotor.Is3D)
        {
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
                m_ObstacleDirs[i] = new Vector3(x, 0, z);
            }
        }
        else
        {
            for (int i = 0; i < m_RayCount; i++)
            {
                float t = (float)i / (m_RayCount - 1);
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = ANGLE_INCREMENT * i;

                float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                float z = Mathf.Cos(inclination);

                float viewAngle = SteeringMotor.Fov.FovType == FieldOfView.EFOVType.ANGULAR ? SteeringMotor.Fov.ViewAngle : 360;
                if (Vector3.Angle(m_ObstacleDirs[i], transform.forward) < viewAngle / 2)
                {
                    m_ObstacleDirs[i] = new Vector3(x, y, z);
                }
            }
        }
    }

    private bool IsHeadingObstacle()
    {
        Ray ray = new Ray(m_RayCastLocation.position, SteeringMotor.Velocity.normalized);
        RaycastHit hit;
        ray = new Ray(m_RayCastLocation.position, transform.forward.normalized);
        if (Physics.SphereCast(ray, 0.25f,out hit, m_ObstaclePerceptionRange, m_ObstacleLayer))
        {
            obsDir = hit.point - transform.position;
            return true;
        }

        return false;
    }

    private Vector3 TestGrounded()
    {
        List<RaycastHit> obstacles = new List<RaycastHit>();
        List<Vector3> freeDirs = new List<Vector3>();

        //if (!Physics.Raycast(m_RayCastLocation.position, m_PrevBestDir, m_ObstaclePerceptionRange, m_ObstacleLayer))
        //{
        //    return m_PrevBestDir;
        //}

        for (int i = m_RayCount - 1; i >= 0; i--)
        {
            Ray ray = new Ray(m_RayCastLocation.position, m_ObstacleDirs[i].normalized);

            RaycastHit hit;
            // if ray hits nothing
            if (!Physics.Raycast(ray, out hit, m_ObstaclePerceptionRange, m_ObstacleLayer))
            {
                freeDirs.Add(m_ObstacleDirs[i]);
                //freeDirs.Add(hit.point - transform.position);
                //return m_ObstacleDirs[i];
            }
            obstacles.Add(hit);
        }
        float best = -2f;
        int index = 0;
        for (int i = 0; i < freeDirs.Count; i++)
        {
            float value = Vector3.Dot(obsDir.normalized, freeDirs[i].normalized);
            if (value > best)
            {
                best = value;
                index = i;
            }

        }
        //Debug.Log(index);
        m_PrevBestDir = freeDirs[index];
        return freeDirs[index]; 
    }


    /// <summary>
    /// Calculates optimal direction 
    /// </summary>
    /// <returns>best dir scaled by the magnitude</returns>
    private Vector3 CalculateBestDirGrounded()
    {
        if (!Physics.Raycast(m_RayCastLocation.position, m_PrevBestDir, m_ObstaclePerceptionRange, m_ObstacleLayer))
        {
            return m_PrevBestDir;
        }

        List<RaycastHit> obstacles = new List<RaycastHit>();
        for (int i = m_RayCount - 1; i >= 0; i--)
        {
            Ray ray = new Ray(m_RayCastLocation.position, m_ObstacleDirs[i].normalized);

            RaycastHit hit;

            // if ray hits nothing
            if (!Physics.Raycast(ray, out hit, m_ObstaclePerceptionRange, m_ObstacleLayer))
            {
                m_PrevBestDir = m_ObstacleDirs[i];
                return m_ObstacleDirs[i];
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
        return (best.point - m_RayCastLocation.position);
    }

    private Vector3 CalculateBestDirFlying()
    {
        Ray r = new Ray(m_RayCastLocation.position, m_PrevBestDir.normalized);
        if (!Physics.SphereCast(r, 1f, m_ObstaclePerceptionRange, m_ObstacleLayer))
        {
            //Debug.DrawRay(m_RayCastLocation.position, m_PrevBestDir);
            return m_PrevBestDir;
        }

        List<RaycastHit> obstacles = new List<RaycastHit>();
        for (int i = 0; i < m_ObstacleDirs.Length; i++)
        {
            Ray ray = new Ray(m_RayCastLocation.position, m_ObstacleDirs[i].normalized);
            RaycastHit hit;

            // if ray hits nothing
            if (!Physics.SphereCast(ray, 1f, out hit, m_ObstaclePerceptionRange, m_ObstacleLayer))
            {
                //Debug.DrawRay(m_RayCastLocation.position, m_ObstacleDirs[i].normalized * m_ObstaclePerceptionRange);
                m_PrevBestDir = m_ObstacleDirs[i] * m_ObstaclePerceptionRange;
                return m_ObstacleDirs[i];
            }

            obstacles.Add(hit);
        }
        Debug.Log("asd");
        Vector3 best = Vector3.zero;
        // find the ray with the longest distance
        for (int i = 1; i < obstacles.Count; i++)
        {
            if ((obstacles[i].point - transform.position).sqrMagnitude + 1f >
                (best.normalized * m_ObstaclePerceptionRange - transform.position).sqrMagnitude)
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
            Gizmos.color = Color.red;
            //Gizmos.DrawRay(m_RayCastLocation.position, m_PrevBestDir * m_ObstaclePerceptionRange);
            for (int i = 0; i < m_RayCount; i++)
            {
                float t = (float)i / m_RayCount;
                Ray ray = new Ray(m_RayCastLocation.position, m_ObstacleDirs[i].normalized);
                RaycastHit hit;
                Gizmos.color = new Color(0, t, 0);
                //Gizmos.color = c
                Gizmos.DrawRay(m_RayCastLocation.position, m_ObstacleDirs[i].normalized * m_ObstaclePerceptionRange);
                if (Physics.Raycast(ray, out hit, m_ObstaclePerceptionRange, m_ObstacleLayer))
                {
                    //Gizmos.color = Color.blue;
                    Gizmos.DrawRay(m_RayCastLocation.position, m_ObstacleDirs[i].normalized * m_ObstaclePerceptionRange);
                }
            }
        }
    }
}
