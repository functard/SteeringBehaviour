using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private int m_FrameCounter;
    private int m_FrameUpdateCount = 30;

    [SerializeField]
    private bool m_Debug = false;

    [SerializeField]
    private LayerMask m_SteeringObjectLayer;

    public static List<Motor> AllSteeringUnits = new List<Motor>();

    private enum EPERCEPTIONTYPE
    {
        COLLIDER,
        LIST
    }

    [SerializeField]
    private EPERCEPTIONTYPE m_PerceptionType = EPERCEPTIONTYPE.COLLIDER;

    public enum EFOVType
    {
        ANGULAR,
        RADIAL
    }

    [SerializeField]
    private EFOVType m_FovType = EFOVType.ANGULAR;

    public EFOVType FovType { get { return m_FovType; } }

    [SerializeField]
    private float m_PerceptionRadius = 10f;

    [SerializeField]
    private float m_ViewAngle = 200f;

    public float ViewAngle { get { return m_ViewAngle; } }


    public float PerceptionRadius
    {
        get { return m_PerceptionRadius; }
    }

    private List<GameObject> m_objectsInFov = new List<GameObject>();
    public List<GameObject> ObjectsInFov
    {
        get { return m_objectsInFov; }
    }

    private void Update()
    {
        if (m_FrameCounter > m_FrameUpdateCount)
            m_objectsInFov = GetObjectsInFOV();

        m_FrameCounter++;
    }

    private List<GameObject> GetObjectsInFOV()
    {
        if (m_PerceptionType == EPERCEPTIONTYPE.COLLIDER)
        {
            if (m_FovType == EFOVType.ANGULAR)
                return AngularCollider();
            else
                return RadialCollider();
        }
        else
        {
            if (m_FovType == EFOVType.ANGULAR)
                return AngularList();
            else
                return RadialList();
        }


        List<GameObject> AngularCollider()
        {
            Collider[] objectsInRadius = Physics.OverlapSphere(transform.position, m_PerceptionRadius, m_SteeringObjectLayer);
            List<GameObject> objectsInFov = new List<GameObject>();

            foreach (var other in objectsInRadius)
            {
                if (other.gameObject == this.gameObject)
                    continue;

                Vector3 diff = other.transform.position - transform.position;
                if (Vector3.Angle(transform.forward, diff) < m_ViewAngle / 2)
                {
                    objectsInFov.Add(other.gameObject);
                }
            }
            return objectsInFov;
        }

        List<GameObject> RadialCollider()
        {
            List<GameObject> objectsInFov = new List<GameObject>();
            Collider[] objectsInRadius = Physics.OverlapSphere(transform.position, m_PerceptionRadius, m_SteeringObjectLayer);
            foreach (var other in objectsInRadius)
            {
                if (other.gameObject == this.gameObject)
                    continue;

                objectsInFov.Add(other.gameObject);
            }
            return objectsInFov;
        }

        List<GameObject> RadialList()
        {
            List<GameObject> objectsInFov = new List<GameObject>();
            foreach (var other in AllSteeringUnits)
            {
                if (other.gameObject == this.gameObject)
                    continue;

                float sqrDist = (other.transform.position - transform.position).sqrMagnitude;
                if (sqrDist < m_PerceptionRadius * m_PerceptionRadius)
                    objectsInFov.Add(other.gameObject);
            }

            return objectsInFov;
        }

        List<GameObject> AngularList()
        {
            List<GameObject> objectsInFov = new List<GameObject>();
            foreach (var other in AllSteeringUnits)
            {
                if (other.gameObject == this.gameObject)
                    continue;

                float sqrDist = (other.transform.position - transform.position).sqrMagnitude;
                if (sqrDist < m_PerceptionRadius * m_PerceptionRadius)
                {
                    Vector3 diff = other.transform.position - transform.position;
                    if (Vector3.Angle(transform.forward, diff) < m_ViewAngle / 2)
                        objectsInFov.Add(other.gameObject);
                }
            }
            return objectsInFov;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_Debug)
        {
            if (this == null)
                return;

            if (m_FovType == EFOVType.RADIAL)
                Gizmos.DrawWireSphere(transform.position, m_PerceptionRadius);

            else
            {
                float angleLeft = (-m_ViewAngle / 2) + transform.eulerAngles.y;
                float angleRight = (m_ViewAngle / 2) + transform.eulerAngles.y;
                Vector3 left = new Vector3(Mathf.Sin(angleLeft * Mathf.Deg2Rad), 0, Mathf.Cos(angleLeft * Mathf.Deg2Rad));
                Vector3 right = new Vector3(Mathf.Sin(angleRight * Mathf.Deg2Rad), 0, Mathf.Cos(angleRight * Mathf.Deg2Rad));
                Gizmos.DrawLine(transform.position, transform.position + left * m_PerceptionRadius);
                Gizmos.DrawLine(transform.position, transform.position + right * m_PerceptionRadius);
                Gizmos.DrawWireSphere(transform.position, m_PerceptionRadius);
            }
        }
    }
}