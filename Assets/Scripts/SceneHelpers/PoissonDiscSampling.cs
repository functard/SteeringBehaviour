using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscSampling : MonoBehaviour
{
    [SerializeField]
    private Vector2Int m_GridSize;

    [SerializeField]
    private GameObject m_Preab;

    [SerializeField]
    private float m_PointSize;

    private Vector3[] m_Grid;

    private List<int> m_ActiveList;

    private float m_CellSize;

    [SerializeField]
    private int m_StepCount = 5;

    [SerializeField]
    private bool m_Debug = false;

    private GameObject m_ParentTransform;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        if (m_ActiveList.Count > 0)
        {
            for (int i = 0; i < m_StepCount; i++)
            {
                Step();
            }
        }
    }

    private void Init()
    {
        m_ParentTransform = new GameObject();

        m_CellSize = m_PointSize / Mathf.Sqrt(2);

        m_ActiveList = new List<int>();

        m_Grid = new Vector3[m_GridSize.x * m_GridSize.y];

        for (int index = 0; index < m_Grid.Length; index++)
        {
            m_Grid[index] = Vector3.negativeInfinity;
        }

        Vector2 randomPoint = new Vector2(Random.Range(0, m_GridSize.x * m_CellSize), Random.Range(0, m_GridSize.y * m_CellSize));

        int indexX = Mathf.FloorToInt(randomPoint.x / m_CellSize);

        int indexY = Mathf.FloorToInt(randomPoint.y / m_CellSize);

        m_Grid[indexX + indexY * m_GridSize.x] = new Vector3(randomPoint.x, 0.0f, randomPoint.y);

        m_ActiveList.Add(indexX + indexY * m_GridSize.x);
    }

    private void Step()
    {
        if (m_ActiveList.Count <= 0)
        {
            return;
        }

        int currentIndex = m_ActiveList[Random.Range(0, m_ActiveList.Count)];

        bool gotSample = false;
        for (int k = 0; k < 30; k++)
        {
            Vector2 randomPoint = Random.insideUnitCircle;
            randomPoint = randomPoint.normalized * Random.Range(m_PointSize, m_PointSize * 2);
            Vector3 pointToTest = m_Grid[currentIndex] + new Vector3(randomPoint.x, 0.0f, randomPoint.y);

            int indexX = Mathf.FloorToInt(pointToTest.x / m_CellSize);

            int indexY = Mathf.FloorToInt(pointToTest.z / m_CellSize);

            if (indexX < 0 || indexX >= m_GridSize.x || indexY < 0 || indexY >= m_GridSize.y ||
                m_Grid[indexX + indexY * m_GridSize.x].x > Vector3.negativeInfinity.x + 1)
            {
                continue;
            }

            bool found = true;
            foreach (int neighbourIndex in GetNeighbours(indexX + indexY * m_GridSize.x))
            {
                //Debug.Log($"{Vector3.Distance(pointToTest, m_Grid[neighbourIndex])} {indexX} {indexY}")

                if (Vector3.Distance(pointToTest, m_Grid[neighbourIndex]) <= m_PointSize)
                {
                    found = false;
                    break;
                }
            }

            if (found)
            {
                //Debug.Log($"{pointToTest} index: {indexX} {indexY}");
                m_Grid[indexX + indexY * m_GridSize.x] = pointToTest;
                m_ActiveList.Add(indexX + indexY * m_GridSize.x);
                Instantiate(m_Preab, new Vector3(pointToTest.x, m_Preab.transform.localScale.y / 2, pointToTest.z), Quaternion.identity, m_ParentTransform.transform); ;
                gotSample = true;
                break;
            }
        }

        if (!gotSample)
        {
            m_ActiveList.Remove(currentIndex);
        }
    }

    private List<int> GetNeighbours(int index)
    {
        List<int> neighbours = new List<int>();

        int StartX = index % m_GridSize.x;
        int StartY = index / m_GridSize.x;

        for (int x = StartX - 1; x <= StartX + 1; x++)
        {
            for (int y = StartY - 1; y <= StartY + 1; y++)
            {
                if (x > 0 && x < m_GridSize.x && y > 0 && y < m_GridSize.y)
                {
                    if (m_Grid[x + y * m_GridSize.x].x > Vector3.negativeInfinity.x + 1)
                    {
                        neighbours.Add(x + y * m_GridSize.x);
                    }
                }
            }

        }

        return neighbours;
    }


    private void OnDrawGizmos()
    {
        if (m_Debug)
        {
            if (m_Grid == null)
            {
                return;
            }
            foreach (var item in m_Grid)
            {
                Gizmos.DrawSphere(item, 1);
            }
        }

    }
}
