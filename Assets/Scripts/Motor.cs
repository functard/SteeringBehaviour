using UnityEngine;

public class Motor : MonoBehaviour
{
    private Vector3 m_SteringForceAccumulation;

    private FieldOfView m_Fov;

    public FieldOfView Fov { get { return m_Fov; } }

    private Vector3 m_Velocity = Vector3.zero;

    public Vector3 Velocity { get { return m_Velocity; } }

    private Vector3 m_Acceleration = Vector3.zero;

    private Vector3 m_TurnRateVel;


    [SerializeField] private float m_MaxDefaultSteeringForce = 15f;

    private float m_MaxSteeringForce = 15f;

    public float MaxSteeringForce
    {
        get { return m_MaxSteeringForce; }
        set { m_MaxSteeringForce = value; }
    }

    [SerializeField]
    private float m_MaxSpeed = 5f;

    public float MaxSpeed
    { get { return m_MaxSpeed; } }

    [SerializeField]
    private float m_TurnRate;

    [SerializeField]
    private bool m_RandomizeDirOnStart = false;

    [SerializeField]
    private bool m_Is3D;

    public bool Is3D { get { return m_Is3D; } }

    private void Awake()
    {
        m_Fov = GetComponent<FieldOfView>();
    }
    private void Start()
    {
        FieldOfView.AllSteeringUnits.Add(this);
        if (m_RandomizeDirOnStart)
        {
            if (m_Is3D)
                transform.forward = Random.insideUnitSphere;
            else
            {
                float random = Random.Range(0, 360);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, random, transform.rotation.eulerAngles.z);
            }
        }
    }

    private void Update()
    {
        m_Acceleration = Vector3.zero;
        ApplySteeringBehaviors();

        m_Velocity += m_Acceleration * Time.deltaTime;

        if (!m_Is3D)
            m_Velocity.y = 0f;

        m_Velocity = Vector3.ClampMagnitude(m_Velocity, m_MaxSpeed);

        Debug.Log(m_Velocity.magnitude);
        transform.position += m_Velocity * Time.deltaTime;
        SetDirection();
    }

    private void ApplySteeringBehaviors()
    {
        m_SteringForceAccumulation = Vector3.ClampMagnitude(m_SteringForceAccumulation, m_MaxSteeringForce);

        if (!m_Is3D)
            m_SteringForceAccumulation.y = 0f;

        m_Acceleration += m_SteringForceAccumulation;

        m_SteringForceAccumulation = Vector3.zero;

        // resets max steering force to default value in case it is changed elsewhere
        m_MaxSteeringForce = m_MaxDefaultSteeringForce;

    }

    public void AccumulateForce(Vector3 _force)
    {
        m_SteringForceAccumulation += _force;
    }

    private void SetDirection()
    {
        if (m_Velocity != Vector3.zero && m_Velocity.magnitude >= 0.2f)
            transform.forward = Vector3.SmoothDamp(transform.forward, m_Velocity, ref m_TurnRateVel, m_TurnRate);
    }
}
