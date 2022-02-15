using UnityEngine;

public class SpawnRects : MonoBehaviour
{
    [SerializeField]
    private int m_Cols, m_Rows;

    [SerializeField]
    private GameObject m_Prefab;

    [SerializeField]
    private GameObject m_MouseGO;

    private void Start()
    {
        for (int w = 0; w < m_Rows; w++)
        {
            for (int h = 0; h < m_Cols; h++)
            {
                Vector3 pos = new Vector3(w, 0, h);
                GameObject tmp = Instantiate(m_Prefab, pos, Quaternion.identity);
                Transform t = new GameObject().transform;
                t.position = pos;
                tmp.GetComponent<Arrival>().SetTarget(t);
                tmp.GetComponent<Flee>().SetTarget(m_MouseGO.transform);

            }
        }
    }
}
