using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    [SerializeField] private GameObject m_Character;
    [SerializeField] private GameObject m_ExplosionParticle;
    [SerializeField] private LayerMask m_CharacterLayer;
    [SerializeField] private float m_ExplosionRadius = 10f;
    [SerializeField] private float m_RandomExplosionRange = 10f;
    [SerializeField] private float m_ExplosionForce = 200f;

    private Camera m_Cam;

    private void Awake()
    {
        m_Cam = Camera.main;
    }
    private void Start()
    {
        StartCoroutine(SpawnRandomExplosions());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Explosion(hit.point);
            }
        }
    }
    private void Explosion(Vector3 _pos)
    {
        Instantiate(m_ExplosionParticle, _pos, Quaternion.identity);
        Collider[] collidersInRange = Physics.OverlapSphere(_pos, m_ExplosionRadius, m_CharacterLayer);
        foreach (Collider character in collidersInRange)
        {
            Flee tmp = character.GetComponent<Flee>();
            if (tmp != null)
                StartCoroutine(tmp.FleeFrom(_pos, m_ExplosionForce));
        }

    }

    private IEnumerator SpawnRandomExplosions(float _interval = 5f)
    {
        while (true)
        {
            yield return new WaitForSeconds(_interval);
            Vector2 randomPos = Random.insideUnitCircle * m_RandomExplosionRange + new Vector2(m_Character.transform.position.x, m_Character.transform.position.z);
            Explosion(new Vector3(randomPos.x, 0, randomPos.y));
        }
    }

}
