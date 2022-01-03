using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Prefab;

    [SerializeField]
    private int m_SpawnCount = 10;

    private void Spawn()
    {
        for (int i = 0; i < m_SpawnCount; i++)
        {

        }
    }
}