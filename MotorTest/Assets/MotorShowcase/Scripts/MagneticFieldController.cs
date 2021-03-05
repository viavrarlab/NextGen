using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class MagneticFieldController : MonoBehaviour
{
    public float m_UpdateInterval = 0.1f;

    public float m_MagneticFieldArrowInterval = 1f;

    public Vector3 m_MagneticFieldBounds = Vector3.one;

    public MagneticParticle m_MagneticParticlePrefab;

    public List<MagnetObject> m_Magnets = new List<MagnetObject>();
    public List<MagneticParticle> m_MagneticParticles;

    private float m_Timer;


    void Start()
    {
        m_Timer = m_UpdateInterval;
        SpawnMagneticParticleGrid();

        m_Magnets.AddRange( new List<MagnetObject>(FindObjectsOfType<MagnetObject>()));
        m_MagneticParticles = new List<MagneticParticle>(FindObjectsOfType<MagneticParticle>());
    }

    void Update()
    {
        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0f)
        {
            ApplyForces();
        }
    }

    void ApplyForces()
    {
        foreach (MagneticParticle particle in m_MagneticParticles)
        {
            Vector3 attractiveDirection = Vector3.zero;
            foreach (MagnetObject magnet in m_Magnets)
            {
                float distance = Vector3.Distance(particle.transform.position, magnet.transform.position);
                float force = (1000f * particle.m_Charge * magnet.m_Charge) / Mathf.Pow(distance, 2);

                Vector3 direction = particle.transform.position - magnet.transform.position;
                direction.Normalize();

                attractiveDirection += force * direction;
            }
            //particle.transform.LookAt(attractiveDirection);
            particle.transform.LookAt(attractiveDirection);
        }

        m_Timer = m_UpdateInterval;
    }

    public void SpawnMagneticParticleGrid()
    {
        for (float i = 0; i <= m_MagneticFieldBounds.x; i += m_MagneticFieldArrowInterval)
        {
            for (float j = 0; j <= m_MagneticFieldBounds.y; j += m_MagneticFieldArrowInterval)
            {
                for (float k = 0; k <= m_MagneticFieldBounds.z; k += m_MagneticFieldArrowInterval)
                {
                    Vector3 pos = transform.position;
                    pos += new Vector3(i - (m_MagneticFieldBounds.x / 2f), j - (m_MagneticFieldBounds.y / 2f), k - (m_MagneticFieldBounds.z / 2f));
                    MagneticParticle p = Instantiate(m_MagneticParticlePrefab, pos, Quaternion.identity);
                    p.transform.SetParent(transform);
                    p.gameObject.layer = gameObject.layer;
                    m_MagneticParticles.Add(p);
                }

            }
        }
    }
}
