using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    private Camera m_Cam;

    private void Awake()
    {
        m_Cam = Camera.main;
    }
    void Update()
    {
        Debug.Log(m_Cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_Cam.transform.position.y)));

        transform.position = m_Cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_Cam.transform.position.y));
    }
}
