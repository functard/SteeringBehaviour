using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    private Motor m_steeringMotor;

    [SerializeField]
    private float m_BehaviourWeigth = 1;

    //[SerializeField]
    //protected bool m_PercieveUnitsGlobally = true;

    [SerializeField]
    protected bool m_Debug = false;

    protected Vector3 m_DesiredVelocity;

    public float BehaviourWeigth
    {
        get { return m_BehaviourWeigth; }
        set { m_BehaviourWeigth = value; }
    }
    protected Motor SteeringMotor
    {
        get { return m_steeringMotor; }
    }

    public abstract Vector3 CalculateSteeringBehaviour();

    private void Awake()
    {
        m_steeringMotor = GetComponent<Motor>();
    }

    private void Update()
    {
        Vector3 force = CalculateSteeringBehaviour() * BehaviourWeigth;
        m_steeringMotor.AccumulateForce(force);
        //m_steeringMotor.SteringForceAccumulation += CalculateSteeringBehaviour() * BehaviourWeigth;
    }

    protected Vector3 SteerTowards(Vector3 _target)
    {
        Vector3 force = _target.normalized * SteeringMotor.MaxSpeed - SteeringMotor.Velocity;
        return Vector3.ClampMagnitude(force, SteeringMotor.MaxSteeringForce);
    }
}
