using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Slider m_Slider;

    [SerializeField]
    private Animator m_Animator;

    private void Update()
    {
        m_Animator.speed = m_Slider.value;
    }
}
