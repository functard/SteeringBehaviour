using UnityEngine;

public class CircularMover : MonoBehaviour
{
    [SerializeField] private float m_Width = 10f;
    [SerializeField] private float m_Heigth = 10f;
    [SerializeField] private float m_Speed = 1f;

    void Update()
    {
        float x = Mathf.Sin(Time.time * m_Speed) * m_Width; 
        float z = Mathf.Cos(Time.time * m_Speed) * m_Heigth;
        transform.position = new Vector3(x, 0.5f, z);
    }
}
