using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeedController : MonoBehaviour
{
    [SerializeField] private float m_Scale = 1;

    private Motor m_Motor;
    private Animator m_Animator;
    private void Awake()
    {
        m_Motor = GetComponent<Motor>();
        m_Animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        m_Animator.speed = m_Motor.Velocity.magnitude / m_Motor.MaxSpeed * m_Scale;
    }
}
