using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FliesSpawner : MonoBehaviour
{
    [SerializeField] private Path m_Path;

    [SerializeField] private GameObject[] m_Flies;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        int spawnInterval = m_Path.GetPath().Count / m_Flies.Length;
        foreach (var flies in m_Flies)
        {
            flies.transform.position = m_Path.GetNodeAt(i);
            i += spawnInterval;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        int spawnInterval = m_Path.GetPath().Count / m_Flies.Length;
        foreach (var flies in m_Flies)
        {
            flies.transform.position = m_Path.GetNodeAt(i);
            i += spawnInterval;
        }
    }
}
